using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb.Liquidation;

namespace SundihomeApp.IServices.ILiquidation
{
    public interface ILiquidationCategoryService
    {
        List<LiquidationCategory> GetLiquidations();
        LiquidationCategory GetById(int Id);
    }
}
