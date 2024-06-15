namespace ODataCovid.Services
{
    public interface IConfigManager
	{
        //string EfyCreContract { get; }
        //string EfyAccount { get; }

        //string GetConnectionString(string connectionName);

        IConfigurationSection GetConfigurationSection(string Key);
    }
}

