using System.Windows;
using System.Windows.Controls;
using BarrierFree_Kiosk.Navigation;

namespace BarrierFree_Kiosk.Controls
{
    /// <summary>
    /// 클릭 시 <see cref="KioskNavigator"/>로 지정한 페이지로 이동하는 버튼입니다.
    /// 일반 Button과 동일하게 스타일·템플릿을 적용할 수 있습니다.
    /// </summary>
    public class NavigateButton : Button
    {
        public static readonly DependencyProperty TargetPageIdProperty = DependencyProperty.Register(
            nameof(TargetPageId),
            typeof(KioskPageId),
            typeof(NavigateButton),
            new PropertyMetadata(KioskPageId.None));

        public KioskPageId TargetPageId
        {
            get => (KioskPageId)GetValue(TargetPageIdProperty);
            set => SetValue(TargetPageIdProperty, value);
        }

        protected override void OnClick()
        {
            base.OnClick();

            if (TargetPageId != KioskPageId.None)
                KioskNavigator.Navigate(TargetPageId);
        }
    }
}
