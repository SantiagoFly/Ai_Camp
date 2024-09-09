using Camposol.Common.Interfaces;
using Camposol.Common.ViewModels;


namespace Camposol.ViewModels
{
    /// <summary>
    /// Sample entry form
    /// </summary>
    public class EntryFormViewModel : BaseViewModel
    {
      
        /// <summary>
        /// Gets by DI the required services
        /// </summary>
        public EntryFormViewModel(IServiceProvider provider) : base(provider)
        {
        }
    }
}
