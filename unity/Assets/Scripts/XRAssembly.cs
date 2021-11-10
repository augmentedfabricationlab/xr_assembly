namespace XRAssembly
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Assembly
    {
        [JsonProperty("dea")]
        public Dea Dea { get; set; }

        [JsonProperty("adjacency")]
        public Dictionary<string, Dictionary<string, Adjacency>> Adjacency { get; set; }

        [JsonProperty("node")]
        public Dictionary<string, Node> Node { get; set; }

        [JsonProperty("max_node")]
        public long MaxNode { get; set; }

        [JsonProperty("attributes")]
        public AssemblyAttributes Attributes { get; set; }

        [JsonProperty("dna")]
        public Dna Dna { get; set; }

        [JsonProperty("edge")]
        public Dictionary<string, Dictionary<string, Adjacency>> Edge { get; set; }
    }

    public partial class Adjacency
    {
        [JsonProperty("interface_size")]
        public double InterfaceSize { get; set; }

        [JsonProperty("interface_uvw")]
        public double[][] InterfaceUvw { get; set; }

        [JsonProperty("interface_points")]
        public double[][] InterfacePoints { get; set; }

        [JsonProperty("interface_origin")]
        public double[] InterfaceOrigin { get; set; }

        [JsonProperty("interface_type")]
        public InterfaceType InterfaceType { get; set; }
    }

    public partial class AssemblyAttributes
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("robot_base_frame")]
        public RobotBaseFrame RobotBaseFrame { get; set; }
    }

    public partial class RobotBaseFrame
    {
        [JsonProperty("yaxis")]
        public double[] Yaxis { get; set; }

        [JsonProperty("xaxis")]
        public double[] Xaxis { get; set; }

        [JsonProperty("point")]
        public double[] Point { get; set; }
    }

    public partial class Dea
    {
    }

    public partial class Dna
    {
        [JsonProperty("is_built")]
        public bool IsBuilt { get; set; }

        [JsonProperty("custom_attr_3")]
        public object CustomAttr3 { get; set; }

        [JsonProperty("custom_attr_2")]
        public object CustomAttr2 { get; set; }

        [JsonProperty("custom_attr_1")]
        public object CustomAttr1 { get; set; }

        [JsonProperty("is_placed")]
        public bool IsPlaced { get; set; }

        [JsonProperty("is_planned")]
        public bool IsPlanned { get; set; }

        [JsonProperty("y")]
        public long Y { get; set; }

        [JsonProperty("x")]
        public long X { get; set; }

        [JsonProperty("z")]
        public long Z { get; set; }

        [JsonProperty("idx_v")]
        public object IdxV { get; set; }
    }

    public partial class Node
    {
        [JsonProperty("z")]
        public double Z { get; set; }

        [JsonProperty("idx_v")]
        public long IdxV { get; set; }

        [JsonProperty("is_built")]
        public bool IsBuilt { get; set; }

        [JsonProperty("element")]
        public Element Element { get; set; }

        [JsonProperty("typology")]
        public Typology Typology { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("course")]
        public long Course { get; set; }
    }

    public partial class Element
    {
        [JsonProperty("_tool_frame")]
        public RobotBaseFrame ToolFrame { get; set; }

        [JsonProperty("frame")]
        public RobotBaseFrame Frame { get; set; }

        [JsonProperty("_mesh")]
        public Mesh Mesh { get; set; }

        [JsonProperty("path")]
        public RobotBaseFrame[] Path { get; set; }

        [JsonProperty("_source")]
        public Source Source { get; set; }
    }

    public partial class Mesh
    {
        [JsonProperty("dea")]
        public Dea Dea { get; set; }

        [JsonProperty("dfa")]
        public Dea Dfa { get; set; }

        [JsonProperty("vertex")]
        public Dictionary<string, Dva> Vertex { get; set; }

        [JsonProperty("max_face")]
        public long MaxFace { get; set; }

        [JsonProperty("face")]
        public Dictionary<string, long[]> Face { get; set; }

        [JsonProperty("facedata")]
        public Dictionary<string, Dea> Facedata { get; set; }

        [JsonProperty("attributes")]
        public MeshAttributes Attributes { get; set; }

        [JsonProperty("edgedata")]
        public Dea Edgedata { get; set; }

        [JsonProperty("max_vertex")]
        public long MaxVertex { get; set; }

        [JsonProperty("dva")]
        public Dva Dva { get; set; }
    }

    public partial class MeshAttributes
    {
        [JsonProperty("name")]
        public Name Name { get; set; }
    }

    public partial class Dva
    {
        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("z")]
        public double Z { get; set; }
    }

    public partial class Source
    {
        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("dtype")]
        public Dtype Dtype { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("ysize")]
        public double Ysize { get; set; }

        [JsonProperty("xsize")]
        public double Xsize { get; set; }

        [JsonProperty("frame")]
        public RobotBaseFrame Frame { get; set; }

        [JsonProperty("zsize")]
        public double Zsize { get; set; }
    }

    public enum InterfaceType { FaceFace };

    public enum Name { Box };

    public enum Dtype { CompasGeometryShapesBoxBox };

    public enum Typology { Full, Half };


    public partial class Assembly
    {
        public static Assembly FromJson(string json) => JsonConvert.DeserializeObject<Assembly>(json, XRAssembly.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Assembly self) => JsonConvert.SerializeObject(self, XRAssembly.Converter.Settings);

    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                InterfaceTypeConverter.Singleton,
                NameConverter.Singleton,
                DtypeConverter.Singleton,
                TypologyConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class InterfaceTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(InterfaceType) || t == typeof(InterfaceType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "face_face")
            {
                return InterfaceType.FaceFace;
            }
            throw new Exception("Cannot unmarshal type InterfaceType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (InterfaceType)untypedValue;
            if (value == InterfaceType.FaceFace)
            {
                serializer.Serialize(writer, "face_face");
                return;
            }
            throw new Exception("Cannot marshal type InterfaceType");
        }

        public static readonly InterfaceTypeConverter Singleton = new InterfaceTypeConverter();
    }

    internal class NameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Name) || t == typeof(Name?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "Box")
            {
                return Name.Box;
            }
            throw new Exception("Cannot unmarshal type Name");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Name)untypedValue;
            if (value == Name.Box)
            {
                serializer.Serialize(writer, "Box");
                return;
            }
            throw new Exception("Cannot marshal type Name");
        }

        public static readonly NameConverter Singleton = new NameConverter();
    }

    internal class DtypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Dtype) || t == typeof(Dtype?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "compas.geometry.shapes.box/Box")
            {
                return Dtype.CompasGeometryShapesBoxBox;
            }
            throw new Exception("Cannot unmarshal type Dtype");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Dtype)untypedValue;
            if (value == Dtype.CompasGeometryShapesBoxBox)
            {
                serializer.Serialize(writer, "compas.geometry.shapes.box/Box");
                return;
            }
            throw new Exception("Cannot marshal type Dtype");
        }

        public static readonly DtypeConverter Singleton = new DtypeConverter();
    }

    internal class TypologyConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Typology) || t == typeof(Typology?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "full":
                    return Typology.Full;
                case "half":
                    return Typology.Half;
            }
            throw new Exception("Cannot unmarshal type Typology");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Typology)untypedValue;
            switch (value)
            {
                case Typology.Full:
                    serializer.Serialize(writer, "full");
                    return;
                case Typology.Half:
                    serializer.Serialize(writer, "half");
                    return;
            }
            throw new Exception("Cannot marshal type Typology");
        }

        public static readonly TypologyConverter Singleton = new TypologyConverter();
    }
}
