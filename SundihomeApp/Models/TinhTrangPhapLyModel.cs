using System;
using System.Collections.Generic;
using SundihomeApp.Resources;

namespace SundihomeApp.Models
{
    public class TinhTrangPhapLyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public TinhTrangPhapLyModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class TinhTrangPhapLyData
    {
        public static List<TinhTrangPhapLyModel> GetList()
        {
            return new List<TinhTrangPhapLyModel>()
            {
                new TinhTrangPhapLyModel(0,Language.so_do),
                new TinhTrangPhapLyModel(1,Language.so_hong),
                new TinhTrangPhapLyModel(2,"Sổ trắng"),
                new TinhTrangPhapLyModel(3,"Giấy chứng nhận quyền sở hữu"),
                new TinhTrangPhapLyModel(4,"Giấy tờ hợp lệ"),
                new TinhTrangPhapLyModel(5,Language.giay_phep_xay_dung),
                new TinhTrangPhapLyModel(6,Language.giay_phep_kinh_doanh),
                new TinhTrangPhapLyModel(7,"Giấy viết tay"),
                new TinhTrangPhapLyModel(8,"Đang hợp thức hoá"),
                new TinhTrangPhapLyModel(9,Language.chua_xac_dinh),
            };
        }
    }
}
