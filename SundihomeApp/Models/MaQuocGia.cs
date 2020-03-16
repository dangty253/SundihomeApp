using System;
namespace SundihomeApp.Models
{
    public class MaQuocGia
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Value => Code != "" ? "+" + Code : "";

        public MaQuocGia(string name, string code)
        {
            Name = name;
            Code = code;
        }
        public string Display => Code != "" ? Name + $" (" + Value + ")" : Name;

        public string SelectedItemDisplay
        {
            get
            {
                if (this.Value == "")
                {
                    return "---";
                }
                else
                {
                    return this.Value;
                }
            }
        }
    }
}
