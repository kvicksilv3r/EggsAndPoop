using UnityEngine;

public static class AnimalNameGenerator
{
    private static readonly string MaleNamesPath = "AnimalGenQuirkchamp/male_names";
    private static readonly string FemaleNamesPath = "AnimalGenQuirkchamp/female_names";

    private static string[] _maleNames;
    private static string[] _femaleNames;

    private static string[] MaleNames => _maleNames ??= LoadNames(MaleNamesPath);
    private static string[] FemaleNames => _femaleNames ??= LoadNames(FemaleNamesPath);

    public static string GetRandomName(AnimalSex sex)
    {
        var pool = sex == AnimalSex.Female ? FemaleNames : MaleNames;
        if (pool == null || pool.Length == 0) return "Unknown";
        return pool[Random.Range(0, pool.Length)];
    }

    private static string[] LoadNames(string resourcePath)
    {
        var asset = Resources.Load<TextAsset>(resourcePath);
        if (asset == null)
        {
            Debug.LogWarning($"AnimalNameGenerator: could not load {resourcePath}");
            return new string[0];
        }
        return asset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
    }
}
