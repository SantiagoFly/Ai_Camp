using Camposol.Models;

namespace Camposol.Common.Interfaces
{

    /// <summary>
    /// Data access service interface
    /// </summary>
    public interface IDataService
    {

        /// <summary>
        /// Checks if the synchronization is mandatory
        /// </summary>
        /// <returns></returns>
        //Task<bool> MustSynchronizeAsync();
     

        /// <summary>
        /// Save a collections of items
        /// </summary>
        Task<int> SaveItemsAsync<T>(IEnumerable<T> items, string tableName) where T : class;

        /// <summary>
        /// Save a collections of items 
        /// </summary>
        Task<int> InsertOrUpdateItemsAsync<T>(T item) where T : class;
     
        
        /// <summary>
        /// Remove a item
        /// </summary>
        Task<int> DeleteItemAsync<T>(T item) where T : class;


        /// <summary>
        /// Loads records from the local storage
        /// </summary>
        Task<List<Recording>> LoadRecordsAsync();


        /// <summary>
        /// Loads settings from the local storage
        /// </summary>
        Task<Settings> LoadsettingsAsync();

        /// <summary>
        /// Save file in blob container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <param name="contentType"></param>
        /// <returns>Task<bool></bool></returns>
        Task<bool> SaveFileAsync(string container, string fileName, Stream file, string contentType);
    }
}
