using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvatodo.Models
{
    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "desc")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "complete")]
        public bool Completed { get; set; }
    }
}