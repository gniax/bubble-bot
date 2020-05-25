using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BubbleBot.Views.Accounts.MapViewer
{
    public partial class JsonMap
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("topNeighbourId")]
        public long TopNeighbourId { get; set; }

        [JsonProperty("bottomNeighbourId")]
        public long BottomNeighbourId { get; set; }

        [JsonProperty("leftNeighbourId")]
        public long LeftNeighbourId { get; set; }

        [JsonProperty("rightNeighbourId")]
        public long RightNeighbourId { get; set; }

        [JsonProperty("shadowBonusOnEntities")]
        public long ShadowBonusOnEntities { get; set; }

        [JsonProperty("cells")]
        public List<Cell> Cells { get; set; }

        [JsonProperty("midgroundLayer")]
        public Dictionary<string, List<MidgroundLayer>> MidgroundLayer { get; set; }

        [JsonProperty("atlasLayout")]
        public AtlasLayout AtlasLayout { get; set; }

        [JsonProperty("foreground", NullValueHandling = NullValueHandling.Ignore)]
        public object Foreground { get; set; }

    }

    public partial class AtlasLayout
    {
        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("graphicsPositions")]
        public Dictionary<string, GraphicsPosition> GraphicsPositions { get; set; }
    }

    public partial class GraphicsPosition
    {
        [JsonProperty("sx")]
        public long Sx { get; set; }

        [JsonProperty("sy")]
        public long Sy { get; set; }

        [JsonProperty("sw")]
        public long Sw { get; set; }

        [JsonProperty("sh")]
        public long Sh { get; set; }

        [JsonProperty("cy", NullValueHandling = NullValueHandling.Ignore)]
        public long? Cy { get; set; }

        [JsonProperty("cw", NullValueHandling = NullValueHandling.Ignore)]
        public long? Cw { get; set; }

        [JsonProperty("ch", NullValueHandling = NullValueHandling.Ignore)]
        public long? Ch { get; set; }
    }

    public partial class Cell
    {
        [JsonProperty("l", NullValueHandling = NullValueHandling.Ignore)]
        public long? L { get; set; }

        [JsonProperty("c", NullValueHandling = NullValueHandling.Ignore)]
        public long? C { get; set; }
    }

    public partial class MidgroundLayer
    {
        [JsonProperty("g", NullValueHandling = NullValueHandling.Ignore)]
        public long? G { get; set; }

        [JsonProperty("x")]
        public float X { get; set; }

        [JsonProperty("y")]
        public float Y { get; set; }

        [JsonProperty("hue")]
        public List<long> Hue { get; set; }

        [JsonProperty("cw", NullValueHandling = NullValueHandling.Ignore)]
        public long? Cw { get; set; }

        [JsonProperty("ch", NullValueHandling = NullValueHandling.Ignore)]
        public long? Ch { get; set; }

        [JsonProperty("sx", NullValueHandling = NullValueHandling.Ignore)]
        public float? Sx { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("cy", NullValueHandling = NullValueHandling.Ignore)]
        public double? Cy { get; set; }

        [JsonProperty("look", NullValueHandling = NullValueHandling.Ignore)]
        public long? Look { get; set; }

        [JsonProperty("cx", NullValueHandling = NullValueHandling.Ignore)]
        public double? Cx { get; set; }

        [JsonProperty("sy", NullValueHandling = NullValueHandling.Ignore)]
        public float? Sy { get; set; }

        [JsonProperty("rotation", NullValueHandling = NullValueHandling.Ignore)]
        public double? Rotation { get; set; }
    }
}
