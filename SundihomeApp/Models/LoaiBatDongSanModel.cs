using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApp.Resources;

namespace SundihomeApp.Models
{
    public class LoaiBatDongSanModel
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public bool RowCheck { get; set; }

        public LoaiBatDongSanModel(short id, string name, int? parentId = null)
        {
            Id = id;
            Name = name;
        }
        public LoaiBatDongSanModel()
        {

        }

        public static LoaiBatDongSanModel GetById(int Id)
        {
            return GetList(null).SingleOrDefault(x => x.Id == Id);
        }

        public static List<LoaiBatDongSanModel> GetList(int? type)
        {
            List<LoaiBatDongSanModel> list = new List<LoaiBatDongSanModel>();
            if (type == 0 || type == 2) // ban + can mua
            {
                list.Add(new LoaiBatDongSanModel(0, Language.can_ho));
                list.Add(new LoaiBatDongSanModel(2, Language.nha_rieng));
                list.Add(new LoaiBatDongSanModel(3, Language.nha_mat_pho_shophouse));
                list.Add(new LoaiBatDongSanModel(4, Language.nha_biet_thu_lien_ke));
                list.Add(new LoaiBatDongSanModel(9, Language.dat));
                list.Add(new LoaiBatDongSanModel(10, Language.bat_dong_san_khac));
            }
            else if (type == 1 || type == 3) // cho thue can thue
            {
                list.Add(new LoaiBatDongSanModel(0, Language.can_ho));
                list.Add(new LoaiBatDongSanModel(1, Language.can_ho_dich_vu));
                list.Add(new LoaiBatDongSanModel(2, Language.nha_rieng));
                list.Add(new LoaiBatDongSanModel(3, Language.nha_mat_pho_shophouse));
                list.Add(new LoaiBatDongSanModel(4, Language.nha_biet_thu_lien_ke));
                list.Add(new LoaiBatDongSanModel(5, Language.nha_tro_phong_tro));
                list.Add(new LoaiBatDongSanModel(6, Language.van_phong));
                list.Add(new LoaiBatDongSanModel(7, Language.cua_hang_mat_hang_ban_le));
                list.Add(new LoaiBatDongSanModel(8, Language.dat_nha_xuong_kho_bai));
                //list.Add(new LoaiBatDongSanModel(9, "Đất"));
                list.Add(new LoaiBatDongSanModel(10, Language.bat_dong_san_khac));
            }
            else
            {
                list.Add(new LoaiBatDongSanModel(0, Language.can_ho));
                list.Add(new LoaiBatDongSanModel(1, Language.can_ho_dich_vu));
                list.Add(new LoaiBatDongSanModel(2, Language.nha_rieng));
                list.Add(new LoaiBatDongSanModel(3, Language.nha_mat_pho_shophouse));
                list.Add(new LoaiBatDongSanModel(4, Language.nha_biet_thu_lien_ke));
                list.Add(new LoaiBatDongSanModel(5, Language.nha_tro_phong_tro));
                list.Add(new LoaiBatDongSanModel(6, Language.van_phong));
                list.Add(new LoaiBatDongSanModel(7, Language.cua_hang_mat_hang_ban_le));
                list.Add(new LoaiBatDongSanModel(8, Language.dat_nha_xuong_kho_bai));
                list.Add(new LoaiBatDongSanModel(9, Language.dat));
                list.Add(new LoaiBatDongSanModel(10, Language.bat_dong_san_khac));
            }
            return list;
        }
    }
}
