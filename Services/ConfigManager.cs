using System;
namespace ODataCovid.Services
{
	public class ConfigManager: IConfigManager
    {
        private readonly IConfiguration _configuration;
        public ConfigManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IConfigurationSection GetConfigurationSection(string key)
        {
            return this._configuration.GetSection(key);
        }
    }
}

