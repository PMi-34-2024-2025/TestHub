using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for ShowingResultWindow.xaml
    /// </summary>
    public partial class ShowingResultWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private readonly Ellipse _circle;
        private readonly Label _resultLabel;
        private double _progress;
        private readonly double _scorePercentage;

        public ShowingResultWindow(double totalScore, double maxScore) 
        {
            Title = "Результати тесту";
            Width = 400;
            Height = 400;
            Background = Brushes.White;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _scorePercentage = totalScore / maxScore * 100;
            Grid grid = new Grid();
            this.Content = grid;
            _circle = new Ellipse
            {
                Width = 200,
                Height = 200,
                Stroke = Brushes.Gray,
                StrokeThickness = 15,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            grid.Children.Add(_circle);
            _resultLabel = new Label
            {
                Content = "0%",
                FontSize = 28,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.Black
            };
            grid.Children.Add(_resultLabel);
            Color circleColor = GetResultColor(_scorePercentage);
            _circle.Stroke = new SolidColorBrush(circleColor);
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(20)
            };
            _timer.Tick += AnimateProgress;
            _timer.Start();
        }

        private void AnimateProgress(object? sender, EventArgs e)
        {
            if (_progress >= _scorePercentage)
            {
                _timer.Stop();
                _resultLabel.Content = $"{Math.Round(_scorePercentage, 1)}%";
            }
            else
            {
                _progress += 1;
                _resultLabel.Content = $"{Math.Round(_progress)}%";
                UpdateCircle(_progress);
            }
        }

        private void UpdateCircle(double progress)
        {
            double angle = progress / 100 * 360;
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                Point center = new Point(200, 200);
                Point start = new Point(200, 50);
                double radians = Math.PI * (angle - 90) / 180.0;
                Point end = new Point(
                    center.X + 150 * Math.Cos(radians),
                    center.Y + 150 * Math.Sin(radians)
                );

                context.BeginFigure(start, false, false);
                context.ArcTo(end, new Size(150, 150), 0, angle > 180, SweepDirection.Clockwise, true, false);
            }

            Path path = new Path
            {
                Data = geometry,
                Stroke = _circle.Stroke,
                StrokeThickness = _circle.StrokeThickness
            };
            if (!((Grid)this.Content).Children.Contains(path))
                ((Grid)this.Content).Children.Add(path);
        }

        private Color GetResultColor(double percentage)
        {
            if (percentage >= 80)
            {
                return Colors.Green;
            }
            else if (percentage >= 50)
            {
                return Colors.Orange;
            }
            else
            {
                return Colors.Red;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            new MainWindowWithTests().Show();
        }
    }
}
