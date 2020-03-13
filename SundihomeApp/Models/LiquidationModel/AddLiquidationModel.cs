using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb.Liquidation;

namespace SundihomeApp.Models.LiquidationModel
{
    public class AddLiquidationModel : BaseModel
    {
        public Guid Id { get; set; }

        private LiquidationCategory _liquidationCategory;
        public LiquidationCategory LiquidationCategory
        {
            get => _liquidationCategory;
            set
            {
                _liquidationCategory = value;
                OnPropertyChanged(nameof(LiquidationCategory));
            }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

        // su dung cho TODAY
        private decimal _priceToDay;
        public decimal PriceToDay
        {
            get => _priceToDay;
            set { _priceToDay = value; OnPropertyChanged(nameof(PriceToDay)); }
        }

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

        private string _code;
        public string Code { get => _code; set { _code = value; OnPropertyChanged(nameof(Code)); } }

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }

        #region Mo ta
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
