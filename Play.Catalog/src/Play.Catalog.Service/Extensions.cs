using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item) => new ItemDto(item.Id, item.Name, item.Description, item.Price, item.Date);
        public static Item AsModel(this ItemDto itemdto) => new Item() { Id = itemdto.Id, Name = itemdto.Name, Description = itemdto.Description, Price = itemdto.Price, Date = itemdto.Date };
    }
}
