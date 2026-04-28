using System;
using System.Windows;
using System.Windows.Controls;

namespace BarrierFree_Kiosk.WorkSpace.Sua
{
    public partial class CouponPaymentWindow : Window
    {
        private int _discountAmount;

        public CouponPaymentWindow()
        {
            InitializeComponent();
            Loaded += CouponPaymentWindow_Loaded;
        }

        private void CouponPaymentWindow_Loaded(object sender, RoutedEventArgs e)
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

        private void NumpadButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string digit)
            {
                TxtCouponCode.Text += digit;
                TxtCouponCode.CaretIndex = TxtCouponCode.Text.Length;
            }
        }

        private void BtnBackspace_Click(object sender, RoutedEventArgs e)
        {
            if (TxtCouponCode.Text.Length > 0)
            {
                TxtCouponCode.Text = TxtCouponCode.Text[..^1];
                TxtCouponCode.CaretIndex = TxtCouponCode.Text.Length;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TxtCouponCode.Text = string.Empty;
            _discountAmount = 0;
            TxtDiscountAmount.Text = "총 할인 금액(₩) 0";
            TxtCouponStatus.Text = string.Empty;
        }

        private void BtnSearchCoupon_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtCouponCode.Text) || TxtCouponCode.Text.Length != 8)
            {
                _discountAmount = 0;
                TxtDiscountAmount.Text = "총 할인 금액(₩) 0";
                TxtCouponStatus.Text = "쿠폰 번호(8자리)를 확인해 주십시오.";

                var messagePopupWindow = new MessagePopupWindow("쿠폰 번호(8자리)를 확인해 주십시오.")
                {
                    Owner = this
                };
                messagePopupWindow.ShowDialog();
                return;
            }

            // TODO: 실제 쿠폰/서버 조회 로직 연결 지점
            _discountAmount = TxtCouponCode.Text.Length >= 6 ? 2000 : 1000;
            TxtDiscountAmount.Text = $"총 할인 금액(₩) {_discountAmount:N0}";
            TxtCouponStatus.Text = "쿠폰 조회가 완료되었습니다.";
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            if (_discountAmount <= 0)
            {
                TxtCouponStatus.Text = "쿠폰 조회 후 사용 여부를 선택해 주세요.";
                return;
            }

            DialogResult = true;
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
