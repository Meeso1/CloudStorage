using CloudStorage.Database;

namespace CloudStorage.Models;

public record User(string Username)
{
    public static User FromEntity(UserEntity entity)
    {
        return new User(entity.Username);
    }
}