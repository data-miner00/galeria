namespace WebJobs;

using Autofac;
using Autofac.Configuration;
using WebJobs.Jobs;
using WebJobs.Models;
using WebJobs.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Microsoft.Azure.Cosmos;

/// <summary>
/// The IoC container configurations. Registers all dependencies of the program.
/// </summary>
internal static class ContainerConfig
{
    /// <summary>
    /// Registers the dependencies accordingly.
    /// </summary>
    /// <returns>The IoC container.</returns>
    public static IContainer Configure()
    {
        var builder = new ContainerBuilder();

        builder
            .RegisterOptions()
            .RegisterRepositories()
            .RegisterDatabaseConnection()
            .RegisterJobs();

        return builder.Build();
    }

    private static ContainerBuilder RegisterOptions(this ContainerBuilder builder)
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonFile("settings.json");

        var config = configBuilder.Build();

        var module = new ConfigurationModule(config);

        builder.RegisterModule(module);

        var databaseOption = config
            .GetSection(CosmosDbOption.SectionName)
            .Get<CosmosDbOption>();

        var softDeleteCleanupOption = config
            .GetSection(SoftDeleteCleanupOption.SectionName)
            .Get<SoftDeleteCleanupOption>();

        ArgumentNullException.ThrowIfNull(databaseOption);
        ArgumentNullException.ThrowIfNull(softDeleteCleanupOption);

        builder.RegisterInstance(databaseOption);
        builder.RegisterInstance(softDeleteCleanupOption);

        return builder;
    }

    private static ContainerBuilder RegisterDatabaseConnection(this ContainerBuilder builder)
    {
        builder
            .Register(ctx =>
            {
                var option = ctx.Resolve<CosmosDbOption>();

                var connection = new CosmosClient(option.ConnectionString);

                return connection;
            })
            .AsSelf()
            .SingleInstance();

        return builder;
    }

    private static ContainerBuilder RegisterJobs(this ContainerBuilder builder)
    {
        builder
            .Register(ctx =>
            {
                var softDeleteCleanupOption = ctx.Resolve<SoftDeleteCleanupOption>();

                var jobDescriptors = new List<JobDescriptor>
                {
                    new()
                    {
                        JobType = typeof(SoftDeleteCleanupJob),
                        Description = "Regularly cleanup the stale soft-deleted image records.",
                        CronExpression = softDeleteCleanupOption.CronExpression,
                        Enabled = softDeleteCleanupOption.Enabled,
                    },
                };

                return jobDescriptors;
            })
            .As<IEnumerable<JobDescriptor>>()
            .SingleInstance();

        var defaultScheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();

        builder.RegisterType<JobFactory>().As<IJobFactory>();
        builder.RegisterType<SoftDeleteCleanupJob>().SingleInstance();
        builder.RegisterInstance(defaultScheduler).As<IScheduler>().SingleInstance();
        builder.RegisterType<JobScheduler>().SingleInstance();
        builder.RegisterType<WebJobService>().As<IHostedService>().SingleInstance();

        return builder;
    }

    private static ContainerBuilder RegisterRepositories(this ContainerBuilder builder)
    {
        // CosmosDbRepo
        // BlobStorage client
        // Meilisearch client

        return builder;
    }
}
