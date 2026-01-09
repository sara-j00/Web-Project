namespace Domain.Entities;

public class Profile
{
    public int Id { get; set; }

    public string UserId { get; set; }  // FK to IdentityUser
    
    public string FullName { get; set; }
    public string MobileNumber { get; set; }
    public string Address { get; set; }
}
