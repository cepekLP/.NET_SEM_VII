using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    internal class WheelTemperatureSensor : Sensor
    {
        public int WheelTemperature;
        public WheelTemperatureSensor(int id, float dataFrequencyHz, int minValue, int maxValue, bool timerEnabled = true) :
            base(id, dataFrequencyHz, "WheelTemperature", minValue, maxValue, timerEnabled)
        {
        }

        protected override void GenerateData()
        {
            WheelTemperature = Random.Shared.Next(MinValue, MaxValue);
            PublishData();
        }

        protected override string GetDataAsString()
        {
            return WheelTemperature.ToString();
        }
    }
}
