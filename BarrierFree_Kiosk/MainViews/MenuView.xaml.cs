using System.Windows;
using System.Windows.Controls;
using BarrierFree_Kiosk.WorkSpace.Sua;

namespace BarrierFree_Kiosk.MainViews
{
    public partial class MenuView : Page
    {
        public MenuView()
        {
            InitializeComponent();
        }

        private void MediaElement_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            MainVideo.Position = System.TimeSpan.Zero;
            MainVideo.Play();
        }

        private void BtnCardSelect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void BtnCart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void BtnPayment_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var hostWindow = Window.GetWindow(this);
            var popup = new PaymentPopupWindow();

            if (hostWindow is not null)
            {
                popup.Owner = hostWindow;
            }

            popup.ShowDialog();
        }
    }
}
