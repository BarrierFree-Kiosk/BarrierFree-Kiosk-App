using System;
using System.Collections.Generic;
using System.Windows.Controls;
using BarrierFree_Kiosk.MainViews;
using BarrierFree_Kiosk.WorkSpace;
namespace BarrierFree_Kiosk.Navigation
{
    /// <summary>
    /// 페이지 키와 실제 <see cref="Page"/> 생성 로직을 한곳에서 관리합니다.
    /// XAML이 없어도 이 파일만 수정하면 라우팅을 확장할 수 있습니다.
    /// </summary>
    public static class KioskPageRegistry
    {
        private static readonly Dictionary<KioskPageId, Func<Page>> Factories = new()
        {
            [KioskPageId.Home] = static () => new HomeView(),
            [KioskPageId.Minsoo] = static () => new MinsooPage(),
            [KioskPageId.Menu] = static () => new MenuView(),
        };

        public static Page Create(KioskPageId id)
        {
            if (id == KioskPageId.None)
                throw new ArgumentException("유효한 페이지 ID가 아닙니다.", nameof(id));

            if (!Factories.TryGetValue(id, out var factory))
                throw new InvalidOperationException($"등록되지 않은 페이지입니다: {id}. {nameof(KioskPageRegistry)}에 팩토리를 추가하세요.");

            return factory();
        }

        public static bool IsRegistered(KioskPageId id) =>
            id != KioskPageId.None && Factories.ContainsKey(id);
    }
}
