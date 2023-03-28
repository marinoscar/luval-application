using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security;

namespace Luval.AI
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<SecureString>(GetApiKey().Result);
            builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }

        private static async Task<SecureString> GetApiKey()
        {
            if(!await FileSystem.AppPackageFileExistsAsync("Secure/OpenAIKey.txt")) return null;

            using var stream = await FileSystem.OpenAppPackageFileAsync("Secure\\OpenAIKey.txt");
            using var reader = new StreamReader(stream);
            return new NetworkCredential("", reader.ReadToEnd()).SecurePassword;
        }
    }
}