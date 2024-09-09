using Camposol.Common.Interfaces;
using Camposol.Common.ViewModels;
using Camposol.Models;
using Camposol.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Windows.Input;

namespace Camposol.ViewModels
{
    /// <summary>
    /// Home logic
    /// </summary>
    public partial class HomeViewModel : BaseViewModel
    {

        /// <summary>
        /// Settings
        /// </summary>
        [ObservableProperty]
        private Settings settings;

        /// <summary>
        /// Is Visible Send All
        /// </summary>
        [ObservableProperty]
        private bool isVisibleSendAll;

        ///// <summary>
        ///// Menu
        ///// </summary>
        [ObservableProperty]
        private ObservableCollection<Recording> recordings;

        /// <summary>
        /// Gets by DI the required services
        /// </summary>
        public HomeViewModel(IServiceProvider provider) :  base(provider)
        {
            
        }

        public override async void OnAppearing()
        {
            this.Recordings = new ObservableCollection<Recording>(await this.DataService.LoadRecordsAsync());

            this.IsVisibleSendAll= this.Recordings.Any(x => !x.Sended);

            this.Settings = await this.DataService.LoadsettingsAsync();

            this.Settings ??= new Settings 
            {
                MaxDurationInMinutes = 2
            };

            await this.DataService.InsertOrUpdateItemsAsync(this.settings);
        }

        /// <summary>
        /// Send record command
        /// </summary>
        public ICommand SendRecordCommand => new Command<Recording>(async (record) =>
        {
            try 
            {
                await this.NotificationService.ConfirmAsync("Enviar", "¿Desea enviar la grabación?", "Si", "No", async (result) =>
                {
                    this.IsBusy = true;

                    if (result)
                    {
                        var blobName = $"{record.Lote}-{record.Row}-{record.Plant}-{record.Date.ToString("ddMMyyyyHHMMss")}.wav";

                        FileStream fileStream = File.OpenRead(record.AudioPath);

                        var sended = await this.DataService.SaveFileAsync(Constants.ContainerName, blobName, fileStream, "audio/wav");

                        record.Sended = sended;
                        await this.DataService.InsertOrUpdateItemsAsync(record);

                        if (sended) 
                        {
                            _ = await this.NotificationService.NotifyAsync("Enviado", "La grabación ha sido enviada", "Cerrar");
                        }
                        else 
                        {
                            _ = await this.NotificationService.NotifyAsync("Error", "No se pudo enviar la grabacion", "Cerrar");
                        }
                        this.IsVisibleSendAll = this.Recordings.Any(x => !x.Sended);
                        this.IsBusy = false;
                    }
                });
            }
            catch
            {
                await this.NotificationService.NotifyErrorAsync("Error", "No se ha podido enviar la grabación");
                this.IsBusy = false;
            }
            
        });

        /// <summary>
        /// Send record command
        /// </summary>
        public ICommand SendAllRecordCommand => new Command(async () =>
        {
            try
            {
                await this.NotificationService.ConfirmAsync("Enviar", "¿Desea enviar las grabaciónes pendientes?", "Si", "No", async (result) =>
                {
                    this.IsBusy = true;

                    string errors = string.Empty;

                    if (result)
                    {
                        var records = this.Recordings.Where(x => !x.Sended).ToList();

                        records.ForEach(async (record) =>
                        {
                            var blobName = $"{record.Lote}-{record.Row}-{record.Plant}-{record.Date.ToString("ddMMyyyyHHMMss")}.wav";

                            FileStream fileStream = File.OpenRead(record.AudioPath);

                            var sended = await this.DataService.SaveFileAsync(Constants.ContainerName, blobName, fileStream, "audio/wav");

                            if(sended) 
                            {
                                errors.Concat($"Lote: {record.Lote}, Hilera: {record.Row}, Planta: {record.Plant}\n");
                            }

                            record.Sended = true;
                            await this.DataService.InsertOrUpdateItemsAsync(record);

                        });

                        if (!errors.Any())
                        {
                            _ = await this.NotificationService.NotifyAsync("Enviado", "Las grabaciónes han sido enviadas", "Cerrar");
                        }
                        else
                        {
                            _ = await this.NotificationService.NotifyAsync("Error", $"No enviados:\n {errors}", "Cerrar");
                        }

                        this.IsVisibleSendAll = this.Recordings.Any(x => !x.Sended);
                        this.IsBusy = false;
                    }
                });
            }
            catch
            {
                await this.NotificationService.NotifyErrorAsync("Error", "No se ha podido enviar la grabación");
                this.IsBusy = false;
            }

        });


        /// <summary>
        /// Remove record command
        /// </summary>
        public ICommand RemoveRecordCommand => new Command<Recording>(async (record) =>
        {
            try
            {
                await this.NotificationService.ConfirmAsync("Eliminar", "¿Desea eliminar la grabación?", "Si", "No", async (result) =>
                            {
                                if (result)
                                {
                                    this.IsBusy = true;

                                    if (File.Exists(record.AudioPath))
                                    {
                                        File.Delete(record.AudioPath);
                                    }

                                    await this.DataService.DeleteItemAsync(record);

                                    this.Recordings.Remove(record);

                                    this.IsBusy = false;
                                }
                            });
            }
            catch
            {
                await this.NotificationService.NotifyErrorAsync("Error", "No se ha podido eliminar la grabación");
                this.IsBusy = false;
            }
        });


        /// <summary>
        /// New record command
        /// </summary>
        public ICommand NewRecordCommand => new Command(async () =>
        {
            await this.NavigationService.Navigate<AddRecordViewModel>();

        });

        /// <summary>
        /// Start record command
        /// </summary>
        public ICommand ChangeSettingsCommand => new Command<Settings>(async (settings) =>
        {
            try
            {
                if (settings.MaxDurationInMinutes > 0)
                {
                    await this.DataService.InsertOrUpdateItemsAsync<Settings>(settings);
                }
                else
                {
                    await this.NotificationService.NotifyErrorAsync("Error", "El valor no puede ser menor a 1");
                }
            }
            catch
            {
                await this.NotificationService.NotifyErrorAsync("Error", "No se ha podido guardar la grabacion");
                this.IsBusy = false;
            }
        });
    }
}
