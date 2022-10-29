using CloudStorage.Database;

namespace CloudStorage.Models;

public record User(Guid Id, string Username)
{
    public static User FromEntity(UserEntity entity)
    {
        return new User(entity.Id, entity.Username);
    }

    public UserResponse ToResponse()
    {
        return new UserResponse
        {
            Id = Id,
            Username = Username
        };
    }
}