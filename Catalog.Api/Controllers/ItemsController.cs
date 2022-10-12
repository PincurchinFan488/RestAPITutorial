using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Catalog.Api.Repositories;
using System.Collections.Generic;
using Catalog.Api.Entities;
using Catalog.Api.Dtos;
using Microsoft.Extensions.Logging;

namespace Catalog.Api.Controllers
{
[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IItemsRepository repository;

    public ItemsController(IItemsRepository repository)
    {
        this.repository = repository;

    }

    // Get /items
    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetItemsAsync(string name = null)
    {
        var items = (await repository.GetItemsAsync()).Select(item => item.AsDto());

        if(!string.IsNullOrWhiteSpace(name))
        {
            items = items.Where(item => item.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        return items;
    }

    // Get /items/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
    {
        var item = await repository.GetItemAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return item.AsDto();
    }

    //POst into items
    [HttpPost]
    public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
    {
        Item item = new(){
            Id = new Guid(),
            Name = itemDto.Name,
            Description =itemDto.Description,
            Price = itemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await repository.CreateItemAsync(item);

        return CreatedAtAction(nameof(GetItemAsync), new {id = item.Id}, item.AsDto());
    }

    // Put / items/ {id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
    {
        var existingItem = await repository.GetItemAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        existingItem.Name = itemDto.Name;
        existingItem.Price = itemDto.Price;

        await repository.UpdateItemAsync(existingItem);

        return NoContent();
    }

    // DELETE / items /{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteItemAsync(Guid id)
    {
         var existingItem = await repository.GetItemAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        await repository.DeleteItemAsync(id);

        return NoContent();
    }

    }

}

