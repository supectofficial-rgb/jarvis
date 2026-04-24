namespace Insurance.UserService.AppCore.Shared.Organizations.Commands.Create;

using OysterFx.AppCore.Shared.Commands;
using System;

public class CreateOrganizationCommand : ICommand<Guid>
{
    public string? Title { get; set; }
}