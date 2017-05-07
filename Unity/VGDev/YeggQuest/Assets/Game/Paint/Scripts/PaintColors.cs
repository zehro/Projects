using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PaintColors is a static utility class which can convert a given
// PaintColor (as in the enum) to an actual Color which can be used
// in materials, shaders, etc. This is the consolidated place to
// change the appearance of a color.

namespace YeggQuest.NS_Paint
{
    public static class PaintColors
    {
        public static Color ToColor(PaintColor color)
        {
            switch (color)
            {
                case PaintColor.Cyan:       return RGB(112, 226, 255);
                case PaintColor.Magenta:    return RGB(255,  86, 225);
                case PaintColor.Yellow:     return RGB(255, 236, 112);
                case PaintColor.Black:      return RGB( 62,  54,  69);
                case PaintColor.Red:        return RGB(251,  99,  58);
                case PaintColor.Green:      return RGB( 93, 249, 126);
                case PaintColor.Blue:       return RGB( 83,  79, 247);
                default: return Color.clear;
            }
        }

        private static Color RGB(int r, int g, int b)
        {
            return new Color(r / 255f, g / 255f, b / 255f);
        }
    }

    // PaintColor is an enum of the paint colors in the game.

    public enum PaintColor
    {
        Clear, Cyan, Magenta, Yellow, Black, Red, Green, Blue
    }
}