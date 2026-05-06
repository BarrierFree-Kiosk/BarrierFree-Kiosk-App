using System.Windows;
using BarrierFree_Kiosk.Navigation;
using wpfpslib;

namespace BarrierFree_Kiosk.MainViews
{
    public partial class NavigationHostWindow : Window
    {
        private bool _isHighContrastEnabled;
        private bool _isZoomEnabled;

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

        private void OnHighContrastClick(object sender, RoutedEventArgs e)
        {
            _isHighContrastEnabled = !_isHighContrastEnabled;
            ApplyHighContrast(_isHighContrastEnabled);
            if (sender is System.Windows.Controls.Button button)
            {
                button.Content = _isHighContrastEnabled ? "고대비 해제" : "고대비";
            }
        }

        private void OnZoomClick(object sender, RoutedEventArgs e)
        {
            _isZoomEnabled = !_isZoomEnabled;
            double zoomScale = _isZoomEnabled ? 1.2 : 1.0;
            if (FindName("GlobalScaleTransform") is System.Windows.Media.ScaleTransform scaleTransform)
            {
                scaleTransform.ScaleX = zoomScale;
                scaleTransform.ScaleY = zoomScale;
            }

            if (sender is System.Windows.Controls.Button button)
            {
                button.Content = _isZoomEnabled ? "확대 해제" : "화면 확대";
            }
        }

        private void ApplyHighContrast(bool enable)
        {
            if (FindName("HostRoot") is System.Windows.Controls.Grid hostRoot)
            {
                hostRoot.Effect = enable
                    ? new InvertEffect { Amount = 1.0 }
                    : null;
            }

        }
    }
}
