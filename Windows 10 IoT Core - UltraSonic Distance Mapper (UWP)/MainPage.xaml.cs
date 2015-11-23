using SonarScope.Library.Communication;
using SonarScope.Library.Sensor;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace SonarScope
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        // This sample is geared towards servos with a 140 degree sweep.
        // Particularly the xaml UI

        /*
            'CurrentRotation' Holds the current rotation
            ============|===========================|===========
            Description | Actual Data Feed To Servo | Assumption
            ============|===========================|===========
            Left Most   |   0                       | -70
            Center      |  70                       |   0
            Right Most  | 140                       | +70

        */

        const int MinServoDegrees = 0;
        const int MaxServoDegrees = 140;
        const int TotalServoDegrees = MaxServoDegrees - MinServoDegrees;
        const int MidpointServoDegrees = TotalServoDegrees / 2;

        Stopwatch frameTimer = new Stopwatch();
        const int FrameTimeMilliseconds = 70;

        ArduinoGateway gw = new ArduinoGateway();
        UltrasonicDistanceSensor DistanceSensor = new UltrasonicDistanceSensor(UltrasonicDistanceSensor.AvailableGpioPin.GpioPin_20, UltrasonicDistanceSensor.AvailableGpioPin.GpioPin_22);


        public MainPage()
        {
            this.InitializeComponent();

            Task.Run(() => ScannerTask());
        }

        public async void ScannerTask()
        {
            int direction = 1;
            double distance;
            int currentAngle = MidpointServoDegrees;
            int nextAngle = 0;

            gw.SetServoAngle(MidpointServoDegrees);  // set servo midpoint ready for first distance measure
            await Task.Delay(500); // give servo enough time to get to rotation midpoint

            while (true)  // Scan infinitely
            {
                frameTimer.Restart();  // measures time to sense distance, move servo and update UI 

                distance = DistanceSensor.GetDistance();

                nextAngle = CalculateNextAngle(currentAngle, ref direction);

                MoveServo(nextAngle);  // more servo in readiness for next distance measurement       

                UpdateUI(currentAngle, distance);

                frameTimer.Stop();

                if (frameTimer.ElapsedMilliseconds < FrameTimeMilliseconds)
                {
                    // drive consisent sonar scan cadence 
                    await Task.Delay(FrameTimeMilliseconds - (int)frameTimer.ElapsedMilliseconds);
                }

                currentAngle = nextAngle;
            }
        }

        private int CalculateNextAngle(int currentAngle, ref int direction)
        {
            if (currentAngle >= MaxServoDegrees) { direction = -1; }
            else if (currentAngle <= MinServoDegrees) { direction = 1; }

            return currentAngle + direction;
        }

        private void MoveServo(int position)
        {
            int ServoAngle = (TotalServoDegrees - position > 0) ? TotalServoDegrees - position : 0;
            gw.SetServoAngle(ServoAngle);
        }

        private void UpdateUI(int currentAngle, double Distance)
        {
            var placeholder = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                /* Convert current rotation from 0 to 140 into -70 to +70 */
                ScannerLine.Angle = currentAngle - MidpointServoDegrees;

                RemoveExpiredPoints();

                /* Plot distance into LiDAR map */
                if (Distance > 0 && Distance < 270)
                {
                    Grid_Mapper.Children.Add(Library.DistanceMapper.GetMapper(ScannerLine.Angle, (int)Distance));
                }

            });
        }

        private void RemoveExpiredPoints()
        {
            if (ScannerLine.Angle == -MidpointServoDegrees || ScannerLine.Angle == MidpointServoDegrees)
            {
                foreach (var item in Grid_Mapper.Children)
                {
                    if ((item as Grid).Children.Count > 0)
                    {
                        var e = (item as Grid).Children[0];
                        if (e is Ellipse && e.Opacity <= 0)
                        {
                            Grid_Mapper.Children.Remove(item);
                        }
                    }
                }
            }
        }
    }
}
