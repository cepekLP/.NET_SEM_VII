namespace Generator
{
    public class Program
    {
        public static void Main(string[] args)
        {
           var sensor = new WheelSpeedSensor(1, 1, "WheelSpeed", 0, 100);

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}