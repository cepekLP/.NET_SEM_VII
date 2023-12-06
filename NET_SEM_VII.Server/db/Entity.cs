using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace NET_SEM_VII.Server.db
{
    public class Entity
    {
        [BsonId]
        [JsonIgnore]
        public ObjectId Id { get; set; }

        public DateTime Date { get; set; } = DateTime.MinValue;
        public string SensorType { get; set; } = null!;

        public string? SensorId { get; set; } = null!;

        public float Value { get; set; }



        override public string ToString()
        {
            return $@"{Date.ToString()}, {Value}, {SensorType}";
        }
        public string ToCSV()
        {
            return $@"{Date.ToString()}, {SensorType},{SensorId}, {Value}";
        }
    }
}
