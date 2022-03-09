using System;
using System.Diagnostics;
using System.Windows;

namespace AutoShutdown
{
    public partial class MainWindow : Window
    {
        private DateTime _endTime = new DateTime();
        private System.Windows.Threading.DispatcherTimer _dispatcherTimer;
        private string _initialTitle = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            _initialTitle = Title;
        }

        private void StartOrStop_Click(object sender, RoutedEventArgs e)
        {
            if (_dispatcherTimer == null)
                Start();
            else
                Stop();
        }

        private void Start()
        {
            try
            {
                var text = int.Parse(MinutesOrHours.Text);

                if (TimeType.Text == "Seconds")
                    _endTime = DateTime.UtcNow + TimeSpan.FromSeconds(text);
                else if (TimeType.Text == "Minutes")
                    _endTime = DateTime.UtcNow + TimeSpan.FromMinutes(text);
                else
                    _endTime = DateTime.UtcNow + TimeSpan.FromHours(text);

                _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                _dispatcherTimer.Tick += OnTick;
                _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                _dispatcherTimer.Start();

                StartOrStop.Content = "Stop";
                InputContainer.Visibility = Visibility.Collapsed;
                Remaining.Visibility = Visibility.Visible;
                UpdateTimeLeft(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (DateTime.UtcNow < _endTime)
            {
                UpdateTimeLeft(DateTime.UtcNow);
                return;
            }

            Process.Start("shutdown", "/s /t 0");
        }

        private void Stop()
        {
            _dispatcherTimer?.Stop();
            _dispatcherTimer = null;
            _endTime = default;

            Title = _initialTitle;
            StartOrStop.Content = "Start";
            InputContainer.Visibility = Visibility.Visible;
            Remaining.Visibility = Visibility.Collapsed;
        }

        private void UpdateTimeLeft(DateTime now)
        {
            Remaining.Text = FormatRemainingTime(_endTime - now);
            Title = $"{_initialTitle} - {Remaining.Text}";
        }

        private string FormatRemainingTime(TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0)
                return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            if (timeSpan.Hours > 0)
                return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }
}
