namespace Insurance.PanelPayamakService.Infra.KavehNegarProvider.Services;

using Insurance.PanelPayamakService.Infra.Abstractions;
using Insurance.PanelPayamakService.Infra.Abstractions.Dtos;
using Insurance.PanelPayamakService.Infra.KavehNegarProvider;
using Microsoft.Extensions.Options;

public class KaveNegarPayamakSender : IPayamakSender
{
    private readonly KavehNegarOptions _kavehNegarOption;
    private readonly IHttpClientFactory _httpClientFactory;

    public KaveNegarPayamakSender(IOptions<KavehNegarOptions> options, IHttpClientFactory httpClientFactory)
    {
        _kavehNegarOption = options.Value;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<PayamakResultDto> ExecuteAsync(PayamakInputDto input)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("KavehNegar");
            var servicePath = string.Format(_kavehNegarOption.ServicePath!, input.Receptor, input.Message);
            await client.GetAsync(servicePath);
            var result = new PayamakResultDto
            {
                IsSuccess = true,
                Message = "پیام با موفقیت ارسال شد"
            };
            return await Task.FromResult(result);
        }
        catch (Kavenegar.Exceptions.ApiException ex)
        {
            var result = new PayamakResultDto
            {
                IsSuccess = false,
                Message = ex.Message
            };
            return await Task.FromResult(result);
        }

        catch (Kavenegar.Exceptions.HttpException)
        {
            var result = new PayamakResultDto
            {
                IsSuccess = false,
                Message = "خطا در برقراری ارتباط با پنل پیامکی"
            };
            return await Task.FromResult(result);
        }
        catch (Exception)
        {
            var result = new PayamakResultDto
            {
                IsSuccess = false,
                Message = "خطا در برقراری ارتباط با پنل پیامکی"
            };
            return await Task.FromResult(result);
        }
    }
}