using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BarrierFree_Kiosk.MainViews
{
    public partial class CartItemControl : UserControl
    {
        public static readonly DependencyProperty CardImageSourceProperty =
            DependencyProperty.Register(nameof(CardImageSource), typeof(ImageSource), typeof(CartItemControl));

        public static readonly DependencyProperty CardTypeImageSourceProperty =
            DependencyProperty.Register(nameof(CardTypeImageSource), typeof(ImageSource), typeof(CartItemControl));

        public static readonly DependencyProperty CardPriceProperty =
            DependencyProperty.Register(nameof(CardPrice), typeof(int), typeof(CartItemControl), new PropertyMetadata(0));

        public CartItemControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler? RemoveRequested;

        public ImageSource? CardImageSource
        {
            get => (ImageSource?)GetValue(CardImageSourceProperty);
            set => SetValue(CardImageSourceProperty, value);
        }

        public ImageSource? CardTypeImageSource
        {
            get => (ImageSource?)GetValue(CardTypeImageSourceProperty);
            set => SetValue(CardTypeImageSourceProperty, value);
        }

        public int CardPrice
        {
            get => (int)GetValue(CardPriceProperty);
            set => SetValue(CardPriceProperty, value);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveRequested?.Invoke(this, e);
        }
    }
}
