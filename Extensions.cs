namespace Catalog
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto
        {
            Id = items.Id, 
            Name = items.Name,
            Price = items.Price,
            CreatedDate = items.CreatedDate
        });
        }
    }
}