using System;
using System.Device.Gpio;

namespace Esp32PwmFanController.Services
{
    public sealed class TachometerService
    {
        private readonly GpioController _gpio;
        private long _pulseCount;
        private DateTime _lastSampleUtc;

        public TachometerService(GpioController gpio)
        {
            _gpio = gpio;
            _gpio.OpenPin(DeviceConfig.FanTachPin, PinMode.InputPullUp);
            _gpio.RegisterCallbackForPinValueChangedEvent(
                DeviceConfig.FanTachPin,
                PinEventTypes.Falling,
                OnTachPulse);

            _lastSampleUtc = DateTime.UtcNow;
        }

        private void OnTachPulse(object sender, PinValueChangedEventArgs e)
        {
            _pulseCount++;
        }

        public int ReadRpmIfReady()
        {
            var elapsed = DateTime.UtcNow - _lastSampleUtc;
            if (elapsed.TotalMilliseconds < DeviceConfig.TachSampleTimeMs)
            {
                return -1;
            }

            long pulses = _pulseCount;
            _pulseCount = 0;
            _lastSampleUtc = DateTime.UtcNow;

            // 2 pulses per revolution
            return (int)((pulses * 60000) / (DeviceConfig.TachSampleTimeMs * 2));
        }
    }
}