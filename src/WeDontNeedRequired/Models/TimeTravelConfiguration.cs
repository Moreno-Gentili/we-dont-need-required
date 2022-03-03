namespace WeDontNeedRequired.Models;

public record TimeTravelConfiguration(
    DateTimeOffset Destination,
    IReadOnlyList<LivingBeing> Passengers)
    {
        public FluxCapacitorSettings? OverrideDefaultFluxCapacitorSettings { get; init; }
    };

public record LivingBeing(
    string Name,
    int Age,
    bool IsHuman);

public record FluxCapacitorSettings(
    FluxCapacitorSettingsGroup PhiGroup,
    FluxCapacitorSettingsGroup RhoGroup,
    FluxCapacitorSettingsGroup SigmaGroup)
{
    public FluxCapacitorSettingsGroup? TopGroup { get; init; }
    public FluxCapacitorSettingsGroup? LeftGroup { get; init; }
    public FluxCapacitorSettingsGroup? RightGroup { get; init; }
    public FluxCapacitorSettingsGroup? BottomGroup { get; init; }
    public FluxCapacitorSettingsGroup? FrontGroup { get; init; }
    public FluxCapacitorSettingsGroup? BackGroup { get; init; }
    public FluxCapacitorSettingsGroup? MiddleGroup { get; init; }
}

public record FluxCapacitorSettingsGroup(
    double Amplitude,
    TimeSpan Period,
    int Permutations,
    bool HasDampening)
{
    public bool? ShowFlickeringLight { get; init; }
    public int? BuzzingSoundPitch { get; init; }
    public int? BuzzingSoundFrequency { get; init; }
    public TimeSpan? RestartInterval { get; init; }
    public decimal? JigowattThreshold { get; init; }
    public bool? BeepOnMaintenanceRequired { get; init; }
}