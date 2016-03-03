using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace SonarScope.Arduino {
    class ArduinoBridge : IDisposable {

        enum ArduinoCmd : byte {
            InitialiseNeoPixel = (byte)'I',     //Initialize NeoPixel
            SetNeoPixel = (byte)'P',               //set a NeoPixel pixel value
            DisplayNeoPixel = (byte)'D',         //render NeoPixel pixels to the display
            ServoMovePosition = (byte)'M'           //set Servo position
        }


        byte[] ArdunioCmdPacket = new byte[6];  // if length changed, must be reflected in the Arduino code



        class ArduinoPin {
            private byte pinNumber;
            public bool PinSet { get; private set; }
            public bool Initialised { get; set; } = false;

            public byte PinNumber {
                get { return pinNumber; }
                set {
                    pinNumber = value;
                    PinSet = true;
                }
            }
        }


        ArduinoPin neoPixel = new ArduinoPin();
        ArduinoPin servo = new ArduinoPin();


        public byte NeoPixelPin { get { return neoPixel.PinNumber; } set { neoPixel.PinNumber = value; } }
        public byte ServoPin { get { return servo.PinNumber; } set { servo.PinNumber = value; } }



        public int I2C_ADDRESS { get; set; } = 64;

        public static bool IsInitialised { get; private set; } = false;

        private static I2cDevice I2CDevice;
        private static object DeviceLock = new object();

        public string I2cControllerName { get; set; } = "I2C1";  /* For Raspberry Pi 2, use I2C1 */




        private async Task EnsureInitializedAsync() {
            if (IsInitialised) { return; }

            try {
                var settings = new I2cConnectionSettings(I2C_ADDRESS);
                settings.BusSpeed = I2cBusSpeed.StandardMode;

                string aqs = I2cDevice.GetDeviceSelector(I2cControllerName);  /* Find the selector string for the I2C bus controller                   */
                var dis = await DeviceInformation.FindAllAsync(aqs);            /* Find the I2C bus controller device with our selector string           */
                I2CDevice = await I2cDevice.FromIdAsync(dis[0].Id, settings);    /* Create an I2cDevice with our selected bus controller and I2C settings */


                IsInitialised = true;
            }
            catch (Exception ex) {
                throw new Exception("I2C Initialization Failed", ex);
            }
        }

        private void InitNeoPixel(int length) {
            if (neoPixel.PinSet) {
                ArdunioCmdPacket[0] = (byte)ArduinoCmd.InitialiseNeoPixel;
                ArdunioCmdPacket[1] = (byte)(length >> 8);
                ArdunioCmdPacket[2] = (byte)(length);
                ArdunioCmdPacket[3] = neoPixel.PinNumber;
                WriteI2cPacket(ArdunioCmdPacket);

                neoPixel.Initialised = true;
            }
        }

        private void InitialiseI2c() {
            if (!IsInitialised) { EnsureInitializedAsync().Wait(); }
        }

        public void FrameDraw(Pixel[] frame, int delayMilliseconds) {
            if (neoPixel.PinSet) {
                InitialiseI2c();
                if (!neoPixel.Initialised) { InitNeoPixel(frame.Length); }

                for (int i = 0; i < frame.Length; i++) {
                    ArdunioCmdPacket[0] = (byte)ArduinoCmd.SetNeoPixel;  //Set Pixel
                    ArdunioCmdPacket[1] = (byte)(i >> 8);
                    ArdunioCmdPacket[2] = (byte)(i);
                    ArdunioCmdPacket[3] = frame[i].Red;
                    ArdunioCmdPacket[4] = frame[i].Green;
                    ArdunioCmdPacket[5] = frame[i].Blue;

                    I2cTransferResult result = I2CDevice.WritePartial(ArdunioCmdPacket);
                }

                ArdunioCmdPacket[0] = (byte)ArduinoCmd.DisplayNeoPixel; // Render Frame
                ArdunioCmdPacket[1] = (byte)(frame.Length >> 8);
                ArdunioCmdPacket[2] = (byte)(frame.Length);
                ArdunioCmdPacket[3] = neoPixel.PinNumber;
                WriteI2cPacket(ArdunioCmdPacket);

                Task.Delay(delayMilliseconds).Wait();
            }
            else {
                throw new Exception("NeoPixel pin number not set");
            }
        }

        public double GetDistance() {
            byte[] result = new byte[1];

            lock (DeviceLock) {
                I2cTransferResult resultShow = I2CDevice.ReadPartial(result);
            }

            return result[0];
        }

        public void ServoPosition(ushort position) {
            if (servo.PinSet) {
                InitialiseI2c();

                ArdunioCmdPacket[0] = (byte)ArduinoCmd.ServoMovePosition; // Render Frame
                ArdunioCmdPacket[1] = (byte)(position >> 8);
                ArdunioCmdPacket[2] = (byte)(position);
                ArdunioCmdPacket[3] = servo.PinNumber;

                WriteI2cPacket(ArdunioCmdPacket);
            }
            else {
                throw new Exception("Servo pin number not set");
            }
        }



        private void WriteI2cPacket(byte[] data) {
            lock (DeviceLock) {
                I2cTransferResult resultShow = I2CDevice.WritePartial(data);
            }
        }

        public void Dispose() {
            I2CDevice.Dispose();
        }
    }
}
