using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace JToolbox.AppConfig
{
    public class AppConfigService : IAppConfigService
    {
        protected NameValueCollection AppSettings => ConfigurationManager.AppSettings;

        public string GetValue([CallerMemberName] string key = null, bool throwIfNotExists = true)
        {
            if (AppSettings[key] == null && throwIfNotExists)
            {
                throw new NoAppConfigKeyException();
            }
            return AppSettings[key];
        }
    }
}