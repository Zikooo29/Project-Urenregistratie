using Microsoft.Extensions.Logging;

namespace Urenregistratie_Applicatie
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.Services.AddSingleton<DatabaseService>(s =>
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
                return new DatabaseService(dbPath);
            });
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();


        }
    }
}
