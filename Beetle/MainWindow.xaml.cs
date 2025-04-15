using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Beetle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Kever? _beetle;     
        Point? _startPoint;
        
        Random _rand = new Random();
        DispatcherTimer _moveTimer;
        DateTime _startTime;
        
        
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            _moveTimer = new DispatcherTimer();
            
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)        //[ChatGPT tip to workaround NullReferenceExceptions]
        {
            speedLbl.Content = $"{speedSlider.Value}";
            sizeLbl.Content = $"{sizeSlider.Value}";
        }

        private void _moveTimer_Tick(object? sender, EventArgs e)
        {
            _beetle.AutoMove(paperCanvas);
            UpdateBeetle();
        }

        private void speedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (speedLbl != null) { speedLbl.Content = $"{speedSlider.Value}"; }
        }

        private void sizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sizeLbl != null) { sizeLbl.Content = $"{sizeSlider.Value}"; }
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            if (startBtn.Content.ToString() == "Start")
            {
                outputLbl.Content = String.Empty ;

                if (_beetle == null)
                {
                    GenerateStartSpot();
                    _beetle = new Kever(_startPoint.Value.X, _startPoint.Value.Y, speedSlider.Value, sizeSlider.Value);
                    _beetle.VisualizeBeetle(paperCanvas);
                }

                double stepPerSecond = _beetle.BeetleSpeed / (_beetle.BeetleSize * 0.01);   // [ChatGPT]
                double intervalMs = 1000 / stepPerSecond;

                _moveTimer.Interval = TimeSpan.FromMilliseconds(intervalMs);
                _moveTimer.Tick -= _moveTimer_Tick;     //[ChatGPT: dubbele koppeling voorkomen]
                _moveTimer.Tick += _moveTimer_Tick;
                _moveTimer.Start();

                _startTime = DateTime.Now;

                startBtn.Content = "Stop";
            }
            else if (startBtn.Content.ToString() == "Stop")
            {
                _moveTimer.Stop();

                var timeMoved = DateTime.Now - _startTime;

                outputLbl.Content = $"Distance covered: {_beetle.TotalDistanceCovered:0.00}\n" +
                    $"Time passed: {timeMoved.TotalSeconds:0.00} seconds";

                startBtn.Content = "Start";
            }
            
        }
        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            if (startBtn.Content.ToString() == "Start")         //Workaround nullReferenceException when AutoMove() is running
            {
                _beetle = null;
                _startPoint = null;
                paperCanvas.Children.Clear();
                outputLbl.Content = string.Empty;
                speedSlider.Value = speedSlider.Minimum;        //eventHandler ValueChanged is triggered and will change labelcontent
                sizeSlider.Value = sizeSlider.Minimum;
            } 
            else
            {
                outputLbl.Content = "Reset is only possible when paused";
            }
            
        }

        private void upBtn_Click(object sender, RoutedEventArgs e)
        {
            _beetle.MoveBeetle("up", paperCanvas);
            UpdateBeetle();
        }

        private void rightBtn_Click(object sender, RoutedEventArgs e)
        {
            _beetle.MoveBeetle("right", paperCanvas);
            UpdateBeetle();
        }

        private void downBtn_Click(object sender, RoutedEventArgs e)
        {
            _beetle.MoveBeetle("down", paperCanvas);
            UpdateBeetle() ;
        }

        private void leftBtn_Click(object sender, RoutedEventArgs e)
        {
            _beetle.MoveBeetle("left", paperCanvas);
            UpdateBeetle();
        }
        private void UpdateBeetle()
        {
            paperCanvas.Children.Clear();
            _beetle.VisualizeBeetle(paperCanvas);
        }
        private void GenerateStartSpot()
        {
            _startPoint = null;
            int centerX = (int)paperCanvas.Width / 2;
            int centerY = (int)paperCanvas.Height / 2;

            while (_startPoint == null)
            {
                int tryX = _rand.Next(30, (int)paperCanvas.Width - 30);       //Generate startspot
                int tryY = _rand.Next(30, (int)paperCanvas.Height - 30);

                if (CalcDistance(tryX, centerX, tryY, centerY) >= 100) 
                {
                    _startPoint = new Point(tryX,tryY); 
                }
            }
        }


        //Only needed for _startPoint as _beetle.TotalDistanceCovered tracks all steps taken
        
        private double CalcDistance(double x1, double x2, double y1, double y2)
        { 
            return Math.Sqrt(Math.Pow(x1 - x2,2) + Math.Pow(y1 - y2,2));
        }

    }
}