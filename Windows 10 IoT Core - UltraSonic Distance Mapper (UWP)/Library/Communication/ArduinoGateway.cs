/*
    THIS GATEWAY CODE IS PROVIDED FOR SAMPLE PURPOSE ONLY.
    THIS CODE IS SPECIALLY DEVELOPED FOR THIS PROJECT ONLY.

    I have already developed gateway module that is available with project:
    https://www.hackster.io/AnuragVasanwala/windows-10-iot-core-hydroflyer


*/

// replaced by dglover@microsoft.com
using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace SonarScope.Library.Communication
{
    public class ArduinoGateway
    {

        public int I2C_ADDRESS { get; set; } = 64;
        
        public static bool IsInitialised { get; private set; } = false;

        private static I2cDevice I2CDevice;

        public string I2cControllerName { get; set; } = "I2C1";  /* For Raspberry Pi 2, use I2C1 */


        public void Initialise()
        {
            if (!IsInitialised)
            {
                EnsureInitializedAsync().Wait();
            }
        }

        private async Task EnsureInitializedAsync()
        {
            if (IsInitialised) { return; }

            try
            {
                var settings = new I2cConnectionSettings(I2C_ADDRESS);
                settings.BusSpeed = I2cBusSpeed.FastMode;

                string aqs = I2cDevice.GetDeviceSelector(I2cControllerName);  /* Find the selector string for the I2C bus controller                   */
                var dis = await DeviceInformation.FindAllAsync(aqs);            /* Find the I2C bus controller device with our selector string           */
                I2CDevice = await I2cDevice.FromIdAsync(dis[0].Id, settings);    /* Create an I2cDevice with our selected bus controller and I2C settings */


                IsInitialised = true;
            }
            catch (Exception ex)
            {
                throw new Exception("I2C Initialization Failed", ex);
            }
        }

        public void SetAngle(byte angle)
        {
            Initialise();
            I2CDevice.Write(new byte[] { angle });

        }
    }
}
