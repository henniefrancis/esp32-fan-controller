using System.Device.Adc;
using System.Device.Gpio;
using Esp32PwmFanController.Models;

namespace Esp32PwmFanController.Services
{
    public sealed class FanControllerService
    {
        private readonly GpioController _gpio;
        private readonly FanPwmService _fanPwm;
        private readonly TachometerService _tachometer;
        private readonly TemperatureSensorService _temperatureSensor;
        private readonly PotentiometerService _potentiometer;
        private readonly OledDisplayService _oled;

        private int _currentRpm;

        public FanControllerService(int tempAdcChannel, int potAdcChannel)
        {
            _gpio = new GpioController();
            _gpio.OpenPin(DeviceConfig.AlarmLedPin, PinMode.Output);

            _fanPwm = new FanPwmService();
            _tachometer = new TachometerService(_gpio);

            var adc = new AdcController();
            _temperatureSensor = new TemperatureSensorService(adc, tempAdcChannel);
            _potentiometer = new PotentiometerService(adc, potAdcChannel);

            _oled = new OledDisplayService();
            _oled.ShowBoot();
        }

        public FanStatus Update()
        {
            double tempC = _temperatureSensor.ReadTemperatureC();
            int potPercent = _potentiometer.ReadPercent();
            bool manualMode = potPercent > 3;

            int speedPercent;

            if (manualMode)
            {
                speedPercent = potPercent;
            }
            else
            {
                if (tempC < DeviceConfig.TempFanOnC)
                {
                    speedPercent = 0;
                }
                else if (tempC >= DeviceConfig.TempFullSpeedC)
                {
                    speedPercent = 100;
                }
                else
                {
                    double range = DeviceConfig.TempFullSpeedC - DeviceConfig.TempFanOnC;
                    double normalized = (tempC - DeviceConfig.TempFanOnC) / range;

                    speedPercent = 30 + (int)(normalized * 70.0);
                }
            }

            if (speedPercent < 0) speedPercent = 0;
            if (speedPercent > 100) speedPercent = 100;

            int rpmMaybe = _tachometer.ReadRpmIfReady();
            if (rpmMaybe >= 0)
            {
                _currentRpm = rpmMaybe;
            }

            bool fanFailure =
                speedPercent > DeviceConfig.MinSpeedForFailureCheckPercent &&
                _currentRpm < DeviceConfig.MinRpmThreshold;

            if (fanFailure)
            {
                _fanPwm.Stop();
                _gpio.Write(DeviceConfig.AlarmLedPin, PinValue.High);
            }
            else
            {
                _fanPwm.SetSpeedPercent(speedPercent);
                _gpio.Write(DeviceConfig.AlarmLedPin, PinValue.Low);
            }

            var status = new FanStatus
            {
                TemperatureC = tempC,
                PotPercent = potPercent,
                SpeedPercent = speedPercent,
                Rpm = _currentRpm,
                ManualMode = manualMode,
                FanFailure = fanFailure
            };

            _oled.Render(status);
            return status;
        }
    }
}