using CommunityToolkit.Maui;
using Camposol.Common.Extensions;
using Camposol.Common.Interfaces;
using Camposol.Common.Services;
using Camposol.DataAccess;
using Plugin.Maui.Audio;
using Mopups.Hosting;
using Mopups.Interfaces;
using Mopups.Services;

namespace Camposol;

/// <summary>
/// MAUI entry point 
/// </summary>
public static class MauiProgram
{
	/// <summary>
	/// Creates the app.
	/// Declares the font, depedenc
	/// y injection components
	/// </summary>
	/// <returns></returns>
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()	
			.RegisterViewModelsAndServices()
            .ConfigureMopups()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddDbContext<DatabaseContext>();
        builder.Services.AddSingleton<IDataService, DataService>();
        builder.Services.AddLocalization();
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();
        builder.Services.AddSingleton<IPopupNavigation>(MopupService.Instance);
        return builder.Build();
	}
}
