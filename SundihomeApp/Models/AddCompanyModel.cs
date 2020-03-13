using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Models
{
    public class AddCompanyModel : BaseViewModel
    {
        public Guid Id { get; set; }
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

        private string _shortName;
        public string ShortName
        {
            get => _shortName;
            set
            {
                _shortName = value;
                OnPropertyChanged(nameof(ShortName));
            }
        }

        private string _nameEng;
        public string NameEng
        {
            get => _nameEng;
            set
            {
                _nameEng = value;
                OnPropertyChanged(nameof(NameEng));
            }
        }

        private string _mst;
        public string MST
        {
            get => _mst;
            set
            {
                _mst = value;
                OnPropertyChanged(nameof(MST));
            }
        }

        private string _slogan;
        public string Slogan
        {
            get => _slogan;
            set
            {
                _slogan = value;
                OnPropertyChanged(nameof(Slogan));
            }
        }

        private string _introduction;
        public string Introduction
        {
            get => _introduction;
            set
            {
                _introduction = value;
                OnPropertyChanged(nameof(Introduction));
            }
        }

        public int ProvinceId { get; set; }
        private Province _province;
        public Province Province
        {
            get => _province;
            set
            {
                _province = value;
                OnPropertyChanged(nameof(Province));
                SetAddress();
            }
        }

        public int DistrictId { get; set; }
        private District _district;
        public District District
        {
            get => _district;
            set
            {
                _district = value;
                OnPropertyChanged(nameof(District));
                SetAddress();
            }
        }
        
        public int WardId { get; set; }
        private Ward _ward;
        public Ward Ward
        {
            get => _ward;
            set
            {
                _ward = value;
                OnPropertyChanged(nameof(Ward));
                SetAddress();
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

        public short NganhNgheId { get; set; }
        private LoaiCongTyModel _loaiCongTy;
        public LoaiCongTyModel LoaiCongTy
        {
            get => _loaiCongTy;
            set
            {
                _loaiCongTy = value;
                OnPropertyChanged(nameof(LoaiCongTy));
            }
        }

        
        private string _logo;
        public string Logo
        {
            get => _logo;
            set
            {
                _logo = value;
                OnPropertyChanged(nameof(Logo));
            }
        }
        private string _logoFullUrl;
        public string LogoFullUrl
        {
            get => _logoFullUrl;
            set
            {
                _logoFullUrl = value;
                OnPropertyChanged(nameof(LogoFullUrl));
            }
        }

        private string _images;
        public string Images
        {
            get => _images;
            set
            {
                _images = value;
                OnPropertyChanged(nameof(Images));
            }
        }
        private DateTime _createdDate;
        public DateTime CreatedDate
        {
            get => _createdDate;
            set
            {
                _createdDate = value;
                OnPropertyChanged(nameof(CreatedDate));
            }
        }
        private Guid _createdById;
        public Guid CreatedById
        {
            get => _createdById;
            set
            {
                _createdById = value;
                OnPropertyChanged(nameof(CreatedById));
            }
        }

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

