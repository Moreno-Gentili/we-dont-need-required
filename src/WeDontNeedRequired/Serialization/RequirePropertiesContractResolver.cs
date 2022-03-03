using System.Collections.Concurrent;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WeDontNeedRequired.Serialization;

public class RequirePropertiesContractResolver : DefaultContractResolver
{
    private static readonly ConcurrentDictionary<Type, JsonObjectContract> ObjectContractCache = new();

    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        return ObjectContractCache.GetOrAdd(objectType, CreateAllPropertiesRequiredJsonObjectContract);
    }

    private void SetAllPropertiesAsRequired(Type objectType, JsonObjectContract contract)
    {
        foreach (var property in contract.Properties)
        {
            if (property.PropertyType is not null)
            {
                property.Required = GetRequiredForProperty(property);
            }
        }
    }

    private Required GetRequiredForProperty(JsonProperty jsonProperty)
    {
        if (jsonProperty.DeclaringType is null || jsonProperty.UnderlyingName is null)
        {
            return Required.Default;
        }

        PropertyInfo? propertyInfo = jsonProperty.DeclaringType.GetProperty(jsonProperty.UnderlyingName, BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo is null)
        {
            return Required.Default;
        }

        NullabilityInfoContext nullabilityContext = new();
        NullabilityInfo info = nullabilityContext.Create(propertyInfo);
        return info.ReadState == NullabilityState.Nullable ? Required.AllowNull : Required.Always;
    }

    private JsonObjectContract CreateAllPropertiesRequiredJsonObjectContract(Type objectType)
    {
        JsonObjectContract contract = base.CreateObjectContract(objectType);
        SetAllPropertiesAsRequired(objectType, contract);
        return contract;
    }
}
