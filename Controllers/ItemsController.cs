namespace Catalog.Controllers

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly InMenItemsRepository repository;

    public ItemsController()
    {
        repository = new InMenItemsRepository();
    }

    [HttpGet]
    public IEnumerable<Item> GetItems()
    {
        var items = repository.GetItems();
        return items;
    }
}