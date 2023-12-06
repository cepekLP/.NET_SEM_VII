using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{

    internal class DamperPositionSensor : Sensor
    {
        public int DamperPosition;
        public DamperPositionSensor(int id, int dataFrequencyHz, int minValue, int maxValue, bool timerEnabled = true) :
            base(id, dataFrequencyHz, "DamperPosition", minValue, maxValue, timerEnabled)
        {
        }

        protected override void GenerateData()
        {
            DamperPosition = Random.Shared.Next(MinValue, MaxValue);
            PublishData();
        }

        protected override string GetDataAsString()
        {
            return DamperPosition.ToString();
        }
    }
}
