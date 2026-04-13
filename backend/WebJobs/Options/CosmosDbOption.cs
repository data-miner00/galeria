using System;
using System.Collections.Generic;
using System.Text;

namespace WebJobs.Options
{
    internal class CosmosDbOption
    {
        public const string SectionName = "CosmosDbOption";

        public string ConnectionString { get; set; }
    }
}
