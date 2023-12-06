namespace Generator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Sensor[] sensors = new Sensor[16];
            int WheelSpeedSensorFrequency = 1;
            int DamperPositionSensorFrequency = 1;
            int RideHeightSensorFrequency = 1;
            int WheelTemperatureSensorFrequency = 1;

            for (int i = 0; i < 4; i++)
            {
                sensors[i] = new WheelSpeedSensor(i, WheelSpeedSensorFrequency, 0, 100);
                sensors[i+4] = new DamperPositionSensor(i, DamperPositionSensorFrequency,
                    0, 100);
                sensors[i+8] = new RideHeightSensor(i, RideHeightSensorFrequency, 0, 10);
                sensors[i+12] = new WheelTemperatureSensor(i, WheelTemperatureSensorFrequency,
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