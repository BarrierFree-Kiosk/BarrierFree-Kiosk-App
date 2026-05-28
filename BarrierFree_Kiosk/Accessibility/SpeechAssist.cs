using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BarrierFree_Kiosk.Accessibility
{
    /// <summary>
    /// 마우스 호버·창 표시 시 TTS를 연결하는 Attached Property.
    /// </summary>
    public static class SpeechAssist
    {
        private static readonly DependencyProperty HoverSpeechContextProperty =
            DependencyProperty.RegisterAttached(
                "HoverSpeechContext",
                typeof(HoverSpeechContext),
                typeof(SpeechAssist));

        public static readonly DependencyProperty HoverSpeechTextProperty =
            DependencyProperty.RegisterAttached(
                "HoverSpeechText",
                typeof(string),
                typeof(SpeechAssist),
                new PropertyMetadata(null, OnHoverSpeechTextChanged));

        public static readonly DependencyProperty WindowSpeechTextProperty =
            DependencyProperty.RegisterAttached(
                "WindowSpeechText",
                typeof(string),
                typeof(SpeechAssist),
                new PropertyMetadata(null, OnWindowSpeechTextChanged));

        public static string? GetHoverSpeechText(DependencyObject element) =>
            (string?)element.GetValue(HoverSpeechTextProperty);

        public static void SetHoverSpeechText(DependencyObject element, string? value) =>
            element.SetValue(HoverSpeechTextProperty, value);

        public static string? GetWindowSpeechText(DependencyObject element) =>
            (string?)element.GetValue(WindowSpeechTextProperty);

        public static void SetWindowSpeechText(DependencyObject element, string? value) =>
            element.SetValue(WindowSpeechTextProperty, value);

        private static void OnHoverSpeechTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
            {
                return;
            }

            UnwireHoverSpeech(element);
            element.Loaded -= ElementOnLoadedForHover;

            if (e.NewValue is string text && !string.IsNullOrWhiteSpace(text))
            {
                element.Loaded += ElementOnLoadedForHover;

                if (element.IsLoaded)
                {
                    ScheduleWireHoverSpeech(element);
                }
            }
        }

        private static void OnWindowSpeechTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Window window)
            {
                return;
            }

            window.Loaded -= WindowOnLoadedSpeak;

            if (e.NewValue is string text && !string.IsNullOrWhiteSpace(text))
            {
                window.Loaded += WindowOnLoadedSpeak;
            }
        }

        private static void ElementOnLoadedForHover(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                ScheduleWireHoverSpeech(element);
            }
        }

        private static void ScheduleWireHoverSpeech(FrameworkElement element)
        {
            element.Dispatcher.BeginInvoke(
                () => WireHoverSpeech(element),
                System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private static void WireHoverSpeech(FrameworkElement root)
        {
            UnwireHoverSpeech(root);

            var text = GetHoverSpeechText(root);
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var context = new HoverSpeechContext(text);
            root.SetValue(HoverSpeechContextProperty, context);

            root.MouseEnter += context.OnMouseEnter;
            root.MouseLeave += context.OnMouseLeave;
            root.PreviewMouseLeftButtonDown += context.OnPreviewMouseLeftButtonDown;

            AttachHoverHandlerToDescendants(root, context.OnMouseEnter);
        }

        private static void AttachHoverHandlerToDescendants(DependencyObject parent, MouseEventHandler handler)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is UIElement uiElement && !ReferenceEquals(child, parent))
                {
                    uiElement.MouseEnter += handler;
                }

                AttachHoverHandlerToDescendants(child, handler);
            }
        }

        private static void UnwireHoverSpeech(FrameworkElement root)
        {
            if (root.GetValue(HoverSpeechContextProperty) is not HoverSpeechContext context)
            {
                return;
            }

            root.MouseEnter -= context.OnMouseEnter;
            root.MouseLeave -= context.OnMouseLeave;
            root.PreviewMouseLeftButtonDown -= context.OnPreviewMouseLeftButtonDown;

            DetachHoverHandlerFromDescendants(root, context.OnMouseEnter);
            root.ClearValue(HoverSpeechContextProperty);
        }

        private static void DetachHoverHandlerFromDescendants(DependencyObject parent, MouseEventHandler handler)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is UIElement uiElement && !ReferenceEquals(child, parent))
                {
                    uiElement.MouseEnter -= handler;
                }

                DetachHoverHandlerFromDescendants(child, handler);
            }
        }

        private static void WindowOnLoadedSpeak(object sender, RoutedEventArgs e)
        {
            if (sender is not Window window)
            {
                return;
            }

            var text = GetWindowSpeechText(window);
            window.Dispatcher.BeginInvoke(
                () => KioskSpeechService.Default.Speak(text),
                System.Windows.Threading.DispatcherPriority.Input);
        }

        private sealed class HoverSpeechContext
        {
            private readonly string _text;
            private bool _suppressHoverUntilLeave;

            public HoverSpeechContext(string text)
            {
                _text = text;
            }

            public void OnMouseEnter(object sender, MouseEventArgs e)
            {
                if (_suppressHoverUntilLeave)
                {
                    return;
                }

                KioskSpeechService.Default.Speak(_text, KioskSpeechKind.Hover);
            }

            public void OnMouseLeave(object sender, MouseEventArgs e)
            {
                _suppressHoverUntilLeave = false;
                KioskSpeechService.Default.StopHover();
            }

            public void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                _suppressHoverUntilLeave = true;
                KioskSpeechService.Default.StopHover();
            }
        }
    }
}
