using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using BarrierFree_Kiosk.Accessibility;
using BarrierFree_Kiosk.Navigation;
using wpfpslib;

namespace BarrierFree_Kiosk.MainViews
{
    public partial class NavigationHostWindow : Window
    {
        private const double ZoomContentScale = 1.35;

        // sub_bg_03.png(1080×1312) 하단 보라색 띠: y 1242~1311 (70px), MenuView 하단 7* 영역에 Fill
        private const double SubBgPurpleBandSourceHeight = 70.0;
        private const double SubBgImageSourceHeight = 1312.0;
        private const double DesignPageWidth = 1080.0;
        private const double DesignPageHeight = 1920.0;
        private const double PageBottomSectionRatio = 8.0 / 10.0;
        private const double DockBandHeightScale = 1.2;

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
            UpdateAccessibilityDockLayout();
            UpdateAccessibilitySpeechTexts();
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            KioskNavigator.Detach(RootFrame);
            EndPanDrag(releaseCapture: true);
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateAccessibilityDockLayout();
            ClampPanIfZoomed();
        }

        private void OnPanZoomContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateAccessibilityDockLayout();
            ClampPanIfZoomed();
        }

        private void OnHighContrastClick(object sender, RoutedEventArgs e)
        {
            _isHighContrastEnabled = !_isHighContrastEnabled;
            ApplyHighContrast(_isHighContrastEnabled);
            SetToolButtonActive(HighContrastButton, HighContrastLabel, _isHighContrastEnabled);
            HighContrastLabel.Text = _isHighContrastEnabled ? "고대비 해제" : "고대비";
            UpdateAccessibilitySpeechTexts();
            KioskSpeechService.Default.Speak(
                _isHighContrastEnabled ? "고대비 모드가 켜졌습니다." : "고대비 모드가 꺼졌습니다.");
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

            SetToolButtonActive(ZoomButton, ZoomLabel, _isZoomEnabled);
            ZoomLabel.Text = _isZoomEnabled ? "확대 해제" : "화면 확대";
            UpdateAccessibilitySpeechTexts();
            KioskSpeechService.Default.Speak(
                _isZoomEnabled ? "화면 확대가 켜졌습니다. 드래그로 이동할 수 있습니다." : "화면 확대가 꺼졌습니다.");

            ClampPanIfZoomed();
        }

        private void UpdateAccessibilityDockLayout()
        {
            if (PanZoomContainer.ActualWidth <= 0 || PanZoomContainer.ActualHeight <= 0)
            {
                return;
            }

            // Viewbox Uniform 기준: 페이지(1080×1920)가 실제로 그려지는 영역
            var scale = Math.Min(
                PanZoomContainer.ActualWidth / DesignPageWidth,
                PanZoomContainer.ActualHeight / DesignPageHeight);
            var scaledPageHeight = DesignPageHeight * scale;
            var letterboxBottom = Math.Max(0, (PanZoomContainer.ActualHeight - scaledPageHeight) / 2.0);

            var bottomSectionHeight = scaledPageHeight * PageBottomSectionRatio;
            var purpleBandHeight = bottomSectionHeight * (SubBgPurpleBandSourceHeight / SubBgImageSourceHeight);

            // 띠만 20% 키움(하단 고정 → 위로 확장), 버튼 크기는 기존 띠 기준 유지
            var dockHeight = purpleBandHeight * DockBandHeightScale;
            AccessibilityDock.Height = dockHeight;
            AccessibilityDock.MinHeight = dockHeight;
            AccessibilityDock.MaxHeight = dockHeight;
            AccessibilityDock.Margin = new Thickness(0, 0, 0, letterboxBottom);

            var buttonMinHeight = purpleBandHeight * 0.96;
            HighContrastButton.MinHeight = buttonMinHeight;
            HomeButton.MinHeight = buttonMinHeight;
            ZoomButton.MinHeight = buttonMinHeight;

            var iconSize = purpleBandHeight * 0.48;
            var fontSize = Math.Max(14, purpleBandHeight * 0.24);
            ApplyDockControlMetrics(iconSize, fontSize);
        }

        private void ApplyDockControlMetrics(double iconSize, double fontSize)
        {
            foreach (var button in new[] { HighContrastButton, HomeButton, ZoomButton })
            {
                if (button.Content is not StackPanel panel)
                {
                    continue;
                }

                foreach (var child in panel.Children)
                {
                    switch (child)
                    {
                        case Viewbox icon:
                            icon.Width = iconSize;
                            icon.Height = iconSize;
                            break;
                        case TextBlock label:
                            label.FontSize = fontSize;
                            break;
                    }
                }
            }
        }

        private void UpdateAccessibilitySpeechTexts()
        {
            SpeechAssist.SetHoverSpeechText(HighContrastButton,
                _isHighContrastEnabled ? "고대비 모드를 해제합니다." : "고대비 모드를 켭니다.");
            SpeechAssist.SetHoverSpeechText(ZoomButton,
                _isZoomEnabled
                    ? "화면 확대를 해제합니다."
                    : "화면을 확대합니다. 드래그로 이동할 수 있습니다.");
        }

        private void SetToolButtonActive(Button button, TextBlock label, bool isActive)
        {
            button.Tag = isActive ? "Active" : null;
            label.Foreground = (Brush)FindResource(
                isActive ? "AccessibilityToolActiveOnPurpleBrush" : "AccessibilityToolMutedOnPurpleBrush");
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
