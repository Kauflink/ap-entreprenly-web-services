using System.Text.Json.Serialization;
using Entreprenly.WebServices.Iam.Domain.Model.Entities;

namespace Entreprenly.WebServices.Iam.Domain.Model.Aggregates;

/// <summary>
///     The user aggregate root.
/// </summary>
public partial class User
{
    public User()
    {
        Email = string.Empty;
        PasswordHash = string.Empty;
        Roles = [];
    }

    public User(string email, string passwordHash)
    {
        Email = email;
        PasswordHash = passwordHash;
        Roles = [];
    }

    public User(string email, string passwordHash, List<Role> roles) : this(email, passwordHash)
    {
        AddRoles(roles);
    }

    public int Id { get; }
    public string Email { get; private set; }

    [JsonIgnore] public string PasswordHash { get; private set; }

    public ICollection<Role> Roles { get; private set; }

    /// <summary>
    ///     Adds a single role to the user.
    /// </summary>
    public User AddRole(Role role)
    {
        Roles.Add(role);
        return this;
    }

    /// <summary>
    ///     Adds a validated set of roles to the user.
    /// </summary>
    public User AddRoles(List<Role> roles)
    {
        var validated = Role.ValidateRoleSet(roles);
        foreach (var role in validated) Roles.Add(role);
        return this;
    }

    /// <summary>
    ///     Updates the user's email.
    /// </summary>
    public User UpdateEmail(string email)
    {
        Email = email;
        return this;
    }

    /// <summary>
    ///     Updates the user's password hash.
    /// </summary>
    public User UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        return this;
    }
}
