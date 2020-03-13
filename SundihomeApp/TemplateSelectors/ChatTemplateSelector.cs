using System;
using SundihomeApi.Entities.Mongodb;
using SundihomeApp.Models;
using SundihomeApp.Settings;
using SundihomeApp.Views.Cells;
using Xamarin.Forms;

namespace SundihomeApp.TemplateSelectors
{
    public class ChatTemplateSelector : DataTemplateSelector
    {
        DataTemplate incomingDataTemplate;
        DataTemplate incomingPostDataTemplate;
        DataTemplate incomingFurnitureProductDataTemplate;
        DataTemplate incomingLiquidationPostDataTemplate;
        DataTemplate outgoingDataTemplate;
        DataTemplate outgoingPostDataTemplate;
        DataTemplate outgoingFurnitureProductDataTemplate;
        DataTemplate outgoingLiquidationPostDataTemplate;
        DataTemplate breakTimeDataTemlate;

        public ChatTemplateSelector()
        {
            this.incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.incomingPostDataTemplate = new DataTemplate(typeof(IncomingPostViewCell));
            this.incomingFurnitureProductDataTemplate = new DataTemplate(typeof(IncomingFurnitureProductViewCell));
            this.incomingLiquidationPostDataTemplate = new DataTemplate(typeof(IncomingLiquidationPostViewCell));
            this.outgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
            this.outgoingPostDataTemplate = new DataTemplate(typeof(OutgoingPostViewCelll));
            this.outgoingFurnitureProductDataTemplate = new DataTemplate(typeof(OutgoingFurnitureProductViewCell));
            this.outgoingLiquidationPostDataTemplate = new DataTemplate(typeof(OutgoingLiquidationPostViewCell));
            this.breakTimeDataTemlate = new DataTemplate(typeof(BreakTimeViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as MessageItem;
            if (messageVm == null)
                return null;

            if (messageVm.SenderId == null && messageVm.ReceiveId == null)
            {
                return breakTimeDataTemlate;
            }
            else
            if (messageVm.SenderId == UserLogged.Id)
            {
                if (messageVm.Type == MessageItemType.Post)
                {
                    return outgoingPostDataTemplate;
                }
                else if (messageVm.Type == MessageItemType.FurnitureProduct)
                {
                    return outgoingFurnitureProductDataTemplate;
                }
                else if (messageVm.Type == MessageItemType.LiquidationPost)
                {
                    return outgoingLiquidationPostDataTemplate;
                }
                else
                {
                    return outgoingDataTemplate;
                }

            }
            else
            {
                if (messageVm.Type == MessageItemType.Post)
                {
                    return incomingPostDataTemplate;
                }
                else if (messageVm.Type == MessageItemType.FurnitureProduct)
                {
                    return incomingFurnitureProductDataTemplate;
                }
                else if (messageVm.Type == MessageItemType.LiquidationPost)
                {
                    return incomingLiquidationPostDataTemplate;
                }
                else
                {
                    return incomingDataTemplate;
                }
            }
        }

    }
}
