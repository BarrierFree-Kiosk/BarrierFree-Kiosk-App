using System;
using System.Windows;
using System.Windows.Controls;
using System.Speech.Synthesis; // 참조 추가 필요 (System.Speech)

namespace KioskProject
{
    public partial class TestWindow : Window
    {
        private SpeechSynthesizer tts = new SpeechSynthesizer();
        private double currentScale = 1.0;

        public TestWindow()
        {
            InitializeComponent();
            tts.SpeakAsync("키오스크가 시작되었습니다. 방향키로 메뉴를 이동하세요.");
        }

        // ① 커서(포커스) 위치 음성 리딩
        private void Button_GotFocus(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                tts.SpeakAsyncCancelAll(); // 이전 음성 취소
                tts.SpeakAsync($"{btn.Content} 메뉴입니다.");

                // 포커스 시 시각적 효과 (강조)
                btn.BorderBrush = System.Windows.Media.Brushes.Red;
                btn.BorderThickness = new Thickness(5);
            }
        }

        // ② 화면 확대 기능
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            currentScale += 0.2;
            ApplyZoom();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (currentScale > 1.0) currentScale -= 0.2;
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            // Viewbox의 높이/너비를 조절하여 내부 컨텐츠를 확대
            MainViewbox.Width = this.ActualWidth * currentScale;
            MainViewbox.Height = this.ActualHeight * 0.7 * currentScale;
            tts.SpeakAsync($"화면이 {Math.Round(currentScale, 1)}배 확대되었습니다.");
        }

        // ③ 고대비 모드 (간단 예시)
        private void HighContrast_Click(object sender, RoutedEventArgs e)
        {
            this.Background = System.Windows.Media.Brushes.Black;
            tts.SpeakAsync("고대비 모드가 활성화되었습니다.");
        }
    }
}