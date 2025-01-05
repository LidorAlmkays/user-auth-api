using Domain.Enums;

namespace Domain.Enums
{
    public enum Role
    {
        Guest,
        User,
        Admin
    }
}

public static class RoleExtensions
{
    // Convert from enum to string
    public static string ToFriendlyString(this Role role)
    {
        return role.ToString();
    }

    // Convert from string to enum, return null if invalid
    public static Role? TryParse(string value)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            Enum.TryParse(value, true, out Role result) &&
            Enum.IsDefined(typeof(Role), result))
        {
            return result;
        }

        return null;
    }
}
