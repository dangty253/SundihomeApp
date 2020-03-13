using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApp.ViewModels;

namespace SundihomeApp.Models
{
    public class ContactModel: BaseViewModel
    {
        public ContactModel(ContactModel contactModel)
        {
            this.Id = contactModel.Id;
            this.FullName = contactModel.FullName;
            this.Phone = contactModel.Phone;
            this.GroupId = contactModel.GroupId;
            this.SelectGroup = contactModel.SelectGroup;
            this.ProvinceId = contactModel.ProvinceId;
            this.Province = contactModel.Province;
            this.District = contactModel.District;
            this.DistrictId = contactModel.DistrictId;
            this.Ward = contactModel.Ward;
            this.WardId = contactModel.WardId;
            this.Address = contactModel.Address;
            this.Street = contactModel.Street;
            this.Address = contactModel.Address;
            this.CreatedById = contactModel.CreatedById;
            this.CompanyById = contactModel.CompanyById;
        }
        public ContactModel()
        {
            
        }
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
        private int _groupId;
        public int GroupId
        {
            get => _groupId;
            set
            {
                _groupId = value;
                OnPropertyChanged(nameof(GroupId));
            }
        }

        private Option _selectGroup;
        public Option SelectGroup
        {
            get => _selectGroup;
            set
            {
                _selectGroup = value;
                OnPropertyChanged(nameof(SelectGroup));
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

        private Guid _companyById;
        public Guid CompanyById
        {
            get => _companyById;
            set
            {
                _companyById = value;
                OnPropertyChanged(nameof(CompanyById));
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

        private DateTime _createdDate;
        public DateTime CreatedDate
        {
            get => _createdDate;
            set {
                _createdDate = value;
                OnPropertyChanged(nameof(CreatedDate));
            }
        }

    }
}
