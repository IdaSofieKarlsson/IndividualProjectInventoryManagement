using IndividualProjectInventoryManagement.Models;

namespace IndividualProjectInventoryManagement.Data
{
    // What Load returns: the products plus everything that went wrong
    public record LoadResult(
        bool FileFound,
        List<Product> Products,
        int NextId,
        List<string> Warnings);

    // ------------------------------------------------------------------
    // IProductRepository - "something that can store products"
    // ------------------------------------------------------------------
    // An INTERFACE is a contract: it lists WHAT must be possible, with no
    // code and no word about HOW. Inventory only knows this contract, not
    // that the products end up in a JSON file. To store in a database
    // instead, write another class with the same two methods. Inventory
    // does not change. (Tests can also use a fake in-memory repository.)
    // ------------------------------------------------------------------
    public interface IProductRepository
    {
        // Where the data is kept, for display
        string Location { get; }

        LoadResult Load();

        void Save(IReadOnlyCollection<Product> products, int nextId);
    }
}