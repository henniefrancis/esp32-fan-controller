namespace Esp32PwmFanController.Models
{
    public class FanStatus
    {
        public double TemperatureC { get; set; }
        public int PotPercent { get; set; }
        public int SpeedPercent { get; set; }
        public int Rpm { get; set; }
        public bool ManualMode { get; set; }
        public bool FanFailure { get; set; }
    }
}