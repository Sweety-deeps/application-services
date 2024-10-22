using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class SingleOrArrayConverter<T> : JsonConverter<List<T>>
    {
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var list = new List<T>();

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                try
                {
                    list = JsonSerializer.Deserialize<List<T>>(ref reader); //currently we handle unexpected errors in the catch block due to deserialization failure. Need to change in future and aim for better solution.
                }
                catch(Exception ex)
                {                   
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            var errorDetails = new ErrorDetailsResponseModel
                            {
                                Message = reader.GetString()
                            };
                            list.Add((T)(object)errorDetails);
                        }
                        else if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            break;
                        }

                    }
                }
            }
            else
            {
                var item = JsonSerializer.Deserialize<T>(ref reader, options);
                list.Add(item);
            }

            return list;
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
