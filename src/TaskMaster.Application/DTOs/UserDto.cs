// For returning user data
public class UserDto
{
    public int UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
}

// For creating a user
public class UserCreateDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
}

// For updating a user
public class UserUpdateDto
{
    public int UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
}

// For login
public class UserLoginDto
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
}

// For returning result messages
public class UserOperationResultDto
{
    public string ResultMessage { get; set; } = string.Empty;
}
