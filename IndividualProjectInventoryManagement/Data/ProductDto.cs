namespace IndividualProjectInventoryManagement.Data
{
    // ------------------------------------------------------------------
    // ProductDto - what ONE product looks like in the JSON file
    // ------------------------------------------------------------------
    // DTO = Data Transfer Object: a plain "box of values" with no rules and
    // no behaviour, made only to be written to and read from a file.
    //
    // Why not save the Yarn / Needle / Hook objects directly?
    //   - They have get-only properties and validating constructors. JSON
    //     libraries struggle with those.
    //   - Four different classes in one list needs extra "type" markers.
    //   - The file format would be tied to the model classes. Rename a
    //     property in Yarn and every saved file breaks.
    // With a DTO the file format is its own small, flat, stable thing.
    //
    // Brand, name and material are saved as TEXT (the enum's name, like
    // "Jarbo" or "MorningMist"), never as numbers. The file is readable, and
    // adding a new enum value in the middle can never change the meaning of
    // saved data. The enum name is also plain ASCII: "Jarbo", not "Järbo".
    //
    // Size, Length and NeedleType may be null. null = "does not apply".
    // ------------------------------------------------------------------
    public class ProductDto
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Material { get; set; }
        public string? NeedleType { get; set; }
        public decimal? Size { get; set; }
        public decimal? Length { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
    }

    // The whole file: a version number, the next free ID and the products
    public class InventoryFileDto
    {
        // If the file format ever changes, the version tells us which format
        // an old file has
        // Version 2: yarn without size/length/material, purchase price, description
        public const int CurrentVersion = 2;

        public int Version { get; set; } = CurrentVersion;

        // Saved so that an ID is never reused, even after the product with
        // the highest ID was deleted
        public int NextId { get; set; } = 1;

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}