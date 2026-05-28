using System;
using System.Windows.Controls;

namespace BarrierFree_Kiosk.Navigation
{
    /// <summary>
    /// 호스트 창의 <see cref="Frame"/>에 페이지를 넣는 진입점입니다.
    /// 창이 로드될 때 한 번 <see cref="Attach"/>를 호출한 뒤, 버튼 등에서 <see cref="Navigate"/>를 사용합니다.
    /// </summary>
    public static class KioskNavigator
    {
        private static Frame? _frame;

        /// <summary>네비게이션에 사용할 프레임을 연결합니다. 보통 창 Loaded에서 호출합니다.</summary>
        public static void Attach(Frame frame)
        {
            _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        /// <summary>연결을 해제합니다. 창이 닫힐 때 선택적으로 호출할 수 있습니다.</summary>
        public static void Detach(Frame? frame)
        {
            if (_frame == frame)
                _frame = null;
        }

        public static void Navigate(KioskPageId id)
        {
            if (_frame is null)
                throw new InvalidOperationException(
                    "네비게이션 Frame이 연결되지 않았습니다. 호스트 창에서 KioskNavigator.Attach(frame)을 먼저 호출하세요.");

            if (id == KioskPageId.None)
                return;

            var page = KioskPageRegistry.Create(id);
            _frame.Navigate(page);
        }

        public static bool IsFrameAttached => _frame is not null;
    }
}
