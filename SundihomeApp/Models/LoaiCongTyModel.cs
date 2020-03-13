using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.Models
{
    public class LoaiCongTyModel
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public LoaiCongTyModel(short id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    public class LoaiCongTyData
    {
        public static List<LoaiCongTyModel> GetListNganhNghe()
        {
            return new List<LoaiCongTyModel>()
            {
                new LoaiCongTyModel(0,Language.dau_tu_du_an),
                new LoaiCongTyModel(1,Language.vat_lieu_xay_dung),
                new LoaiCongTyModel(2,Language.trang_tri_noi_that),
                new LoaiCongTyModel(3,Language.thiet_ke_kien_truc),
                new LoaiCongTyModel(4,Language.thi_cong_xay_dung),
                new LoaiCongTyModel(5,Language.tai_chinh),
                new LoaiCongTyModel(6,Language.phap_ly),
                new LoaiCongTyModel(7,Language.sua_chua),
                new LoaiCongTyModel(8,Language.van_chuyen),
                new LoaiCongTyModel(9,Language.moi_gioi_dia_oc),
                new LoaiCongTyModel(10,Language.khac),
            };
        }
        public static LoaiCongTyModel GetById(short Id)
        {
            return GetListNganhNghe().Where(x => x.Id == Id).SingleOrDefault();
        }
    }
    
}

