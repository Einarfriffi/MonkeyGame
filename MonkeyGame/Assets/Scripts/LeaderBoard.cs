using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    private static string Key(string levelId) => $"Top5_{levelId}";

    // parse "12.34,14.02
    public static List<float> GetTopTimes(string levelId)
    {
        string raw = PlayerPrefs.GetString(Key(levelId), string.Empty);
        if (string.IsNullOrEmpty(raw)) return new List<float>();
        var parts = raw.Split(',');
        var list = new List<float>(parts.Length);
        foreach (var p in parts)
        {
            if (float.TryParse(p, System.Globalization.NumberStyles.Float,
                                System.Globalization.CultureInfo.InvariantCulture, out var v))
                list.Add(v);
        }
        list.Sort();
        return list;
    }

    // Insert new run, keep only best 5, save back
    public static void SubmitTime(string levelId, float seconds)
    {
        if (seconds <= 0f || float.IsNaN(seconds) || float.IsInfinity(seconds)) return;
        var list = GetTopTimes(levelId);
        list.Add(seconds);
        list.Sort();
        if (list.Count > 5) list = list.Take(5).ToList();

        // save comma floats
        string raw = string.Join(",", list.Select(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        PlayerPrefs.SetString(Key(levelId), raw);
        PlayerPrefs.Save();
    }

    public static float? GetBest(string levelId)
    {
        var list = GetTopTimes(levelId);
        return list.Count > 0 ? list[0] : (float?)null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
