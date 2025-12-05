using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ATRI.OneBot.Messages;

namespace ATRI.OneBot.JsonConverters;

public class MsgSegmentJsonConverter : JsonConverter<MsgSegment>
{
    public override MsgSegment? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader)!;
        string type = node["type"]!.GetValue<string>();
        var data = node["data"]!;
        if (data["type"] == null)
        {
            data["type"] = type;
            return type switch
            {
                "text" => data.Deserialize<PlainText>(options),
                "image" => data.Deserialize<ImageRecv>(options),
                "face" => data.Deserialize<Face>(options),
                _ => throw new NotSupportedException($"Unsupported MsgSegment type")
            };
        }
        throw new NotSupportedException($"Unsupported MsgSegment type");
    }

    public override void Write(Utf8JsonWriter writer, MsgSegment value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("type", value switch
        {
            PlainText => "text",
            Image => "image",
            Face => "face",
            _ => throw new NotSupportedException($"Unsupported MsgSegment type")
        });
        writer.WritePropertyName("data");
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
        writer.WriteEndObject();
    }
}