using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Base : Resource
    {
        [JsonProperty(PropertyName = "tenantId")]
        public string TenantId { get; set; }
    }
}
