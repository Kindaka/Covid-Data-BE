using Quartz;
namespace ODataCovid.Services.Quartz;
public static class ServiceCollectionQuartzConfiguratorExtensions
{
    public static void AddJobAndTrigger<T>(
    this IServiceCollectionQuartzConfigurator quartz, IConfiguration config
    ) where T : IJob
    {
        string jobName = typeof(T).Name;
        var configKey = $"Quartz:{jobName}";
        DateTime now = DateTime.Now;
        int second = now.Second;
        int minute = now.Minute;
        int hour = now.Hour;
        int dayOfMonth = now.Day;
        int month = now.Month;
        string cronExpression = $"{second} {minute} {hour} {dayOfMonth} {month} ? *";

        //var cronSchedule = cronExpression;
        var cronSchedule = config[configKey];
        if (string.IsNullOrEmpty(cronSchedule))
        {
            throw new Exception($"No Quartz.Net Cron schedule found for job in configuration at {configKey}");
        }
        var jobKey = new JobKey(jobName);
        quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));
        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity(jobName + "--trigger")
            .WithCronSchedule(cronSchedule)
        );
    }
}

