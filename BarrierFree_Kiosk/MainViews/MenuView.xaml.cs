using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BarrierFree_Kiosk.Accessibility;
using BarrierFree_Kiosk.WorkSpace.Sua;

namespace BarrierFree_Kiosk.MainViews
{
    public partial class MenuView : Page
    {
        private const int DefaultCardPrice = 4000;

        private SelectableCardControl? _selectedCard;
        private int _cartSubtotal;
        private int _couponDiscount;

        public MenuView()
        {
            InitializeComponent();
            InitializeCardList();
            UpdateTotalAmountText();
        }

        private void InitializeCardList()
        {
            var card = new SelectableCardControl
            {
                CardImageSource = ToImage("\\Resources\\Images\\PhotoCard\\10.bmp"),
                CardTypeImageSource = ToImage("\\Resources\\Images\\ko_kr\\main\\imgCardType_01.png"),
                CardPrice = DefaultCardPrice,
            };

            card.CardClicked += BtnCardSelect_Click;
            CardListHost.Children.Add(card);
        }

        private void MediaElement_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            MainVideo.Position = System.TimeSpan.Zero;
            MainVideo.Play();
        }

        private void BtnCardSelect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is not SelectableCardControl clickedCard)
            {
                return;
            }

            if (_selectedCard is not null)
            {
                _selectedCard.IsSelected = false;
            }

            _selectedCard = clickedCard;
            _selectedCard.IsSelected = true;
        }

        private void BtnCart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_selectedCard is null)
            {
                return;
            }

            var cartItem = new CartItemControl
            {
                CardImageSource = _selectedCard.CardImageSource,
                CardTypeImageSource = _selectedCard.CardTypeImageSource,
                CardPrice = _selectedCard.CardPrice,
            };

            cartItem.RemoveRequested += (_, _) =>
            {
                CartItemsHost.Children.Remove(cartItem);
                _cartSubtotal -= cartItem.CardPrice;
                if (_cartSubtotal < 0)
                {
                    _cartSubtotal = 0;
                }

                if (CartItemsHost.Children.Count == 0)
                {
                    _couponDiscount = 0;
                }

                UpdateTotalAmountText();
                KioskSpeechService.Default.Speak("일반 카드가 장바구니에서 삭제되었습니다.");
            };

            CartItemsHost.Children.Add(cartItem);
            _cartSubtotal += cartItem.CardPrice;
            UpdateTotalAmountText();
            KioskSpeechService.Default.Speak("일반 카드를 장바구니에 담았습니다.");
        }

        private void BtnPayment_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var itemCount = CartItemsHost.Children.Count;
            KioskSpeechService.Default.Speak(
                $"일반카드 총 {itemCount}개, 총 {GetFinalAmount():N0}원 입니다. 카드 결제 또는 쿠폰 사용을 진행해주세요.");

            var hostWindow = Window.GetWindow(this);
            var popup = new PaymentPopupWindow();

            if (hostWindow is not null)
            {
                popup.Owner = hostWindow;
            }

            if (popup.ShowDialog() == true && popup.AppliedCouponDiscount > 0)
            {
                _couponDiscount = popup.AppliedCouponDiscount;
                UpdateTotalAmountText();
                KioskSpeechService.Default.Speak(
                    $"쿠폰이 적용되어 총 {GetFinalAmount():N0}원 입니다.");
            }
        }

        private static BitmapImage ToImage(string relativeOrAbsoluteUri)
        {
            return new BitmapImage(new System.Uri(relativeOrAbsoluteUri, System.UriKind.RelativeOrAbsolute));
        }

        private int GetFinalAmount()
        {
            var finalAmount = _cartSubtotal - _couponDiscount;
            return finalAmount < 0 ? 0 : finalAmount;
        }

        private void UpdateTotalAmountText()
        {
            TxtTotalAmount.Text = GetFinalAmount().ToString("N0");
        }
    }
}
