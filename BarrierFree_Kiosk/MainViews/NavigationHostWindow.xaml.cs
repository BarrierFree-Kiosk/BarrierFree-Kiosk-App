using System.Windows;
using BarrierFree_Kiosk.Navigation;

namespace BarrierFree_Kiosk.MainViews
{
    public partial class NavigationHostWindow : Window
    {
        public NavigationHostWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closed += OnClosed;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            KioskNavigator.Attach(RootFrame);
            KioskNavigator.Navigate(KioskPageId.Home);
        }

        private void OnClosed(object? sender, System.EventArgs e)
        {
            KioskNavigator.Detach(RootFrame);
        }
    }
}
