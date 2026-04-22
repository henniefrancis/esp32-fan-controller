//using System;
//using System.Threading;
//using System.Device.Adc;

//namespace Esp32Test
//{
//    public class Program
//    {
//        public static void Main()
//        {
//            var adc = new AdcController();

//            // GPIO35 → usually ADC1 channel 7
//            var pot = adc.OpenChannel(7);

//            int adcMax = (1 << adc.ResolutionInBits) - 1;

//            while (true)
//            {
//                int raw = pot.ReadValue();
//                int percent = (raw * 100) / adcMax;

//                System.Diagnostics.Debug.WriteLine(
//                    $"Raw={raw} | {percent}%"
//                );

//                Thread.Sleep(300);
//            }
//        }
//    }
//}