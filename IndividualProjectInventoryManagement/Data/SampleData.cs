using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;

namespace IndividualProjectInventoryManagement.Data
{
    // ------------------------------------------------------------------
    // SampleData - a ready-made shop to start with
    // ------------------------------------------------------------------
    // Used the first time the program starts, when no saved file exists yet.
    //
    // The products are mixed on purpose: some well stocked, some low and one
    // out of stock, so the colours and the report have something to show.
    // The price in each line is the PURCHASE price. The selling price is
    // calculated (x 3, rounded down), for example 9.5 -> 28.
    // The IDs are left at 0: Inventory hands them out when a product is added.
    // ------------------------------------------------------------------
    public static class SampleData
    {
        public static List<Product> Create()
        {
            return new List<Product>
            {
                // ---------------- Yarn: brand, name, quantity, purchase price, description ----------------
                new Yarn(YarnBrand.Jarbo,   YarnName.MorningMist, 24, 13m,   "Soft everyday wool blend in pale grey"),
                new Yarn(YarnBrand.Jarbo,   YarnName.Dusk,        8,  16m,   "Warm and dense, good for sweaters"),         // low
                new Yarn(YarnBrand.Drops,   YarnName.NordicSky,   30, 9.5m,  "Light and airy, good for scarves"),
                new Yarn(YarnBrand.Drops,   YarnName.CloudSoft,   0,  23m,   "Extra soft yarn for baby clothes"),          // out of stock
                new Yarn(YarnBrand.Sandnes, YarnName.Fjordline,   15, 11.5m, "Strong cotton for dishcloths and bags"),
                new Yarn(YarnBrand.Sandnes, YarnName.Granite,     10, 16m,   "Hard-wearing yarn for socks and mittens"),   // low

                // ---------------- Needles: size = mm, length = cm ----------------
                new Needle(NeedleBrand.Jarbo,   NeedleName.Classic,    NeedleMaterial.Birch,     NeedleType.Straight,      4.0m, 40m,  12, 29m,   "Smooth birch needles, good for beginners"),
                new Needle(NeedleBrand.Addi,    NeedleName.FineLace,   NeedleMaterial.Birch,     NeedleType.Circular,      4.0m, 80m,  6,  43m,   "Fine tip for lace and detailed work"),
                new Needle(NeedleBrand.KnitPro, NeedleName.Smooth,     NeedleMaterial.Aluminium, NeedleType.Circular,      5.0m, 100m, 2,  33m,   "Light aluminium needles, easy on the hands"),   // low
                new Needle(NeedleBrand.KnitPro, NeedleName.SockMaster, NeedleMaterial.Bamboo,    NeedleType.DoublePointed, 2.5m, 20m,  9,  26.5m, "Set for knitting socks in the round"),

                // ---------------- Hooks: size = mm, length = cm ----------------
                new Hook(HookBrand.Addi,    HookName.SoftGrip, HookMaterial.Aluminium, 3.5m, 14m, 14, 19.5m, "Soft grip handle for long crochet sessions"),
                new Hook(HookBrand.Clover,  HookName.Petite,   HookMaterial.Steel,     2.0m, 14m, 7,  16m,   "Small steel hook for fine thread work"),
                new Hook(HookBrand.KnitPro, HookName.Ergo,     HookMaterial.Bamboo,    5.0m, 15m, 3,  23m,   "Ergonomic bamboo hook"),                      // low

                // ---------------- Other supplies: size = pieces per pack ----------------
                new OtherSupply(OtherSupplyBrand.Prym,    OtherSupplyName.StitchMarkers, OtherSupplyMaterial.Plastic, 20m, 18, 9.5m, "Pack of markers for keeping track of stitches"),
                new OtherSupply(OtherSupplyBrand.Clover,  OtherSupplyName.MeasuringTape, OtherSupplyMaterial.Plastic, 1m,  4,  13m,  "Retractable tape measure"),                       // low
                new OtherSupply(OtherSupplyBrand.KnitPro, OtherSupplyName.NeedleGauge,   OtherSupplyMaterial.Steel,   1m,  9,  15m,  "Gauge for checking the size of needles")
            };
        }
    }
}