using System.Runtime.CompilerServices;

namespace JToolbox.AppConfig
{
    public interface IAppConfigService
    {
        string GetValue([CallerMemberName] string key = null, bool throwIfNotExists = true);
    }
}