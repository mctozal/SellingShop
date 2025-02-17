using System;

public class BasketItem
{
    public BasketItem : IValidatableObject
	{
		[Required]
    public string ProductId { get; set; }
    [Required]
    public string ProductName { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public int Quantity { get; set; }
    [Required]
    public string Color { get; set; }
    [Required]
    public string ImageUrl { get; set; }
    [Required]
    public string Category { get; set; }
    [Required]
    public string Brand { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string UserName { get; set; }

    }

}
