using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NET_SEM_VII.Server
{
    public class Entity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public DateTime Date { get; set; } = DateTime.MinValue;

        public float Value { get; set; }

        public string SensorType { get; set; } = null!;

        public string? SensorId { get; set; } = null!;

        override public string ToString()
        {
            return $@"{Date.ToString()}, {Value}, {SensorType}";
        }
    }
}
