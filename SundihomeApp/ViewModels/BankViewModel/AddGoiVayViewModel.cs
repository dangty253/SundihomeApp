using System;
using System.Collections.Generic;
using SundihomeApp.Models;
using SundihomeApp.Resources;

namespace SundihomeApp.ViewModels.BankViewModel
{
    public class AddGoiVayViewModel : BaseViewModel
    {
        private GoiVayModel _goiVayModel;
        public GoiVayModel GoiVayModel { get => _goiVayModel; set { _goiVayModel = value; OnPropertyChanged(nameof(Models.GoiVayModel)); } }

        private Option _maxTimeOption;
        public Option MaxTimeOption { get => _maxTimeOption; set { _maxTimeOption = value; OnPropertyChanged(nameof(MaxTimeOption)); } }

        private Option _maxTimeUnitOption;
        public Option MaxTimeUnitOption { get => _maxTimeUnitOption; set { _maxTimeUnitOption = value; OnPropertyChanged(nameof(MaxTimeUnitOption)); } }

        public int BankId { get; set; }

        public List<Option> MaxTimeOptions { get; set; }
        public List<Option> MaxTimeUnitOptions = new List<Option>()
        {
            new Option(){Id=0,Name=Language.year},
            new Option(){Id=1,Name=Language.month},
        };

        public AddGoiVayViewModel()
        {
            GoiVayModel = new GoiVayModel();
            MaxTimeOptions = new List<Option>();
            for (int i = 1; i < 200; i++)
            {
                MaxTimeOptions.Add(new Option() { Id = i });
            }
        }
    }
}
