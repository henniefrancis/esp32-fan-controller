//using System.Device.Gpio;
//using System.Threading;

//public class Program
//{
//    private static GpioController s_GpioController;
//    public static void Main()
//    {
//        s_GpioController = new GpioController();

//        // ESP32 DevKit: 4 is a valid GPIO pin in, some boards like Xiuxin ESP32 may require GPIO Pin 2 instead.
//        GpioPin led = s_GpioController.OpenPin(
//        2,
//        PinMode.Output);

//        led.Write(PinValue.Low);

//        while (true)
//        {
//            led.Toggle();
//            Thread.Sleep(125);
//            led.Toggle();
//            Thread.Sleep(125);
//            led.Toggle();
//            Thread.Sleep(125);
//            led.Toggle();
//            Thread.Sleep(525);
//        }
//    }
//}