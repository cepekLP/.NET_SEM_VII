using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Packets;
using MQTTnet.Protocol;



namespace NET_SEM_VII.Server.Controllers
{
    public class MQTTController
    {
       public static async Task Init(string[] args)
        {

            SubscribeTopics();
        }

        private Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs x)
        {
            string topic = x.ApplicationMessage.Topic;
            string receiveMsg = x.ApplicationMessage.ConvertPayloadToString();

            Console.WriteLine($"Topic: {topic}. Message Received: {receiveMsg}");
            //...

            return Task.CompletedTask;
        }
        public static async void SubscribeTopics()
        {
            /*
             * This sample subscribes to several topics in a single request.
             */

            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1").Build();

                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                // Create the subscribe options including several topics with different options.
                // It is also possible to all of these topics using a dedicated call of _SubscribeAsync_ per topic.
                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(
                        f =>
                        {
                            f.WithTopic("mqttnet/samples/topic/1");
                        })
                    .WithTopicFilter(
                        f =>
                        {
                            f.WithTopic("mqttnet/samples/topic/2").WithNoLocal();
                        })
                    .WithTopicFilter(
                        f =>
                        {
                            f.WithTopic("mqttnet/samples/topic/3").WithRetainHandling(MqttRetainHandling.SendAtSubscribe);
                        })
                    .Build();

                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                    Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                    Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                    Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                    Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                    Console.WriteLine();

                    return Task.CompletedTask;
                };  
                var response = await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
               
                Console.WriteLine("MQTT client subscribed to topics.");

              
                      // The response contains additional data sent by the server after subscribing.
                      // response.DumpToConsole();
            }
        }

    }
}
