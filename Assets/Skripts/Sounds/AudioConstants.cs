public enum VolumeType
{
    Master,
    Music,
    Effects
}

public static class AudioConstants
{
    public const string MasterVolume = "MasterVolume";
    public const string MusicVolume = "MusicVolume";
    public const string EffectsVolume = "EffectsVolume";

    public const float DefaultVolume = 1f;
    public const int DefaultEnabled = 1;
}