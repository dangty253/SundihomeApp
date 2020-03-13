using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Models
{
    public class AddProjectModel : BaseViewModel
    {
        public Guid Id { get; set; }

        public Guid? CompanyId { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _street;
        public string Street
        {
            get => _street;
            set
            {
                _street = value;
                OnPropertyChanged(nameof(Street));
                SetAddress();
            }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
        public string Avatar { get; set; }

        public string Images { get; set; }

        public string CategoriBDS { get; set; }
        private LoaiBatDongSanModel _loaiBatDongSan;
        public LoaiBatDongSanModel LoaiBatDongSan
        {
            get => _loaiBatDongSan;
            set
            {
                _loaiBatDongSan = value;
                OnPropertyChanged(nameof(LoaiBatDongSan));
            }
        }

        public string ImageUtilities { get; set; }
        private Option _tienIchDuAn;
        public Option TienIchDuAn
        {
            get => _tienIchDuAn;
            set
            {
                _tienIchDuAn = value;
                OnPropertyChanged(nameof(TienIchDuAn));
            }
        }

        public string Status { get; set; }
        private ProjectStatusModel _trangThai;
        public ProjectStatusModel TrangThai
        {
            get => _trangThai;
            set
            {
                _trangThai = value;
                OnPropertyChanged(nameof(TrangThai));
            }
        }


        private Province _provice;
        public Province Province { get => _provice; set { _provice = value; OnPropertyChanged(nameof(Province)); SetAddress(); } }

        private District _district;
        public District District { get => _district; set { _district = value; OnPropertyChanged(nameof(District)); SetAddress(); } }

        private Ward _ward;
        public Ward Ward { get => _ward; set { _ward = value; OnPropertyChanged(nameof(Ward)); SetAddress(); } }

        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private string _soLuongSanPham;
        public string SoLuongSanPham
        {
            get => _soLuongSanPham;
            set
            {
                _soLuongSanPham = value;
                OnPropertyChanged(nameof(SoLuongSanPham));
            }
        }

        private string _matDoXayDung;
        public string MatDoXayDung
        {
            get => _matDoXayDung;
            set
            {
                _matDoXayDung = value;
                OnPropertyChanged(nameof(MatDoXayDung));
            }
        }

        public decimal? MatDoXayDungPercent { get; set; }

        private string _tongVonDauTu;
        public string TongVonDauTu
        {
            get => _tongVonDauTu;
            set
            {
                _tongVonDauTu = value;
                OnPropertyChanged(nameof(TongVonDauTu));
            }
        }

        private int? _soTang;
        public int? SoTang
        {
            get => _soTang;
            set
            {
                _soTang = value;
                OnPropertyChanged(nameof(SoTang));
            }
        }

        private int? _tangHam;
        public int? TangHam
        {
            get => _tangHam;
            set
            {
                _tangHam = value;
                OnPropertyChanged(nameof(TangHam));
            }
        }

        private int? _soLuongToaNha;
        public int? SoLuongToaNha
        {
            get => _soLuongToaNha;
            set
            {
                _soLuongToaNha = value;
                OnPropertyChanged(nameof(SoLuongToaNha));
            }
        }

        private int? _soThangMay;
        public int? SoThangMay
        {
            get => _soThangMay;
            set
            {
                _soThangMay = value;
                OnPropertyChanged(nameof(SoThangMay));
            }
        }

        private DateTime? _thoiGianMoBan;
        public DateTime? ThoiGianMoBan
        {
            get => _thoiGianMoBan;
            set
            {
                _thoiGianMoBan = value;
                OnPropertyChanged(nameof(ThoiGianMoBan));
            }
        }

        private DateTime? _thoiGianBanGiao;
        public DateTime? ThoiGianBanGiao
        {
            get => _thoiGianBanGiao;
            set
            {
                _thoiGianBanGiao = value;
                OnPropertyChanged(nameof(ThoiGianBanGiao));
            }
        }

        private int? _namKhoiCong;
        public int? NamKhoiCong
        {
            get => _namKhoiCong;
            set
            {
                _namKhoiCong = value;
                OnPropertyChanged(nameof(NamKhoiCong));
            }
        }

        private int? _namHoanThanh;
        public int? NamHoanThanh
        {
            get => _namHoanThanh;
            set
            {
                _namHoanThanh = value;
                OnPropertyChanged(nameof(NamHoanThanh));
            }
        }

        private string _tongDienTichSan;
        public string TongDienTichSan
        {
            get => _tongDienTichSan;
            set
            {
                _tongDienTichSan = value;
                OnPropertyChanged(nameof(TongDienTichSan));
            }
        }

        #region gia tien
        private bool _isNegotiate;
        public bool IsNegotiate
        {
            get => _isNegotiate;
            set
            {
                _isNegotiate = value;
                OnPropertyChanged(nameof(IsNegotiate));
                if (value == true)
                {
                    PriceFrom = null;
                    PriceTo = null;
                }
            }

        }

        private bool _isPriceRange;
        public bool IsPriceRange
        {
            get => _isPriceRange;
            set
            {
                _isPriceRange = value;
                OnPropertyChanged(nameof(IsPriceRange));
                if (value == false)
                {
                    PriceTo = null;
                }
            }
        }

        public string PriceBDS { get; set; }
        private decimal? _priceFrom;
        public decimal? PriceFrom
        {
            get => _priceFrom;
            set
            {
                _priceFrom = value;
                OnPropertyChanged(nameof(PriceFrom));
            }
        }

        private decimal? _priceTo;
        public decimal? PriceTo
        {
            get => _priceTo;
            set
            {
                _priceTo = value;
                OnPropertyChanged(nameof(PriceTo));
            }
        }
        #endregion

        #region Dien tich san trung binh

        public string DienTichSanTrungBinh { get; set; }
        private decimal? _dienTichSanTrungBinh_From;
        public decimal? DienTichSanTrungBinh_From
        {
            get => _dienTichSanTrungBinh_From;
            set
            {
                _dienTichSanTrungBinh_From = value;
                OnPropertyChanged(nameof(DienTichSanTrungBinh_From));
            }
        }

        private decimal? _dienTichSanTrungBinh_To;
        public decimal? DienTichSanTrungBinh_To
        {
            get => _dienTichSanTrungBinh_To;
            set
            {
                _dienTichSanTrungBinh_To = value;
                OnPropertyChanged(nameof(DienTichSanTrungBinh_To));
            }
        }

        private bool _isDienTichSanTrungBinh;
        public bool IsDienTichSanTrungBinh
        {
            get => _isDienTichSanTrungBinh;
            set
            {
                _isDienTichSanTrungBinh = value;
                OnPropertyChanged(nameof(IsDienTichSanTrungBinh));
                if (value == false)
                {
                    DienTichSanTrungBinh_To = null;
                }
            }
        }
        #endregion

        #region Dien tich xay dung
        public string DienTichXayDung { get; set; }
        private decimal? _dienTichXayDung_From;
        public decimal? DienTichXayDung_From
        {
            get => _dienTichXayDung_From;
            set
            {
                _dienTichXayDung_From = value;
                OnPropertyChanged(nameof(DienTichXayDung_From));
            }
        }

        private decimal? _dienTichXayDung_To;
        public decimal? DienTichXayDung_To
        {
            get => _dienTichXayDung_To;
            set
            {
                _dienTichXayDung_To = value;
                OnPropertyChanged(nameof(DienTichXayDung_To));
            }
        }

        private bool _isDienTichXayDung;
        public bool IsDienTichXayDung
        {
            get => _isDienTichXayDung;
            set
            {
                _isDienTichXayDung = value;
                OnPropertyChanged(nameof(IsDienTichXayDung));
                if (value == false)
                {
                    DienTichXayDung_To = null;
                }
            }
        }
        #endregion

        #region Dien tich cay xanh
        public string DienTichCayXanh { get; set; }
        private decimal? _dienTichCayXanh_From;
        public decimal? DienTichCayXanh_From
        {
            get => _dienTichCayXanh_From;
            set
            {
                _dienTichCayXanh_From = value;
                OnPropertyChanged(nameof(DienTichCayXanh_From));
            }
        }

        private decimal? _dienTichCayXanh_To;
        public decimal? DienTichCayXanh_To
        {
            get => _dienTichCayXanh_To;
            set
            {
                _dienTichCayXanh_To = value;
                OnPropertyChanged(nameof(DienTichCayXanh_To));
            }
        }

        private bool _isDienTichCayXanh;
        public bool IsDienTichCayXanh
        {
            get => _isDienTichCayXanh;
            set
            {
                _isDienTichCayXanh = value;
                OnPropertyChanged(nameof(IsDienTichCayXanh));
                if (value == false)
                {
                    DienTichCayXanh_To = null;
                }
            }
        }
        #endregion

        #region Dien tich khu dat
        public string DienTichKhuDat { get; set; }
        private decimal? _dienTichKhuDat_From;
        public decimal? DienTichKhuDat_From
        {
            get => _dienTichKhuDat_From;
            set
            {
                _dienTichKhuDat_From = value;
                OnPropertyChanged(nameof(DienTichKhuDat_From));
            }
        }

        private decimal? _dienTichKhuDat_To;
        public decimal? DienTichKhuDat_To
        {
            get => _dienTichKhuDat_To;
            set
            {
                _dienTichKhuDat_To = value;
                OnPropertyChanged(nameof(DienTichKhuDat_To));
            }
        }

        private bool _isDienTichKhuDat;
        public bool IsDienTichKhuDat
        {
            get => _isDienTichKhuDat;
            set
            {
                _isDienTichKhuDat = value;
                OnPropertyChanged(nameof(IsDienTichKhuDat));
                if (value == false)
                {
                    DienTichKhuDat_To = null;
                }
            }
        }
        #endregion

        public double? Lat { get; set; }
        public double? Long { get; set; }

        public void SetAddress()
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrWhiteSpace(this.Street))
            {
                list.Add(this.Street.Trim());
            }
            if (this.Ward != null)
            {
                list.Add(Ward.Name);
            }
            if (this.District != null)
            {
                list.Add(District.Name);
            }
            if (this.Province != null)
            {
                list.Add(Province.Name);
            }

            Address = string.Join(", ", list.ToArray());
        }
        private string _chuDauTu;
        public string ChuDauTu
        {
            get => _chuDauTu;
            set
            {
                _chuDauTu = value;
                OnPropertyChanged(nameof(ChuDauTu));
            }
        }
        private string _donViThietKeThiCong;
        public string DonViThietKeThiCong
        {
            get => _donViThietKeThiCong;
            set
            {
                _donViThietKeThiCong = value;
                OnPropertyChanged(nameof(DonViThietKeThiCong));
            }
        }
        private string _phanKhuDuAn;
        public string PhanKhuDuAn
        {
            get => _phanKhuDuAn;
            set
            {
                _phanKhuDuAn = value;
                OnPropertyChanged(nameof(PhanKhuDuAn));
            }
        }
        private string _tienIchNoiKhu;
        public string TienIchNoiKhu
        {
            get => _tienIchNoiKhu;
            set
            {
                _tienIchNoiKhu = value;
                OnPropertyChanged(nameof(TienIchNoiKhu));
            }
        }
        private string _tienIchNgoaiKhu;
        public string TienIchNgoaiKhu
        {
            get => _tienIchNgoaiKhu;
            set
            {
                _tienIchNgoaiKhu = value;
                OnPropertyChanged(nameof(TienIchNgoaiKhu));
            }
        }
    }
}

