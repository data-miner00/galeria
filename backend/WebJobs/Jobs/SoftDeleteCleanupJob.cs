using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebJobs.Jobs
{
    [DisallowConcurrentExecution]
    internal class SoftDeleteCleanupJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Console.Out.WriteLineAsync("Hello, world");
        }
    }
}
