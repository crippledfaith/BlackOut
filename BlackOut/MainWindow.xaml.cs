using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace BlackOut
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer hideCorsorTimer = new Timer(5000);
        Timer showCorsorTimer = new Timer(500);
        private bool enableShowTimer = true;
        public MainWindow()
        {
            InitializeComponent();
            this.MouseMove += MainWindowMouseMove;
            hideCorsorTimer.Elapsed += HideTimerElapsed;
            hideCorsorTimer.Start();
            showCorsorTimer.Elapsed += ShowTimerElapsed;
            showCorsorTimer.Stop();
        }

        private void ShowTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            showCorsorTimer.Stop();
            Application.Current.Dispatcher.Invoke(() =>
            {
                enableShowTimer = true;
            });
        }

        private void HideTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            hideCorsorTimer.Stop();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Cursor = Cursors.None;
                TipTextBlock.Visibility = Visibility.Hidden;
                enableShowTimer = false;
                showCorsorTimer.Start();
            });
        }

        private void MainWindowMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (enableShowTimer)
            {
                Cursor = Cursors.Arrow;
                TipTextBlock.Visibility = Visibility.Visible;
                hideCorsorTimer.Start();
            }

        }
    }
}
