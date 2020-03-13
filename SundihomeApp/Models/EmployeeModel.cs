using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Models
{
    public class EmployeeModel : BaseViewModel
    {
        public Guid Id { get; set; }
        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }
        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        private string _phone;
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
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
                SetAddress();
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
                SetAddress();
            }
        }

        public int? WardId { get; set; }
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

            Address = string.Join(",", list.ToArray());
        }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? RoleId { get; set; }

        public DateTime RegisterDate { get; set; }
    }
}

