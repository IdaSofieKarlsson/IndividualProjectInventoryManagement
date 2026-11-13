using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Rules;

namespace IndividualProjectInventoryManagement.Models
{
    // A Needle IS a Product (inheritance).
    // Size = diameter in mm, Length = length in cm.
    public class Needle : Product
    {
        public NeedleBrand Brand { get; }
        public NeedleName Name { get; }
        public NeedleMaterial Material { get; }

        // An extra field that only needles have
        public NeedleType Type { get; }

        public override string BrandText => Brand.ToDisplayText();
        public override string NameText => Name.ToDisplayText();
        public override string MaterialText => Material.ToDisplayText();

        // Overrides the virtual ExtraText from Product, so that DisplayProductInfo() also shows "Type : Circular" for needles.
        public override string? ExtraText => Type.ToDisplayText();

        // Needles are bought less often than yarn
        public override int LowStockThreshold => 3;

        public Needle(
            NeedleBrand brand,
            NeedleName name,
            NeedleMaterial material,
            NeedleType type,
            decimal size,
            decimal length,
            int quantity,
            decimal purchasePrice,
            string productDescription)
            : base(ProductCategory.Needle, size, length, quantity, purchasePrice, productDescription)
        {
            if (!BrandCatalog.IsValidCombination(brand, name))
            {
                throw new ArgumentException($"{brand.ToDisplayText()} does not sell a needle called {name.ToDisplayText()}.");
            }

            Brand = brand;
            Name = name;
            Material = material;
            Type = type;
        }
    }
}