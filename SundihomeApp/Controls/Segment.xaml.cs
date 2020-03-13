using System;
using System.Collections.Generic;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class Segment : RadBorder
    {
        private const string ACTIVE = "Active";
        private const string INACTIVE = "InActive";
        public event EventHandler OnSelectedIndexChanged;
        private int CurrentIndex;

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(List<string>), typeof(Segment), null, BindingMode.TwoWay, null, propertyChanged: ItemSourceChange);
        public List<string> ItemsSource
        {
            get => (List<string>)GetValue(ItemsSourceProperty);
            set
            {
                SetValue(ItemsSourceProperty, value);
                OnPropertyChanged(nameof(ItemsSource));
            }
        }

        public Segment()
        {
            InitializeComponent();
        }

        public void SetActive(int index)
        {
            if (this.CurrentIndex > -1)
            {
                RadBorder radBordercu = this.grSegment.Children[this.CurrentIndex] as RadBorder;

                VisualStateManager.GoToState(radBordercu, INACTIVE);
                VisualStateManager.GoToState((radBordercu.Content as Label), INACTIVE);
            }

            RadBorder rad = grSegment.Children[index] as RadBorder;
            VisualStateManager.GoToState(rad, ACTIVE);
            VisualStateManager.GoToState((rad.Content as Label), ACTIVE);
            this.CurrentIndex = index;
        }

        public int GetCurrentIndex()
        {
            return this.CurrentIndex;
        }

        public void SetGrid()
        {
            if (ItemsSource.Count > 0)
            {
                for (int i = 0; i < ItemsSource.Count; i++)
                {
                    RadBorder radBorder = new RadBorder();
                    radBorder.Style = (Style)this.Resources["RadStyle"];

                    Label label = new Label()
                    {
                        Text = ItemsSource[i],
                        Style = (Style)this.Resources["LabelStyle"]
                    };
                    VisualStateManager.GoToState(label, INACTIVE);


                    radBorder.Content = label;

                    TapGestureRecognizer tapGesture = new TapGestureRecognizer();
                    tapGesture.CommandParameter = i;
                    tapGesture.Tapped += TapGesture_Tapped;
                    radBorder.GestureRecognizers.Add(tapGesture);

                    Grid.SetColumn(radBorder, i);


                    VisualStateManager.GoToState(radBorder, INACTIVE);
                    grSegment.Children.Add(radBorder);
                }
                this.CurrentIndex = -1;
            }

        }

        private void TapGesture_Tapped(object sender, EventArgs e)
        {
            // xu ly

            int index = (int)((sender as RadBorder).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;

            this.SetActive(index);

            OnSelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        private static void ItemSourceChange(BindableObject bindable, object oldValue, object value)
        {
            Segment control = (Segment)bindable;
            control.SetGrid();
        }
    }
}
