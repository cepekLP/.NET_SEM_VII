using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using NET_SEM_VII.Server.db;



namespace NET_SEM_VII.Server.Controllers
{
    public class MQTTController
    {
        MqttFactory? mqttFactory;
        IMqttClient? mqttClient;
        SensorsService sensorsService;

        public event Func<String, Task>? ApplicationMessageReceivedAsync;
        public MQTTController()
        {
            SubscribeTopics();
            sensorsService = new SensorsService();
            ApplicationMessageReceivedAsync = null;
        }

        public async void SubscribeTopics()
        {
            /*
             * This sample subscribes to several topics in a single request.
             */

            mqttFactory = new MqttFactory();

            mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("localhost").Build();

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            // Create the subscribe options including several topics with different options.
            // It is also possible to all of these topics using a dedicated call of _SubscribeAsync_ per topic.
            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("mqttnet/WheelSpeed");
                    })
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("mqttnet/DamperPosition");
                    })
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("mqttnet/RideHeight");
                    })
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("mqttnet/WheelTemperature");
                    })
                .Build();

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var entity = new Entity();
                string[] data = e.ApplicationMessage.ConvertPayloadToString().Split(";");
                if(data.Length != 3)
                {
                    Console.WriteLine($@"DataLenghts == {data.Length}");
                    return Task.CompletedTask;
                }
                entity.SensorId = data[0];
                entity.Value = float.Parse(data[1].Replace(",", "."));                
                entity.SensorType = e.ApplicationMessage.Topic.Split("/").Last();
                entity.Date = DateTime.Now;

                sensorsService.SaveEntity(entity);
                if (ApplicationMessageReceivedAsync != null)
                {
                    ApplicationMessageReceivedAsync(entity.SensorType+","+entity.SensorId);
                }
                //Console.WriteLine(entity.ToString());

                return Task.CompletedTask;
            };
            var response = await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Console.WriteLine("MQTT client subscribed to topics.");


            // The response contains additional data sent by the server after subscribing.
            // response.DumpToConsole();

        }

    }
}
