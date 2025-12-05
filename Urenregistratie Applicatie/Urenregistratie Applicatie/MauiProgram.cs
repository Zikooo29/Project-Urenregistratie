using Microsoft.Extensions.Logging;
using Urenregistratie_Applicatie.Services;
using Urenregistratie_Applicatie.Views;

namespace Urenregistratie_Applicatie
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

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "urenregistratie.db");
            var databaseService = new DatabaseService(dbPath);
            builder.Services.AddSingleton(s => databaseService);

        
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<Page1>();
            builder.Services.AddSingleton<Page2>();
            builder.Services.AddSingleton<Page3>();

            


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

