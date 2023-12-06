using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    internal class RideHeightSensor : Sensor
    {
        public float RideHeight;
        public RideHeightSensor(int id, float dataFrequencyHz, int minValue, int maxValue, bool timerEnabled = true) :
            base(id, dataFrequencyHz, "RideHeight", minValue, maxValue, timerEnabled)
        {
        }

        protected override void GenerateData()
        {
            RideHeight = Random.Shared.Next(MinValue*100, MaxValue*100) /100.0f;
            PublishData();
        }

        protected override string GetDataAsString()
        {
            return RideHeight.ToString();
        }
    }
}
