using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SundihomeApp.Models;
using SundihomeApp.Resources;

namespace SundihomeApp.ViewModels.BankViewModel
{
    public class CalculatorPageViewModel : BaseViewModel
    {
        public ObservableCollection<ChiTietLichTraKhoanVay> CalcResult { get; set; } = new ObservableCollection<ChiTietLichTraKhoanVay>();
        public List<Option> MethodOptions { get; set; } = new List<Option>()
        {
            new Option(){Id=0,Name=Language.goc_co_dinh_lai_giam_dan},
            new Option(){Id=1,Name=Language.lai_va_goc_co_dinh},
        };

        public List<Option> TimeOptions { get; set; }

        private Option _method;
        public Option Method { get => _method; set { _method = value; OnPropertyChanged(nameof(Method)); } }

        private decimal? _soTienVay;
        public decimal? SoTienVay { get => _soTienVay; set { _soTienVay = value; OnPropertyChanged(nameof(SoTienVay)); } }

        private decimal? _laiSuat;
        public decimal? LaiSuat { get => _laiSuat; set { _laiSuat = value; OnPropertyChanged(nameof(LaiSuat)); } }
        public string LaiSuatText { get; set; }

        private int? _thoiHanvay;
        public int? ThoiHanVay { get => _thoiHanvay; set { _thoiHanvay = value; OnPropertyChanged(nameof(ThoiHanVay)); } }

        private int? _thoiHanvayCoDinh;
        public int? ThoiHanVayCoDinh { get => _thoiHanvayCoDinh; set { _thoiHanvayCoDinh = value; OnPropertyChanged(nameof(ThoiHanVayCoDinh)); } }

        private bool _laiSuatNoi;
        public bool LaiSuatNoi { get => _laiSuatNoi; set { _laiSuatNoi = value; OnPropertyChanged(nameof(LaiSuatNoi)); } }

        private decimal? _laiSuatTrungBinh;
        public decimal? LaiSuatTrungBinh { get => _laiSuatTrungBinh; set { _laiSuatTrungBinh = value; OnPropertyChanged(nameof(LaiSuatTrungBinh)); } }
        public string LaiSuatTrungBinhText { get; set; }
    }
}
