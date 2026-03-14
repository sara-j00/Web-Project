namespace Domain.Entities;
public class Cart
{
    public int Id { get; set; }
    public int ProfileId { get; set; } 
    
    public Profile Profile { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
