using System.Device.Adc;

namespace Esp32PwmFanController.Services
{
    public sealed class PotentiometerService
    {
        private readonly AdcChannel _channel;
        private readonly int _adcMax;

        public PotentiometerService(AdcController adc, int channelNumber)
        {
            _channel = adc.OpenChannel(channelNumber);
            _adcMax = (1 << adc.ResolutionInBits) - 1;
        }

        public int ReadPercent()
        {
            int raw = _channel.ReadValue();
            int percent = (raw * 100) / _adcMax;

            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;

            return percent;
        }

        public bool IsManualOverrideActive()
        {
            return ReadPercent() > 3;
        }
    }
}