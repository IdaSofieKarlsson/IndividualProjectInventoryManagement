using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Rules;
using System.Text;


Console.WriteLine("Inventory Management - Knappen's Yarn Store");

// TEMPORARY test program: creates one product of each category and tries
// the rules, so we can see that the model classes work. It will be replaced
// by the real start-up code later.

// Makes the console show å, ä and ö correctly
Console.OutputEncoding = Encoding.UTF8;

// ---------------------------------------------------------------
// 1. Valid products
// ---------------------------------------------------------------
// The list type is Product, but it holds a Yarn, a Needle, a Hook and an
// OtherSupply side by side. That is polymorphism.
List<Product> products = new List<Product>
{
    //                 brand            name                 material            size   length qty price
    new Yarn(YarnBrand.Jarbo, YarnName.MorningMist, YarnMaterial.Merino, 50m, 200m, 24, 39m),
    new Yarn(YarnBrand.Drops, YarnName.NordicSky, YarnMaterial.Wool, 100m, 150m, 8, 45m),
    new Needle(NeedleBrand.Addi, NeedleName.FineLace, NeedleMaterial.Birch, NeedleType.Circular, 4.0m, 80m, 6, 129m),
    new Hook(HookBrand.Clover, HookName.Petite, HookMaterial.Aluminium, 3.5m, 14m, 0, 59m),
    new OtherSupply(OtherSupplyBrand.Prym, OtherSupplyName.StitchMarkers, OtherSupplyMaterial.Plastic, 10m, 12, 25m)
};

// Inventory will hand out IDs later. For this test we number them ourselves.
int nextId = 1;

foreach (Product product in products)
{
    product.Id = nextId++;

    // One call, different output per object: polymorphism in action
    Console.WriteLine(product.DisplayProductInfo());
    Console.WriteLine();
}

// ---------------------------------------------------------------
// 2. Stock status and adjusting stock
// ---------------------------------------------------------------
Console.WriteLine("STOCK STATUS");
foreach (Product product in products)
{
    // Yarn warns at 10 or fewer, needles and hooks at 3 or fewer, supplies at 5 or fewer
    Console.WriteLine($"  {product,-40} {product.Quantity,3} pcs  {product.GetStockStatus().ToDisplayText()} (low at {product.LowStockThreshold})");
}

Product hook = products[3];
hook.AdjustQuantity(10);
Console.WriteLine();
Console.WriteLine($"After a delivery of 10: {hook} now has {hook.Quantity} pcs, {hook.GetStockStatus().ToDisplayText()}");

// ---------------------------------------------------------------
// 3. Invalid input must be rejected
// ---------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("REJECTED:");

try
{
    // 2.7 mm is not an allowed needle size
    new Needle(NeedleBrand.Addi, NeedleName.FineLace, NeedleMaterial.Birch, NeedleType.Circular, 2.7m, 80m, 6, 129m);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"  {ex.Message}");
}

try
{
    // MorningMist is a Järbo yarn, not a Drops yarn
    new Yarn(YarnBrand.Drops, YarnName.MorningMist, YarnMaterial.Wool, 50m, 200m, 5, 39m);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"  {ex.Message}");
}

try
{
    // A negative quantity is rejected by the Product class
    new Hook(HookBrand.Addi, HookName.SoftGrip, HookMaterial.Steel, 3.0m, 14m, -1, 59m);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"  {ex.Message}");
}

try
{
    // Removing more than we have
    products[0].AdjustQuantity(-100);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  {ex.Message}");
}
