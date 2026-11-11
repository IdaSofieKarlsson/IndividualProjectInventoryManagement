using IndividualProjectInventoryManagement.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IndividualProjectInventoryManagement.Rules
{
    // ------------------------------------------------------------------
    // BrandCatalog - which product names belong to which brand
    // ------------------------------------------------------------------
    // An enum can list brands and it can list names, but it cannot say
    // "MorningMist belongs to Järbo" by itself. That is what the [Brand]
    // labels in the name enums are for. This class READS those labels once,
    // when the program starts, and builds one list per brand.
    //
    // There is nothing to maintain in this file. All changes to the range
    // are made in the enums (YarnEnums.cs and so on).
    //
    // This class is used in two places:
    //   - The menu: after the user picks a brand, show only THAT brand's names.
    //   - Validation: reject a combination like "Jarbo + NordicSky".
    //
    // "static" = no objects are created, there is only one catalog:
    //     BrandCatalog.GetNames(YarnBrand.Drops)
    // ------------------------------------------------------------------
    public static class BrandCatalog
    {
        // A Dictionary stores key/value pairs. Here the key is a brand and
        // the value is an array with that brand's names. Build<,> fills each
        // dictionary from the [Brand] labels. These run once, the first time
        // BrandCatalog is used.
        private static readonly Dictionary<YarnBrand, YarnName[]> YarnNames = Build<YarnBrand, YarnName>();
        private static readonly Dictionary<NeedleBrand, NeedleName[]> NeedleNames = Build<NeedleBrand, NeedleName>();
        private static readonly Dictionary<HookBrand, HookName[]> HookNames = Build<HookBrand, HookName>();

        // ----------------------------------------------------------
        // Build - reads the [Brand] labels and groups the names by brand
        // ----------------------------------------------------------
        // GENERICS: <TBrand, TName> are placeholders for types. The same code
        // works for yarn, needles and hooks; the caller says which.
        // "where ... : struct, Enum" means both must be enums.
        private static Dictionary<TBrand, TName[]> Build<TBrand, TName>()
            where TBrand : struct, Enum
            where TName : struct, Enum
        {
            Dictionary<TBrand, List<TName>> grouped = new Dictionary<TBrand, List<TName>>();

            // Go through every value of the name enum, e.g. all 15 YarnNames
            foreach (TName name in Enum.GetValues<TName>())
            {
                // REFLECTION again: find the enum member and read its [Brand] label
                FieldInfo field = typeof(TName).GetField(name.ToString())!;
                BrandAttribute? label = field.GetCustomAttribute<BrandAttribute>();

                // Fail loudly if someone forgets the label. A clear error at
                // start-up is much better than a product that silently
                // belongs to no brand.
                if (label == null)
                {
                    throw new InvalidOperationException(
                        $"{typeof(TName).Name}.{name} is missing a [Brand(...)] label.");
                }

                // The label must contain the right KIND of brand: a yarn must
                // not be labelled with a needle brand.
                if (label.Brand is not TBrand brand)
                {
                    throw new InvalidOperationException(
                        $"{typeof(TName).Name}.{name} is labelled with a {label.Brand.GetType().Name}, " +
                        $"but a {typeof(TBrand).Name} is required.");
                }

                // First name for this brand? Create its list.
                if (!grouped.ContainsKey(brand))
                {
                    grouped[brand] = new List<TName>();
                }

                grouped[brand].Add(name);
            }

            // Turn each List into an array: the catalog should not be editable
            // by the code that uses it.
            return grouped.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());
        }

        // Returns the names of a brand, or an empty array if it has none yet.
        // TryGetValue returns false instead of crashing when the key is missing.
        private static TName[] NamesFor<TBrand, TName>(Dictionary<TBrand, TName[]> catalog, TBrand brand)
            where TBrand : notnull
        {
            return catalog.TryGetValue(brand, out TName[]? names) ? names : Array.Empty<TName>();
        }

        // ----------------------------------------------------------
        // GetNames - METHOD OVERLOADING
        // ----------------------------------------------------------
        // Four methods with the SAME name but different parameter types.
        // C# picks the right one from the type you pass in:
        //     GetNames(YarnBrand.Drops)   -> returns YarnName[]
        //     GetNames(HookBrand.Clover)  -> returns HookName[]
        public static YarnName[] GetNames(YarnBrand brand) => NamesFor(YarnNames, brand);
        public static NeedleName[] GetNames(NeedleBrand brand) => NamesFor(NeedleNames, brand);
        public static HookName[] GetNames(HookBrand brand) => NamesFor(HookNames, brand);

        // Other supplies are not tied to a brand: every brand sells every kind.
        // Enum.GetValues returns ALL values of the enum.
        public static OtherSupplyName[] GetNames(OtherSupplyBrand brand) => Enum.GetValues<OtherSupplyName>();

        // ----------------------------------------------------------
        // IsValidCombination - is this name sold by this brand?
        // ----------------------------------------------------------
        // Used by the Product constructors, so an invalid combination can
        // never be created, even if it comes from a hand-edited JSON file.
        public static bool IsValidCombination(YarnBrand brand, YarnName name) => GetNames(brand).Contains(name);
        public static bool IsValidCombination(NeedleBrand brand, NeedleName name) => GetNames(brand).Contains(name);
        public static bool IsValidCombination(HookBrand brand, HookName name) => GetNames(brand).Contains(name);
        public static bool IsValidCombination(OtherSupplyBrand brand, OtherSupplyName name) => GetNames(brand).Contains(name);
    }
}
