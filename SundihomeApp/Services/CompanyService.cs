using System;
using System.Collections.Generic;
using MongoDB.Driver;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApp.IServices;
using Xamarin.Forms;

namespace SundihomeApp.Services
{
    public class CompanyService : ICompanyService
    {
        private MongoClient _client;
        public IMongoCollection<InviteUser> _inviteUser { get; private set; }
        private IMongoDbService _mongoDbService;
        public CompanyService()
        {
            _mongoDbService = DependencyService.Get<IMongoDbService>();
            _inviteUser = _mongoDbService.GetCollection<InviteUser>("InviteUser");
        }

        public InviteUser FindInviteUser(string NumPhone, string OptCode)
        {
            return _inviteUser.Find(x => x.PhoneNumber == NumPhone && x.InviteCode == OptCode).SingleOrDefault();
        }

        public void UpdateInviteUser(InviteUser inviteUser)
        {
            _inviteUser.ReplaceOne(x => x.Id == inviteUser.Id, inviteUser);
        }

        public bool FindInvitedUser(string Phone, string CompanyId)
        {
            long countInvitedUser = _inviteUser.Find(x => x.PhoneNumber == Phone && x.CompanyId == CompanyId).Count();
            if (countInvitedUser > 0)
            {
                return true;
            }
            return false;
        }

        public List<InviteUser> GetListInvitedUser(string NumPhone, string CompanyId)// lay ra danh sach invitedUser de update thanh HUY
        {
            List<InviteUser> listInvitedUser =  _inviteUser.Find(x => x.PhoneNumber == NumPhone && x.CompanyId == CompanyId).ToList();
            return listInvitedUser;
        }

        public void DeleteInvitedUser(string UserId)
        {
            _inviteUser.DeleteOne(x => x.Id == UserId);
        }

    }
}

