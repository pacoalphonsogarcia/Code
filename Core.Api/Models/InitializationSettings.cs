using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Api.Models
{
    public class InitializationSettings
    {
        public bool ShouldInitializeDatabase { get; set; }
        public string CoreConnectionString { get; set; }
        public string DefaultAuthorizationScheme { get; set; }
    }
}
