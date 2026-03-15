using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Profile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string UserId { get; set; }  // FK to IdentityUser
    public string Username { get; set; }
    public string MobileNumber { get; set; }

    public Cart Cart { get; set; } = null!;

    private Profile() { } // For EF

    public Profile(string userId, string username, string mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Profile must be associated with a user.");
        
        UserId = userId;
        MobileNumber = mobileNumber;
        Username = username;
    }

    public void UpdateMobileNumber(string mobileNumber)
    {
        MobileNumber = mobileNumber;
    }

    public void UpdateUsername(string username)
    {
        Username = username;
    }
}
