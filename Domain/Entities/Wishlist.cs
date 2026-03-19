namespace Domain.Entities;

public class Wishlist
{
    public int Id { get; set; }
    public int ProfileId { get; set; }

    // Navigation
    public Profile Profile { get; set; } = null!;
    public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
}