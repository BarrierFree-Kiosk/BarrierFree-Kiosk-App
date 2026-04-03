using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BarrierFree_Kiosk.WorkSpace.Jungmin
{
    /// <summary>
    /// Jungmin.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            //영상의 재생 위치(Position)를 다시 0(처음)으로 초기화
            MainVideo.Position = TimeSpan.Zero;

            //멈춘 영상을 다시 재생 상태로 전환
            MainVideo.Play();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            Page2 nextPage = new Page2();

            //NavigationService를 사용하여 지체 없이 화면을 전환합니다.
            if (this.NavigationService != null)
            {
                // [동작] 현재 화면이 사라지고 nextPage 화면이 나타납니다.
                this.NavigationService.Navigate(nextPage);
            }
        }
    }
}
