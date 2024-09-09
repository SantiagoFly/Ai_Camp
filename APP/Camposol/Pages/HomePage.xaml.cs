
using Camposol.Common.Pages;
using Camposol.CustomPopup;
using Camposol.ViewModels;
using Mopups.Interfaces;
using Mopups.Services;

namespace Camposol.Pages;

/// <summary>
/// Home UI
/// </summary>
public partial class HomePage
{

    IPopupNavigation popupNavigation;

    /// <summary>
    /// Receives the depedencies by DI
    /// </summary>
    public HomePage(HomeViewModel viewModel, IPopupNavigation popupNavigation) : base(viewModel, "Inicio")
	{
		InitializeComponent();
        this.popupNavigation = popupNavigation;

    }


    /// <summary>
	/// Popups settings.
	/// </summary>
	private async void AddSettingsClicked(object sender, EventArgs e)
    {
        await popupNavigation.PushAsync(new SetSettingsPage(this.popupNavigation,ViewModel.ChangeSettingsCommand, ViewModel.Settings));
    }
}

