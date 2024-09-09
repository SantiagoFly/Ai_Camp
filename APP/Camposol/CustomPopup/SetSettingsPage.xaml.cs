using Camposol.Models;
using Mopups.Interfaces;
using System.Windows.Input;

namespace Camposol.CustomPopup;

public partial class SetSettingsPage
{
    IPopupNavigation popupNavigation;

    ICommand changeSettingsCommand;

    Settings Settings;

    public SetSettingsPage(IPopupNavigation popupNavigation, ICommand changeSettingsCommand, Settings settings)
	{
		InitializeComponent();
        this.popupNavigation = popupNavigation;
        this.changeSettingsCommand = changeSettingsCommand;
        this.Settings = settings;
        this.MaxDuration.Text = settings.MaxDurationInMinutes.ToString();
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await this.popupNavigation.PopAllAsync();
    }

    private async void Ok_Clicked(object sender, EventArgs e)
    {
        this.Settings.MaxDurationInMinutes = int.Parse(this.MaxDuration.Text);
        this.changeSettingsCommand.Execute(this.Settings);
        await this.popupNavigation.PopAllAsync();
    }
}