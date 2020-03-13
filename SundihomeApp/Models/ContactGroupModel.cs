using System;
using System.Collections.Generic;
using SundihomeApp.Resources;

namespace SundihomeApp.Models
{
    public class ContactGroupModel
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public bool RowCheck { get; set; }

        public ContactGroupModel(short id, string name)
        {
            Id = id;
            Name = name;
        }

        public static List<ContactGroupModel> GetList()
        {
            List<ContactGroupModel> list = new List<ContactGroupModel>();
            list.Add(new ContactGroupModel(0, Language.moi));
            list.Add(new ContactGroupModel(1, Language.dau_tu));
            list.Add(new ContactGroupModel(2, Language.da_mua));
            return list;
        }
    }
}
