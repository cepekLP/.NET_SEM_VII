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
        private MqttFactory? mqttFactory;
        private IMqttClient? mqttClient;
        public float WheelSpeed { get; set; }
        public WheelSpeedSensor(int id, int dataFrequencyHz, String type, int minValue, int maxValue) :
            base(id, dataFrequencyHz, type, minValue, maxValue)
        {
            ConnectToBroker();
        }

        public override void GenerateData()
        {
            WheelSpeed = Random.Shared.Next(MinValue * 10, MaxValue * 10) / 10.0f;
            PublishData();
        }
        private async void ConnectToBroker()
        {
            mqttFactory = new MqttFactory();

            mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1").Build();

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        }

        private async void PublishData()
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("mqttnet/samples/topic/1")
                .WithPayload("Hello World")
                .Build();

            if (mqttClient != null && mqttClient.IsConnected)
            {
                mqttClient.PublishAsync(message, CancellationToken.None);
                Console.WriteLine("MQTT application message is published.");
            }
            else
            {
                Console.WriteLine("MQTT application message is not published.");
            }
        }
    }
}
