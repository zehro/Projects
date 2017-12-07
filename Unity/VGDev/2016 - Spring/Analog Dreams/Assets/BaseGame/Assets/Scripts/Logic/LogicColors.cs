using UnityEngine;
using System.Collections;

public static class LogicColors
{
    public static Vector3 colorToVector(LogicColor c)
    {
        Vector3 result = Vector3.zero;
        result.x = ((int)c >> 2) & 1;
        result.y = ((int)c >> 1) & 1;
        result.z = ((int)c >> 0) & 1;
        return result;
    }

    public static LogicColor vectorToColor(Vector3 v)
    {
        int result = 0;
        result += (Mathf.FloorToInt(v.x) == 1 ? 4 : 0);
        result += (Mathf.FloorToInt(v.y) == 1 ? 2 : 0);
        result += (Mathf.FloorToInt(v.z) == 1 ? 1 : 0);
        return (LogicColor) result;
    }

    public static string indexToString(int i)
    {
        if (i == 0)
            return "Red";
        if (i == 1)
            return "Green";
        if (i == 2)
            return "Blue";
        return "";
    }

    public static Color indexToColor(int i)
    {
        if (i == 0)
            return Color.red;
        if (i == 1)
            return Color.green;
        if (i == 2)
            return Color.blue;
        return Color.black;
    }

    public static LogicColor indexToLogicColor(int i)
    {
        if (i == 0)
            return LogicColor.Red;
        if (i == 1)
            return LogicColor.Green;
        if (i == 2)
            return LogicColor.Blue;
        return LogicColor.Black;
    }
}

public enum LogicColor
{
    Black, Blue, Green, Cyan, Red, Magenta, Yellow, White
}