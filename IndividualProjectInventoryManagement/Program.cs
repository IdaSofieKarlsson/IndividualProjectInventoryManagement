using System.Text;
using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Rules;


Console.WriteLine("Inventory Management - Knappen's Yarn Store");

// TEMPORARY test program: prints every brand and its products so we can
// see that the enums and BrandCatalog fit together. It will be replaced
// by the real start-up code later.

// Makes the console show å, ä and ö correctly
Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("YARN");
// Enum.GetValues<YarnBrand>() returns all brands: Jarbo, Drops, Sandnes
foreach (YarnBrand brand in Enum.GetValues<YarnBrand>())
{
    // ToDisplayText() comes from EnumExtensions: "Jarbo" is shown as "Järbo"
    Console.WriteLine($"  {brand.ToDisplayText()}");

    foreach (YarnName name in BrandCatalog.GetNames(brand))
    {
        Console.WriteLine($"    - {name.ToDisplayText()}");
    }
}

Console.WriteLine();
Console.WriteLine("NEEDLES");
foreach (NeedleBrand brand in Enum.GetValues<NeedleBrand>())
{
    Console.WriteLine($"  {brand.ToDisplayText()}");

    foreach (NeedleName name in BrandCatalog.GetNames(brand))
    {
        Console.WriteLine($"    - {name.ToDisplayText()}");
    }
}

Console.WriteLine();
Console.WriteLine("HOOKS");
foreach (HookBrand brand in Enum.GetValues<HookBrand>())
{
    Console.WriteLine($"  {brand.ToDisplayText()}");

    foreach (HookName name in BrandCatalog.GetNames(brand))
    {
        Console.WriteLine($"    - {name.ToDisplayText()}");
    }
}

Console.WriteLine();
Console.WriteLine("OTHER SUPPLIES");
foreach (OtherSupplyBrand brand in Enum.GetValues<OtherSupplyBrand>())
{
    Console.WriteLine($"  {brand.ToDisplayText()}");

    foreach (OtherSupplyName name in BrandCatalog.GetNames(brand))
    {
        Console.WriteLine($"    - {name.ToDisplayText()}");
    }
}

// A quick check of the validation method:
// MorningMist is a Järbo yarn, but NordicSky belongs to Drops, so it is not valid for Järbo
Console.WriteLine();
Console.WriteLine($"Jarbo + MorningMist valid? {BrandCatalog.IsValidCombination(YarnBrand.Jarbo, YarnName.MorningMist)}");
Console.WriteLine($"Jarbo + NordicSky valid?   {BrandCatalog.IsValidCombination(YarnBrand.Jarbo, YarnName.NordicSky)}");
