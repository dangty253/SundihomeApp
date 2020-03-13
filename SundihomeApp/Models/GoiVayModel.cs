using System;
namespace SundihomeApp.Models
{
    public class GoiVayModel : BaseModel
    {
        public Guid Id { get; set; }

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }

        private decimal _maxPrice;
        public decimal MaxPrice { get => _maxPrice; set { _maxPrice = value; OnPropertyChanged(nameof(MaxPrice)); } }

        private decimal _laiSuat;
        public decimal LaiSuat { get => _laiSuat; set { _laiSuat = value; OnPropertyChanged(nameof(LaiSuat)); } }

        private string _maxPriceFormatText;
        public string MaxPriceFormatText { get => _maxPriceFormatText; set { _maxPriceFormatText = value; OnPropertyChanged(nameof(MaxPriceFormatText)); } }

        private string _condition;
        public string Condition { get => _condition; set { _condition = value; OnPropertyChanged(nameof(Condition)); } }

        private string _description;
        public string Description { get => _description; set { _description = value; OnPropertyChanged(nameof(Description)); } }

        public string Image { get; set; }
    }
}
