using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common.Service.Repositories;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;
        private readonly IPublishEndpoint publishEndpoint;
        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;

        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> Get() => (await itemsRepository.GetAllListAsync()).Select(i => i.AsDto());

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var foundItem = (await itemsRepository.GetAsync(id))?.AsDto();
            return foundItem is null ? NotFound() : foundItem;
        }
        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var newItem = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
            await itemsRepository.CreateAsync(newItem.AsModel());
            await publishEndpoint.Publish(new CatalogItemCreated(newItem.Id, newItem.Name, newItem.Description));
            return CreatedAtAction(nameof(GetByIdAsync), new { id = newItem.Id }, newItem);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var itemToUpdate = (await itemsRepository.GetAsync(id));

            if (itemToUpdate is null) return NotFound();

            itemToUpdate.Name = updateItemDto.Name;
            itemToUpdate.Description = updateItemDto.Description;
            itemToUpdate.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(itemToUpdate).ConfigureAwait(false);
            await publishEndpoint.Publish(new CatalogItemUpdated(itemToUpdate.Id, itemToUpdate.Name, itemToUpdate.Description));

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await itemsRepository.RemoveAsync(id);
            await publishEndpoint.Publish(new CatalogItemDeleted(id));
            return NoContent();
        }
    }
}