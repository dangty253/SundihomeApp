using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class ScrollTab : ScrollView
    {
        public event EventHandler<SelectedIndexChangedEventArgs> SelectedIndexChanged;

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(ScrollTab), null, BindingMode.TwoWay, null, propertyChanged: ItemSourceChange);
        public IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set { SetValue(ItemsSourceProperty, value); } }

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int?), typeof(ScrollTab), null, BindingMode.TwoWay, propertyChanged: IndexChange);
        public int? SelectedIndex { get => (int?)GetValue(SelectedIndexProperty); set { SetValue(SelectedIndexProperty, value); } }

        public int? oldValue { get; set; }
        public int? newValue { get; set; }
        public ScrollTab()
        {
            InitializeComponent();
            this.BindingContext = this;
            StackLayoutFilter.SetBinding(BindableLayout.ItemsSourceProperty, "ItemsSource");
        }

        private async void OnTaped(object sender, EventArgs e)
        {
            var radBorder = sender as RadBorder;
            var index = StackLayoutFilter.Children.IndexOf(radBorder);
            if (index != SelectedIndex)
                SelectedIndex = index;

        }

        public class SelectedIndexChangedEventArgs
        {
            public int? Index { get; set; }
        }

        public void SetData(BindableObject bindable)
        {
            if(ItemsSource!=null && SelectedIndex!=null)
            {
                try
                {
                    ScrollTab scrollTab = (ScrollTab)bindable;
                    StackLayout stackLayout = scrollTab.FindByName<StackLayout>("StackLayoutFilter");
                    if (scrollTab.oldValue != null)
                    {
                        var old = stackLayout.Children[(int)scrollTab.oldValue] as RadBorder;
                        old.BackgroundColor = Color.White;
                    }
                    if (scrollTab.newValue != null)
                    {
                        var newRad = stackLayout.Children[(int)scrollTab.newValue] as RadBorder;
                        newRad.BackgroundColor = Color.FromHex("#eeeeee");
                    }
                    int? index = null;
                    if (scrollTab.newValue != null)
                    {
                        index = (int)scrollTab.newValue;
                    }
                    if(scrollTab.oldValue != null && scrollTab.newValue != null)
                    scrollTab.SelectedIndexChanged?.Invoke(scrollTab, new SelectedIndexChangedEventArgs()
                    {
                        Index = index
                    });
                }
                catch (Exception ex)
                {

                }

            }
            
        }
        private static void ItemSourceChange(BindableObject bindable, object oldValue, object newValue)
        {
            ScrollTab scrollTab = (ScrollTab)bindable;
            scrollTab.SetData(bindable);
        }
        private static void IndexChange(BindableObject bindable, object oldValue, object newValue)
        {
            ScrollTab scrollTab = (ScrollTab)bindable;
            scrollTab.oldValue = (int?)oldValue;
            scrollTab.newValue = (int?)newValue;
            scrollTab.SetData(bindable);
        }
    }
}
