using System;
using System.Collections.Generic;

namespace SundihomeApp.Models
{
    public class AreaFilterOtion : Option
    {
        public decimal? ValueFrom { get; set; }
        public decimal? ValueTo { get; set; }

        public AreaFilterOtion(short id, string name, decimal? from, decimal? to)
        {
            this.Id = id;
            this.Name = name;
            this.ValueFrom = from;
            this.ValueTo = to;
        }

        public static List<AreaFilterOtion> GetList()
        {
            return new List<AreaFilterOtion>()
            {
                new AreaFilterOtion(1,"Dưới 30m2",null,30),
                new AreaFilterOtion(2,"30m2 - 50m2",30,50),
                new AreaFilterOtion(3,"50m2 - 70m2",50,70),
                new AreaFilterOtion(4,"70m2 - 100m2",70,100),
                new AreaFilterOtion(5,"100m2 - 150m2",100,150),
                new AreaFilterOtion(6,"150m2 - 300m2",150,300),
                new AreaFilterOtion(7,"300m2 - 500m2",300,500),
                new AreaFilterOtion(8,"500m2 trở lên",500,null)
            };
        }
    }
}
