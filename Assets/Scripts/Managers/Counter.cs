using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Counter  {
    public static readonly string KeyScore = "Score";
    public static readonly string KeyHeart = "Heart";
    public static readonly string KeyBonus = "Bonus";
    public static readonly string KeyStar = "Star";
    public static readonly string KeyCrown = "Crown";

    public static Dictionary<string, int> note;
    public static int bad;

    private static Dictionary<string, int> countData;

    static Counter(){
        countData = new Dictionary<string, int>(10);
    }

    /// <summary>
    /// Increase value of specified key, if the key is not existed, a new one will be created. And return the value of that key, after increasing
    /// </summary>
    /// <returns>Value of the key, after count the new one</returns>
    public static int Count(string key, int value = 1) {
        if (countData.ContainsKey(key)) {
            countData[key] += value;
        }
        else {
            countData.Add(key, value);
        }

        return countData[key];
    }

    /// <summary>
    /// Get current value of specified key
    /// </summary>
    public static int GetQuantity(string key) {
        if (countData.ContainsKey(key)) {
            return countData[key];
        }
        else {
            return 0;
        }
    }

    /// <summary>
    /// Remove all counting key
    /// </summary>
    public static void Clear() {
        countData.Clear();
        if (note != null)
            note.Clear();

        bad = 0;
    }

    public static void AddNote(string keyId, int _score)
    {
        if (note == null)
            note = new Dictionary<string, int>();

        if (!note.ContainsKey(keyId))
            note.Add(keyId, _score);
        else
            note[keyId] = _score;

            bad++;
    }

    /// <summary>
    /// Remove specified key from collection
    /// </summary>
    public static void Remove(string key) {
        if (countData.ContainsKey(key)) {
            countData.Remove(key);
        }
    }
}

