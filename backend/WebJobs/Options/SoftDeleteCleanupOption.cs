using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WebJobs.Filters;

namespace WebJobs.Options
{
    internal class SoftDeleteCleanupOption
    {
        public const string SectionName = "SoftDeleteCleanupOption";

        /// <summary>
        /// Gets or sets the cron expression for the job.
        /// </summary>
        [Required]
        [CronExpression]
        required public string CronExpression { get; set; }

        /// <summary>
        /// Gets or sets the maximum timeout in seconds.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        required public int TimeoutInSeconds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this job should be enabled or not.
        /// </summary>
        public bool Enabled { get; set; }
    }
}
