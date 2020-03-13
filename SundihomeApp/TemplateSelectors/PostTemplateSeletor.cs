using System;
using SundihomeApi.Entities;
using Xamarin.Forms;

namespace SundihomeApp.TemplateSelectors
{
    public class PostTemplateSeletor : DataTemplateSelector
    {
        DataTemplate HasImageTemplate;
        DataTemplate NoImageTemplate;
        public PostTemplateSeletor()
        {
            this.HasImageTemplate = new DataTemplate(typeof(Views.Cells.PostViewCell));
            this.NoImageTemplate = new DataTemplate(typeof(Views.Cells.PostNoImageViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var post = item as Post;
            if (post.PostType == 0 || post.PostType == 1)
            {
                return HasImageTemplate;
            }
            else
            {
                return NoImageTemplate;
            }
        }
    }
}

