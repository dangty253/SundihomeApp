using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class LookUpMultipleOptions : ContentView
    {
        public CenterModal CenterModal { get; set; }
        public event EventHandler OnSave;
        public event EventHandler OnDelete;

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(List<Option>), typeof(LookUpMultipleOptions), null, BindingMode.TwoWay, null, propertyChanged: ItemSourceChange);
        public List<Option> ItemsSource { get => (List<Option>)GetValue(ItemsSourceProperty); set { SetValue(ItemsSourceProperty, value); } }

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(LookUpMultipleOptions), null, BindingMode.TwoWay);
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        public static readonly BindableProperty SelectedIdsProperty = BindableProperty.Create(nameof(SelectedIds), typeof(List<int>), typeof(LookUpMultipleOptions), null, BindingMode.TwoWay, null, propertyChanged: ItemSourceChange);
        public List<int> SelectedIds { get => (List<int>)GetValue(SelectedIdsProperty); set { SetValue(SelectedIdsProperty, value); } }


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

        public LookUpMultipleOptions()
        {
            InitializeComponent();
            this.Entry.SetBinding(Entry.PlaceholderProperty, new Binding("Placeholder") { Source = this });
            this.Entry.SetBinding(Entry.TextProperty, new Binding("Text") { Source = this });
        }

        public async void OpenLookUp_Tapped(object sender1, EventArgs e1)
        {
            CenterModal.CustomCloseButton(CancelButton_Clicked);
            CenterModal.Title = Placeholder;
            Button saveButton = new Button()
            {
                Text = Language.luu,
                BackgroundColor = (Color)App.Current.Resources["MainDarkColor"],
                TextColor = Color.White,
                Padding = new Thickness(10, 5)
            };
            saveButton.Clicked += SaveButton_Clicked;

            Button cancelButton = new Button()
            {
                Text = Language.huy,
                TextColor = (Color)App.Current.Resources["MainDarkColor"],
                BackgroundColor = Color.White,
                BorderColor = (Color)App.Current.Resources["MainDarkColor"],
                Padding = new Thickness(10, 5),
                BorderWidth = 1
            };
            cancelButton.Clicked += CancelButton_Clicked;

            Grid gridButton = new Grid()
            {
                ColumnSpacing = 2,
                Margin = new Thickness(5, 0, 5, 5)
            };
            gridButton.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(35) });
            gridButton.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gridButton.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gridButton.Children.Add(cancelButton);
            gridButton.Children.Add(saveButton);
            Grid.SetRow(cancelButton, 0);
            Grid.SetRow(saveButton, 0);
            Grid.SetColumn(cancelButton, 0);
            Grid.SetColumn(saveButton, 1);

            CenterModal.Footer = gridButton;

            if (CenterModal != null)
            {
                var items = ItemsSource.Cast<object>().ToList();


                ListView lookUpListView = new ListView(ListViewCachingStrategy.RecycleElement);
                lookUpListView.BackgroundColor = Color.White;
                lookUpListView.HasUnevenRows = true;
                lookUpListView.SelectionMode = ListViewSelectionMode.None;
                lookUpListView.SeparatorVisibility = SeparatorVisibility.None;
                var dataTemplate = new DataTemplate(() =>
                {
                    RadBorder st = new RadBorder();
                    st.BorderThickness = new Thickness(0, 0, 0, 1);
                    st.BorderColor = Color.FromHex("#eeeeee");
                    st.Padding = 10;

                    Grid grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                    st.Content = grid;



                    Label lb = new Label();
                    lb.TextColor = Color.Black;
                    lb.FontSize = 16;
                    lb.SetBinding(Label.TextProperty, "Name");
                    Grid.SetColumn(lb, 0);
                    grid.Children.Add(lb);


                    Label labelCheck = new Label();
                    Grid.SetColumn(labelCheck, 1);
                    labelCheck.Text = "\uf00c";
                    labelCheck.TextColor = Color.DarkGreen;
                    labelCheck.FontFamily = FontAwesomeHelper.GetFont("FontAwesomeSolid");
                    labelCheck.SetBinding(Label.IsVisibleProperty, "IsSelected");
                    grid.Children.Add(labelCheck);

                    return new ViewCell { View = st };
                });
                lookUpListView.ItemTemplate = dataTemplate;
                lookUpListView.ItemsSource = ItemsSource;

                lookUpListView.ItemTapped += LookUpListView_ItemTapped;

                CenterModal.Body = lookUpListView;

                await CenterModal.Show();
            }

        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            ItemsSource.Where(x => x.IsSelected == true).ToList().ForEach(x => x.IsSelected = false);
            if (SelectedIds != null)
            {
                ItemsSource.Where(x => SelectedIds.Any(id => id == x.Id)).ToList().ForEach(x => x.IsSelected = true);
            }
            await CenterModal.Hide();
        }

        public async void SaveButton_Clicked(object sender, EventArgs e)
        {
            
            var checkedItems = ItemsSource.Where(x => x.IsSelected).ToList();
            if (checkedItems.Any())
            {
                string[] names = checkedItems.Select(x => x.Name).ToArray();
                SelectedIds = checkedItems.Select(x => x.Id).ToList();
                this.Text = string.Join(", ", names);
                SetList(checkedItems);
            }
            else
            {
                SelectedIds = null;
                this.Text = null;
                ClearFlexLayout();
            }
            
            await CenterModal.Hide();
            OnSave?.Invoke(this, EventArgs.Empty);
        }

        private void LookUpListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as Option;
            item.IsSelected = !item.IsSelected;
        }

        private void Clear_Clicked(object sender, EventArgs e)
        {
            this.Text = null;
            ItemsSource.ForEach(x => x.IsSelected = false);
            SelectedIds = null;
            OnDelete?.Invoke(this, EventArgs.Empty);
        }

        public void setData()
        {
            if (this.SelectedIds != null && this.SelectedIds.Any() && ItemsSource != null)
            {
                var selectedInSource = ItemsSource.Where(x => SelectedIds.Any(s => s == x.Id)).ToList();
                foreach (var item in selectedInSource)
                {
                    item.IsSelected = true;
                }
                this.Text = string.Join(", ", selectedInSource.Select(x => x.Name).ToArray());

                SetList(selectedInSource);
            }
            else
            {
                this.Text = null;
                ClearFlexLayout();
            }
        }
        public void SetList(List<Option> selectedInSource)
        {
            this.Entry.IsVisible = false;
            this.flexLayout.IsVisible = true;
            selectedInSource.Add(new Option()
            {
                Id = 0
            }) ;

            BindableLayout.SetItemsSource(flexLayout, selectedInSource);
            var last = flexLayout.Children.Last() as StackLayout;
            //var radBorder = flexLayout.Children.Last() as RadBorder;
            var radBorder = last.Children[0] as RadBorder;
            radBorder.BackgroundColor = Color.Gray;
            (radBorder.Content as Label).Text = "\uf00d";
            (radBorder.Content as Label).FontSize = Device.RuntimePlatform == Device.iOS ? 16 : 17;
            (radBorder.Content as Label).HorizontalTextAlignment = TextAlignment.Center;
            (radBorder.Content as Label).VerticalTextAlignment = TextAlignment.Center;
            (radBorder.Content as Label).FontFamily = FontAwesomeHelper.GetFont("FontAwesomeSolid");
            (radBorder.Content as Label).TextColor = Color.White;

            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += Clear_Clicked;

            radBorder.GestureRecognizers.Add(tap);
        }
        public void ClearFlexLayout()
        {
            flexLayout.IsVisible = false;
            Entry.IsVisible = true;
        }

        private static void ItemSourceChange(BindableObject bindable, object oldValue, object value)
        {
            LookUpMultipleOptions control = (LookUpMultipleOptions)bindable;
            control.setData();
        }
    }
}
