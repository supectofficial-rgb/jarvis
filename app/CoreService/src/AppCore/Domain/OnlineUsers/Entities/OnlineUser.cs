namespace Insurance.AppCore.Domain.OnlineUsers.Entities;

using System;

/// <summary>
/// کاربران انلاین
/// </summary>
public sealed class OnlineUser
{
    public string? UserName { get; private set; }
    public DateTime? LoginDateTime { get; private set; }
    public string? IP { get; private set; }
    public string? LocationInSite { get; private set; }
}