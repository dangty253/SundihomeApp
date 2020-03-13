using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace SundihomeApp.Models
{
    public class FloatButtonItem : BaseModel
    {

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public string Icon { get; set; }

        private string _fontFamily;
        public string FontFamily
        {
            get => _fontFamily;
            set
            {
                _fontFamily = value;
                OnPropertyChanged(nameof(FontFamily));
            }
        }


        public ICommand OnClickCommand { get; set; }
        public EventHandler OnClickeEvent { get; set; }

        public FloatButtonItem(string text, string fontfamily, string icon, Command onClickCommand, EventHandler onClickeEvent)
        {
            this.Text = text;
            this.Icon = icon;
            this.FontFamily = fontfamily;
            this.OnClickCommand = onClickCommand;
            this.OnClickeEvent = onClickeEvent;
        }
    }
}
