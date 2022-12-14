﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using ServerConnection;
//
//    var welcome = Welcome.FromJson(jsonString);

namespace ServerConnection
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Welcome
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("allPurchasedItems")]
        public AllPurchasedItems AllPurchasedItems { get; set; }
    }

    public partial class AllPurchasedItems
    {
        [JsonProperty("nodes")]
        public List<Node> Nodes { get; set; }
    }

    public partial class Node
    {
        [JsonProperty("sockets")]
        public object Sockets { get; set; }

        [JsonProperty("z")]
        public object Z { get; set; }

        [JsonProperty("x")]
        public object X { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("uid")]
        public Guid Uid { get; set; }

        [JsonProperty("owner")]
        public Guid Owner { get; set; }
    }

    public partial class Welcome
    {
        public static Welcome FromJson(string json) => JsonConvert.DeserializeObject<Welcome>(json, ServerConnection.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Welcome self) => JsonConvert.SerializeObject(self, ServerConnection.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
