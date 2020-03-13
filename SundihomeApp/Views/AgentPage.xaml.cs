using System;
using System.Collections.Generic;
using SundihomeApp.Resources;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    [QueryProperty("SelectedIndex", "index")]
    public partial class AgentPage : ContentPage
    {
        public string SelectedIndex
        {
            set
            {
                //
                if (value == "0")
                {
                    Segment.SetActive(0);
                    Segment_OnSelectedIndexChanged(null, EventArgs.Empty);
                }
                else
                {
                    Segment.SetActive(1);
                    Segment_OnSelectedIndexChanged(null, EventArgs.Empty);
                }
            }
        }

        public AgentPage()
        {
            InitializeComponent();
            Segment.ItemsSource = new List<string>() { Language.moi_gioi, Language.goi_vay };
            Segment.SetActive(0);
            this.ContentAgent.Content = new AgentContentView();
        }

        private void Segment_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ContentAgent.IsVisible = false;
            ContentLoans.IsVisible = false;

            var index = Segment.GetCurrentIndex();
            if (index == 0)
            {
                if (this.ContentAgent.Content == null)
                {
                    this.ContentAgent.Content = new AgentContentView();
                }

                ContentAgent.IsVisible = true;
            }
            else
            {
                if (this.ContentLoans.Content == null)
                {
                    this.ContentLoans.Content = new LoanContentView();
                }
                ContentLoans.IsVisible = true;
            }
        }
    }
}
