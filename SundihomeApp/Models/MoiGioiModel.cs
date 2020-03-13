using System;
using SundihomeApi.Entities;

namespace SundihomeApp.Models
{
    public class MoiGioiModel: BaseModel
    {
        public Guid Id { get; set; }

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

        public int? StartYear { get; set; }

        private Option _type;
        public Option Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public int? TypeId { get; set; }

        private Province _province;
        public Province Province
        {
            get => _province;
            set
            {
                _province = value;
                OnPropertyChanged(nameof(Province));
            }
        }

        public int? ProvinceId { get; set; }

        private District _district;
        public District District
        {
            get => _district;
            set
            {
                _district = value;
                OnPropertyChanged(nameof(District));
            }
        }

        public int? DistrictId { get; set; }
    }
}
