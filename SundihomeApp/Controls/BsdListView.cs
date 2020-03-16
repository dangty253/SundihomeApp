using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extended;

namespace SundihomeApp.Controls
{
    public class BsdListView : ListView
    {
        public BsdListView() : this(ListViewCachingStrategy.RetainElement)
        {

        }

        public BsdListView(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
        {
            this.ItemAppearing += OnItemAppearing;
            this.ItemTapped += OnItemTapped;

            this.IsPullToRefreshEnabled = true;
            this.HasUnevenRows = true;
            this.SelectionMode = ListViewSelectionMode.None;
            this.SeparatorVisibility = SeparatorVisibility.None;
            this.BackgroundColor = Color.FromHex("#eeeeee");
            this.SetBinding(RefreshCommandProperty, new Binding("RefreshCommand"));
            this.SetBinding(IsRefreshingProperty, new Binding("IsRefreshing"));
            this.SetBinding(ItemsSourceProperty, new Binding("Data"));

            InfiniteScrollBehavior behavior = new InfiniteScrollBehavior();
            behavior.SetBinding(InfiniteScrollBehavior.IsLoadingMoreProperty, new Binding("IsBusy"));
            this.Behaviors.Add(behavior);
        }
        public static readonly BindableProperty ItemAppearingCommandProperty =
            BindableProperty.Create(nameof(ItemAppearingCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        public ICommand ItemAppearingCommand
        {
            get { return (ICommand)GetValue(ItemAppearingCommandProperty); }
            set { SetValue(ItemAppearingCommandProperty, value); }
        }


        public static readonly BindableProperty TappedCommandProperty =
            BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        public ICommand TappedCommand
        {
            get { return (ICommand)GetValue(TappedCommandProperty); }
            set { SetValue(TappedCommandProperty, value); }
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (TappedCommand != null)
            {
                TappedCommand?.Execute(e.Item);
            }
            SelectedItem = null;
        }


        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (ItemAppearingCommand != null)
            {
                ItemAppearingCommand?.Execute(e.Item);
            }
        }
    }
}
