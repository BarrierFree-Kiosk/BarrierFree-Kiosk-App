using System;
using System.Windows;
using System.Windows.Threading;

namespace BarrierFree_Kiosk.WorkSpace.Sua
{
    public partial class CardInsertWindow : Window
    {
        private readonly DispatcherTimer _progressTimer;

        public CardInsertWindow()
        {
            InitializeComponent();

            _progressTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(350)
            };
            _progressTimer.Tick += ProgressTimer_Tick;
            Loaded += CardInsertWindow_Loaded;
            Closed += CardInsertWindow_Closed;
        }

        private void CardInsertWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LockWindowSize();
            _progressTimer.Start();
        }

        private void LockWindowSize()
        {
            var boundWidth = Owner?.ActualWidth > 0 ? Owner.ActualWidth : SystemParameters.WorkArea.Width;
            var boundHeight = Owner?.ActualHeight > 0 ? Owner.ActualHeight : SystemParameters.WorkArea.Height;
            var width = Math.Min(ActualWidth, boundWidth * 0.96);
            var height = Math.Min(ActualHeight, boundHeight * 0.96);

            SizeToContent = SizeToContent.Manual;
            Width = width;
            Height = height;
            MinWidth = width;
            MaxWidth = width;
            MinHeight = height;
            MaxHeight = height;

            if (Owner is not null)
            {
                Left = Owner.Left + (Owner.ActualWidth - Width) / 2;
                Top = Owner.Top + (Owner.ActualHeight - Height) / 2;
            }
        }

        private void CardInsertWindow_Closed(object? sender, EventArgs e)
        {
            _progressTimer.Stop();
        }

        private void ProgressTimer_Tick(object? sender, EventArgs e)
        {
            if (ConnectionProgress.Value < 100)
            {
                ConnectionProgress.Value += 20;
            }
            else
            {
                _progressTimer.Stop();
                TxtConnectionStatus.Text = "카드 단말기 연결 완료. 카드를 삽입해 주세요.";
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
