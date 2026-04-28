using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BarrierFree_Kiosk.MainViews
{
    public partial class SelectableCardControl : UserControl
    {
        public static readonly DependencyProperty CardImageSourceProperty =
            DependencyProperty.Register(nameof(CardImageSource), typeof(ImageSource), typeof(SelectableCardControl));

        public static readonly DependencyProperty CardTypeImageSourceProperty =
            DependencyProperty.Register(nameof(CardTypeImageSource), typeof(ImageSource), typeof(SelectableCardControl));

        public static readonly DependencyProperty CardPriceProperty =
            DependencyProperty.Register(nameof(CardPrice), typeof(int), typeof(SelectableCardControl), new PropertyMetadata(0));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SelectableCardControl),
                new PropertyMetadata(false, OnIsSelectedChanged));

        public static readonly DependencyProperty SelectionBorderBrushProperty =
            DependencyProperty.Register(nameof(SelectionBorderBrush), typeof(Brush), typeof(SelectableCardControl),
                new PropertyMetadata(Brushes.Transparent));

        private static readonly Brush SelectedBorderBrush = new SolidColorBrush(Color.FromRgb(0x1E, 0x88, 0xE5));

        public SelectableCardControl()
        {
            InitializeComponent();
            SelectionBorderBrush = Brushes.Transparent;
        }

        public event RoutedEventHandler? CardClicked;

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

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public Brush SelectionBorderBrush
        {
            get => (Brush)GetValue(SelectionBorderBrushProperty);
            private set => SetValue(SelectionBorderBrushProperty, value);
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not SelectableCardControl control)
            {
                return;
            }

            control.SelectionBorderBrush = (bool)e.NewValue ? SelectedBorderBrush : Brushes.Transparent;
        }

        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            CardClicked?.Invoke(this, e);
        }
    }
}
