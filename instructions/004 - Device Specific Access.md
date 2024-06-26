Update `Home.razor` to enable demos

## Updated Home.razor
```` razor
@page "/"
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize]

@inject IJSRuntime JSRuntime

<h1>Hello, world!</h1>

<p>Welcome to your new app.</p>

<p>
    <button class="btn btn-primary" @onclick="TestJsInterop">Test JS Interop</button>
</p>

<p>
    <button class="btn btn-primary" @onclick="TestJsInteropParameters">Test JS Interop w/ Parameters</button>
</p>

<p>
    <button class="btn btn-primary" @onclick="TestJsInteropReceive">Test JS Interop w/ Return Value</button>
</p>

<p>The following was received from JS '@FromJavascript'</p>

<h4>Device Specific Features</h4>

<p>
    <button class="btn btn-primary" @onclick="TryVibrateDevice">Try Vibrate Device</button>
</p>

<p>
    <button class="btn btn-primary" @onclick="TryCachedLocation">Try Cached Location (Last Known: @LastKnownLocation)</button>
</p>

<p>
    <button class="btn btn-primary" @onclick="GetPowerSource">Get Power Source Information: @PowerSource)</button>
</p>

@code {

    public string FromJavascript { get; set; }
    public string LastKnownLocation { get; set; }
    public string PowerSource { get; set; } = "Not Checked";

    private async Task TestJsInterop()
    {
        await JSRuntime.InvokeVoidAsync("demoMethod");
    }

    private async Task TestJsInteropParameters()
    {
        await JSRuntime.InvokeVoidAsync("alert", "I can even pass stuff!");
    }

    private async Task TestJsInteropReceive()
    {
        FromJavascript = await JSRuntime.InvokeAsync<string>("showPrompt", "Please provide a value to display");
    }

    private async Task TryVibrateDevice()
    {
        //https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/vibrate?view=net-maui-8.0&tabs=windows
        if (Vibration.Default.IsSupported)
        {
            Vibration.Default.Vibrate(TimeSpan.FromSeconds(3));
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("alert", "Vibration is not supported");
        }
    }

    private async Task TryCachedLocation()
    {
        // Documentation: https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/geolocation?view=net-maui-8.0&tabs=android
        try
        {
            var location = await Geolocation.Default.GetLastKnownLocationAsync();

            if (location != null)
                LastKnownLocation = $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}";
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            // Handle not supported on device exception
            LastKnownLocation = "Not Supported";
        }
        catch (FeatureNotEnabledException fneEx)
        {
            // Handle not enabled on device exception
            LastKnownLocation = "Not Enabled";
        }
        catch (PermissionException pEx)
        {
            // Handle permission exception
            LastKnownLocation = "No Permission";
        }
        catch (Exception ex)
        {
            // Unable to get location
            LastKnownLocation = $"Error: {ex.Message}";
        }

        LastKnownLocation = "Unknown";
    }

    private void GetPowerSource()
    {
        //https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/battery?view=net-maui-8.0&tabs=windows
        PowerSource = Battery.Default.PowerSource switch
        {
            BatteryPowerSource.Wireless => "Wireless charging",
            BatteryPowerSource.Usb => "USB cable charging",
            BatteryPowerSource.AC => "Device is plugged in to a power source",
            BatteryPowerSource.Battery => "Device isn't charging",
            _ => "Unknown"
        };
    }
}
````