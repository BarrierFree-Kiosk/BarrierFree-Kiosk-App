using System.Windows;

namespace BarrierFree_Kiosk.WorkSpace.Sua
{
    public partial class MessagePopupWindow : Window
    {
        public MessagePopupWindow(string message)
        {
            InitializeComponent();
            TxtMessage.Text = message;
            Loaded += MessagePopupWindow_Loaded;
        }

        private void MessagePopupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var boundWidth = Owner?.ActualWidth > 0 ? Owner.ActualWidth : SystemParameters.WorkArea.Width;
            var boundHeight = Owner?.ActualHeight > 0 ? Owner.ActualHeight : SystemParameters.WorkArea.Height;
            var width = ActualWidth < boundWidth * 0.9 ? ActualWidth : boundWidth * 0.9;
            var height = ActualHeight < boundHeight * 0.9 ? ActualHeight : boundHeight * 0.9;

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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
