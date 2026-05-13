using System.Collections.Generic;

namespace Insurance.InventoryDashboard.Panel.Services.Localization;

public interface IUiTextService
{
    string Language { get; }

    string this[string key] { get; }

    string Get(string key);

    IReadOnlyDictionary<string, string> GetAll();

    string ToClientDictionaryJson();
}
