using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Windows_10_IoT_Core___UltraSonic_Distance_Mapper__UWP_.Library
{
    public static class DistanceMapper
    {
        private static Grid _Grid;
        private static Ellipse _Ellipse;
        private static Line _Line;
        private static CompositeTransform _CompositeTransform;
        private static RowDefinition _RowDefination;

        /// <summary>
        /// Returns a grid containing line with exact length and in the rotation specified for LiDAR Map.
        /// </summary>
        /// <param name="Angle">Angle to be map</param>
        /// <param name="Distance">Distance to be map on specified angle</param>
        /// <returns>Returns the grid that contain line and an ellipse that will exactly of length 'Distance' on specified 'Angle'.</returns>
        public static Grid GetMapper(double Angle, int Distance)
        {
            /* Create a new composite transform for Rotation and set angle */
            _CompositeTransform = new CompositeTransform();
            _CompositeTransform.Rotation = Angle;
            
            /* Crete new grid object and apply transformation */
            _Grid = new Grid();
            _Grid.RenderTransform = _CompositeTransform;

            /* <RowDefination Height="Auto"/> */
            _RowDefination = new RowDefinition();
            _RowDefination.Height = GridLength.Auto;
            _Grid.RowDefinitions.Add(_RowDefination);

            /* <RowDefination Height="*"/> */
            _RowDefination = new RowDefinition();
            _Grid.RowDefinitions.Add(_RowDefination);

            _Grid.HorizontalAlignment = HorizontalAlignment.Center;
            _Grid.VerticalAlignment = VerticalAlignment.Bottom;
            
            _Grid.RenderTransformOrigin = new Windows.Foundation.Point(0, 1);

            int size = Distance / 10;

            /* Ellipse is the point that will be mapped on the specified distance from the origin */
            _Ellipse = new Ellipse();
            _Ellipse.Height = size;
            _Ellipse.Width = size;
            _Ellipse.RenderTransformOrigin = new Windows.Foundation.Point(0, 0);
            _Ellipse.Margin = new Thickness(-size, 0, -size, 0);
            
            /* Apply different color for different region like < 50cm will be red and so on */
            if(Distance < 50)
            {
                _Ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }
            else if(Distance < 100)
            {
                _Ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, 200, 100, 0));
            }
            else
            {
                _Ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
            }

            /* Add ellipse in Grid.Row=0 & Grid.Column=0 */
            _Grid.Children.Add(_Ellipse);

            /* Create new object of line*/
            _Line = new Line();

            /* Add line in Grid.Row=1 & Grid.Column=0 */
            Grid.SetRow(_Line, 1);

            /* Specify line starting and ending point */
            _Line.X1 = 0;
            _Line.X2 = 0;
            _Line.Y1 = Distance + 3;
            _Line.Y2 = 0;

            /* Line thickness. Set to ZERO if not needed */
            _Line.StrokeThickness = 0;
            _Line.Stroke = _Ellipse.Fill;

            /* Add line to grid created above */
            _Grid.Children.Add(_Line);

            /* Return the complete grid back so that it will be plotted */
            return _Grid;
        }

        /*
            Equivalent Xaml Code To Above Function:
            <Grid HorizontalAlignment="Center" VerticalAlignment="Bottom" Width="Auto" Height="Auto" RenderTransformOrigin="0,1">
                    <Grid.RenderTransform>
                        <CompositeTransform x:Name="ObjectA_Angle" Rotation="20"/>
                    </Grid.RenderTransform>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto"/>
                        <RowDefinition/>
                    </Grid.RowDefinitions>
                    <Ellipse Name="ObjectA_Color" Height="3" Width="3" Fill="#0F0" RenderTransformOrigin="0,0" Margin="-3,0,-3,0"/>
                    <Line Grid.Row="1" Name="ObjectA_Distance" X1="0" X2="0" Y1="253" Y2="0" Stroke="#0000" StrokeThickness="2"/>
            </Grid>
        */

    }
}
