namespace BarrierFree_Kiosk.Navigation
{
    /// <summary>
    /// 키오스크 화면 식별자. 새 Page를 추가할 때마다 여기에 값을 추가하고
    /// <see cref="KioskPageRegistry"/>에 팩토리를 등록하세요.
    /// </summary>
    public enum KioskPageId
    {
        None = 0,
        Home,
        /// <summary>WorkSpace/Minsoo/Minsoo.xaml (<see cref="MinsooPage"/>)</summary>
        Minsoo,
        Menu,
    }
}
