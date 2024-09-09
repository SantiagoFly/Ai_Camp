using Camposol.Common.Interfaces;
using Camposol.Common.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.Maui.Audio;
using Camposol.Models;
using System.Windows.Input;
using Newtonsoft.Json;


namespace Camposol.ViewModels
{
    /// <summary>
    /// Logic to show info from an item selected
    /// </summary>
    public partial class AddRecordViewModel : BaseViewModel
    {

        private readonly IAudioManager audioManager;
        private readonly IAudioRecorder audioRecorder;


        /// <summary>
        /// Columns
        /// </summary>
        [ObservableProperty]
        private List<Lote> lotes;

        /// <summary>
        /// Selected columns
        /// </summary>
        [ObservableProperty]
        private string selectedPlant;

        /// <summary>
        /// Selected row
        /// </summary>
        [ObservableProperty]
        private string selectedRow;
        
        /// <summary>
        /// Selected lote
        /// </summary>
        [ObservableProperty]
        private Lote selectedLote;

        /// <summary>
        /// Selected lote
        /// </summary>
        [ObservableProperty]
        private Lote oldSelectedLote;

        /// <summary>
        /// Is visible start record
        /// </summary>
        [ObservableProperty]
        private bool isVisibleStartRecord;

        /// <summary>
        /// Is visible start record
        /// </summary>
        [ObservableProperty]
        private bool isVisibleCancelOrOKRecord;

        /// <summary>
        /// Is visible start record
        /// </summary>
        [ObservableProperty]
        private bool isVisibleSelectLote;
        
        /// <summary>
        /// Is visible start record
        /// </summary>
        [ObservableProperty]
        private bool isVisibleRecord;

        /// <summary>
        /// Timer
        /// </summary>
        [ObservableProperty]
        private IDispatcherTimer timer;

        /// <summary>
        /// Is visible start record
        /// </summary>
        [ObservableProperty]
        private IAudioSource audio;


        /// <summary>
        /// Is visible start record
        /// </summary>
        [ObservableProperty]
        private bool isEnableStarOrEndRecord;


        /// <summary>
        /// Gets by DI the required services
        /// </summary>
        public AddRecordViewModel(IServiceProvider provider, IAudioManager audioManager) : base(provider)
        {
            this.audioManager = audioManager;
            this.audioRecorder = audioManager.CreateRecorder();
        }

        /// <summary>
        /// Start record command
        /// </summary>
        public ICommand StartRecordCommand => new Command(async () =>
        {
            try
            {
                this.IsVisibleStartRecord = false;

                if (await Permissions.RequestAsync<Permissions.Microphone>() != PermissionStatus.Granted)
                {
                    return;
                }

                if (!this.audioRecorder.IsRecording)
                {
                    await this.audioRecorder.StartAsync();
                }
            }
            catch
            {
                await this.NotificationService.NotifyErrorAsync("Error", "No se ha podido grabar");
            }

        });

        /// <summary>
        /// End record command
        /// </summary>
        public ICommand EndRecordCommand => new Command(async () =>
        {
            try
            {
                this.IsVisibleStartRecord = true;

                if (this.audioRecorder.IsRecording)
                {
                    audio = await this.audioRecorder.StopAsync();

                    this.IsVisibleCancelOrOKRecord = true;

                    this.IsEnableStarOrEndRecord = false;

                }
            }
            catch
            {
                await this.NotificationService.NotifyErrorAsync("Error", "No se ha podido grabar");
            }
        });
        
        /// <summary>
        /// Back command
        /// </summary>
        public ICommand BackCommand => new Command(async () =>
        {
            try
            {
                await this.NavigationService.Navigate<HomeViewModel>();
            }
            catch
            {
                await this.NotificationService.NotifyErrorAsync("Error", "No se ha podido regresar");
            }
        });

        public ICommand SelectLoteCommand => new Command(() =>
        {
            this.IsVisibleSelectLote = true;
            this.IsVisibleRecord = false;
            this.OldSelectedLote = this.SelectedLote;
        });

        public ICommand OkSelectLoteCommand => new Command(() =>
        {
            this.IsVisibleSelectLote = false;
            this.IsVisibleRecord = true;
        });

        public ICommand CancelSelectLoteCommand => new Command(() =>
        {
            this.IsVisibleSelectLote = false;
            this.IsVisibleRecord = true;
            this.SelectedLote = this.OldSelectedLote;
        });

        /// <summary>
        /// Cancel record command
        /// </summary>
        public ICommand OkRecordCommand => new Command(async () =>
        {
            try
            {
                await this.NotificationService.ConfirmAsync("Guardar", "¿Desea guardar la grabación?", "Si", "No", async (result) =>
                {
                    this.IsVisibleRecord = false;
                    this.IsBusy = true;

                    if (result)
                    {
                        this.IsVisibleStartRecord = true;

                        if (this.audio != null && !string.IsNullOrEmpty(this.SelectedPlant) && !string.IsNullOrEmpty(this.SelectedRow) && this.SelectedLote != null)
                        {
                            var file = (FileAudioSource)audio;

                            var player = AudioManager.Current.CreatePlayer(file.GetAudioStream());

                            this.IsVisibleCancelOrOKRecord = false;

                            await this.DataService.InsertOrUpdateItemsAsync<Recording>(new Recording
                            {
                                Id = Guid.NewGuid().ToString(),
                                Date = DateTime.Now,
                                Duration = player.Duration,
                                AudioPath = file.GetFilePath(),
                                Lote = this.SelectedLote.Name,
                                Plant = this.SelectedPlant,
                                Row = this.SelectedRow
                            });

                            await this.NavigationService.Navigate<HomeViewModel>();

                        }
                        else
                        {
                            await this.NotificationService.NotifyErrorAsync("Error", "Faltan seleccionar campos");
                        }
                    }

                    this.IsBusy = false;
                });

            }
            catch
            {
                await this.NotificationService.NotifyErrorAsync("Error", "No se ha podido guardar la grabacion");
                this.IsBusy = false;
            }

        });


        /// <summary>
        /// On appearing
        /// </summary>
        public override async void OnAppearing()
        {
            this.Lotes = new List<Lote>();

            var filePath = "Lotes.json";
            var stream = await FileSystem.OpenAppPackageFileAsync(filePath);

            if (stream != null)
            {
                var lotes = (new System.IO.StreamReader(stream)).ReadToEnd();
                this.Lotes = JsonConvert.DeserializeObject<List<Lote>>(lotes);
            }


            this.IsVisibleStartRecord = true;
            this.IsVisibleRecord = true;
            this.IsVisibleCancelOrOKRecord = false;
            this.IsEnableStarOrEndRecord = true;

        }
    }
}
