using MQTTnet.Client;
using MQTTnet;
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
        protected MqttFactory? mqttFactory;
        protected IMqttClient? mqttClient;
        private System.Timers.Timer timer;
        public Sensor(int id, int dataFrequencyHz, String type, int minValue, int maxValue, bool timerEnabled = true)
        {
            this.Id = id;
            this.DataFrequencyHz = dataFrequencyHz;
            this.Type = type;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            timer = new System.Timers.Timer(1000 / dataFrequencyHz);
            timer.Elapsed += (sender, e) => GenerateData();
            timer.AutoReset = true;
            timer.Enabled = timerEnabled;

            ConnectToBroker();
        }

        protected virtual void GenerateData() { }

        private async void ConnectToBroker()
        {
            mqttFactory = new MqttFactory();

            mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1").Build();

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        }

        public async void PublishData()
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("mqttnet/" + Type)
                .WithPayload(Id.ToString() + ";" + GetDataAsString() + ";")
                .Build();

            if (mqttClient != null && mqttClient.IsConnected)
            {
                await mqttClient.PublishAsync(message, CancellationToken.None);
                // Console.WriteLine("MQTT application message is published.");
            }
            else
            {
                // Console.WriteLine("MQTT application message is not published.");
            }
        }

        protected virtual String GetDataAsString()
        {
            return "";
        }
    }
}
