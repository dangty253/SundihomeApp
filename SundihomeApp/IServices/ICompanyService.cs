using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using Xamarin.Forms;

namespace SundihomeApp.IServices
{
    public interface ICompanyService
    {
        InviteUser FindInviteUser(string NumPhone, string OptCode);
        bool FindInvitedUser(string Phone,string CompanyId);
        void UpdateInviteUser (InviteUser inviteUser);
        void DeleteInvitedUser(string UserId);
        List<InviteUser> GetListInvitedUser(string NumPhone, string CompanyId);
    }
}

