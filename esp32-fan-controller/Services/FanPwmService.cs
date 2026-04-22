using System;
using System.Device.Pwm;
using nanoFramework.Hardware.Esp32;

namespace Esp32PwmFanController.Services
{
    public sealed class FanPwmService
    {
        private readonly PwmChannel _pwm;

        public FanPwmService()
        {
            Configuration.SetPinFunction(DeviceConfig.FanPwmPin, DeviceFunction.PWM1);

            _pwm = PwmChannel.CreateFromPin(
                DeviceConfig.FanPwmPin,
                DeviceConfig.PwmFrequencyHz,
                0.0);

            if (_pwm == null)
            {
                throw new Exception("Failed to create PWM channel.");
            }

            _pwm.Start();
        }

        public void SetSpeedPercent(int percent)
        {
            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;

            _pwm.DutyCycle = percent / 100.0;
        }

        public void Stop()
        {
            _pwm.DutyCycle = 0.0;
        }
    }
}