namespace Insurance.PanelPayamakService.Infra.Abstractions;

using Insurance.PanelPayamakService.Infra.Abstractions.Dtos;

public interface IPayamakSender
{
    Task<PayamakResultDto> ExecuteAsync(PayamakInputDto input);
}