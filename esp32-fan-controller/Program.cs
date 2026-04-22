using System;
using System.Threading;
using Esp32PwmFanController.Services;

namespace Esp32PwmFanController
{
    public class Program
    {
        public static void Main()
        {
            Debug("ESP32 PWM Fan Controller starting...");

            // IMPORTANT:
            // In nanoFramework on ESP32, ADC channel numbers can differ from raw GPIO numbers.
            // Start by using the channels that correspond to your board image/runtime mapping.
            //
            // For many ESP32 nanoFramework setups:
            // ADC1 channel 6 -> GPIO34
            // ADC1 channel 7 -> GPIO35
            //
            // If your readings are wrong, this is the first thing to adjust.
            const int tempAdcChannel = 6;
            const int potAdcChannel = 7;

            var controller = new FanControllerService(tempAdcChannel, potAdcChannel);

            while (true)
            {
                var status = controller.Update();

                Debug(
                    "Temp=" + status.TemperatureC.ToString("F1") + "C" +
                    " | Pot=" + status.PotPercent + "%" +
                    " | Speed=" + status.SpeedPercent + "%" +
                    " | RPM=" + status.Rpm +
                    " | Mode=" + (status.ManualMode ? "MANUAL" : "AUTO") +
                    " | Failure=" + (status.FanFailure ? "YES" : "NO"));

                Thread.Sleep(200);
            }
        }

        private static void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine("[FanCtrl] " + message);
        }
    }
}