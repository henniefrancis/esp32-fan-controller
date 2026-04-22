using System;
using System.Threading;
using Iot.Device.DHTxx.Esp32;

namespace Esp32Test
{
    public class Program
    {
        public static void Main()
        {
            const int pinEcho = 12;
            const int pinTrigger = 14;

            using var dht = new Dht11(pinEcho, pinTrigger);

            while (true)
            {
                var temperature = dht.Temperature;
                var humidity = dht.Humidity;

                if (dht.IsLastReadSuccessful)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Temp=" + temperature.DegreesCelsius.ToString("F1") +
                        "C | Humidity=" + humidity.Percent.ToString("F0") + "%");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("DHT11 read failed");
                }

                Thread.Sleep(3000);
            }
        }
    }
}