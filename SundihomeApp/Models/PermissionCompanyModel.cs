using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SundihomeApp.Models
{
    public class PermissionCompanyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public PermissionCompanyModel(int id,string name)
        {
            Id = id;
            Name = name;
        }
    }
    public class PermissionCompanyData
    {
        public static List<PermissionCompanyModel> GetListPermission()
        {
            return new List<PermissionCompanyModel>()
            {
                new PermissionCompanyModel(0,"Admin"),
                new PermissionCompanyModel(1,"Manager"),
                new PermissionCompanyModel(2,"Sale")
            };
        }
    }
}

