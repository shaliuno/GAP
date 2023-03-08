using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Stas.Utils {
    public class JSON {
        public class ValueTupleFactory : JsonConverterFactory {
            public override bool CanConvert(Type typeToConvert) {
                Type iTuple = typeToConvert.GetInterface("System.Runtime.CompilerServices.ITuple");
                return iTuple != null;
            }

            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
                Type[] genericArguments = typeToConvert.GetGenericArguments();

                Type converterType = genericArguments.Length switch {
                    1 => typeof(ValueTupleConverter<>).MakeGenericType(genericArguments),
                    2 => typeof(ValueTupleConverter<,>).MakeGenericType(genericArguments),
                    3 => typeof(ValueTupleConverter<,,>).MakeGenericType(genericArguments),
                    // And add other cases as needed
                    _ => throw new NotSupportedException(),
                };
                return (JsonConverter)Activator.CreateInstance(converterType);
            }
        }

        public class ValueTupleConverter<T1> : JsonConverter<ValueTuple<T1>> {
            public override ValueTuple<T1> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
                ValueTuple<T1> result = default;

                if (!reader.Read()) {
                    throw new JsonException();
                }

                while (reader.TokenType != JsonTokenType.EndObject) {
                    if (reader.ValueTextEquals("Item1") && reader.Read()) {
                        result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
                    }
                    else {
                        throw new JsonException();
                    }
                    reader.Read();
                }

                return result;
            }

            public override void Write(Utf8JsonWriter writer, ValueTuple<T1> value, JsonSerializerOptions options) {
                writer.WriteStartObject();
                writer.WritePropertyName("Item1");
                JsonSerializer.Serialize<T1>(writer, value.Item1, options);
                writer.WriteEndObject();
            }
        }

        public class ValueTupleConverter<T1, T2> : JsonConverter<ValueTuple<T1, T2>> {
            public override (T1, T2) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
                (T1, T2) result = default;

                if (!reader.Read()) {
                    throw new JsonException();
                }

                while (reader.TokenType != JsonTokenType.EndObject) {
                    if (reader.ValueTextEquals("Item1") && reader.Read()) {
                        result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
                    }
                    else if (reader.ValueTextEquals("Item2") && reader.Read()) {
                        result.Item2 = JsonSerializer.Deserialize<T2>(ref reader, options);
                    }
                    else {
                        throw new JsonException();
                    }
                    reader.Read();
                }

                return result;
            }

            public override void Write(Utf8JsonWriter writer, (T1, T2) value, JsonSerializerOptions options) {
                writer.WriteStartObject();
                writer.WritePropertyName("Item1");
                JsonSerializer.Serialize<T1>(writer, value.Item1, options);
                writer.WritePropertyName("Item2");
                JsonSerializer.Serialize<T2>(writer, value.Item2, options);
                writer.WriteEndObject();
            }
        }

        public class ValueTupleConverter<T1, T2, T3> : JsonConverter<ValueTuple<T1, T2, T3>> {
            public override (T1, T2, T3) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
                (T1, T2, T3) result = default;

                if (!reader.Read()) {
                    throw new JsonException();
                }

                while (reader.TokenType != JsonTokenType.EndObject) {
                    if (reader.ValueTextEquals("Item1") && reader.Read()) {
                        result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
                    }
                    else if (reader.ValueTextEquals("Item2") && reader.Read()) {
                        result.Item2 = JsonSerializer.Deserialize<T2>(ref reader, options);
                    }
                    else if (reader.ValueTextEquals("Item3") && reader.Read()) {
                        result.Item3 = JsonSerializer.Deserialize<T3>(ref reader, options);
                    }
                    else {
                        throw new JsonException();
                    }
                    reader.Read();
                }

                return result;
            }

            public override void Write(Utf8JsonWriter writer, (T1, T2, T3) value, JsonSerializerOptions options) {
                writer.WriteStartObject();
                writer.WritePropertyName("Item1");
                JsonSerializer.Serialize<T1>(writer, value.Item1, options);
                writer.WritePropertyName("Item2");
                JsonSerializer.Serialize<T2>(writer, value.Item2, options);
                writer.WritePropertyName("Item3");
                JsonSerializer.Serialize<T3>(writer, value.Item3, options);
                writer.WriteEndObject();
            }
        }

        public static T FromUTF8Byte<T>(byte[] inp, int start = 0) {
            var str = Encoding.UTF8.GetString(inp, start, inp.Length - start);
            return JsonSerializer.Deserialize<T>(str);
        }

        public static byte[] ToUT8Byte<T>(T t) {
            var opt = new JsonSerializerOptions {
                WriteIndented = true, 
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                IgnoreReadOnlyProperties = true,
                IncludeFields = false
            };
            // DateTime .ToString("yyyy-MM-dd HH:mm:ss");
            return JsonSerializer.SerializeToUtf8Bytes<object>(t, opt);
        }
        public static T FromZipByte<T>(byte[] zba) {
            var opt = new JsonSerializerOptions {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                IgnoreReadOnlyProperties = false,
                IncludeFields = false
            };
            if (zba == null) {
                Console.WriteLine("JSON.FromZipByte err: ba == null");
                return default(T);
            }
            else {
                var ba = ZIP.UnZip(zba);
                try {
                    return JsonSerializer.Deserialize<T>(ba, opt);
                }
                catch (Exception ex) {
                    Console.WriteLine("JSON.FromZipByte err:" + ex.Message);
                    return default(T);
                }
            }
        }
        public static byte[] ToZipByte<T>(T t) {
            try {
                var ba = ToUT8Byte(t);
                var zip = ZIP.ToZip(ba);
                return zip;
            }
            catch (Exception ex) {
                Console.WriteLine("JSON.ToZipByte ERR:" + ex.Message);
                return null;
            }
        }
    }
}
