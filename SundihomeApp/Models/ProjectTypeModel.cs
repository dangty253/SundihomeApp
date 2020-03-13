using System;
using System.Collections.Generic;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.Models
{
    public class ProjectTypeModel
    {
        public short Id { get; set; }
        public string Name { get; set; }

        public ProjectTypeModel(short id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    public class ProjectTypeData
    {
        public static List<ProjectTypeModel> GetListProjectType()
        {
            return new List<ProjectTypeModel>()
            {
                new ProjectTypeModel(0,Language.cao_oc_van_phong),
                new ProjectTypeModel(1,Language.biet_thu),
                new ProjectTypeModel(2,Language.can_ho_cao_cap),
                new ProjectTypeModel(3,Language.can_ho_chung_cu),
                new ProjectTypeModel(4,Language.du_lich_nghi_duong),
                new ProjectTypeModel(5,Language.dat_phan_lo),
                new ProjectTypeModel(6,Language.do_thi_moi),
                new ProjectTypeModel(7,Language.khu_cong_nghiep),
                new ProjectTypeModel(8,Language.khu_dan_cu),
                new ProjectTypeModel(9,Language.nha_o_xa_hoi),
                new ProjectTypeModel(10,Language.phuc_hop),
                new ProjectTypeModel(11,Language.trung_tam_thuong_mai),
                new ProjectTypeModel(12,Language.khac),
            };
        }
    }
}

