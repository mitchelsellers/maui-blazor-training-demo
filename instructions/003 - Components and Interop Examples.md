Components & Interop Demos

* Add a new Razor Component `Alert.razor`
* Add the following to Login.razor to demonstrate usage `<Alert title="This is a more clean way to do things!"></Alert>`
* Add a JS Method to the /wwwroot/index.html page
* Replace the contents of the `Home.razor` to see the usage


## Alert.razor
```` razor
<div class="alert alert-info">
    @Title
</div>

@code {
    [Parameter]
    public string Title { get; set; }
}
````

## Additions to index.html
```` html
<script>
    function demoMethod() {
        alert('This is from JS!');
    }
</script>
````

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

@code {

    private async Task TestJsInterop()
    {
        await JSRuntime.InvokeVoidAsync("demoMethod");
    }

    private async Task TestJsInteropParameters()
    {
        await JSRuntime.InvokeVoidAsync("alert", "I can even pass stuff!");
    }
}
````