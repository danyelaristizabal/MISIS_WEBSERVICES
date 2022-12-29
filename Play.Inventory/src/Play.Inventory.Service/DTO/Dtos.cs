namespace Play.Inventory.Service.DTO
{
    public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);
    public record InventoryItemDto(Guid CatalogItemId, string Name, string Description, int Quantity, DateTimeOffset AquiredDate);
    public record CatalogItemDto(Guid Id, string Name, string Description);
    public record DeleteItemDto(Guid UserId, Guid CatalogItemId, int Quantity);
}
