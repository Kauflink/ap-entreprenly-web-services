using Entreprenly.WebServices.Iam.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Iam.Domain.Model.Entities;

/// <summary>
///     Role domain entity.
/// </summary>
public class Role
{
    public Role()
    {
    }

    public Role(ERoles name)
    {
        Name = name;
    }

    public int Id { get; }
    public ERoles Name { get; private set; }

    /// <summary>
    ///     Gets the role name as its string representation.
    /// </summary>
    public string StringName => Name.ToString();

    /// <summary>
    ///     Gets the default role assigned to a newly registered user.
    /// </summary>
    public static Role GetDefaultRole()
    {
        return new Role(ERoles.User);
    }

    /// <summary>
    ///     Builds a role from its string name.
    /// </summary>
    public static Role FromName(string name)
    {
        return new Role(Enum.Parse<ERoles>(name, true));
    }

    /// <summary>
    ///     Returns the provided role set, or the default role set when none is supplied.
    /// </summary>
    public static List<Role> ValidateRoleSet(List<Role>? roles)
    {
        return roles is null || roles.Count == 0 ? [GetDefaultRole()] : roles;
    }
}
