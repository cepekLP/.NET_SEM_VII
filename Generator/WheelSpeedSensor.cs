using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace Generator
{
    internal class WheelSpeedSensor : Sensor
    {
        public float WheelSpeed { get; set; }
        public WheelSpeedSensor(int id, float dataFrequencyHz, int minValue, int maxValue, bool timerEnabled = true) :
            base(id, dataFrequencyHz, "WheelSpeed", minValue, maxValue, timerEnabled)
        {

        }

        protected override void GenerateData()
        {
            WheelSpeed = Random.Shared.Next(MinValue * 10, MaxValue * 10) / 10.0f;
            PublishData();
        }

        protected override string GetDataAsString()
        {
            return WheelSpeed.ToString();
        }
    }
}
