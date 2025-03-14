using UnityEngine;

public static class NormalisingUtils
{
    public static float NormalizeStat(float value, float minValue = 10, float maxValue = 150, float minChance = 0.1f, float maxChance = 0.95f)
    {
        if (value <= minValue) return minChance;
        if (value >= maxValue) return maxChance;

        float normalized = Mathf.Log(value - (minValue - 1)) / Mathf.Log(maxValue - (minValue - 1));
        return Mathf.Clamp01(minChance + normalized * (maxChance - minChance));
    }
    public static string AddSpacesBeforeCapitals(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string result = "" + input[0];

        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                result += " ";
            }
            result += input[i];
        }
        return result;
    }
}
