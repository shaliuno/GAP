using System.Text.Json.Serialization;
using System.Text.Json;
namespace Stas.Utils;
using V2 = System.Numerics.Vector2;

public class FILE {
    internal sealed class V2Converter : JsonConverter<V2> {
        public override V2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var str = reader.GetString();
            var fa = str.Split(",");
            return new V2(float.Parse(fa[0]), float.Parse(fa[1]));
        }

        public override void Write(Utf8JsonWriter writer, V2 value, JsonSerializerOptions options) {
            writer.WriteStringValue(value.X + ", " + value.Y);
        }
    }
    static JsonReaderOptions comment_opt = new JsonReaderOptions {
        CommentHandling = JsonCommentHandling.Allow
    };

    public static void SaveAsJson<T>(T t, string fname) {
        var opt = new JsonSerializerOptions {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReadCommentHandling = JsonCommentHandling.Skip,
            IgnoreReadOnlyProperties = false,
            IncludeFields = false
        };
        opt.Converters.Add(new V2Converter());
        var str = JsonSerializer.Serialize<object>(t, opt);
        str.SafelyWriteToFile(fname);
    }

    public static T LoadJson<T>(string fname, Action if_err = null) {
        var str = File.ReadAllText(fname);
        if (str.Length == 0) {
            if_err?.Invoke();
            return default(T);
        }
        try {
            var opt = new JsonSerializerOptions {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReadCommentHandling = JsonCommentHandling.Skip,
                IgnoreReadOnlyProperties = false,
                IncludeFields = false
            };
            opt.Converters.Add(new V2Converter());
            return JsonSerializer.Deserialize<T>(str, opt);
        }
        catch (Exception) {
            if_err?.Invoke();
            return default(T);
        }
    }
}
