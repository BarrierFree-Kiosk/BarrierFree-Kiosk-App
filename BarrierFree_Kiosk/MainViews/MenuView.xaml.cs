using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BarrierFree_Kiosk.WorkSpace.Sua;

namespace BarrierFree_Kiosk.MainViews
{
    public partial class MenuView : Page
    {
        private const int DefaultCardPrice = 4000;

        private SelectableCardControl? _selectedCard;
        private int _totalAmount;

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
                _totalAmount -= cartItem.CardPrice;
                if (_totalAmount < 0)
                {
                    _totalAmount = 0;
                }

                UpdateTotalAmountText();
            };

            CartItemsHost.Children.Add(cartItem);
            _totalAmount += cartItem.CardPrice;
            UpdateTotalAmountText();
        }

        private void BtnPayment_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var hostWindow = Window.GetWindow(this);
            var popup = new PaymentPopupWindow();

            if (hostWindow is not null)
            {
                popup.Owner = hostWindow;
            }

            popup.ShowDialog();
        }

        private static BitmapImage ToImage(string relativeOrAbsoluteUri)
        {
            return new BitmapImage(new System.Uri(relativeOrAbsoluteUri, System.UriKind.RelativeOrAbsolute));
        }

        private void UpdateTotalAmountText()
        {
            TxtTotalAmount.Text = _totalAmount.ToString("N0");
        }
    }
}
