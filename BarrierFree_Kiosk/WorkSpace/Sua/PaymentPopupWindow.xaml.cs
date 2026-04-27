using System;
using System.Windows;

namespace BarrierFree_Kiosk.WorkSpace.Sua
{
    public partial class PaymentPopupWindow : Window
    {
        public PaymentPopupWindow()
        {
            InitializeComponent();
            Loaded += PaymentPopupWindow_Loaded;
        }

        private void PaymentPopupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LockWindowSize();
        }

        private void LockWindowSize()
        {
            var boundWidth = Owner?.ActualWidth > 0 ? Owner.ActualWidth : SystemParameters.WorkArea.Width;
            var boundHeight = Owner?.ActualHeight > 0 ? Owner.ActualHeight : SystemParameters.WorkArea.Height;
            var width = Math.Min(ActualWidth, boundWidth * 0.96);
            var height = Math.Min(ActualHeight, boundHeight * 0.96);

            SizeToContent = SizeToContent.Manual;
            Width = width;
            Height = height;
            MinWidth = width;
            MaxWidth = width;
            MinHeight = height;
            MaxHeight = height;

            if (Owner is not null)
            {
                Left = Owner.Left + (Owner.ActualWidth - Width) / 2;
                Top = Owner.Top + (Owner.ActualHeight - Height) / 2;
            }
        }

        private void BtnCardPayment_Click(object sender, RoutedEventArgs e)
        {
            var cardInsertWindow = new CardInsertWindow
            {
                Owner = this
            };
            cardInsertWindow.ShowDialog();

            DialogResult = true;
            Close();
        }

        private void BtnCouponPayment_Click(object sender, RoutedEventArgs e)
        {
            var couponPaymentWindow = new CouponPaymentWindow
            {
                Owner = Owner ?? this
            };
            couponPaymentWindow.ShowDialog();

            DialogResult = false;
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
