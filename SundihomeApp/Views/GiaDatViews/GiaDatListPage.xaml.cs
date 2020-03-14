using System;
using System.Collections.Generic;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.Views.GiaDatViews
{
    public class PageOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }

        public PageOption()
        {

        }

        public PageOption(int id, string name, Type type)
        {
            this.Id = id;
            this.Name = name;
            this.Type = type;
        }
    }

    public partial class GiaDatListPage : ContentPage
    {
        public List<PageOption> ProvinceList;
        public GiaDatListPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            ProvinceList = new List<PageOption>()
            {
                new PageOption(){Id=1,Name="Hà Nội",Type=typeof(GiaDatHaNoiPage)},
                new PageOption(){Id=79,Name="TP Hồ Chí Minh",Type= typeof(GiaDatHCMPage)},
                new PageOption(){Id=75,Name="Đồng Nai" ,Type=typeof(GiaDatDongNaiPage)},
                new PageOption(){Id=74,Name="Bình Dương" ,Type= typeof(GiaDatBinhDuongPage)},
                new PageOption(){Id=36,Name="Nam Định",Type=typeof(GiaDatNamDinhPage) },
                new PageOption(){Id=35,Name="Hà Nam" },
                new PageOption(){Id=27,Name="Bắc Ninh",Type=typeof(GiaDatBacNinhPage) },
                new PageOption(){Id=33,Name="Hưng Yên" },
                new PageOption(){Id=37,Name="Ninh Bình" },
                new PageOption(){Id=15,Name="Yên Bái",Type=typeof(GiaDatYenBaiPage) },
                new PageOption(){Id=11,Name="Điện Biên" },
                new PageOption(){Id=17,Name="Hòa Bình" },
                new PageOption(){Id=14,Name="Sơn La",Type=typeof(GiaDatSonLaPage) },
                new PageOption(){Id=6,Name="Bắc Kạn" },
                new PageOption(){Id=20,Name="Lạng Sơn" },
                new PageOption(){Id=19,Name="Thái Nguyên" },
                new PageOption(){Id=24,Name="Bắc Giang" },
                new PageOption(){Id=42,Name="Hà Tĩnh" },
                new PageOption(){Id=45,Name="Quảng Trị" },
                new PageOption(){Id=46,Name="Thừa Thiên Huế" },
                new PageOption(){Id=54,Name="Phú Yên" },
                new PageOption(){Id=62,Name="Kon Tum" },
                new PageOption(){Id=72,Name="Tây Ninh " },
                new PageOption(){Id=77,Name="Bà Rịa - Vũng Tàu" },
                new PageOption(){Id=87,Name="Đồng Tháp" },
                new PageOption(){Id=86,Name="Vĩnh Long" },
                new PageOption(){Id=83,Name="Bến Tre" },
                new PageOption(){Id=84,Name="Trà Vinh" },
                new PageOption(){Id=91,Name="Kiên Giang" },
                new PageOption(){Id=96,Name="Cà Mau" }
            };
            BindableLayout.SetItemsSource(BsdLvProvince, ProvinceList);
        }
        public async void GoTo_Province_Tapped(object sender, EventArgs e)
        {
            PageOption pageOption = (PageOption)((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (pageOption.Type != null)
            {
                Page page = (Page)Activator.CreateInstance(pageOption.Type);
                await this.Navigation.PushAsync(page);
            }
            else
            {
                await DisplayAlert("", Language.chuc_nang_dang_hoan_thien_vui_long_quay_lai_sau, Language.dong);
            }

        }
    }
}
