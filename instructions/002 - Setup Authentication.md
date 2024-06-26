The goal of this stage is to setup the application for proper usage with authentication.

* Install the following NuGet Package
  * Install-Pacakge Microsoft.AspNetCore.Components.Authorization
* Add new item to the /Components/_Imports.razor file `@using Microsoft.AspNetCore.Components.Authorization`
* Create a New Class "CustomAuthenticationStateProvider.cs"
* Register Our Provider & Replace Default with ours in Program.cs using the snippet below
* Update /Components/Routes.razor to support authentication
* Create a folder "Models" and add "LoginViewModel" (Code Below)
* Add Login.razor, Logout.razor to the /Components/Pages folder
* Add RedirectolOgin.razor
* Update NavMenu.razor to have an authorized view
* Add the [Authorize] attribute to Counter, Home, and Weather pages



## CustomAuthenticationStateProvider.cs

```` csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace DemoApplication.UI;

/// <summary>
///     This is our custom implementation of an authentication state provider.  The primary purpose of this is
///     to implement the overriden method, which is used internally by .NET to determine the current user identity.
///     For clarity, and ease of use I put the code here that is used to actually trigger login & logout.
/// </summary>
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    /// <summary>
    ///     This method should be called upon a successful user login, and it will store the user's JWT token in SecureStorage.
    ///     Upon saving this it will also notify .NET that the authentication state has changed which will enable authenticated
    ///     views
    /// </summary>
    /// <param name="token">Our JWT to store</param>
    /// <returns></returns>
    public async Task Login(string token)
    {
        //Again, this is what I'm doing, but you could do/store/save anything as part of this process
        await SecureStorage.SetAsync("accounttoken", token);

        //Providing the current identity ifnormation
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    ///     This method should be called to log-off the user from the application, which simply removed the stored token and
    ///     then
    ///     notifies of the change
    /// </summary>
    /// <returns></returns>
    public async Task Logout()
    {
        SecureStorage.Remove("accounttoken");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    ///     This is the key method that is called by .NET to accomplish our goal.  It is the method that is queried to get the
    ///     current
    ///     AuthenticationState for the user.  In our base, if we have a token in secure storage, we are logged in, but we
    ///     could easily
    ///     do much more than this.
    /// </summary>
    /// <returns></returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userInfo = await SecureStorage.GetAsync("accounttoken");
            if (userInfo != null)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "Sample User") };
                var identity = new ClaimsIdentity(claims, "Custom authentication");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
        }
        catch (Exception ex)
        {
            //This should be more properly handled
            Console.WriteLine("Request failed:" + ex);
        }

        return new AuthenticationState(new ClaimsPrincipal());
    }
}
````

## MauiProgram.cs Changes
```` csharp
builder.Services.AddAuthorizationCore();
//Add Authentication
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
//When asking for the default Microsoft one, give ours!
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomAuthenticationStateProvider>());
````

## Routes.razor Changes
```` razor
<Router AppAssembly="@typeof(MauiProgram).Assembly">
    <Found Context="routeData">
        <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(Layout.MainLayout)">
            <Authorizing>
                Authorizing...
            </Authorizing>
            <NotAuthorized>
                <RedirectToLogin />
            </NotAuthorized>
        </AuthorizeRouteView>
    </Found>
    <NotFound>
        <CascadingAuthenticationState>
            <LayoutView Layout="@typeof(Layout.MainLayout)">
                <p>Sorry, there's nothing at this address.</p>
            </LayoutView>
        </CascadingAuthenticationState>
    </NotFound>
</Router>
````

## LoginViewModel.cs
```` csharp
using System.ComponentModel.DataAnnotations;

namespace DemoApplication.UI.Models;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    public bool LoginFailureHidden { get; set; } = true;

    public bool ValidateLogin(out string jwtToken)
    {
        if (Username.Equals("Test") && Password.Equals("Test"))
        {
            jwtToken = "123456";
            return true;
        }

        //Not valid
        jwtToken = null;
        LoginFailureHidden = false;
        return false;
    }
}
````

## Login.razor
```` razor
﻿@page "/login"
@using DemoApplication.UI.Models
@inject NavigationManager Navigation;
@inject CustomAuthenticationStateProvider AuthStateProvider;

<h3>Login to Access Application</h3>

<div class="alert alert-info">
    This is a dummy login page, providing `Test` for the Username and Password will authenticate you.
</div>

<EditForm Model="@loginModel" OnValidSubmit="@HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />
    <div class="alert alert-danger" hidden="@loginModel.LoginFailureHidden">
        Invalid login attempt, please correct and try again.
    </div>
    <div class="form-group">
        <label>Username</label>
        <InputText id="email" @bind-Value="loginModel.Username" class="form-control" />
    </div>
    <div class="form-group">
        <label>Password</label>
        <InputText id="password" type="password" @bind-Value="loginModel.Password" class="form-control" />
    </div>
    <div class="form-group mt-1">
        <button type="submit" class="btn btn-primary w-100">Login Now</button>
    </div>
</EditForm>

@code {
    private LoginViewModel loginModel = new();

    private async Task HandleValidSubmit()
    {
        //Validate the user Account
        var successful = loginModel.ValidateLogin(out string jwtToken);

        //Not successful, don't need to do anything
        if (!successful)
            return;

        //Call login and redirect
        await AuthStateProvider.Login(jwtToken);
        Navigation.NavigateTo(""); //Root URL
    }
}
````

## Logout.razor
```` razor
@page "/logout"
@inject NavigationManager navigationManager
@inject CustomAuthenticationStateProvider customAuthenticationStateProvider

<div class="loader loader-bouncing"><span>Redirecting...</span></div>
@code{

    protected override async Task OnInitializedAsync()
    {
        await customAuthenticationStateProvider.Logout();
        navigationManager.NavigateTo("/login");
    }
}
````

## /Components/NavMenu.razor
```` razor
<div class="top-row ps-3 navbar navbar-dark">
    <div class="container-fluid">
        <a class="navbar-brand" href="">DemoApplication.UI</a>
    </div>
</div>

<input type="checkbox" title="Navigation menu" class="navbar-toggler" />

<div class="nav-scrollable" onclick="document.querySelector('.navbar-toggler').click()">
    <nav class="flex-column">
        <AuthorizeView>
            <Authorized>
                <div class="nav-item px-3">
                    <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
                        <span class="bi bi-house-door-fill-nav-menu" aria-hidden="true"></span> Home
                    </NavLink>
                </div>
                <div class="nav-item px-3">
                    <NavLink class="nav-link" href="counter">
                        <span class="bi bi-plus-square-fill-nav-menu" aria-hidden="true"></span> Counter
                    </NavLink>
                </div>
                <div class="nav-item px-3">
                    <NavLink class="nav-link" href="weather">
                        <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> Weather
                    </NavLink>
                </div>
                <div class="nav-item px-3">
                    <NavLink class="nav-link" href="logout">
                        <span class="bi bi-house-door-fill-nav-menu" aria-hidden="true"></span> Logout
                    </NavLink>
                </div>
            </Authorized>
            <NotAuthorized>
                <div class="nav-item px-3">
                    <NavLink class="nav-link" href="login">
                        <span class="bi bi-house-door-fill-nav-menu" aria-hidden="true"></span> Login
                    </NavLink>
                </div>
            </NotAuthorized>
        </AuthorizeView>
        
    </nav>
</div>
````