using IndividualProjectInventoryManagement.Models.Enums;

namespace IndividualProjectInventoryManagement.Rules
{
    // ------------------------------------------------------------------
    // ProductRules - the fixed sets of sizes and lengths
    // ------------------------------------------------------------------
    // Size and length are numbers, but only certain values are allowed.
    // An enum cannot hold 2.5, so the allowed numbers are kept in lists here,
    // in ONE place. The menu will show these lists as pick lists, and the
    // Product constructor checks every value against them.
    //
    // Changing which sizes the shop stocks means editing a list below.
    // ------------------------------------------------------------------
    public static class ProductRules
    {
        // IMPORTANT: static fields are filled in the order they are written.
        // The lists must be declared BEFORE the dictionary that uses them.

        // Needles: diameter in millimetres, and length in centimetres
        // 20 cm = double-pointed, 25-40 cm = straight, 60-100 cm = circular
        private static readonly decimal[] NeedleSizesMm = { 2.0m, 2.5m, 3.0m, 3.5m, 4.0m, 4.5m, 5.0m, 5.5m, 6.0m, 7.0m, 8.0m, 10.0m };
        private static readonly decimal[] NeedleLengthsCm = { 20m, 25m, 30m, 40m, 60m, 80m, 100m };

        // Hooks: diameter in millimetres, and length in centimetres
        // 14-15 cm = normal crochet hook, 30 cm = long (Tunisian) hook
        private static readonly decimal[] HookSizesMm = { 2.0m, 2.5m, 3.0m, 3.5m, 4.0m, 4.5m, 5.0m, 5.5m, 6.0m, 7.0m, 8.0m, 10.0m };
        private static readonly decimal[] HookLengthsCm = { 14m, 15m, 30m };

        // Other supplies: pieces per pack. They have no length.
        private static readonly decimal[] SupplyPackSizes = { 1m, 5m, 10m, 20m };
        private static readonly decimal[] NoValues = Array.Empty<decimal>();

        // One switch from category to its rules
        private static readonly Dictionary<ProductCategory, CategoryRules> rulesByCategory = new()
        {
            // Yarn has no size or length (every ball is 50 g): empty lists
            { ProductCategory.Yarn, new CategoryRules(NoValues, "", 0, NoValues, "") },
            { ProductCategory.Needle, new CategoryRules(NeedleSizesMm, "mm", 1, NeedleLengthsCm, "cm") },
            { ProductCategory.Hook, new CategoryRules(HookSizesMm, "mm", 1, HookLengthsCm, "cm") },
            { ProductCategory.OtherSupply, new CategoryRules(SupplyPackSizes, "pcs", 0, NoValues, "") }
        };

        // The rules for one category, for example to build a pick list in the menu
        public static CategoryRules For(ProductCategory category) => rulesByCategory[category];

        // "decimal?" means the value may be null (= not given)
        public static bool IsValidSize(ProductCategory category, decimal? size)
            => IsAllowed(For(category).Sizes, size);

        public static bool IsValidLength(ProductCategory category, decimal? length)
            => IsAllowed(For(category).Lengths, length);

        // Is the value in the list of allowed values?
        private static bool IsAllowed(decimal[] allowed, decimal? value)
        {
            // An empty list means "not applicable": the only valid value is null
            if (allowed.Length == 0)
            {
                return value is null;
            }

            // Otherwise a value is REQUIRED and must be in the list
            return value is not null && allowed.Contains(value.Value);
        }
    }
}