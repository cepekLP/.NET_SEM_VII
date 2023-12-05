using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    internal class Sensor
    {
        public int Id { get; set; }
        public int DataFrequencyHz { get; set; }
        public String Type { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        private System.Timers.Timer timer;
        public Sensor(int id, int dataFrequencyHz, String type, int minValue, int maxValue)
        {
            this.Id = id;
            this.DataFrequencyHz = dataFrequencyHz;
            this.Type = type;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            timer = new System.Timers.Timer(1000 / dataFrequencyHz);
            timer.Elapsed += (sender, e) => GenerateData();
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public virtual void GenerateData() { }
    }
}
