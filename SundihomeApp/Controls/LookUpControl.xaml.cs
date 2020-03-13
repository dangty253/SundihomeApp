using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApp.Models;
using SundihomeApp.Views;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class LookUpControl : ContentView
    {
        public event EventHandler PreOpen;
        public event EventHandler<LookUpChangeEvent> SelectedItemChange;
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(LookUpControl), null, BindingMode.TwoWay);
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(LookUpControl), null, BindingMode.TwoWay);
        public object SelectedItem
        {
            get => (object)GetValue(SelectedItemProperty);
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly BindableProperty NameDipslayProperty = BindableProperty.Create(nameof(SelectedItem), typeof(string), typeof(LookUpControl), null, BindingMode.TwoWay, propertyChanged: DisplayNameChang);

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(LookUpControl), null, BindingMode.TwoWay, null);
        public IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set { SetValue(ItemsSourceProperty, value); } }

        public ContentView ModalPopup { get; set; }

        public BottomModal BottomModal { get; set; }

        public string NameDisplay
        {
            get
            {
                return (string)GetValue(NameDipslayProperty);
            }

            set
            {
                SetValue(NameDipslayProperty, value);
            }
        }

        public LookUpControl()
        {
            InitializeComponent();
            this.Entry.BindingContext = this;
            this.Entry.SetBinding(Entry.PlaceholderProperty, "Placeholder");
            this.BtnClear.SetBinding(Button.IsVisibleProperty, new Binding("SelectedItem") { Source = this, Converter = new Converters.NullToHideConverter() });
        }
        public void Clear_Clicked(object sender, EventArgs e)
        {
            this.SelectedItem = null;
            SelectedItemChange?.Invoke(this, new LookUpChangeEvent());
        }
        public void HideClearButton()
        {
            BtnClear.IsVisible = false;
        }
        public async void OpenLookUp_Tapped(object sender, EventArgs e)
        {
            if (PreOpen != null)
            {
                PreOpen.Invoke(this, EventArgs.Empty);
            }

            if (this.ItemsSource == null) return;

            var modal = new LookUpPage();
            modal.SetList(ItemsSource.Cast<object>().ToList(), NameDisplay);
            modal.lookUpListView.ItemTapped += async (lookUpSender, lookUpTapEvent) =>
            {
                bool change = false; // kiem tra xem co chon cai khac ko
                if (this.SelectedItem != lookUpTapEvent.Item)
                {
                    this.SelectedItem = lookUpTapEvent.Item;
                    change = true;
                }

                if (BottomModal != null)
                {
                    await BottomModal.Hide();
                }
                else
                {
                    CloseModal_Clicked(null, EventArgs.Empty);
                }

                if (change)
                {
                    SelectedItemChange?.Invoke(this, new LookUpChangeEvent());
                }
            };

            if (BottomModal != null)
            {
                BottomModal.Title = Placeholder;
                BottomModal.ModalContent = modal;
                await BottomModal.Show();
            }
            else
            {
                var ModalPopupContent = ModalPopup.Content as StackLayout;
                var ModalTitle = ((ModalPopupContent.Children[0] as Telerik.XamarinForms.Primitives.RadBorder).Content as StackLayout).FindByName<Label>("ModalTitle");
                ModalTitle.Text = Placeholder;
                await ShowModal(modal);
            }
        }

        private static void DisplayNameChang(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue == null) return;
            LookUpControl control = (LookUpControl)bindable;
            control.Entry.SetBinding(Entry.TextProperty, "SelectedItem." + newValue);
        }

        private async Task ShowModal(LookUpPage modal)
        {
            var ModalPopupContent = ModalPopup.Content as StackLayout;
            if (ModalPopupContent.Children.Count > 1)
            {
                ModalPopupContent.Children.Remove(ModalPopupContent.Children[1]);
            }
            ModalPopupContent.Children.Add(modal);
            ModalPopup.IsVisible = true;
            await ModalPopup.TranslateTo(0, 0, 150);
        }

        public async Task OpenModal()
        {
            this.OpenLookUp_Tapped(null, EventArgs.Empty);
        }

        public async void CloseModal_Clicked(object sender, EventArgs e)
        {
            await ModalPopup.TranslateTo(0, ModalPopup.Height, 50);
            ModalPopup.IsVisible = false;
        }
    }
}
