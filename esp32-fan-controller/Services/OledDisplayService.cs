using System;
using System.Device.I2c;
using Iot.Device.Ssd13xx;
using nanoFramework.Hardware.Esp32;
using Esp32PwmFanController.Models;

namespace Esp32PwmFanController.Services
{
    public sealed class OledDisplayService
    {
        private readonly Ssd1306 _display;
        private readonly byte[] _buffer = new byte[128 * 64 / 8];

        public OledDisplayService()
        {
            Configuration.SetPinFunction(DeviceConfig.OledSdaPin, DeviceFunction.I2C2_DATA);
            Configuration.SetPinFunction(DeviceConfig.OledSclPin, DeviceFunction.I2C2_CLOCK);

            var settings = new I2cConnectionSettings(
                DeviceConfig.OledI2cBusId,
                DeviceConfig.OledI2cAddress,
                I2cBusSpeed.FastMode);

            _display = new Ssd1306(
                I2cDevice.Create(settings),
                Ssd13xx.DisplayResolution.OLED128x64);
        }

        public void ShowBoot()
        {
            Clear();
            DrawText(0, 0, "ESP32 FAN CTRL");
            DrawText(0, 12, "INIT...");
            Flush();
        }

        public void Render(FanStatus status)
        {
            Clear();

            DrawText(0, 0, status.ManualMode ? "MODE: MANUAL" : "MODE: AUTO");
            DrawText(0, 10, "TEMP:");
            DrawText(42, 10, Format1Dp(status.TemperatureC) + "C");

            DrawText(0, 22, "SPD:");
            DrawText(42, 22, status.SpeedPercent + "%");

            DrawText(0, 34, "RPM:");
            DrawText(42, 34, status.Rpm.ToString());

            DrawFrame(0, 46, 128, 10);
            DrawHBar(2, 48, 124, 6, status.SpeedPercent);

            if (status.FanFailure)
            {
                DrawText(0, 58, "FAIL");
            }
            else
            {
                DrawText(0, 58, "OK");
            }

            Flush();
        }

        private string Format1Dp(double value)
        {
            // Avoid format-string surprises on tiny frameworks
            int whole = (int)value;
            int tenth = (int)Math.Abs((value - whole) * 10.0);
            return whole + "." + tenth;
        }

        private void Clear()
        {
            Array.Clear(_buffer, 0, _buffer.Length);
        }

        private void Flush()
        {
            _display.DrawDirectAligned(0, 0, 128, 64, _buffer);
        }

        private void SetPixel(int x, int y, bool on = true)
        {
            if (x < 0 || x >= 128 || y < 0 || y >= 64) return;

            int index = x + (y / 8) * 128;
            byte bit = (byte)(1 << (y % 8));

            if (on)
            {
                _buffer[index] |= bit;
            }
            else
            {
                _buffer[index] &= (byte)~bit;
            }
        }

        private void DrawFrame(int x, int y, int w, int h)
        {
            for (int i = x; i < x + w; i++)
            {
                SetPixel(i, y);
                SetPixel(i, y + h - 1);
            }

            for (int i = y; i < y + h; i++)
            {
                SetPixel(x, i);
                SetPixel(x + w - 1, i);
            }
        }

        private void FillRect(int x, int y, int w, int h)
        {
            for (int px = x; px < x + w; px++)
            {
                for (int py = y; py < y + h; py++)
                {
                    SetPixel(px, py);
                }
            }
        }

        private void DrawHBar(int x, int y, int w, int h, int percent)
        {
            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;

            int fill = (w * percent) / 100;
            FillRect(x, y, fill, h);
        }

        private void DrawText(int x, int y, string text)
        {
            int cursorX = x;

            for (int i = 0; i < text.Length; i++)
            {
                DrawChar(cursorX, y, text[i]);
                cursorX += 6;
            }
        }

        private void DrawChar(int x, int y, char c)
        {
            byte[] glyph = GetGlyph(c);

            for (int col = 0; col < 5; col++)
            {
                byte line = glyph[col];
                for (int row = 0; row < 7; row++)
                {
                    if ((line & (1 << row)) != 0)
                    {
                        SetPixel(x + col, y + row);
                    }
                }
            }
        }

        private byte[] GetGlyph(char c)
        {
            switch (c.ToUpper())
            {
                case 'A': return new byte[] { 0x1E, 0x05, 0x05, 0x1E, 0x00 };
                case 'C': return new byte[] { 0x0E, 0x11, 0x11, 0x11, 0x00 };
                case 'D': return new byte[] { 0x1F, 0x11, 0x11, 0x0E, 0x00 };
                case 'E': return new byte[] { 0x1F, 0x15, 0x15, 0x11, 0x00 };
                case 'F': return new byte[] { 0x1F, 0x05, 0x05, 0x01, 0x00 };
                case 'I': return new byte[] { 0x11, 0x1F, 0x11, 0x00, 0x00 };
                case 'K': return new byte[] { 0x1F, 0x04, 0x0A, 0x11, 0x00 };
                case 'L': return new byte[] { 0x1F, 0x10, 0x10, 0x10, 0x00 };
                case 'M': return new byte[] { 0x1F, 0x02, 0x04, 0x02, 0x1F };
                case 'N': return new byte[] { 0x1F, 0x02, 0x04, 0x1F, 0x00 };
                case 'O': return new byte[] { 0x0E, 0x11, 0x11, 0x0E, 0x00 };
                case 'P': return new byte[] { 0x1F, 0x05, 0x05, 0x02, 0x00 };
                case 'R': return new byte[] { 0x1F, 0x05, 0x0D, 0x12, 0x00 };
                case 'S': return new byte[] { 0x12, 0x15, 0x15, 0x09, 0x00 };
                case 'T': return new byte[] { 0x01, 0x1F, 0x01, 0x00, 0x00 };
                case 'U': return new byte[] { 0x0F, 0x10, 0x10, 0x0F, 0x00 };
                case 'X': return new byte[] { 0x11, 0x0A, 0x04, 0x0A, 0x11 };
                case ':': return new byte[] { 0x00, 0x0A, 0x00, 0x00, 0x00 };
                case '.': return new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00 };
                case '%': return new byte[] { 0x19, 0x19, 0x04, 0x13, 0x13 };
                case ' ': return new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 };

                case '0': return new byte[] { 0x0E, 0x11, 0x11, 0x0E, 0x00 };
                case '1': return new byte[] { 0x12, 0x1F, 0x10, 0x00, 0x00 };
                case '2': return new byte[] { 0x19, 0x15, 0x15, 0x12, 0x00 };
                case '3': return new byte[] { 0x11, 0x15, 0x15, 0x0A, 0x00 };
                case '4': return new byte[] { 0x07, 0x04, 0x04, 0x1F, 0x00 };
                case '5': return new byte[] { 0x17, 0x15, 0x15, 0x09, 0x00 };
                case '6': return new byte[] { 0x0E, 0x15, 0x15, 0x08, 0x00 };
                case '7': return new byte[] { 0x01, 0x01, 0x1D, 0x03, 0x00 };
                case '8': return new byte[] { 0x0A, 0x15, 0x15, 0x0A, 0x00 };
                case '9': return new byte[] { 0x02, 0x15, 0x15, 0x0E, 0x00 };

                default: return new byte[] { 0x1F, 0x11, 0x11, 0x1F, 0x00 };
            }
        }
    }
}