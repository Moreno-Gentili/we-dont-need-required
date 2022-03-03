namespace WeDontNeedRequired.Serialization;

public enum DeserializationMode
{
    SystemTextJson,
    NewtonsoftJsonWithRequiredProperties,
    NewtonsoftJsonWithRequiredPropertiesAndMissingPropertiesHandling
}
