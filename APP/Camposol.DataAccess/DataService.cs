using Microsoft.EntityFrameworkCore;
using Camposol.Common.Interfaces;
using Camposol.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Camposol.Common;
using Azure.Storage.Blobs.Models;

namespace Camposol.DataAccess
{ /// <summary>
  /// Data access service
  /// </summary>
    public class DataService : IDataService
    {

        private BlobServiceClient blobServiceClient;

        /// <summary>
        /// Initialization
        /// </summary>
        public DataService()
        {
            this.blobServiceClient = new BlobServiceClient(Constants.ContainerConnectionString);
        }


        ///<inheritdoc/>
        //public async Task<bool> MustSynchronizeAsync()
        //{
        //    var count = 0;
        //    using (var databaseContext = new DatabaseContext())
        //    {
        //        count = await databaseContext.Items.CountAsync();
        //    }
        //    return count == 0;
        //}


        ///<inheritdoc/>
        public async Task<int> SaveItemsAsync<T>(IEnumerable<T> items, string tableName) where T : class
        {
            using (var databaseContext = new DatabaseContext())
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    await databaseContext.Database.ExecuteSqlRawAsync($"DELETE FROM {tableName}");
                }
                await databaseContext.Set<T>().AddRangeAsync(items);
                var itemsCount = await databaseContext.SaveChangesAsync().ConfigureAwait(false);
                return itemsCount;
            }
        }


        ///<inheritdoc/>
        public async Task<int> InsertOrUpdateItemsAsync<T>(T item) where T : class
        {
            using (var databaseContext = new DatabaseContext())
            {
                var itemsCount = await databaseContext.UpsertRange<T>(item).RunAsync();
                return itemsCount;
            }
        }


        ///<inheritdoc/>
        public async Task<int> DeleteItemAsync<T>(T item) where T : class
        {
            using (var databaseContext = new DatabaseContext())
            {
                databaseContext.Remove(item);
                var itemsCount = await databaseContext.SaveChangesAsync().ConfigureAwait(false);
                return itemsCount;
            }
        }


        ///<inheritdoc/>
        public async Task<List<Recording>> LoadRecordsAsync()
        {
            using (var databaseContext = new DatabaseContext())
            {
                var items = await databaseContext.Recording.ToListAsync().ConfigureAwait(false);
                return items;
            }
        }

        ///<inheritdoc/>
        public async Task<Settings> LoadsettingsAsync()
        {
            using (var databaseContext = new DatabaseContext())
            {
                var items = await databaseContext.Settings.FirstOrDefaultAsync().ConfigureAwait(false);
                return items;
            }
        }

        public async Task<bool> SaveFileAsync(string container,string fileName, Stream file, string contentType) 
        {
            try
            {
                var containerClient = blobServiceClient.GetBlobContainerClient(container);

                await containerClient.CreateIfNotExistsAsync();

                var blobClient = containerClient.GetBlobClient(fileName);

                var uploadOptions = new BlobUploadOptions();

                uploadOptions.HttpHeaders = new BlobHttpHeaders();

                uploadOptions.HttpHeaders.ContentType = contentType;

                await blobClient.UploadAsync(file, uploadOptions);

                return true;
            }
            catch
            {
                return false;
            }
            
        }

    }
}
