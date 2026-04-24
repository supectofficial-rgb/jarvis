namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.InputProcessing;

public interface IInputNormalizer
{
    NormalizedInput Normalize(string text);
}


