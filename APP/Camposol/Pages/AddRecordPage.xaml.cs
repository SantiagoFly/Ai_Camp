using Camposol.CustomPopup;
using Camposol.ViewModels;
using Mopups.Interfaces;
using System.Diagnostics;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Camposol.Pages;

/// <summary>
/// Entry form UI
/// </summary>
public partial class AddRecordPage
{

    IDispatcherTimer timer;
    Stopwatch stopwatch = new Stopwatch();

    /// <summary>
    /// Receives the depedencies by DI
    /// </summary>
    public AddRecordPage(AddRecordViewModel viewModel) : base(viewModel, "Nueva grabacion")
    {
        InitializeComponent();

        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += (s, e) =>
        {
            Timer.Text = string.Format("{0:hh\\:mm\\:ss}", stopwatch.Elapsed.Duration());
            
            if (stopwatch.Elapsed.Duration().Minutes == 2) 
            {
                stopwatch.Stop();
                timer.Stop();
                this.ViewModel.EndRecordCommand.Execute(null);
            }
        };
    }

    void OnStartRecordTime(object sender, EventArgs args)
    {
        stopwatch.Start();
        timer.Start();
    }

    void OnStopRecordTime(object sender, EventArgs args)
    {
        stopwatch.Stop();
        timer.Stop();
    }

    async void OnResetRecordTime(object sender, EventArgs args)
    {
        //consultar para cancelar grabacion

        if (await DisplayAlert("Alerta", "¿Desea cancelar la grabación?", "Si", "No"))
        {
            try
            {
                this.IsBusy = true;
                stopwatch.Reset();
                Timer.Text = "00:00:00";

                ViewModel.IsVisibleStartRecord = true;
                ViewModel.IsVisibleCancelOrOKRecord = false;
                ViewModel.IsEnableStarOrEndRecord = true;
            }
            catch
            {
                await DisplayAlert("Error", "No se ha podido cancelar la grabación", "Cerrar");
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }

    async void SelectedRowEntry_TextChanged(object sender, EventArgs args)
    {
        int numero;
        var isInteger = int.TryParse(this.SelectedRowEntry.Text, out numero);

        if (!string.IsNullOrEmpty(this.SelectedRowEntry.Text) && !isInteger) 
        {
            this.SelectedRowEntry.Text = string.Empty;
        }
        else if (isInteger && (numero > 90 || numero < 1))
        {
            await DisplayAlert("Invalido", "El valor debe estar entre 1 y 90", "Cerrar");
            this.SelectedRowEntry.Text = string.Empty;
        }

    }

    async void SelectedPlantEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        int numero;

        var isInteger = int.TryParse(this.SelectedPlantEntry.Text, out numero);

        if (!string.IsNullOrEmpty(this.SelectedPlantEntry.Text) && !isInteger)
        {
            this.SelectedPlantEntry.Text = string.Empty;
        }
        else if (isInteger && (numero > 90 || numero < 1))
        {
            await DisplayAlert("Invalido", "El valor debe estar entre 1 y 90", "Cerrar");
            this.SelectedPlantEntry.Text = string.Empty;
        }
    }
}

