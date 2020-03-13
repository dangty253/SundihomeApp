using System;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.CompanyViewModels
{
    public class FilterCompanyPostListViewModel : ListViewPageViewModel2<Post>
    {
        public Guid CompanyId { get; set; }
        public bool? IsCommitment { get; set; }
        public int? Status { get; set; }
        public string Keyword;

        public FilterCompanyPostListViewModel()
        {
            PreLoadData = new Command(() =>
            {
                string url = ApiRouter.COMPANY_GETPOSTLIST + $"?CompanyId={this.CompanyId}&page={this.Page}";
                if (this.IsCommitment.HasValue)
                {
                    url += $"&isCommitment={this.IsCommitment.Value}";
                }
                if (this.Status.HasValue)
                {
                    url += $"&Status={this.Status.Value}";
                }
                if (!string.IsNullOrWhiteSpace(Keyword))
                {
                    url += $"&keyword={Keyword}";
                }

                ApiUrl = url;
            });
        }
    }
}
