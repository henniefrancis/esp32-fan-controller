namespace Esp32PwmFanController
{
    public static class DeviceConfig
    {
        // GPIO map for your ESP-WROOM-32 board
        public const int FanPwmPin = 18;
        public const int FanTachPin = 27;
        public const int AlarmLedPin = 16;

        // ADC1 GPIOs chosen specifically to avoid ADC2/Wi-Fi conflicts
        public const int TempSensorGpio = 34;
        public const int PotentiometerGpio = 35;

        // I2C2 so we don't collide with PWM on GPIO18
        public const int OledSdaPin = 25;
        public const int OledSclPin = 26;
        public const int OledI2cBusId = 2;
        public const int OledI2cAddress = 0x3C;

        public const int PwmFrequencyHz = 25000;
        public const int TachSampleTimeMs = 2000;

        public const double TempFanOnC = 35.0;
        public const double TempFullSpeedC = 60.0;

        public const int MinRpmThreshold = 500;
        public const int MinSpeedForFailureCheckPercent = 20;

        // TMP36 formula:
        // temperatureC = (voltage - 0.5) * 100
        //
        // This project uses 3.3V as the practical scaling baseline.
        // ADC calibration can be added later if you want tighter accuracy.
        public const double AdcReferenceVoltage = 3.3;
    }
}