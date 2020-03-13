using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Models;
using SundihomeApp.ViewModels.Furniture;
using Xamarin.Forms;

namespace SundihomeApp.Views.Furniture
{
    public partial class FurnitureProductFiltePage : ContentPage
    {
        private FurnitureProductFiltePageViewModel viewModel;
        public FurnitureProductFiltePage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new FurnitureProductFiltePageViewModel();
            ModalPopupBackground.BackgroundColor = Color.FromRgba(0, 0, 0, 0.3);
            Init();
        }

        private async void Init()
        {
            await Task.WhenAll(viewModel.LoadParentCategories(), viewModel.GetProvinceAsync());
            loadingPopup.IsVisible = false;
        }
        private async void CloseModal_Clicked(object sender, EventArgs e)
        {
            await ModalPopup.TranslateTo(0, ModalPopup.Height, 50);
            ModalPopup.IsVisible = false;
        }

        private async void OnParentCategory_Changed(object sender, EventArgs e)
        {
            viewModel.ChildCategory = null;
            viewModel.ChildCategories.Clear();
            if (viewModel.ParentCategory != null)
            {
                await viewModel.LoadChildCategories();
            }
        }
        public void OnProvice_Change(object sender, LookUpChangeEvent e)
        {
            viewModel.District = null;
        }

        public void OnDistrict_Change(object sender, LookUpChangeEvent e)
        {
            viewModel.Ward = null;
        }

        public async void Filter_Clicked(object sender, EventArgs e)
        {
            FilterFurnitureProductModel filterModel = new FilterFurnitureProductModel();
            //if (segmentPostType.SelectedIndex != -1)
            //{
            //    filterModel.PostType = (short)segmentPostType.SelectedIndex;
            //}

            if (viewModel.ParentCategory != null)
            {
                filterModel.ParentCategoryId = viewModel.ParentCategory.Id;
            }

            if (viewModel.ChildCategory != null)
            {
                filterModel.CategoryId = viewModel.ChildCategory.Id;
            }

            if (viewModel.Province != null)
            {
                filterModel.ProvinceId = viewModel.Province.Id;
            }

            if (viewModel.District != null)
            {
                filterModel.DistrictId = viewModel.District.Id;
            }

            if (viewModel.Ward != null)
            {
                filterModel.WardId = viewModel.Ward.Id;
            }

            if (!string.IsNullOrWhiteSpace(viewModel.Keyword))
            {
                filterModel.Keyword = viewModel.Keyword.Trim();
            }

            await Shell.Current.Navigation.PushAsync(new FurnitureProductFilterResultPage(filterModel));
        }

        public void ResetForm_Clicked(object sender, EventArgs e)
        {
            //segmentPostType.SelectedIndex = -1;
            viewModel.ParentCategory = null;
            OnParentCategory_Changed(null, EventArgs.Empty);
            //viewModel.Project = null;
            //viewModel.LoaiBatDongSan = null;
            viewModel.Province = null;
            //viewModel.PriceFrom = null;
            //viewModel.PriceTo = null;
            //viewModel.Area = null;
            //viewModel.SoPhongNgu = null;
            //viewModel.SoPhongTam = null;
            //viewModel.Keyword = null;
        }
    }
}
