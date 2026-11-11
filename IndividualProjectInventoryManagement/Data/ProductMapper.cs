using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;

namespace IndividualProjectInventoryManagement.Data
{
    // ------------------------------------------------------------------
    // ProductMapper - converts between Product objects and ProductDto
    // ------------------------------------------------------------------
    // ToDto:   Product -> DTO   (when saving)
    // FromDto: DTO -> Product   (when loading)
    //
    // FromDto goes through the normal constructors, so ALL the rules run
    // again: a hand-edited file with brand Drops + name Dusk, size 3.7 or a
    // negative price is rejected with a clear message. A product that
    // breaks the rules can never get into the program through the file.
    // ------------------------------------------------------------------
    public static class ProductMapper
    {
        public static ProductDto ToDto(Product product)
        {
            ProductDto dto = new ProductDto
            {
                Id = product.Id,
                Category = product.Category.ToString(),
                Size = product.Size,
                Length = product.Length,
                Quantity = product.Quantity,
                Price = product.Price
            };

            // "case Yarn yarn" = if the product IS a Yarn, call it "yarn".
            // Pattern matching on the real type. This is the one place that
            // has to know all four subclasses.
            switch (product)
            {
                case Yarn yarn:
                    dto.Brand = yarn.Brand.ToString();
                    dto.Name = yarn.Name.ToString();
                    dto.Material = yarn.Material.ToString();
                    break;

                case Needle needle:
                    dto.Brand = needle.Brand.ToString();
                    dto.Name = needle.Name.ToString();
                    dto.Material = needle.Material.ToString();
                    dto.NeedleType = needle.Type.ToString();
                    break;

                case Hook hook:
                    dto.Brand = hook.Brand.ToString();
                    dto.Name = hook.Name.ToString();
                    dto.Material = hook.Material.ToString();
                    break;

                case OtherSupply supply:
                    dto.Brand = supply.Brand.ToString();
                    dto.Name = supply.Name.ToString();
                    dto.Material = supply.Material.ToString();
                    break;

                default:
                    throw new ArgumentException($"Unknown product type: {product.GetType().Name}.");
            }

            return dto;
        }

        // Throws ArgumentException (with a readable message) if the data is invalid
        public static Product FromDto(ProductDto dto)
        {
            ProductCategory category = ParseEnum<ProductCategory>(dto.Category, "category");

            Product product = category switch
            {
                ProductCategory.Yarn => new Yarn(
                    ParseEnum<YarnBrand>(dto.Brand, "brand"),
                    ParseEnum<YarnName>(dto.Name, "name"),
                    ParseEnum<YarnMaterial>(dto.Material, "material"),
                    Require(dto.Size, "size"),
                    Require(dto.Length, "length"),
                    dto.Quantity,
                    dto.Price),

                ProductCategory.Needle => new Needle(
                    ParseEnum<NeedleBrand>(dto.Brand, "brand"),
                    ParseEnum<NeedleName>(dto.Name, "name"),
                    ParseEnum<NeedleMaterial>(dto.Material, "material"),
                    ParseEnum<NeedleType>(dto.NeedleType, "needle type"),
                    Require(dto.Size, "size"),
                    Require(dto.Length, "length"),
                    dto.Quantity,
                    dto.Price),

                ProductCategory.Hook => new Hook(
                    ParseEnum<HookBrand>(dto.Brand, "brand"),
                    ParseEnum<HookName>(dto.Name, "name"),
                    ParseEnum<HookMaterial>(dto.Material, "material"),
                    Require(dto.Size, "size"),
                    Require(dto.Length, "length"),
                    dto.Quantity,
                    dto.Price),

                ProductCategory.OtherSupply => new OtherSupply(
                    ParseEnum<OtherSupplyBrand>(dto.Brand, "brand"),
                    ParseEnum<OtherSupplyName>(dto.Name, "name"),
                    ParseEnum<OtherSupplyMaterial>(dto.Material, "material"),
                    Require(dto.Size, "size"),
                    dto.Quantity,
                    dto.Price),

                _ => throw new ArgumentException($"Unknown category '{dto.Category}'.")
            };

            // Keep the saved ID
            product.Id = dto.Id;
            return product;
        }

        // Text -> enum value. TryParse returns false instead of crashing.
        // IsDefined stops a text like "99" from becoming a nonexistent enum value.
        private static T ParseEnum<T>(string? text, string what) where T : struct, Enum
        {
            if (text != null && Enum.TryParse(text, ignoreCase: false, out T value) && Enum.IsDefined(value))
            {
                return value;
            }

            throw new ArgumentException($"'{text}' is not a valid {what}.");
        }

        private static decimal Require(decimal? value, string what)
        {
            return value ?? throw new ArgumentException($"The {what} is missing.");
        }
    }
}