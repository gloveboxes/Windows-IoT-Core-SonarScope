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
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 29
            /// </summary>
            GpioPin_5 = 5,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 31
            /// </summary>
            GpioPin_6 = 6,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 32
            /// </summary>
            GpioPin_12 = 12,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 33
            /// </summary>
            GpioPin_13 = 13,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 36
            /// </summary>
            GpioPin_16 = 16,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 12
            /// </summary>
            GpioPin_18 = 18,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 15
            /// </summary>
            GpioPin_22 = 22,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 16
            /// </summary>
            GpioPin_23 = 23,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 18
            /// </summary>
            GpioPin_24 = 24,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 22
            /// </summary>
            GpioPin_25 = 25,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 37
            /// </summary>
            GpioPin_26 = 26,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 13
            /// </summary>
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

            var time = PulseIn(Pin_Echo, GpioPinValue.High, 20); // was 500ms

            // multiply by speed of sound in milliseconds (34000) divided by 2 (cause pulse make rountrip)
            var distance = time * 17164; // at 20 degrees at sea level
            return distance;
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

            return sw.ElapsedMilliseconds < timeout ? sw.Elapsed.TotalSeconds : 0;
        }
        
    }
}
