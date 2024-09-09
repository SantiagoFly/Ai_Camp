using Camposol.Common.Interfaces;
using Microsoft.Extensions.Localization;
using Camposol.Helpers;
using Camposol.Resources.Strings;

namespace Camposol.Services
{
    public class LocalizationService : ILocalizationService
    {
        private IStringLocalizer<Texts> localizer;

        public LocalizationService()
        {
            this.localizer = ServiceHelper.GetService<IStringLocalizer<Texts>>();
        }


        public string GetText(string text)
        {
            var localizedText = this.localizer[text];
            return localizedText;
        }
    }
}
