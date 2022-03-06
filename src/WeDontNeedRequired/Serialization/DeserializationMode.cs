namespace WeDontNeedRequired.Serialization;

public enum DeserializationMode
{
    SystemTextJson,
    NewtonsoftJson,
    NewtonsoftJsonWithRequiredProperties,
    NewtonsoftJsonWithRequiredPropertiesAndMissingPropertiesHandling
}
