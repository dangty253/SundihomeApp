using System;
using System.Collections.Generic;

namespace SundihomeApp.Models
{
    public class PriceOptionData
    {
        public static List<PriceOption> Get()
        {
            return new List<PriceOption>()
            {
                new PriceOption(){Id=4, Name="đ",QuyDoi = 0},
                new PriceOption(){Id=0, Name="Trăm nghìn/m²",QuyDoi = 1000},
                new PriceOption(){Id=1, Name="Triệu",QuyDoi=1000000},
                new PriceOption(){Id=2, Name="Triệu/m²",QuyDoi=1000000},
                new PriceOption(){Id=3, Name="Tỷ",QuyDoi = 1000000000},
            };
        }
    }
}
