using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApp.ViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Extended;

namespace SundihomeApp.Views
{
    public partial class LookUpPage : ContentView
    {
        public ListView lookUpListView { get; set; }
        public LookUpPage()
        {
            InitializeComponent();
        }


        public void SetList<T>(List<T> list, string Name) where T : class
        {
            lookUpListView = new ListView(ListViewCachingStrategy.RecycleElement);
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
                Label lb = new Label();
                lb.TextColor = Color.Black;
                lb.FontSize = 16;
                lb.SetBinding(Label.TextProperty, Name);
                st.Content = lb;
                return new ViewCell { View = st };
            });

            lookUpListView.ItemTemplate = dataTemplate;
            lookUpListView.ItemsSource = list;

            //this.Content = lookUpListView;

            Grid.SetRow(lookUpListView, 1);
            MainGrid.Children.Add(lookUpListView);


            searchBar.TextChanged += async (object sender, TextChangedEventArgs e) =>
            {
                var text = e.NewTextValue;
                if (string.IsNullOrWhiteSpace(text))
                {
                    lookUpListView.ItemsSource = list;
                }
                else
                {
                    lookUpListView.ItemsSource = list.Where(x => GetValObjDy(x, Name).ToString().ToLower().Contains(text.ToLower()));
                }
            };
        }
        public object GetValObjDy(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }
    }
}
