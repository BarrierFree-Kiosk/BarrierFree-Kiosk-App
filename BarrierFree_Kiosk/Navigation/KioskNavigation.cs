using System.Windows;
using System.Windows.Controls;

namespace BarrierFree_Kiosk.Navigation
{
    /// <summary>
    /// 기본 <see cref="Button"/>에 페이지 이동을 붙일 때 사용하는 attached property입니다.
    /// XAML 예: local:KioskNavigation.PageId="Home" (xmlns:local에 이 클래스의 clr-namespace 등록)
    /// </summary>
    public static class KioskNavigation
    {
        public static readonly DependencyProperty PageIdProperty = DependencyProperty.RegisterAttached(
            "PageId",
            typeof(KioskPageId),
            typeof(KioskNavigation),
            new PropertyMetadata(KioskPageId.None, OnPageIdChanged));

        public static KioskPageId GetPageId(DependencyObject obj) =>
            (KioskPageId)obj.GetValue(PageIdProperty);

        public static void SetPageId(DependencyObject obj, KioskPageId value) =>
            obj.SetValue(PageIdProperty, value);

        private static void OnPageIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Button button)
                return;

            button.Click -= OnNavigateClick;

            if (e.NewValue is KioskPageId id && id != KioskPageId.None)
                button.Click += OnNavigateClick;
        }

        private static void OnNavigateClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b)
                return;

            var id = GetPageId(b);
            if (id != KioskPageId.None)
                KioskNavigator.Navigate(id);
        }
    }
}
