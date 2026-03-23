class UserEntity
{
    public UserEntity
    (
        string name,
        string email,
        string password,
        Role role
    )
    {
        Name = name;
        Email = email;
        Password = password;
        Role = role;
    }
    
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
}