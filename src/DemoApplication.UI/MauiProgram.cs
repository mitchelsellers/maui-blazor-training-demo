using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace DemoApplication.UI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });


            builder.Services.AddMauiBlazorWebView();


            builder.Services.AddAuthorizationCore();
            //Add Authentication
            builder.Services.AddScoped<CustomAuthenticationStateProvider>();
            //When asking for the default Microsoft one, give ours!
            builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomAuthenticationStateProvider>());

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
