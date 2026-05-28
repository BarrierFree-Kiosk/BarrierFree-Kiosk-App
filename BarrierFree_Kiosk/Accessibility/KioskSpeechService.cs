using System.Globalization;
using System.Speech.Synthesis;

namespace BarrierFree_Kiosk.Accessibility
{
    public enum KioskSpeechKind
    {
        /// <summary>마우스 호버 안내 — 커서가 벗어나면 중단 가능.</summary>
        Hover,

        /// <summary>클릭·동작 안내 — 커서 이동과 무관하게 끝까지 재생.</summary>
        Action,
    }

    /// <summary>
    /// 앱 전역 TTS(음성 안내) 서비스.
    /// </summary>
    public sealed class KioskSpeechService
    {
        private static readonly Lazy<KioskSpeechService> Instance = new(() => new KioskSpeechService());

        private readonly SpeechSynthesizer _synthesizer = new();

        private string? _lastSpokenText;
        private DateTime _lastSpokenAt = DateTime.MinValue;
        private KioskSpeechKind _currentKind = KioskSpeechKind.Action;

        private KioskSpeechService()
        {
            TrySetKoreanVoice();
            _synthesizer.Rate = 0;
            _synthesizer.Volume = 100;
        }

        public static KioskSpeechService Default => Instance.Value;

        public void Speak(string? text, KioskSpeechKind kind = KioskSpeechKind.Action)
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
            _currentKind = kind;

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

        /// <summary>호버 안내만 중단합니다. 클릭·동작 안내는 계속 재생됩니다.</summary>
        public void StopHover()
        {
            if (_currentKind != KioskSpeechKind.Hover)
            {
                return;
            }

            StopInternal();
        }

        public void Stop()
        {
            StopInternal();
        }

        private void StopInternal()
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
