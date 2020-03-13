using System;
using System.Collections.Generic;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.Models
{
    public class ProjectStatusModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ProjectStatusModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    

    public class ProjectStatusData
    {
        public static List<ProjectStatusModel> GetList()
        {
            return new List<ProjectStatusModel>()
            {
                new ProjectStatusModel(0,Language.dang_mo_ban),
                new ProjectStatusModel(1,Language.dang_ban_giao),
                new ProjectStatusModel(2,Language.sap_mo_ban),
            };
        }
    }
}

