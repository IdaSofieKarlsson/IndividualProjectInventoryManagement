using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using IndividualProjectInventoryManagement.Models.Enums;

namespace IndividualProjectInventoryManagement.Rules
{
    // ------------------------------------------------------------------
    // BrandCatalog - which product names belong to which brand
    // ------------------------------------------------------------------
    // An enum can list brands and it can list names, but it cannot say
    // "MorningMist belongs to Järbo". That relationship lives here, in
    // dictionaries: brand -> the names that brand sells.
    //
    // This class will be used in two places:
    //   - The menu: after the user picks a brand, show only THAT brand's names.
    //   - Validation: reject a combination like "Drops + SwiftLane".
    //
    // "static" = no objects are created, there is only one catalog:
    //     BrandCatalog.GetNames(YarnBrand.Drops)
    // ------------------------------------------------------------------
    public static class BrandCatalog
    {
        // A Dictionary stores key/value pairs. Here the key is a brand and
        // the value is an array with that brand's five names.
        // "new()" without a type is allowed because the type is already
        // written on the left side of the "=".
        private static readonly Dictionary<YarnBrand, YarnName[]> YarnNames = new()
        {
            { YarnBrand.Jarbo, new[] { YarnName.MorningMist, YarnName.Dusk, YarnName.OatField, YarnName.Lingonberry, YarnName.BirchBark } },
            { YarnBrand.Drops, new[] { YarnName.NordicSky, YarnName.IceField, YarnName.PineForest, YarnName.SeaBreeze, YarnName.CloudSoft } },
            { YarnBrand.Sandnes, new[] { YarnName.Fjordline, YarnName.Coastline, YarnName.Highland, YarnName.Lagoon, YarnName.Granite } }
        };

        private static readonly Dictionary<NeedleBrand, NeedleName[]> NeedleNames = new()
        {
            { NeedleBrand.Jarbo, new[] { NeedleName.Classic, NeedleName.Ripple, NeedleName.Pocket, NeedleName.Travel, NeedleName.Studio } },
            { NeedleBrand.Addi, new[] { NeedleName.SwiftLane, NeedleName.FineLace, NeedleName.Comfort, NeedleName.QuickSwap, NeedleName.Everyday } },
            { NeedleBrand.KnitPro, new[] { NeedleName.Smooth, NeedleName.Ergo, NeedleName.ColourBurst, NeedleName.SockMaster, NeedleName.ChunkyCable } }
        };

        private static readonly Dictionary<HookBrand, HookName[]> HookNames = new()
        {
            { HookBrand.Addi, new[] { HookName.SoftGrip, HookName.QuickHook, HookName.LaceFine, HookName.Daily, HookName.Tunisian } },
            { HookBrand.Clover, new[] { HookName.Classic, HookName.Comfort, HookName.Petite, HookName.Jumbo, HookName.Pocket } },
            { HookBrand.KnitPro, new[] { HookName.Smooth, HookName.Ergo, HookName.Bright, HookName.Compact, HookName.Steady } }
        };

        // ----------------------------------------------------------
        // GetNames - METHOD OVERLOADING
        // ----------------------------------------------------------
        // Four methods with the SAME name but different parameter types.
        // C# picks the right one from the type you pass in:
        //     GetNames(YarnBrand.Drops)   -> returns YarnName[]
        //     GetNames(HookBrand.Clover)  -> returns HookName[]
        public static YarnName[] GetNames(YarnBrand brand) => YarnNames[brand];
        public static NeedleName[] GetNames(NeedleBrand brand) => NeedleNames[brand];
        public static HookName[] GetNames(HookBrand brand) => HookNames[brand];

        // Other supplies are not tied to a brand: every brand sells every kind.
        // Enum.GetValues returns ALL values of the enum.
        public static OtherSupplyName[] GetNames(OtherSupplyBrand brand) => Enum.GetValues<OtherSupplyName>();

        // ----------------------------------------------------------
        // IsValidCombination - is this name sold by this brand?
        // ----------------------------------------------------------
        // Used later by the Product constructors, so an invalid combination
        // can never be created, even if it comes from a hand-edited JSON file.
        public static bool IsValidCombination(YarnBrand brand, YarnName name) => YarnNames[brand].Contains(name);
        public static bool IsValidCombination(NeedleBrand brand, NeedleName name) => NeedleNames[brand].Contains(name);
        public static bool IsValidCombination(HookBrand brand, HookName name) => HookNames[brand].Contains(name);
        public static bool IsValidCombination(OtherSupplyBrand brand, OtherSupplyName name) => true;
    }
}
