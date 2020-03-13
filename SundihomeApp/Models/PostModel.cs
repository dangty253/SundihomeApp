using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApp.ViewModels;

namespace SundihomeApp.Models
{
    public class PostModel : BaseViewModel
    {
        public Guid Id { get; set; }
        public Guid? CompanyId { get; set; }
        public int? CompanyStatus { get; set; }
        private short _postType;
        public short PostType
        {
            get => _postType;
            set
            {
                _postType = value;
                OnPropertyChanged(nameof(PostType));
            }
        } // Loai hinh (mua/ban/chothue/canthue)

        #region Du an
        public Guid? ProjectId;
        private Project _project;
        public Project Project
        {
            get => _project;
            set
            {
                if (_project != value)
                {
                    _project = value;
                    OnPropertyChanged(nameof(Project));
                }
            }
        }
        #endregion

        #region dia chi
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
        #endregion

        #region street
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
        #endregion

        #region Loai bat dong san
        public short? LoaiBatDongSanId { get; set; }
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
        #endregion

        #region tinh trang phap ly
        public int? TinhTrangPhapLyId { get; set; }
        private TinhTrangPhapLyModel _tinhTrangPhapLyModel;
        public TinhTrangPhapLyModel TinhTrangPhapLy { get => _tinhTrangPhapLyModel; set { _tinhTrangPhapLyModel = value; OnPropertyChanged(nameof(TinhTrangPhapLy)); } }
        #endregion

        #region Dien tich
        private decimal? _areaFrom;
        public decimal? AreaFrom
        {
            get => _areaFrom;
            set
            {
                _areaFrom = value;
                OnPropertyChanged(nameof(AreaFrom));
            }
        }

        private string _areaFromText;
        public string AreaFromText
        {
            get => _areaFromText;
            set
            {
                _areaFromText = value;
                OnPropertyChanged(nameof(AreaFromText));
            }
        }

        private string _areaToText;
        public string AreaToText
        {
            get => _areaToText;
            set
            {
                _areaToText = value;
                OnPropertyChanged(nameof(AreaToText));
            }
        }

        private decimal? _areaTo;
        public decimal? AreaTo
        {
            get => _areaTo;
            set
            {
                _areaTo = value;
                OnPropertyChanged(nameof(AreaTo));
            }
        }



        private bool _isAreaRange;
        public bool IsAreaRange
        {
            get => _isAreaRange;
            set
            {
                _isAreaRange = value;
                OnPropertyChanged(nameof(IsAreaRange));

                if (value == false)
                {
                    AreaTo = null;
                    AreaToText = null;
                }
            }
        }
        #endregion

        #region Gia tien

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
                    PriceToQuyDoi = null;
                    PriceToText = null;
                    PriceToUnit = null;
                }
            }
        }

        private bool _isNegotiate;
        public bool IsNegotiate
        {
            get => _isNegotiate;
            set
            {
                _isNegotiate = value;
                OnPropertyChanged(nameof(IsNegotiate));

                if (value == true) // gia thuong luong.
                {
                    PriceFrom = null;
                    PriceFromQuyDoi = null;
                    PriceFromText = null;
                    PriceFromUnit = null;

                    PriceTo = null;
                    PriceToQuyDoi = null;
                    PriceToText = null;
                    PriceToUnit = null;
                }
            }
        }

        public decimal? PriceFrom { get; set; }
        public decimal? PriceFromQuyDoi { get; set; }

        private string _priceFromText;
        public string PriceFromText { get => _priceFromText; set { _priceFromText = value; OnPropertyChanged(nameof(PriceFromText)); } }

        public short? PriceFromUnit { get; set; }

        public decimal? PriceTo { get; set; }
        public decimal? PriceToQuyDoi { get; set; }

        private string _priceToText;
        public string PriceToText { get => _priceToText; set { _priceToText = value; OnPropertyChanged(nameof(PriceToText)); } }

        public short? PriceToUnit { get; set; }

        #endregion

        private int? _tang;
        public int? Tang
        {
            get => _tang;
            set
            {
                if (_tang != value)
                {
                    _tang = value;
                    OnPropertyChanged(nameof(Tang));
                }

            }
        }

        public decimal? MatTien { get; set; }

        private string _matTienFormatText;
        public string MatTienFormatText
        {
            get => _matTienFormatText;
            set
            {
                _matTienFormatText = value;
                OnPropertyChanged(nameof(MatTienFormatText));
            }
        }

        public decimal? ChieuSau { get; set; }

        private string _chieuSauFormatText;
        public string ChieuSauFormatText
        {
            get => _chieuSauFormatText;
            set
            {
                if (_chieuSauFormatText != value)
                {
                    _chieuSauFormatText = value;
                    OnPropertyChanged(nameof(ChieuSauFormatText));
                }
            }
        }

        private short? _numOfFloor;
        public short? NumOfFloor
        {
            get => _numOfFloor;
            set
            {
                _numOfFloor = value;
                OnPropertyChanged(nameof(NumOfFloor));
            }
        }

        private short? _numOfBedRoom;
        public short? NumOfBedRoom
        {
            get => _numOfBedRoom;
            set
            {
                _numOfBedRoom = value;
                OnPropertyChanged(nameof(NumOfBedRoom));
            }
        }

        private short? _numOfBathRoom;
        public short? NumOfBathRoom
        {
            get => _numOfBathRoom;
            set
            {
                _numOfBathRoom = value;
                OnPropertyChanged(nameof(NumOfBathRoom));
            }
        }

        public decimal? DuongVao { get; set; }

        private string _duongVaoFormatText;
        public string DuongVaoFormatText
        {
            get => _duongVaoFormatText;
            set
            {
                if (_duongVaoFormatText != value)
                {
                    _duongVaoFormatText = value;
                    OnPropertyChanged(nameof(DuongVaoFormatText));
                }
            }
        }

        public short? HuongId;

        private HuongModel _huong;
        public HuongModel Huong
        {
            get => _huong;
            set
            {
                _huong = value;
                OnPropertyChanged(nameof(Huong));
            }
        }

        public string Utilities { get; set; }

        #region Province/District/Ward
        private Province _provice;
        public Province Province { get => _provice; set { _provice = value; OnPropertyChanged(nameof(Province)); SetAddress(); } }

        private District _district;
        public District District { get => _district; set { _district = value; OnPropertyChanged(nameof(District)); SetAddress(); } }

        private Ward _ward;
        public Ward Ward { get => _ward; set { _ward = value; OnPropertyChanged(nameof(Ward)); SetAddress(); } }
        public int? ProvinceId { get; set; }
        #endregion

        public int? DistrictId { get; set; }
        public int? WardId { get; set; }

        private string _title;
        public string Title { get => _title; set { _title = value; OnPropertyChanged(nameof(Title)); } }

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

        public string Images { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }

        public double? Lat { get; set; }
        public double? Long { get; set; }

        // thong tin bo sung
        private bool _isCommitment;
        public bool IsCommitment { get => _isCommitment; set { this._isCommitment = value; OnPropertyChanged(nameof(IsCommitment)); } }

        private DateTime? _commitmentDateFrom;
        public DateTime? CommitmentDateFrom { get => _commitmentDateFrom; set { this._commitmentDateFrom = value; OnPropertyChanged(nameof(CommitmentDateFrom)); } }

        private DateTime? _commitmentDateTo;
        public DateTime? CommitmentDateTo { get => _commitmentDateTo; set { this._commitmentDateTo = value; OnPropertyChanged(nameof(CommitmentDateTo)); } }

        private string _onwerFullName;
        public string OwnerFullName { get => _onwerFullName; set { this._onwerFullName = value; OnPropertyChanged(nameof(OwnerFullName)); } }

        private string _onwerPhone;
        public string OwnerPhone { get => _onwerPhone; set { this._onwerPhone = value; OnPropertyChanged(nameof(OwnerPhone)); } }

        private decimal _ownerPrice;
        public decimal OwnerPrice { get => _ownerPrice; set { this._ownerPrice = value; OnPropertyChanged(nameof(OwnerPrice)); } }

        private string _onwerAddress;
        public string OwnerAddress { get => _onwerAddress; set { this._onwerAddress = value; OnPropertyChanged(nameof(OwnerAddress)); } }

        private string _onwerDescription;
        public string OwnerDescription { get => _onwerDescription; set { this._onwerDescription = value; OnPropertyChanged(nameof(OwnerDescription)); } }

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
    }
}
