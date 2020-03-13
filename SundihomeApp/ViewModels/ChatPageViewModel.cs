using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Furniture;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Mongodb.Furniture;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Settings;
using Xamarin.Forms;
using MongoDBPost = SundihomeApi.Entities.Mongodb.Post;
using MongoDbFurnitureProduct = SundihomeApi.Entities.Mongodb.Furniture.FurnitureProductChatMessage;
using SundihomeApi.Entities.Mongodb.Liquidation;
using SundihomeApp.Resources;

namespace SundihomeApp.ViewModels
{
    public class ChatPageViewModel : ListViewPageViewModel<MessageItem>
    {
        public FilterModel FilterModel { get; set; }
        public FilterFurnitureProductModel FilterFurnitureProductModel { get; set; }
        public IHubConnectionService _hubConnectionService;
        private string textToSend;
        public string TextToSend
        {
            get => textToSend;
            set
            {
                if (value != textToSend)
                {
                    textToSend = value;
                    OnPropertyChanged(nameof(TextToSend));
                }
            }
        }

        public Command OnSendCommand { get; set; }
        public ICommand MessageAppearingCommand { get; set; }
        public ICommand MessageDisappearingCommand { get; set; }

        public PostItemUser user;
        public ChatPageViewModel(string userId)
        {
            _hubConnectionService = DependencyService.Get<IHubConnectionService>();
            MessageAppearingCommand = new Command<MessageItem>(OnMessageAppearing);
            MessageDisappearingCommand = new Command<MessageItem>(OnMessageDisappearing);
            OnSendCommand = new Command(SendMessage);
            GetUser(userId);
            //ConnectToHub();
        }

        public void GetUser(string userId)
        {
            IUserService userService = DependencyService.Get<IUserService>();
            this.user = userService.Find(userId);
        }

        public async void ConnectToHub()
        {
            if (_hubConnectionService.IsConnect) // dang connect thi kupdate userid
            {
                Console.WriteLine("contact to hub if 1");
                _hubConnectionService.RegisterNewMessage(OnNewMessage);
                await _hubConnectionService.UpdateChatUserId(this.user.UserId);
            }
            else if (_hubConnectionService.Hub != null && !_hubConnectionService.IsConnect) // da tung connect av mat ketnoi. thi ket noi lai va upate userid.
            {
                Console.WriteLine("contact to hub if 2");
                await _hubConnectionService.ReStart();
                _hubConnectionService.RegisterNewMessage(OnNewMessage);
                await _hubConnectionService.UpdateChatUserId(this.user.UserId);
            }
            else
            {
                Console.WriteLine("contact to hub if 3");
                string connectionUrl = $"{Configuration.ApiConfig.HUB_SIGNALR}?UserId={UserLogged.Id}&FullName={UserLogged.FullName}&ChatUserId={user.UserId}";
                await _hubConnectionService.Start(connectionUrl, OnNewMessage);
            }
        }

        public async void DisconnecHub()
        {
            //if (hubConnection.State == HubConnectionState.Disconnected) return;
            //await hubConnection.StopAsync();
            await _hubConnectionService.Stop();
        }

        /// <summary>
        /// Trong ham nay. luc nao tin nhan dau tien cua list view cung la break time.
        /// </summary>
        /// <returns></returns>
        public async override Task LoadData()
        {
            var userToId = user.UserId;
            var resonse = await ApiHelper.Get<List<MessageItem>>($"api/message/" + userToId + "?page=" + Page, true);
            if (resonse.IsSuccess)
            {
                var messageItems = resonse.Content as List<MessageItem>;
                var count = messageItems.Count;
                if (count > 0)
                {
                    // khai bao 1 list moi
                    List<MessageItem> backMessageItem = new List<MessageItem>();

                    // lap de kiem tra. neu cai truoc 
                    for (int i = 0; i < count; i++)
                    {
                        var item = messageItems[i];
                        item.Receive = new PostItemUser()
                        {
                            Avatar = user.Avatar
                        };
                        backMessageItem.Add(item);

                        // khai bao 1 preItem 
                        MessageItem preItem = null;
                        if (i == 0) // lan dau lap vao danh sach moi nay thi ko co preitem set preitem trong list duw lieu hien tai.
                        {
                            // lay cuoi cung trong list data. ko lay ==0 la break time.
                            preItem = Data.Where(x => x.Id != null).LastOrDefault();
                        }
                        else // lay phai truoc trong list.
                        {
                            preItem = backMessageItem[i - 1];
                        }


                        if (preItem != null)
                        {
                            // neu tin nhan truoc va sau cua 1 nguoi, nhung khac nhau,thi van hien thi avatar nguoc lai xoa avatar
                            if (item.CreatedDate.ToShortDateString() == preItem.CreatedDate.ToShortDateString() && preItem.ReceiveId == item.ReceiveId && preItem.SenderId == item.SenderId)
                            {
                                preItem.Receive.Avatar = " ";
                            }
                        }
                    }
                    // group lai theo date.
                    var groupByCreatedDateList = backMessageItem.GroupBy(x => new
                    {
                        CreatedDate = x.CreatedDate.ToShortDateString()
                    }).Select(group => group.First()).OrderByDescending(x => x.CreatedDate).ToList();

                    MessageItem lastBreakDate = Data.LastOrDefault(x => x.Id == null);

                    int groupByCount = groupByCreatedDateList.Count;
                    for (int i = 0; i < groupByCount; i++)
                    {
                        var grouByItem = groupByCreatedDateList[i];
                        if (i == 0)
                        {

                        }
                        // vi du. lay dc list moi nay co 3 group 3 ngay khac nhau. kiem tra ngay breaktime cuoi cung trong list tren man hinh,
                        // --- co trung voi ngay breaktime dau tien trong du lieu moi ko. co thi xoa cai cu di. cai' moi' van~ them vao
                        if (lastBreakDate != null && lastBreakDate.CreatedDate.ToShortDateString() == grouByItem.CreatedDate.ToShortDateString())
                        {
                            // xoa breaktime phia truoc,load cai sau da co ngay reaktime nay roi
                            this.Data.Remove(lastBreakDate);
                        }

                        var messageItemInday = backMessageItem.Where(x => x.CreatedDate.ToShortDateString() == grouByItem.CreatedDate.ToShortDateString()).ToList();
                        for (int j = 0; j < messageItemInday.Count; j++)
                        {
                            Data.Add(messageItemInday[j]);
                        }

                        Data.Add(new MessageItem()
                        {
                            CreatedDate = grouByItem.CreatedDate
                        });
                    }
                }
                else
                {
                    OutOfData = true;
                }
            }
            OnPropertyChanged(nameof(IsEmptyList));
        }

        private void OnNewMessage(MessageItem newMessage)
        {
            if (newMessage.SenderId != user.UserId) return;
            Device.BeginInvokeOnMainThread(() =>
            {
                string avatar = user.AvatarFullUrl;
                //string avatar = "avatar.jpg";
                // first có nghĩa là tin nhắn đầu tiên trong listview nhưng bị đảo ngược thành .
                var lastMessage = Data.Where(x => x.Id != null).FirstOrDefault();
                if (lastMessage != null)
                {
                    // tin nhắn trước cũng là của người này thì ẩn avatar nhưng phải là cùng 1 ngày khác ngày là bị break
                    if (lastMessage.CreatedDate.ToShortDateString() == newMessage.CreatedDate.ToShortDateString())
                    {
                        if (lastMessage.SenderId == user.UserId)
                        {
                            avatar = " ";
                        }
                    }
                    else
                    {
                        //break time
                        Data.Insert(0, new MessageItem()
                        {
                            CreatedDate = newMessage.CreatedDate
                        });
                    }
                }
                else
                {
                    // break time
                    Data.Insert(0, new MessageItem()
                    {
                        CreatedDate = newMessage.CreatedDate
                    });
                }

                Data.Insert(0, new MessageItem()
                {
                    Id = newMessage.Id, // de tranh = 0 trung voi break time.
                    MessageContent = newMessage.MessageContent,
                    Type = newMessage.Type,
                    Post = newMessage.Post,
                    FurnitureProduct = newMessage.FurnitureProduct,
                    LiquidationPost = newMessage.LiquidationPost,
                    SenderId = user.UserId,
                    CreatedDate = newMessage.CreatedDate,
                    Receive = new PostItemUser()
                    {
                        Avatar = avatar,
                    }
                });
            });
        }


        async void SendMessage()
        {
            if (!_hubConnectionService.IsConnect) return;

            if (!string.IsNullOrEmpty(TextToSend))
            {
                if (string.IsNullOrWhiteSpace(TextToSend)) return;

                TextToSend = TextToSend.Trim();

                MessageItem messageToInsert = new MessageItem();
                messageToInsert.Id = Guid.NewGuid().ToString();
                messageToInsert.Type = MessageItemType.OnlyText;
                messageToInsert.MessageContent = TextToSend;
                messageToInsert.SenderId = UserLogged.Id;
                messageToInsert.CreatedDate = DateTime.Now;
                messageToInsert.Sender = new PostItemUser()
                {
                    Avatar = UserLogged.AvatarUrl.Replace(Configuration.ApiConfig.CloudStorageApiCDN, ""),
                };
                messageToInsert.ReceiveId = user.UserId;

                // lay thang dau tien ra kiem tra xem co phai ngay khac ko.
                var firstMessage = this.Data.Where(x => x.Id != null).FirstOrDefault();
                if (firstMessage != null)
                {
                    if (firstMessage.CreatedDate.Year != messageToInsert.CreatedDate.Year
                    || firstMessage.CreatedDate.Month != messageToInsert.CreatedDate.Month
                    || firstMessage.CreatedDate.Day != messageToInsert.CreatedDate.Day)
                    {
                        // break time // id = 0
                        Data.Insert(0, new MessageItem()
                        {
                            CreatedDate = messageToInsert.CreatedDate
                        });
                    }
                }
                else // chua co tin nhan nao.
                {
                    Data.Insert(0, new MessageItem()
                    {
                        CreatedDate = DateTime.Now
                    });
                }

                await _hubConnectionService.Hub.InvokeAsync("SendMessage", messageToInsert);

                Data.Insert(0, messageToInsert);
                TextToSend = string.Empty;
            }
        }

        public async void SendPostMessage(MongoDBPost post)
        {
            MessageItem messageToInsert = new MessageItem();
            messageToInsert.Id = Guid.NewGuid().ToString();
            messageToInsert.Type = MessageItemType.Post;
            messageToInsert.MessageContent = Language.tin_nhan_bat_dong_san;
            messageToInsert.SenderId = UserLogged.Id;
            messageToInsert.CreatedDate = DateTime.Now;
            messageToInsert.Sender = new PostItemUser()
            {
                Avatar = UserLogged.AvatarUrl.Replace(Configuration.ApiConfig.CloudStorageApiCDN, ""),
            };
            messageToInsert.ReceiveId = user.UserId;
            messageToInsert.Post = post;

            await _hubConnectionService.Hub.InvokeAsync("SendMessage", messageToInsert);

            Data.Insert(0, messageToInsert);
        }

        public async void SendFurnitureProductMessage(MongoDbFurnitureProduct product)
        {
            MessageItem messageToInsert = new MessageItem();
            messageToInsert.Id = Guid.NewGuid().ToString();
            messageToInsert.Type = MessageItemType.FurnitureProduct;
            messageToInsert.MessageContent = Language.tin_nhan_noi_that;
            messageToInsert.SenderId = UserLogged.Id;
            messageToInsert.CreatedDate = DateTime.Now;
            messageToInsert.Sender = new PostItemUser()
            {
                Avatar = UserLogged.AvatarUrl.Replace(Configuration.ApiConfig.CloudStorageApiCDN, ""),
            };
            messageToInsert.ReceiveId = user.UserId;
            messageToInsert.FurnitureProduct = product;


            await _hubConnectionService.Hub.InvokeAsync("SendMessage", messageToInsert);

            Data.Insert(0, messageToInsert);
        }

        public async void SendLiquidationtMessage(LiquidationCommentPost post)
        {
            MessageItem messageToInsert = new MessageItem();
            messageToInsert.Id = Guid.NewGuid().ToString();
            messageToInsert.Type = MessageItemType.LiquidationPost;
            messageToInsert.MessageContent = Language.tin_nhan_thanh_ly;
            messageToInsert.SenderId = UserLogged.Id;
            messageToInsert.CreatedDate = DateTime.Now;
            messageToInsert.Sender = new PostItemUser()
            {
                Avatar = UserLogged.AvatarUrl.Replace(Configuration.ApiConfig.CloudStorageApiCDN, ""),
            };
            messageToInsert.ReceiveId = user.UserId;
            messageToInsert.LiquidationPost = post;


            await _hubConnectionService.Hub.InvokeAsync("SendMessage", messageToInsert);

            Data.Insert(0, messageToInsert);
        }

        async void OnMessageAppearing(MessageItem message)
        {
            if (OutOfData == false && message.Id != null && message.Id == Data.Where(x => x.Id != null).LastOrDefault().Id)
            {
                await LoadMoreData();
            }
            //var idx = Data.IndexOf(message);
            //if (idx <= 6)
            //{
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        while (DelayedMessages.Count > 0)
            //        {
            //            Data.Insert(0, DelayedMessages.Dequeue());
            //        }
            //        ShowScrollTap = false;
            //        LastMessageVisible = true;
            //        PendingMessageCount = 0;
            //    });
            //}
        }

        void OnMessageDisappearing(MessageItem message)
        {
            //var idx = Data.IndexOf(message);
            //if (idx >= 6)
            //{
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        ShowScrollTap = true;
            //        LastMessageVisible = false;
            //    });

            //}
        }

        bool CanSendMessage()
        {
            //return IsConnected && !string.IsNullOrEmpty(TextToSend);
            return true;
        }
    }
}
