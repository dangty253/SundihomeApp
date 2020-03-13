using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SundihomeApp.ViewModels;

namespace SundihomeApp.Models
{
    public enum NotificationType
    {
        ViewPost = 0,
        ViewAppointment = 1,
        ViewPostItem = 2,
        ViewMessage = 3,
        UpdateVersion = 4,
        VIewFurniturePostItem = 5,
        ViewLiquidationPostItem = 6,
        ViewB2BPostItem = 7,
        ViewInternalPostItem = 8,
        RegisterEmployeeSuccess = 9
    }
    public class NotificationModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }

        public NotificationType NotificationType { get; set; }

        public Guid? PostId { get; set; } // type 0
        public string PostItemId { get; set; } // type 2 // type 5
        public Guid AppointmentId { get; set; } // type 1
        public string ChatUserId { get; set; } // type 3
        public string NewVersion { get; set; } // type 4 

        [BsonIgnore]
        public int CurrentBadgeCount { get; set; } // su dung trong messagereceiver

        public string Thumbnail { get; set; }

        public Guid UserId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
    }
}
