using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SundihomeApp.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class FirebaseNotificationModel
    {
        public string to { get; set; }
        public string[] registration_ids { get; set; }
        public string collapse_key { get; set; } = "type_a";
        public string priority { get; set; } = "high";
        public FirebaseNotification notification { get; set; }
        public Dictionary<string, object> data { get; set; }
    }
    public class FirebaseNotification
    {
        public string title { get; set; }
        public string body { get; set; }
        public int badge { get; set; }
        public string sound { get; set; } = "Default";
        public bool content_available { get; set; }
    }
}
