using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndividualProjectInventoryManagement.Data
{
    // ------------------------------------------------------------------
    // SampleData - a ready-made shop to start with
    // ------------------------------------------------------------------
    // Used for testing now. Later it is also what the program offers to load
    // the first time it starts, when no saved file exists yet.
    //
    // The products are mixed on purpose: some well stocked, some low and one
    // out of stock, so the colours and the report have something to show.
    // The IDs are left at 0: Inventory hands them out when a product is added.
    // ------------------------------------------------------------------
    public static class SampleData
    {
        public static List<Product> Create()
        {
            return new List<Product>
            {
                // ---------------- Yarn: size = grams, length = metres ----------------
                //  brand              name                  material              size  length  qty  price
                new Yarn(YarnBrand.Jarbo,   YarnName.MorningMist, YarnMaterial.Merino,  50m,  200m,  24,  39m),
                new Yarn(YarnBrand.Jarbo,   YarnName.Dusk,        YarnMaterial.Wool,    100m, 200m,  8,   59m),   // low
                new Yarn(YarnBrand.Drops,   YarnName.NordicSky,   YarnMaterial.Wool,    50m,  100m,  30,  29m),
                new Yarn(YarnBrand.Drops,   YarnName.CloudSoft,   YarnMaterial.Alpaca,  50m,  150m,  0,   69m),   // out of stock
                new Yarn(YarnBrand.Sandnes, YarnName.Fjordline,   YarnMaterial.Cotton,  50m,  100m,  15,  35m),
                new Yarn(YarnBrand.Sandnes, YarnName.Granite,     YarnMaterial.Acrylic, 100m, 300m,  10,  49m),   // low
 
                // ---------------- Needles: size = mm, length = cm ----------------
                //  brand                 name                    material                type                         size  length qty price
                new Needle(NeedleBrand.Jarbo,   NeedleName.Classic,    NeedleMaterial.Birch,     NeedleType.Straight,      4.0m, 35m,  12, 89m),
                new Needle(NeedleBrand.Addi,    NeedleName.FineLace,   NeedleMaterial.Birch,     NeedleType.Circular,      4.0m, 80m,  6,  129m),
                new Needle(NeedleBrand.KnitPro, NeedleName.Smooth,     NeedleMaterial.Aluminium, NeedleType.Circular,      5.0m, 100m, 2,  99m),    // low
                new Needle(NeedleBrand.KnitPro, NeedleName.SockMaster, NeedleMaterial.Bamboo,    NeedleType.DoublePointed, 2.5m, 20m,  9,  79m),
 
                // ---------------- Hooks: size = mm, length = cm ----------------
                //  brand              name                 material              size  length qty price
                new Hook(HookBrand.Addi,    HookName.SoftGrip, HookMaterial.Aluminium, 3.5m, 14m, 14, 59m),
                new Hook(HookBrand.Clover,  HookName.Petite,   HookMaterial.Steel,     2.0m, 12m, 7,  49m),
                new Hook(HookBrand.KnitPro, HookName.Ergo,     HookMaterial.Bamboo,    5.0m, 15m, 3,  69m),   // low
 
                // ---------------- Other supplies: size = pieces per pack ----------------
                //  brand                      name                           material                size  qty price
                new OtherSupply(OtherSupplyBrand.Prym,    OtherSupplyName.StitchMarkers, OtherSupplyMaterial.Plastic, 20m, 18, 29m),
                new OtherSupply(OtherSupplyBrand.Clover,  OtherSupplyName.MeasuringTape, OtherSupplyMaterial.Plastic, 1m,  4,  39m),   // low
                new OtherSupply(OtherSupplyBrand.KnitPro, OtherSupplyName.NeedleGauge,   OtherSupplyMaterial.Steel,   1m,  9,  45m)
            };
        }
    }
}
