using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace SonarScope.Library.Sensor
{
    public class UltrasonicDistanceSensor
    {
        private GpioPin Pin_Trig, Pin_Echo;
        Stopwatch sw = new Stopwatch();

        /// <summary>
        /// Available Gpio Pins. Refer: https://ms-iot.github.io/content/en-US/win10/samples/PinMappingsRPi2.htm
        /// </summary>
        public enum AvailableGpioPin : int
        {
            GpioPin_5 = 5,
            GpioPin_6 = 6,
            GpioPin_12 = 12,
            GpioPin_13 = 13,
            GpioPin_16 = 16,
            GpioPin_18 = 18,
            GpioPin_20 = 20,
            GpioPin_22 = 22,
            GpioPin_23 = 23,
            GpioPin_24 = 24,
            GpioPin_25 = 25,
            GpioPin_26 = 26,
            GpioPin_27 = 27
        }

        public UltrasonicDistanceSensor(AvailableGpioPin TrigPin, AvailableGpioPin EchoPin)
        {
            var gpio = GpioController.GetDefault();

            Pin_Trig = gpio.OpenPin((int)TrigPin);
            Pin_Trig.SetDriveMode(GpioPinDriveMode.Output);
            Pin_Trig.Write(GpioPinValue.Low);

            Pin_Echo = gpio.OpenPin((int)EchoPin);
            Pin_Echo.SetDriveMode(GpioPinDriveMode.Input);
        }

        public double GetDistance()
        {

            Pin_Trig.Write(GpioPinValue.Low);                     // ensure the trigger is off
            Task.Delay(TimeSpan.FromMilliseconds(1)).Wait();  // wait for the sensor to settle

            Pin_Trig.Write(GpioPinValue.High);                          // turn on the pulse
            Task.Delay(TimeSpan.FromMilliseconds(.01)).Wait();      // let the pulse run for 10 microseconds
            Pin_Trig.Write(GpioPinValue.Low);                           // turn off the pulse

            var time = PulseIn(Pin_Echo, GpioPinValue.High, 18); // allows for 308 cm distance

            // speed of sound is 34300 cm per second or 34.3 cm per millisecond
            // since the sound waves traveled to the obstacle and back to the sensor
            // I am dividing by 2 to represent travel time to the obstacle

            return time * 34.3 / 2.0; // at 20 degrees at sea level
        }

        private double PulseIn(GpioPin pin, GpioPinValue value, ushort timeout)
        {
            sw.Restart();

            // Wait for pulse
            while (sw.ElapsedMilliseconds < timeout && pin.Read() != value) {}

            if (sw.ElapsedMilliseconds >= timeout)
            {
                sw.Stop();
                return 0;
            } 
            sw.Restart();

            // Wait for pulse end
            while (sw.ElapsedMilliseconds < timeout && pin.Read() == value) { }

            sw.Stop();

            return sw.ElapsedMilliseconds < timeout ? sw.Elapsed.TotalMilliseconds : 0;
        }
        
    }
}
