using System.Globalization;
using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Rules;

namespace IndividualProjectInventoryManagement.UI
{
    // ------------------------------------------------------------------
    // ProductBuilder - asks the user questions and builds a product
    // ------------------------------------------------------------------
    // Used by "Add product" and by "Change product details".
    //
    // The user can only CHOOSE from lists, never type brand, name, size or
    // length freely. And the lists come from the rules:
    //   - brands          from the enums
    //   - names           from BrandCatalog (only the chosen brand's names)
    //   - sizes, lengths  from ProductRules
    // So when you change the range in the enums or in ProductRules, the
    // menus follow automatically. Nothing here needs to change.
    //
    // Every step can be cancelled (0). Create then returns null.
    // ------------------------------------------------------------------
    public static class ProductBuilder
    {
        public static Product? Create(ProductCategory category) => category switch
        {
            ProductCategory.Yarn => CreateYarn(),
            ProductCategory.Needle => CreateNeedle(),
            ProductCategory.Hook => CreateHook(),
            ProductCategory.OtherSupply => CreateOtherSupply(),
            _ => throw new ArgumentOutOfRangeException(nameof(category))
        };

        private static Product? CreateYarn()
        {
            // Yarn only needs a brand and a name (every ball is 50 g)
            if (!ConsoleInput.TryPick("Brand", Enum.GetValues<YarnBrand>(), item => item.ToDisplayText(), out YarnBrand brand)) return null;
            if (!ConsoleInput.TryPick($"Yarn from {brand.ToDisplayText()}", BrandCatalog.GetNames(brand), item => item.ToDisplayText(), out YarnName name)) return null;
            if (!TryReadStockPriceAndDescription(out int quantity, out decimal purchasePrice, out string description)) return null;

            return new Yarn(brand, name, quantity, purchasePrice, description);
        }

        private static Product? CreateNeedle()
        {
            CategoryRules rules = ProductRules.For(ProductCategory.Needle);

            if (!ConsoleInput.TryPick("Brand", Enum.GetValues<NeedleBrand>(), item => item.ToDisplayText(), out NeedleBrand brand)) return null;
            if (!ConsoleInput.TryPick($"Needles from {brand.ToDisplayText()}", BrandCatalog.GetNames(brand), item => item.ToDisplayText(), out NeedleName name)) return null;
            if (!ConsoleInput.TryPick("Type", Enum.GetValues<NeedleType>(), item => item.ToDisplayText(), out NeedleType type)) return null;
            if (!ConsoleInput.TryPick("Material", Enum.GetValues<NeedleMaterial>(), item => item.ToDisplayText(), out NeedleMaterial material)) return null;
            if (!TryPickSize(rules, out decimal size)) return null;
            if (!TryPickLength(rules, out decimal length)) return null;
            if (!TryReadStockPriceAndDescription(out int quantity, out decimal purchasePrice, out string description)) return null;

            return new Needle(brand, name, material, type, size, length, quantity, purchasePrice, description);
        }

        private static Product? CreateHook()
        {
            CategoryRules rules = ProductRules.For(ProductCategory.Hook);

            if (!ConsoleInput.TryPick("Brand", Enum.GetValues<HookBrand>(), item => item.ToDisplayText(), out HookBrand brand)) return null;
            if (!ConsoleInput.TryPick($"Hooks from {brand.ToDisplayText()}", BrandCatalog.GetNames(brand), item => item.ToDisplayText(), out HookName name)) return null;
            if (!ConsoleInput.TryPick("Material", Enum.GetValues<HookMaterial>(), item => item.ToDisplayText(), out HookMaterial material)) return null;
            if (!TryPickSize(rules, out decimal size)) return null;
            if (!TryPickLength(rules, out decimal length)) return null;
            if (!TryReadStockPriceAndDescription(out int quantity, out decimal purchasePrice, out string description)) return null;

            return new Hook(brand, name, material, size, length, quantity, purchasePrice, description);
        }

        private static Product? CreateOtherSupply()
        {
            CategoryRules rules = ProductRules.For(ProductCategory.OtherSupply);

            if (!ConsoleInput.TryPick("Brand", Enum.GetValues<OtherSupplyBrand>(), item => item.ToDisplayText(), out OtherSupplyBrand brand)) return null;
            if (!ConsoleInput.TryPick("Product", BrandCatalog.GetNames(brand), item => item.ToDisplayText(), out OtherSupplyName name)) return null;
            if (!ConsoleInput.TryPick("Material", Enum.GetValues<OtherSupplyMaterial>(), item => item.ToDisplayText(), out OtherSupplyMaterial material)) return null;
            if (!TryPickSize(rules, out decimal size)) return null;

            // Other supplies have no length, so there is no question about it
            if (!TryReadStockPriceAndDescription(out int quantity, out decimal purchasePrice, out string description)) return null;

            return new OtherSupply(brand, name, material, size, quantity, purchasePrice, description);
        }

        // ----------------------------------------------------------
        // Shared helpers
        // ----------------------------------------------------------

        private static bool TryPickSize(CategoryRules rules, out decimal size)
        {
            string format = "F" + rules.SizeDecimals;

            return ConsoleInput.TryPick(
                $"Size ({rules.SizeUnit})",
                rules.Sizes,
                value => $"{value.ToString(format, CultureInfo.InvariantCulture)} {rules.SizeUnit}",
                out size);
        }

        private static bool TryPickLength(CategoryRules rules, out decimal length)
        {
            return ConsoleInput.TryPick(
                $"Length ({rules.LengthUnit})",
                rules.Lengths,
                value => $"{value.ToString("F0", CultureInfo.InvariantCulture)} {rules.LengthUnit}",
                out length);
        }

        // Asks for quantity, purchase price and description.
        // The selling price is not asked for: it is calculated (price x 3).
        private static bool TryReadStockPriceAndDescription(out int quantity, out decimal purchasePrice, out string description)
        {
            quantity = 0;
            purchasePrice = 0;
            description = string.Empty;

            int? enteredQuantity = ConsoleInput.ReadInt("Quantity in stock", 0, 100000);

            if (enteredQuantity is null)
            {
                return false;
            }

            decimal? enteredPrice = ConsoleInput.ReadDecimal($"Purchase price per piece in {Product.BaseCurrency}", 1m, 100000m);

            if (enteredPrice is null)
            {
                return false;
            }

            // The selling price is shown straight away, so the user sees the rule at work
            int sellingPrice = (int)Math.Floor(enteredPrice.Value * Product.SellingPriceMultiplier);
            Console.WriteLine($"  Selling price will be {sellingPrice} {Product.BaseCurrency} (purchase price x {Product.SellingPriceMultiplier}, rounded down).");

            string? enteredDescription = ReadDescription();

            if (enteredDescription is null)
            {
                return false;
            }

            quantity = enteredQuantity.Value;
            purchasePrice = enteredPrice.Value;
            description = enteredDescription;
            return true;
        }

        // A free text. Enter = cancel (null). Asks again if it is too long.
        private static string? ReadDescription()
        {
            while (true)
            {
                string text = ConsoleInput.ReadText($"Description (max {Product.MaxDescriptionLength} characters, Enter = cancel)");

                if (text.Length == 0)
                {
                    return null;
                }

                if (text.Length <= Product.MaxDescriptionLength)
                {
                    return text;
                }

                Display.Error($"That is {text.Length} characters. The maximum is {Product.MaxDescriptionLength}.");
            }
        }

        // Used by "Update product" to change only the description
        public static string? AskForDescription() => ReadDescription();
    }
}