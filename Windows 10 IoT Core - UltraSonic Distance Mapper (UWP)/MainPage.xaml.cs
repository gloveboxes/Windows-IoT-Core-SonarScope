using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows_10_IoT_Core___UltraSonic_Distance_Mapper__UWP_.Library;
using Windows_10_IoT_Core___UltraSonic_Distance_Mapper__UWP_.Library.Communication;

namespace Windows_10_IoT_Core___UltraSonic_Distance_Mapper__UWP_
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const UInt16 NumberOfSample = 1;
        ArduinoGateway gw = new ArduinoGateway();

        public MainPage()
        {
            this.InitializeComponent();

            /* Initialize task and start it on different thread */
            Task Task1 = new Task(ScannerTask);
            Task1.Start();

        }

        public async void ScannerTask()
        {
            Library.Sensor.UltrasonicDistanceSensor DistanceSensor = new Library.Sensor.UltrasonicDistanceSensor(Library.Sensor.UltrasonicDistanceSensor.AvailableGpioPin.GpioPin_26, Library.Sensor.UltrasonicDistanceSensor.AvailableGpioPin.GpioPin_16);

            /*
                'CurrentRotation' Holds the current rotation
                ============|===========================|===========
                Description | Actual Data Feed To Servo | Assumption
                ============|===========================|===========
                Left Most   |   0                       | -70
                Center      |  70                       |   0
                Right Most  | 140                       | +70
            */
            byte CurrentRotation = 70;      // << HOLDS ACTUAL DATA TO BE FEED TO SERVO

            /*
                Holds direction to move
                +1 means scanner should move right
                -1 means scanner should move left
            */
            int Direction = 1;

            double Distance;
            byte Temp;

            /* Scan infinitely */
            while (true)
            {
                /*
                    Take multiple sample of the distance in current sight
                    Higher the sampling, accurate the result but will cause slower scanning.
                */
                Distance = 0;                                   // Reset Distance
                for (int i = 0; i < NumberOfSample; i++)
                {
                    Distance += DistanceSensor.GetDistance();   // Add distance
                    await Task.Delay(1);                        // Wait for a ms
                }
                Distance /= NumberOfSample;                     // Calculate average

                /*
                    Send current angle to Gateway to set servo
                    This project was developed to map object distances between -70 to +70 degree
                    Thus servo will move between:    00 - 70 - 140  (Actual Angle)
                                                    -70 -  0 - +70  (Assumed Angle)
                */
                Temp = (byte)((140 - CurrentRotation > 0) ? 140 - CurrentRotation : 0);

                /* Send angle to the gateway in a byte */
                /* A dummy byte will be returned from the function */
                gw.SetAngle(Temp);
                //      Temp = Library.Communication.ArduinoGateway.MoveServo(Temp).Result[0];

                /*
                    If right direction is fully mapped, change scanning direction to left.
                    Or else if left is fully scanned, change direction to right.
                */
                if (CurrentRotation > 140)
                {
                    Direction = -1;
                    CurrentRotation = (byte)(CurrentRotation + Direction);
                }
                else if (CurrentRotation == 0)
                {
                    Direction = 1;
                    CurrentRotation = (byte)(CurrentRotation + Direction);
                }


                CurrentRotation = (byte)(CurrentRotation + Direction);

                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    /* Convert current rotation from 0 to 140 into -70 to +70 */
                    ScannerLine.Angle = CurrentRotation - 70;

                    /* If all available angles has been scanned for a direction, clean LiDAR map */
                    if (ScannerLine.Angle == -70 || ScannerLine.Angle == 70)
                    {
                        Grid_Mapper.Children.Clear();
                    }

                    ///* Do not plot object > 500 cm */
                    //if (Distance > 350)
                    //{
                    //    Distance = 0;
                    //}

                    ///* LiDAR map developed for this project is only for up-to 300 cm. Thus eliminate higher distances. */
                    //if (Distance > 299)
                    //{
                    //    Distance = 299;
                    //}

                    /* Plot distance into LiDAR map */
                    if (Distance > 0 && Distance < 270)
                    {
                        Grid_Mapper.Children.Add(DistanceMapper.GetMapper(ScannerLine.Angle, (int)Distance));
                    }

                });

                /* Hold for 5ms to take breath entire system */
                await Task.Delay(5);
            }

        }
    }
}
