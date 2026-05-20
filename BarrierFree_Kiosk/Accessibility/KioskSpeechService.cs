using System.Globalization;
using System.Speech.Synthesis;

namespace BarrierFree_Kiosk.Accessibility
{
    /// <summary>
    /// 앱 전역 TTS(음성 안내) 서비스.
    /// </summary>
    public sealed class KioskSpeechService
    {
        private static readonly Lazy<KioskSpeechService> Instance = new(() => new KioskSpeechService());

        private readonly SpeechSynthesizer _synthesizer = new();

        private string? _lastSpokenText;
        private DateTime _lastSpokenAt = DateTime.MinValue;

        private KioskSpeechService()
        {
            TrySetKoreanVoice();
            _synthesizer.Rate = 0;
            _synthesizer.Volume = 100;
        }

        public static KioskSpeechService Default => Instance.Value;

        public void Speak(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var now = DateTime.UtcNow;
            if (text == _lastSpokenText && (now - _lastSpokenAt).TotalMilliseconds < 600)
            {
                return;
            }

            _lastSpokenText = text;
            _lastSpokenAt = now;

            try
            {
                _synthesizer.SpeakAsyncCancelAll();
                _synthesizer.SpeakAsync(text);
            }
            catch
            {
                // 음성 엔진 오류 시 앱 동작은 유지
            }
        }

        public void Stop()
        {
            try
            {
                _synthesizer.SpeakAsyncCancelAll();
            }
            catch
            {
                // ignore
            }

            _lastSpokenText = null;
        }

        private void TrySetKoreanVoice()
        {
            try
            {
                _synthesizer.SelectVoiceByHints(
                    VoiceGender.NotSet,
                    VoiceAge.NotSet,
                    0,
                    CultureInfo.GetCultureInfo("ko-KR"));
            }
            catch
            {
                // 한국어 음성이 없으면 시스템 기본 음성 사용
            }
        }
    }
}
