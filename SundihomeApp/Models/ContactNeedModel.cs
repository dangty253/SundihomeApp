using System;
using SundihomeApi.Entities;
using SundihomeApp.ViewModels;

namespace SundihomeApp.Models
{
    public class ContactNeedModel : BaseViewModel
    {
        public Guid Id { get; set; }

        private Guid _contactId;
        public Guid ContactId
        {
            get => _contactId;
            set
            {
                _contactId = value;
                OnPropertyChanged(nameof(ContactId));
            }
        }

        private Contact _contact;
        public Contact Contact
        {
            get => _contact;
            set
            {
                _contact = value;
                OnPropertyChanged(nameof(Contact));
            }
        }

        private string _project;
        public string Project
        {
            get => _project;
            set
            {
                _project = value;
                OnPropertyChanged(nameof(Project));
            }
        }
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
        private int _type;
        public int Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }


        public int? ProvinceId { get; set; }
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

        public int? DistrictId { get; set; }
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
        private int? _piority;
        public int? Piority
        {
            get => _piority;
            set
            {
                _piority = value;
                OnPropertyChanged(nameof(Piority));
            }
        }
        private int? _rate;
        public int? Rate
        {
            get => _rate;
            set
            {
                _rate = value;
                OnPropertyChanged(nameof(Rate));
            }

        }
        private decimal? _budget;
        public decimal? Budget
        {
            get => _budget;
            set
            {
                _budget = value;
                OnPropertyChanged(nameof(Budget));
            }

        }
    }
}

