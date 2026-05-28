using System.Windows.Controls;

namespace BarrierFree_Kiosk.MainViews
{
    /// <summary>
    /// HomeView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HomeView : Page
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void MediaElement_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            MainVideo.Position = System.TimeSpan.Zero;
            MainVideo.Play();
        }
    }
}
