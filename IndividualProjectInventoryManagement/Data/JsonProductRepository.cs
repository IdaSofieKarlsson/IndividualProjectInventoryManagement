using System.Text;
using System.Text.Json;
using IndividualProjectInventoryManagement.Models;

namespace IndividualProjectInventoryManagement.Data
{
    // ------------------------------------------------------------------
    // JsonProductRepository - stores the products in a JSON file
    // ------------------------------------------------------------------
    // Safety measures:
    //   - SAVING writes a temporary file first and then swaps it in, so a
    //     crash halfway through can never leave a half-written file.
    //   - The swap keeps the previous version as inventory.json.bak.
    //   - LOADING skips single broken entries (with a warning) instead of
    //     refusing the whole file.
    //   - A file that is not valid JSON at all is moved aside to
    //     inventory.corrupt-<time>.json, never overwritten.
    // ------------------------------------------------------------------
    public class JsonProductRepository : IProductRepository
    {
        private readonly string filePath;

        // Settings for the serializer, created once and shared
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,                             // pretty, readable file
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // "NextId" becomes "nextId"
            PropertyNameCaseInsensitive = true                 // when reading, "nextId" and "NextId" both work
        };

        public JsonProductRepository(string filePath)
        {
            this.filePath = filePath;
        }

        public string Location => filePath;

        public LoadResult Load()
        {
            // First start: no file yet. The caller decides what to do.
            if (!File.Exists(filePath))
            {
                return new LoadResult(false, new List<Product>(), 1, new List<string>());
            }

            List<string> warnings = new List<string>();
            InventoryFileDto? file;

            try
            {
                string json = File.ReadAllText(filePath, Encoding.UTF8);

                // JSON text -> objects. Deserialize<T> is the opposite of Serialize.
                file = JsonSerializer.Deserialize<InventoryFileDto>(json, Options);
            }
            catch (JsonException)
            {
                file = null;
            }

            // An unreadable file, or one written by an older version of the
            // program with a different format
            if (file == null || file.Version != InventoryFileDto.CurrentVersion)
            {
                string aside = MoveCorruptFileAside();
                warnings.Add($"The data file was damaged or from an older version. It was kept as {Path.GetFileName(aside)} and a new empty inventory was started.");
                return new LoadResult(true, new List<Product>(), 1, warnings);
            }

            List<Product> products = new List<Product>();

            for (int i = 0; i < file.Products.Count; i++)
            {
                ProductDto dto = file.Products[i];

                try
                {
                    products.Add(ProductMapper.FromDto(dto));
                }
                catch (ArgumentException ex)
                {
                    warnings.Add($"Skipped entry {i + 1} in the file (ID {dto.Id}): {ex.Message}");
                }
            }

            return new LoadResult(true, products, file.NextId, warnings);
        }

        public void Save(IReadOnlyCollection<Product> products, int nextId)
        {
            InventoryFileDto file = new InventoryFileDto
            {
                NextId = nextId,
                Products = products.Select(ProductMapper.ToDto).ToList()
            };

            // Objects -> JSON text
            string json = JsonSerializer.Serialize(file, Options);

            string? folder = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // 1. Write everything to a temporary file (UTF-8 without BOM)
            string tempPath = filePath + ".tmp";
            File.WriteAllText(tempPath, json, new UTF8Encoding(false));

            // 2. Swap it in. Replace also keeps the old file as .bak.
            if (File.Exists(filePath))
            {
                File.Replace(tempPath, filePath, filePath + ".bak");
            }
            else
            {
                File.Move(tempPath, filePath);
            }
        }

        private string MoveCorruptFileAside()
        {
            string folder = Path.GetDirectoryName(filePath) ?? string.Empty;
            string stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss", System.Globalization.CultureInfo.InvariantCulture);
            string asidePath = Path.Combine(folder, $"inventory.corrupt-{stamp}.json");

            File.Move(filePath, asidePath);
            return asidePath;
        }
    }
}