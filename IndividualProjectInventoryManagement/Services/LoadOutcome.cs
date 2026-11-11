namespace IndividualProjectInventoryManagement.Services
{
    // What happened when the inventory was loaded
    public record LoadOutcome(bool FileFound, IReadOnlyList<string> Warnings);
}
