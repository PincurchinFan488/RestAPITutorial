using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Catalog.Repositories;
using System.Collections.Generic;
using Catalog.Entities;
using Catalog.Dtos;

namespace Catalog.Controllers
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
    public IEnumerable<ItemDto> GetItems()
    {
        var items = repository.GetItems().Select(item => item.AsDto());
        return items;
    }

    // Get /items/{id}
    [HttpGet("{id}")]
    public ActionResult<ItemDto> GetItem(Guid id)
    {
        var item = repository.GetItem(id);

        if (item is null)
        {
            return NotFound();
        }

        return item.AsDto();
    }
}

}

