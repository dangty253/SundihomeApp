using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Furniture;

namespace SundihomeApp.Models.Furniture
{
    public class AddProductModel : BaseModel
    {
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

        public decimal? Price { get; set; }
        public bool? Status { get; set; }

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

        private FurnitureCategory _parentCategory;
        public FurnitureCategory ParentCategory
        {
            get => _parentCategory;
            set
            {
                _parentCategory = value;
                OnPropertyChanged(nameof(ParentCategory));
            }
        }

        private FurnitureCategory _childCategory;
        public FurnitureCategory ChildCategory
        {
            get => _childCategory;
            set
            {
                _childCategory = value;
                OnPropertyChanged(nameof(ChildCategory));
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

        private Province _provice;
        public Province Province { get => _provice; set { _provice = value; OnPropertyChanged(nameof(Province)); SetAddress(); } }

        private District _district;
        public District District { get => _district; set { _district = value; OnPropertyChanged(nameof(District)); SetAddress(); } }

        private Ward _ward;
        public Ward Ward { get => _ward; set { _ward = value; OnPropertyChanged(nameof(Ward)); SetAddress(); } }

        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }

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

        public string Model { get; set; }
        public string Origin { get; set; }
        public int? Guarantee { get; set; }
        public string Videos { get; set; }

        public int ProductStatus { get; set; }
        private bool? _isPromotion;
        public bool? IsPromotion
        {
            get => _isPromotion;
            set
            {
                _isPromotion = value;
                OnPropertyChanged(nameof(IsPromotion));
            }
        }
        private DateTime? _promotionFromDate;
        public DateTime? PromotionFromDate
        {
            get => _promotionFromDate;
            set
            {
                _promotionFromDate = value;
                OnPropertyChanged(nameof(PromotionFromDate));
            }
        }
        private DateTime? _promotionToDate;
        public DateTime? PromotionToDate
        {
            get => _promotionToDate;
            set
            {
                _promotionToDate = value;
                OnPropertyChanged(nameof(PromotionToDate));
            }
        }


        public decimal? PromotionPrice { get; set; }
    }
}
