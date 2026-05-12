using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using BarrierFree_Kiosk.Navigation;
using wpfpslib;

namespace BarrierFree_Kiosk.MainViews
{
    public partial class NavigationHostWindow : Window
    {
        private const double ZoomContentScale = 1.35;

        private bool _isHighContrastEnabled;
        private bool _isZoomEnabled;
        private bool _isPanDragging;
        private Point _lastPanMouse;

        public NavigationHostWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closed += OnClosed;
            SizeChanged += OnWindowSizeChanged;
            PanZoomContainer.SizeChanged += OnPanZoomContainerSizeChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            KioskNavigator.Attach(RootFrame);
            KioskNavigator.Navigate(KioskPageId.Home);
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            KioskNavigator.Detach(RootFrame);
            EndPanDrag(releaseCapture: true);
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClampPanIfZoomed();
        }

        private void OnPanZoomContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClampPanIfZoomed();
        }

        private void OnHighContrastClick(object sender, RoutedEventArgs e)
        {
            _isHighContrastEnabled = !_isHighContrastEnabled;
            ApplyHighContrast(_isHighContrastEnabled);
            if (sender is Button button)
            {
                button.Content = _isHighContrastEnabled ? "고대비 해제" : "고대비";
            }
        }

        private void OnZoomClick(object sender, RoutedEventArgs e)
        {
            _isZoomEnabled = !_isZoomEnabled;
            EndPanDrag(releaseCapture: true);

            ContentZoomScale.ScaleX = _isZoomEnabled ? ZoomContentScale : 1.0;
            ContentZoomScale.ScaleY = _isZoomEnabled ? ZoomContentScale : 1.0;
            ContentPanTranslate.X = 0;
            ContentPanTranslate.Y = 0;

            ZoomViewport.Cursor = _isZoomEnabled ? Cursors.Hand : Cursors.Arrow;

            if (sender is Button button)
            {
                button.Content = _isZoomEnabled ? "확대 해제" : "화면 확대";
            }

            ClampPanIfZoomed();
        }

        private void OnZoomPanPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isZoomEnabled)
            {
                return;
            }

            if (!ShouldStartPanFromSource(e.OriginalSource as DependencyObject))
            {
                return;
            }

            _isPanDragging = true;
            _lastPanMouse = e.GetPosition(ZoomViewport);
            ZoomViewport.CaptureMouse();
            e.Handled = true;
        }

        private void OnZoomPanPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EndPanDrag(releaseCapture: true);
        }

        private void OnZoomPanPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isZoomEnabled || !_isPanDragging || !ZoomViewport.IsMouseCaptured)
            {
                return;
            }

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                EndPanDrag(releaseCapture: true);
                return;
            }

            var pos = e.GetPosition(ZoomViewport);
            var dx = pos.X - _lastPanMouse.X;
            var dy = pos.Y - _lastPanMouse.Y;
            _lastPanMouse = pos;

            ContentPanTranslate.X += dx;
            ContentPanTranslate.Y += dy;
            ClampPan();
        }

        private void OnZoomPanMouseLeave(object sender, MouseEventArgs e)
        {
            EndPanDrag(releaseCapture: true);
        }

        private static bool ShouldStartPanFromSource(DependencyObject? source)
        {
            var current = source;
            while (current != null)
            {
                switch (current)
                {
                    case ButtonBase:
                    case TextBoxBase:
                    case Slider:
                    case ComboBox:
                    case ListBoxItem:
                        return false;
                }

                current = VisualTreeHelper.GetParent(current)
                          ?? LogicalTreeHelper.GetParent(current) as DependencyObject;
            }

            return true;
        }

        private void EndPanDrag(bool releaseCapture)
        {
            _isPanDragging = false;
            if (releaseCapture && ZoomViewport.IsMouseCaptured)
            {
                ZoomViewport.ReleaseMouseCapture();
            }
        }

        private void ClampPanIfZoomed()
        {
            if (_isZoomEnabled)
            {
                ClampPan();
            }
        }

        private void ClampPan()
        {
            var s = ContentZoomScale.ScaleX;
            var w = PanZoomContainer.ActualWidth;
            var h = PanZoomContainer.ActualHeight;
            if (w <= 0 || h <= 0 || s <= 1.0 + double.Epsilon)
            {
                return;
            }

            var maxX = w * (s - 1.0) / 2.0;
            var maxY = h * (s - 1.0) / 2.0;
            ContentPanTranslate.X = Math.Clamp(ContentPanTranslate.X, -maxX, maxX);
            ContentPanTranslate.Y = Math.Clamp(ContentPanTranslate.Y, -maxY, maxY);
        }

        private void ApplyHighContrast(bool enable)
        {
            if (FindName("HostRoot") is Grid hostRoot)
            {
                hostRoot.Effect = enable
                    ? new InvertEffect { Amount = 1.0 }
                    : null;
            }
        }
    }
}
