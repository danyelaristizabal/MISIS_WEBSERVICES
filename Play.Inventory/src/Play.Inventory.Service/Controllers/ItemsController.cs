using Microsoft.AspNetCore.Mvc;
using Play.Common.Service.Repositories;
using Play.Inventory.Service.DTO;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> inventoryItemsRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;
        public ItemsController(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemsRepository)
        {
            this.inventoryItemsRepository = inventoryItemsRepository;
            this.catalogItemsRepository = catalogItemsRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest();

            var iventoryItemEntities = await inventoryItemsRepository.GetAllListAsync(item => item.UserId == userId);
            var itemIds = iventoryItemEntities.Select(item => item.CatalogItemId);
            var catalogItems = await catalogItemsRepository.GetAllListAsync(item => itemIds.Contains(item.Id));


            var inventoryItemDtos = iventoryItemEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItems?.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem?.Name ?? "", catalogItem?.Description ?? "");
            });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await inventoryItemsRepository.GetAsync(
                item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.Now

                };
                await inventoryItemsRepository.CreateAsync(inventoryItem);
                return Ok();
            }

            inventoryItem.Quantity += grantItemsDto.Quantity;
            await inventoryItemsRepository.UpdateAsync(inventoryItem);
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAsync(DeleteItemDto deleteItemDto)
        {
            if (deleteItemDto.UserId == Guid.Empty) return BadRequest();
            var inventoryItem = await inventoryItemsRepository.GetAsync(
                item => item.UserId == deleteItemDto.UserId && item.CatalogItemId == deleteItemDto.CatalogItemId);
            await inventoryItemsRepository.RemoveAsync(inventoryItem.Id);
            return NoContent();
        }
    }
}
