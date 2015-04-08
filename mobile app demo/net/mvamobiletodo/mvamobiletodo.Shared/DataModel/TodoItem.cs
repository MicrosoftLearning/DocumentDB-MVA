using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace mvamobiletodo
{
    public class TodoItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "desc")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }
    }
}
