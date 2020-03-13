using System;
using SundihomeApp.ViewModels;

namespace SundihomeApp.Models
{
    public class Option : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); } }

        public Option()
        {

        }

        public Option(int id, string name, string image, bool isSelected)
        {
            this.Id = id;
            this.Name = name;
            this.Image = image;
            this.IsSelected = isSelected;
        }
    }
}
