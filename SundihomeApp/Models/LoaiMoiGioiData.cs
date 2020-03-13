using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApp.Resources;

namespace SundihomeApp.Models
{
    public class LoaiMoiGioiData
    {
        public static List<Option> GetOptions()
        {
            return new List<Option>()
            {
                new Option{ Id = 0, Name = Language.can_ho_chung_cu},
                new Option{ Id = 1, Name=Language.nha_pho},
                new Option{ Id = 2, Name=Language.dat_nen},
                new Option{ Id = 3, Name=Language.bat_dong_san_nghi_duong },
                new Option{ Id = 4, Name="Shophouse & Officetel" },
                new Option{ Id = 5, Name=Language.phong_tro_cho_thue },
                new Option{ Id = 6, Name=Language.nha_xuong },
                new Option{ Id = 7, Name=Language.hoa_vien_nghia_trang },
                new Option{ Id = 8, Name=Language.khac}
            };
        }

        public static Option GetById(int id)
        {
            return GetOptions().SingleOrDefault(x => x.Id == id);
        }
    }
}
