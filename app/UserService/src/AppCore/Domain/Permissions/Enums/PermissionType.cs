namespace Insurance.UserService.AppCore.Domain.Permissions.Enums;

public enum PermissionType : byte
{
    Unknown = 0,
    Page = 1,
    Menu = 2,
    Button = 3,
    Api = 4,
    Command = 5,
    Query = 6,
    Feature = 7
}
