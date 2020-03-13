using System;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;

namespace SundihomeApp.Models
{
    public class ChiTietLichTraKhoanVay
    {
        public int NoOfmOnth { get; set; }
        public string NoOfmOnthText => Language.month + " " + NoOfmOnth;

        public decimal Lai { get; set; }
        public string LaiText => DecimalHelper.ToCurrency(Lai);

        public decimal SoTienPhaiTra { get; set; }
        public string SoTienPhaiTraText => DecimalHelper.ToCurrency(SoTienPhaiTra);

        public decimal SoGocConLai { get; set; }
        public string SoGocConLaiText => DecimalHelper.ToCurrency(SoGocConLai);

        public string Nam { get; set; }

        public ChiTietLichTraKhoanVay(int month)
        {
            NoOfmOnth = month;
        }
    }
}
