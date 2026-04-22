using System.Device.Adc;

namespace Esp32PwmFanController.Services
{
    public sealed class TemperatureSensorService
    {
        private readonly AdcChannel _channel;
        private readonly int _adcMax;

        public TemperatureSensorService(AdcController adc, int channelNumber)
        {
            _channel = adc.OpenChannel(channelNumber);
            _adcMax = (1 << adc.ResolutionInBits) - 1;
        }

        public double ReadTemperatureC()
        {
            int raw = _channel.ReadValue();
            double voltage = (raw * DeviceConfig.AdcReferenceVoltage) / _adcMax;

            // TMP36: 500mV offset, 10mV / degree C
            return (voltage - 0.5) * 100.0;
        }
    }
}