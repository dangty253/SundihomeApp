using System;
using System.Collections.Generic;
using SundihomeApp.Resources;

namespace SundihomeApp.Models
{
    public class HuongModel
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public HuongModel()
        {
        }
        public HuongModel(short id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class HuongData
    {
        public static List<HuongModel> GetList()
        {
            return new List<HuongModel>()
            {
                new HuongModel(0,Language.huong_dong),
                new HuongModel(1,Language.huong_tay),
                new HuongModel(2,Language.huong_nam),
                new HuongModel(3,Language.huong_bac),
                new HuongModel(4,Language.huong_dong_bac),
                new HuongModel(5,Language.huong_tay_bac),
                new HuongModel(6,Language.huong_dong_nam),
                new HuongModel(7,Language.huong_tay_nam),
            };
        }
    }
}
