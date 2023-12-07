namespace Generator
{
    public class Program
    {
        const float DataPerMinuteToHz = 1.0f / 60.0f;
        public static void Main(string[] args)
        {
            Sensor[] sensors = new Sensor[16];
            float WheelSpeedSensorFrequency = 1f;
            float DamperPositionSensorFrequency = 1f;
            float RideHeightSensorFrequency = 1f;
            float WheelTemperatureSensorFrequency = 0.25f;

            for (int i = 0; i < 4; i++)
            {
                sensors[i] = new WheelSpeedSensor(i, WheelSpeedSensorFrequency, 0, 100);
                sensors[i + 4] = new DamperPositionSensor(i, DamperPositionSensorFrequency,
                    0, 100);
                sensors[i + 8] = new RideHeightSensor(i, RideHeightSensorFrequency, 0, 10);
                sensors[i + 12] = new WheelTemperatureSensor(i, WheelTemperatureSensorFrequency,
                                       0, 150);
            }

            /* create one sensor with specific value */
            /*
            var sensor = new WheelSpeedSensor(0, 1, 0, 100, false);
            sensor.WheelSpeed = 50;
            sensor.PublishData();
            */

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}