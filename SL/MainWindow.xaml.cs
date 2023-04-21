using System;
using System.Windows;
using System.Windows.Threading;

namespace SL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AutoCloseWindowAfterSpecificDelayInSecond(1d * 60);
        }
        private void AutoCloseWindowAfterSpecificDelayInSecond(double delayInSec)
        {
            DispatcherTimer dispatcherTimer = null;
            TimeSpan _time = TimeSpan.FromSeconds(delayInSec);
            dispatcherTimer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                remainingTimeToAutoCloseInSecond.Text = _time.ToString("c");
                if (_time == TimeSpan.Zero)
                {
                    dispatcherTimer.Stop();
                    Close();
                }
                _time = _time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            dispatcherTimer.Start();
        }
    }
}
