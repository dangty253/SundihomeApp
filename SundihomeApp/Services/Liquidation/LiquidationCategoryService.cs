using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb.Liquidation;
using SundihomeApp.IServices;
using SundihomeApp.IServices.ILiquidation;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.Services.Liquidation
{
    public class LiquidationCategoryService : ILiquidationCategoryService
    {
        //public List<LiquidationCategory> _categories = new List<LiquidationCategory>()
        //{
        //    new LiquidationCategory(1,Language.noi_that_gia_dinh,1,"ic_liquidation_noithat.png"),
        //    new LiquidationCategory(2,Language.ngoai_that_san_vuon,2,"ic_liquidation_sanvuon.png"),
        //    new LiquidationCategory(3,Language.do_gia_dung,3,"ic_liquidation_dogiadung.png"),
        //    new LiquidationCategory(4,Language.noi_that_van_phong,4,"ic_liquidation_office.png"),
        //    new LiquidationCategory(5,Language.xe_co,5,"ic_liquidation_moto.png"),
        //    new LiquidationCategory(6,Language.thoi_trang,6,"ic_liquidation_thoitrang.png"),
        //    new LiquidationCategory(7,Language.do_my_nghe,7,"ic_liquidation_mynghe.png"),
        //    new LiquidationCategory(8,Language.dien_tu_dien_may,8,"ic_liquidation_dientu.png"),
        //    new LiquidationCategory(9,Language.the_thao,9,"ic_liquidation_thethao.png"),
        //    new LiquidationCategory(10,Language.vat_nuoi_cay_canh,10,"ic_liquidation_vatnuoi.png"),
        //    new LiquidationCategory(11,Language.sach_bao,11,"ic_liquidation_sachbao.png"),
        //    new LiquidationCategory(12,Language.tre_em,12,"ic_liquidation_treem.png"),
        //    new LiquidationCategory(13,Language.khac,13,"ic_list.png"),
        //};

        public LiquidationCategory GetById(int Id)
        {
            
            return GetLiquidations().SingleOrDefault(x => x.Id == Id);
        }

        public List<LiquidationCategory> GetLiquidations()
        {
            var _categories = new List<LiquidationCategory>()
            {
                new LiquidationCategory(1,Language.noi_that_gia_dinh,1,"ic_liquidation_noithat.png"),
                new LiquidationCategory(2,Language.ngoai_that_san_vuon,2,"ic_liquidation_sanvuon.png"),
                new LiquidationCategory(3,Language.do_gia_dung,3,"ic_liquidation_dogiadung.png"),
                new LiquidationCategory(4,Language.noi_that_van_phong,4,"ic_liquidation_office.png"),
                new LiquidationCategory(5,Language.xe_co,5,"ic_liquidation_moto.png"),
                new LiquidationCategory(6,Language.thoi_trang,6,"ic_liquidation_thoitrang.png"),
                new LiquidationCategory(7,Language.do_my_nghe,7,"ic_liquidation_mynghe.png"),
                new LiquidationCategory(8,Language.dien_tu_dien_may,8,"ic_liquidation_dientu.png"),
                new LiquidationCategory(9,Language.the_thao,9,"ic_liquidation_thethao.png"),
                new LiquidationCategory(10,Language.vat_nuoi_cay_canh,10,"ic_liquidation_vatnuoi.png"),
                new LiquidationCategory(11,Language.sach_bao,11,"ic_liquidation_sachbao.png"),
                new LiquidationCategory(12,Language.tre_em,12,"ic_liquidation_treem.png"),
                new LiquidationCategory(13,Language.khac,13,"ic_list.png"),
            };
            return _categories.OrderBy(x => x.Sort).ToList();
        }
    }
}
