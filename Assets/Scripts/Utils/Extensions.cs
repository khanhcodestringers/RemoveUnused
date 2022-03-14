using System;
using UnityEngine;

public static class RectTransformExtensions {
    /// <summary>
    /// Set the scale to 1,1,1
    /// </summary>
    public static void SetDefaultScale(this RectTransform trans) {
        trans.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// Set the point in which both anchors and the pivot should be placed. This makes it very easy to set positions and scales, but it destroys autoscaling
    /// </summary>
    public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec) {
        trans.pivot = aVec;
        trans.anchorMin = aVec;
        trans.anchorMax = aVec;
    }

    /// <summary>
    /// Get the current size of the RectTransform as a Vector2
    /// </summary>
    public static Vector2 GetSize(this RectTransform trans) {
        return trans.rect.size;
    }

    public static float GetWidth(this RectTransform trans) {
        return trans.rect.width;
    }
    public static float GetHeight(this RectTransform trans) {
        return trans.rect.height;
    }

    /// <summary>
    /// Set the position of the RectTransform within it's parent's coordinates. Depending on the position of the pivot, the RectTransform actual position will differ.
    /// </summary>
    public static void SetLocalPosition(this RectTransform trans, Vector2 newPos) {
        trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
    }

    public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos) {
        trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
    }
    public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos) {
        trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
    }
    public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos) {
        trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
    }
    public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos) {
        trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
    }

    public static void SetSizeDelta(this RectTransform trans, Vector2 newSize) {
        Vector2 oldSize = trans.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
        trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
    }
    public static void SetWidth(this RectTransform trans, float newSize) {
        SetSizeDelta(trans, new Vector2(newSize, trans.rect.size.y));
    }
    public static void SetHeight(this RectTransform trans, float newSize) {
        SetSizeDelta(trans, new Vector2(trans.rect.size.x, newSize));
    }

}

public static class StringExtensions {
    public static string Sanitize(this string text) {
        return StripVietnameseAccent(text);
    }

    public static string StripWhiteSpace(this string text) {
        return text.Replace(" ", "");
    }

    /// <summary>
    /// Remove all accent in Vietnamese and convert to normal text
    /// </summary>
    public static string StripVietnameseAccent(string str) {
        for (int i = 1; i < VietnameseSigns.Length; i++) {
            for (int j = 0; j < VietnameseSigns[i].Length; j++)
                str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
        }

        return str;
    }

    private static readonly string[] VietnameseSigns = new string[]{
        "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
    };
}

public static class UltilityExtensions {
    /// <summary>
    /// Debug or not, depend on a simple switch
    /// </summary>
    /// <param name="behavior"></param>
    /// <param name="message"></param>
    public static void Print(this MonoBehaviour behavior, string message) {
        if(Mio.TileMaster.GameManager.Instance.printDebug) {
            Debug.Log(message);
        }
    }

}

public static class Helpers {
    /// <summary>
    /// Helper function to pass specified value to an action
    /// </summary>
    /// <param name="action">The action to call</param>
    /// <param name="value">The value to pass to action</param>
    public static void CallbackWithValue<T>(Action<T> action, T value) {
        if (action != null) {
            action(value);
        }
    }
    public static void CallbackWithValue<T, T1>(Action<T, T1> action, T value, T1 value1) {
        if (action != null) {
            action(value, value1);
        }
    }
    public static void CallbackWithValue<T, T1, T2>(Action<T, T1, T2> action, T value, T1 value1, T2 value2) {
        if (action != null) {
            action(value, value1, value2);
        }
    }
    public static void CallbackWithValue<T, T1, T2, T3>(Action<T, T1, T2,T3> action, T value, T1 value1, T2 value2, T3 value3) {
        if (action != null) {
            action(value, value1, value2, value3);
        }
    }

    /// <summary>
    /// Helper function to call an action
    /// </summary>
    /// <param name="action">The action to call</param>
    public static void Callback(Action action) {
        if (action != null) {
            action();
        }
    }
}

public static class EpochTimeExtensions {
    /// <summary>
    /// Converts the given date value to epoch time.
    /// </summary>
    public static long ToEpochTime (this DateTime dateTime) {
        var date = dateTime.ToUniversalTime();
        var ticks = date.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
        var ts = ticks / TimeSpan.TicksPerSecond;
        return ts;
    }

    ///// <summary>
    ///// Converts the given date value to epoch time.
    ///// </summary>
    //public static long ToEpochTime (this DateTimeOffset dateTime) {
    //    var date = dateTime.ToUniversalTime();
    //    var ticks = date.Ticks - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).Ticks;
    //    var ts = ticks / TimeSpan.TicksPerSecond;
    //    return ts;
    //}

    /// <summary>
    /// Converts the given epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
    /// </summary>
    public static DateTime ToDateTimeFromEpoch (this long intDate) {
        var timeInTicks = intDate * TimeSpan.TicksPerSecond;
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(timeInTicks);
    }

    ///// <summary>
    ///// Converts the given epoch time to a UTC <see cref="DateTimeOffset"/>.
    ///// </summary>
    //public static DateTimeOffset ToDateTimeOffsetFromEpoch (this long intDate) {
    //    var timeInTicks = intDate * TimeSpan.TicksPerSecond;
    //    return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(timeInTicks);
    //}
}