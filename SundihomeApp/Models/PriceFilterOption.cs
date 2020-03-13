using System;
using System.Collections.Generic;

namespace SundihomeApp.Models
{
    public class PriceFilterOption : Option
    {
        public decimal? Value { get; set; }

        public PriceFilterOption(short id, string name, decimal? value)
        {
            this.Id = id;
            this.Name = name;
            this.Value = value;
        }

        public static List<PriceFilterOption> GetList()
        {
            return new List<PriceFilterOption>()
            {
                new PriceFilterOption(0,"300 triệu",300),
                new PriceFilterOption(1,"500 triệu",500),
                new PriceFilterOption(2,"800 triệu",800),
                new PriceFilterOption(3,"1 tỷ",1000000000),
                new PriceFilterOption(4,"3 tỷ",3000000000),
                new PriceFilterOption(5,"5 tỷ",5000000000),
                new PriceFilterOption(6,"7 tỷ",7000000000),
                new PriceFilterOption(7,"10 tỷ",10000000000),
                new PriceFilterOption(8,"20 tỷ",20000000000),
                new PriceFilterOption(9,"30 tỷ",30000000000),
            };
        }
    }
}
