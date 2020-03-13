using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels.BankViewModel;
using Telerik.XamarinForms.Common.Data;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SundihomeApp.Views.BankViews
{
    public partial class CalculatorPage : ContentPage
    {
        public CalculatorPageViewModel viewModel;
        public CalculatorPage()
        {
            InitializeComponent();
            On<iOS>().SetUseSafeArea(true);
            EntryLaiSuatPerYear.Text = Language.lai_suat + " (%/" + Language.year.ToLower() + ")";
            this.BindingContext = viewModel = new CalculatorPageViewModel();
            ModalCalculatorResult.Body.BindingContext = viewModel;
            Init();
        }

        public async void Init()
        {
            this.LookupMethod.ItemsSource = this.viewModel.MethodOptions;
            this.LookupMethod.HideClearButton();

            this.viewModel.Method = this.viewModel.MethodOptions[0];

            loadingPopup.IsVisible = false;
        }
        private void OnMethod_Changed(object sender, EventArgs e)
        {
            var medthod = this.viewModel.Method;
            if (medthod.Id == 0)
            {
                StackLaiSuatNoi.IsVisible = true;
            }
            else
            {
                StackLaiSuatNoi.IsVisible = false;
            }
            viewModel.LaiSuatNoi = false;
            LaiSuatNoi_Checked(CKLaiSuatNoi, EventArgs.Empty);
        }

        private void LaiSuatNoi_Checked(object sender, EventArgs e)
        {
            bool isChecked = this.viewModel.LaiSuatNoi;
            if (isChecked)
            {
                StackFixTime.IsVisible = true;
                StackLaiSuatTrungBinh.IsVisible = true;

                viewModel.ThoiHanVayCoDinh = null;
                viewModel.LaiSuatTrungBinh = null;
                EntryLaiSuatTrungBinh.SetPrice(null);
            }
            else
            {
                StackFixTime.IsVisible = false;
                StackLaiSuatTrungBinh.IsVisible = false;
            }
        }

        private async void Calculator_Clicked(object sender, EventArgs e)
        {
            if (viewModel.SoTienVay.HasValue == false)
            {
                await DisplayAlert("", Language.vui_long_nhap_so_tien_vay, Language.dong);
                return;
            }

            if (EntryLaiSuat.Price.HasValue == false)
            {
                await DisplayAlert("", Language.vui_long_nhap_lai_suat, Language.dong);
                return;
            }

            if (viewModel.ThoiHanVay.HasValue == false || viewModel.ThoiHanVay <= 0)
            {
                await DisplayAlert("", Language.vui_long_nhap_thoi_han_vay, Language.dong);
                return;
            }


            viewModel.CalcResult.Clear();

            // truong hop 1.
            decimal TV = viewModel.SoTienVay.Value;//Số tiền vay (TV):
            decimal LSN = EntryLaiSuat.Price.Value / 100;//Lãi suất (%) (LSN):
            decimal THV = viewModel.ThoiHanVay.Value;//Thời hạn vay (THV):

            // neu co check lai suat noi.
            decimal TCD;//Thời gian lãi cố định (TCD):
            decimal SUDN;//Lãi suất trung bình sau ưu đãi (SUDN):

            decimal TG; //Số tiền gốc phải trả hằng tháng (TG):
            decimal LST = LSN / 12; // trường hợp 1 thì là Lãi suất tháng trường hợp 2 thì là lãi suất ưu đãi thang.
            decimal TTL;//Tổng tiền lãi phải trả (TTL)
            decimal TTT;//Tổng tiền phải trả (TTT)

            var method = viewModel.Method;

            ChiTietLichTraKhoanVay thang0 = new ChiTietLichTraKhoanVay(0);
            thang0.SoTienPhaiTra = 0;
            thang0.Lai = 0;
            thang0.SoGocConLai = TV;
            thang0.Nam = Language.year + " 1";
            viewModel.CalcResult.Add(thang0);

            int TotalYear = viewModel.ThoiHanVay.Value / 12;
            if (TotalYear < viewModel.ThoiHanVay.Value * 12) TotalYear++;

            if (TotalYear > 1)
            {
                if (!dataGrid.GroupDescriptors.Any())
                {
                    dataGrid.GroupDescriptors.Add(new PropertyGroupDescriptor() { PropertyName = "Nam" });
                }
            }
            else
            {
                dataGrid.GroupDescriptors.Clear();
            }

            if (method.Id == 0) //Tính lãi suất theo gốc cố định, lãi giảm dần
            {
                LabelGocLai.IsVisible = false;

                TG = Math.Round(TV / THV, 3, MidpointRounding.AwayFromZero); //tiền gốc hàng tháng

                if (!viewModel.LaiSuatNoi)
                {
                    for (int i = 1; i <= viewModel.ThoiHanVay.Value; i++)
                    {
                        ChiTietLichTraKhoanVay item = new ChiTietLichTraKhoanVay(i);
                        for (int year = 1; year <= TotalYear; year++)
                        {
                            if (i <= (year * 12))
                            {
                                item.Nam = Language.year + " " + year;
                                //item.NoOfmOnth = i - (12 * (year - 1));
                                break;
                            }
                        }

                        decimal SoGocConLaiThangTruoc = viewModel.CalcResult.Last().SoGocConLai;
                        decimal TL = LST * SoGocConLaiThangTruoc;// tiền lãi phải trả mỗi tháng

                        TL = Math.Round(TL, 3, MidpointRounding.AwayFromZero);

                        item.Lai = TL; // Lãi
                        item.SoTienPhaiTra = TG + TL; // so tien phai tra = tien goc + tien lai
                        item.SoGocConLai = SoGocConLaiThangTruoc - TG;

                        viewModel.CalcResult.Add(item);
                    }
                }
                else
                {
                    if (viewModel.ThoiHanVayCoDinh.HasValue == false)
                    {
                        await DisplayAlert("", Language.vui_long_chon_thoi_gian_co_dinh, Language.dong);
                        return;
                    }

                    if (viewModel.ThoiHanVayCoDinh.Value > viewModel.ThoiHanVay.Value)
                    {
                        await DisplayAlert("", Language.thoi_han_co_dinh_khong_the_vuot_qua_thoi_han_vay, Language.dong);
                        return;
                    }

                    if (EntryLaiSuatTrungBinh.Price.HasValue == false)
                    {
                        await DisplayAlert("", Language.vui_long_nhap_lai_suat_trung_binh, Language.dong);
                        return;
                    }

                    TCD = viewModel.ThoiHanVayCoDinh.Value;
                    SUDN = EntryLaiSuatTrungBinh.Price.Value / 100;
                    decimal SUDT = SUDN / 12; // lai suat thang' sau thoi gian co dinh.

                    for (int i = 1; i <= viewModel.ThoiHanVay.Value; i++)
                    {
                        ChiTietLichTraKhoanVay item = new ChiTietLichTraKhoanVay(i);
                        for (int year = 1; year <= TotalYear; year++)
                        {
                            if (i <= (year * 12))
                            {
                                item.Nam = Language.year + " " + year;
                                break;
                            }
                        }

                        decimal TL;
                        decimal SoGocConLaiThangTruoc = viewModel.CalcResult.Last().SoGocConLai;

                        // thang hien tai nhsao hon hoac bang thoi gian co dinh
                        if (i <= TCD)
                        {
                            TL = SoGocConLaiThangTruoc * LST;    // UDT
                        }
                        else
                        {
                            TL = SoGocConLaiThangTruoc * SUDT;
                        }

                        TL = Math.Round(TL, 3, MidpointRounding.AwayFromZero);
                        item.Lai = TL; // Lãi
                        item.SoTienPhaiTra = TG + TL; // so tien phai tra = tien goc + tien lai
                        item.SoGocConLai = SoGocConLaiThangTruoc - TG;

                        viewModel.CalcResult.Add(item);
                    }
                }
            }
            else if (method.Id == 1) // lãi và gốc cố định.
            {
                decimal GL = (TV * LST) / (1 - (decimal)Math.Pow(Decimal.ToDouble((1 + LST)), Decimal.ToDouble(-THV)));

                for (int i = 1; i <= viewModel.ThoiHanVay.Value; i++)
                {
                    ChiTietLichTraKhoanVay item = new ChiTietLichTraKhoanVay(i);
                    for (int year = 1; year <= TotalYear; year++)
                    {
                        if (i <= (year * 12))
                        {
                            item.Nam = Language.year + " " + year;
                            //item.NoOfmOnth = i - (12 * (year - 1));
                            break;
                        }
                    }

                    item.SoTienPhaiTra = GL;
                    decimal SoGocConLaiThangTruoc = viewModel.CalcResult.Last().SoGocConLai;
                    decimal TL = LST * SoGocConLaiThangTruoc;
                    TL = Math.Round(TL, 3, MidpointRounding.AwayFromZero);

                    item.Lai = TL;

                    decimal TG_MoiThang = GL - TL; // tien goc nay khac tien goc ben ngoai. vi thay doi theo thang.
                    item.SoGocConLai = SoGocConLaiThangTruoc - TG_MoiThang;
                    viewModel.CalcResult.Add(item);
                }

                LabelGocLai.IsVisible = true;
                SpanGL.Text = DecimalHelper.ToCurrency(GL) + "đ";
            }

            TTL = viewModel.CalcResult.Sum(x => x.Lai);
            TTT = viewModel.CalcResult.Sum(x => x.SoTienPhaiTra);

            SpanTTL.Text = DecimalHelper.ToCurrency(TTL) + "đ";
            SpanTTT.Text = DecimalHelper.ToCurrency(TTT) + "đ";

            await ModalCalculatorResult.Show();
        }
    }
}
