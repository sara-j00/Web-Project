namespace Domain.Entities;

public class Profile
{
    public Guid Id { get; set; }

    public string UserId { get; set; }  // FK to IdentityUser
    public string Username { get; set; }
    public string MobileNumber { get; set; }

    private Profile() { } // For EF

    public Profile(string username, string userId, string mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Profile must be associated with a user.");

        Id = Guid.NewGuid();
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
