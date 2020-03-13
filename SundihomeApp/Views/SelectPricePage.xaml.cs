using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class SelectPricePage : ContentPage
    {
        private EventHandler BtnClicked;
        public List<PriceOption> Options { get; set; }
        public PriceOption SelectedOption { get; set; }
        public decimal? Price { get; set; }

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

        public SelectPricePage(decimal? price = null, short? selectedId = null)
        {
            InitializeComponent();
            Options = PriceOptionData.Get();
            this.SelectedOption = this.Options.Where(x => x.Id == (selectedId ?? 0)).SingleOrDefault();
            this.SelectedOption.IsSelected = true;
            if (price.HasValue)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    this.Price = price.Value;
                    this.Text = DecimalHelper.DecimalToText(price);
                }
                else
                {
                    this.Price = price.Value;
                    this.Text = DecimalHelper.DecimalToText(price).Replace(".", "").Replace(',', '.');
                }

            }

            this.BindingContext = this;

            EntryPrice.Focus();
        }

        protected override void OnAppearing()
        {
            EntryPrice.Focus();
            base.OnAppearing();
        }
        public async void Save_Clicked(object sender, EventArgs e)
        {
            if (Price.HasValue == false)
            {
                await DisplayAlert("", Language.vui_long_nhap_gia, Language.dong);
                return;
            }
            decimal maxTram = 9.99m;
            decimal maxTrieu = 999.99m;
            bool valid = true;
            if (SelectedOption.Id == 0 && this.Price > maxTram)
            {
                valid = false;
            }
            else if ((SelectedOption.Id == 1 || SelectedOption.Id == 2) && this.Price.Value > maxTrieu)
            {
                valid = false;
            }

            if (valid)
            {
                BtnClicked.Invoke(BtnSave, EventArgs.Empty);
            }
            else
            {
                await DisplayAlert("", Language.vui_long_nhap_don_vi_hop_le, Language.dong);
            }
        }

        public void Selected_Tapped(object sender, EventArgs e)
        {
            var old = this.Options.SingleOrDefault(x => x.IsSelected);
            if (old != null)
            {
                old.IsSelected = false;
            }

            SelectedOption = ((sender as RadBorder).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as PriceOption;
            SelectedOption.IsSelected = true;
        }

        public void SetSaveEvent(EventHandler e)
        {
            this.BtnClicked = e;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                string text = e.NewTextValue;
                if (text.EndsWith(".", StringComparison.OrdinalIgnoreCase))
                {
                    text = text.Substring(0, text.Length - 1) + ",";
                }

                text = text.Replace(",,", ",");

                if (string.IsNullOrWhiteSpace(text))
                {
                    this.Price = null;
                    return;
                }

                if (Device.RuntimePlatform == Device.iOS)
                {
                    if (text.LastIndexOf(',') == text.Length - 1)
                    {
                        Text = text;
                        return;
                    }

                    var splitFullText = text.Split(',').Where(x => (string.IsNullOrWhiteSpace(x) == false)).ToArray();
                    NumberFormatInfo nfi = new CultureInfo("vi-VN", false).NumberFormat;
                    if (splitFullText.Length == 2)
                    {
                        var giaTriSauDauPhay = splitFullText[1];
                        if (giaTriSauDauPhay.Length == 1) // chi co 1 ky tu sau day phai.
                        {
                            nfi.NumberDecimalDigits = 1;

                            this.Price = DecimalHelper.TextToDecimal(text);
                            this.Text = this.Price.Value.ToString("N", nfi);
                        }
                        else if (giaTriSauDauPhay.Length == 2)
                        {
                            nfi.NumberDecimalDigits = 2;

                            this.Price = DecimalHelper.TextToDecimal(text);
                            this.Text = this.Price.Value.ToString("N", nfi);
                        }
                        else if (giaTriSauDauPhay.Length == 2)
                        {
                            nfi.NumberDecimalDigits = 2;

                            this.Price = DecimalHelper.TextToDecimal(text);
                            this.Text = this.Price.Value.ToString("N", nfi);
                        }
                        //else if (giaTriSauDauPhay.Length > 3)
                        //{
                        //    giaTriSauDauPhay = giaTriSauDauPhay.Substring(0, 3);

                        //    nfi.NumberDecimalDigits = 3;

                        //    string newText = splitFullText[0] + "," + giaTriSauDauPhay;
                        //    this.Price = DecimalHelper.TextToDecimal(newText);
                        //    this.Text = this.Price.Value.ToString("N", nfi);
                        //}
                        else if (giaTriSauDauPhay.Length > 2)
                        {
                            giaTriSauDauPhay = giaTriSauDauPhay.Substring(0, 2);

                            nfi.NumberDecimalDigits = 2;

                            string newText = splitFullText[0] + "," + giaTriSauDauPhay;
                            this.Price = DecimalHelper.TextToDecimal(newText);
                            this.Text = this.Price.Value.ToString("N", nfi);
                        }
                    }
                    else if (splitFullText.Length > 2)
                    {
                        string newText = text.Substring(0, text.LastIndexOf(','));
                        this.Price = DecimalHelper.TextToDecimal(newText);
                        this.Text = this.Price.Value.ToString("N", nfi);
                    }
                    else  // nho hon 2. ko co dau phay.
                    {

                        nfi.NumberDecimalDigits = 0;
                        this.Price = DecimalHelper.TextToDecimal(text);
                        this.Text = this.Price.Value.ToString("N", nfi);
                    }
                }
                else
                {
                    //if (text.LastIndexOf(',') == text.Length - 1)
                    //{
                    //    Text = text;
                    //    return;
                    //}
                    var split = text.Split('.');
                    if (split.Length == 2 && split[1].Length > 0)
                    {
                        if (split[1].Length > 2)
                        {
                            string newText = split[0] + "." + split[1].Substring(0, 2);
                            this.Text = newText;
                            this.Price = DecimalHelper.TextToDecimal(newText.Replace(".", ","));
                            return;
                        }
                    }
                    this.Price = DecimalHelper.TextToDecimal(text.Replace(".", ","));
                }
            }
            catch (Exception ex)
            {
                this.Text = e.OldTextValue;
            }
        }
    }
}
