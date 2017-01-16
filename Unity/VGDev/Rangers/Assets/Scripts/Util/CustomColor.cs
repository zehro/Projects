using UnityEngine;
using System.Reflection;

namespace Assets.Scripts.Util
{
	/// <summary>
	/// Used for converting normal colors to  Unity's colors that go from 0-1
	/// </summary>
	public class CustomColor
	{
		public CustomColor() { }

		private const float MAX_RGBVal = 255.0f;

        #region Colors
        // Default colors
        // Grey levels
        private static Color black = Convert255(0,0,0);
		private static Color darkest_grey = Convert255(31,31,31);
		private static Color darker_grey = Convert255(63,63,63);
		private static Color dark_grey = Convert255(95,95,95);
		private static Color grey = Convert255(127,127,127);
		private static Color light_grey = Convert255(159,159,159);
		private static Color lighter_grey = Convert255(191,191,191);
		private static Color lightest_grey = Convert255(223,223,223);
		private static Color darkest_gray = Convert255(31,31,31);
		private static Color darker_gray = Convert255(63,63,63);
		private static Color dark_gray = Convert255(95,95,95);
		private static Color gray = Convert255(127,127,127);
		private static Color light_gray = Convert255(159,159,159);
		private static Color lighter_gray = Convert255(191,191,191);
		private static Color lightest_gray = Convert255(223,223,223);
		private static Color white = Convert255(255,255,255);

		// Sepia
		private static Color darkest_sepia = Convert255(31,24,15);
		private static Color darker_sepia = Convert255(63,50,31);
		private static Color dark_sepia = Convert255(94,75,47);
		private static Color sepia = Convert255(127,101,63);
		private static Color light_sepia = Convert255(158,134,100);
		private static Color lighter_sepia = Convert255(191,171,143);
		private static Color lightest_sepia = Convert255(222,211,195);

		// Standard colors
		private static Color red = Convert255(255,0,0);
		private static Color flame = Convert255(255,63,0);
		private static Color orange = Convert255(255,127,0);
		private static Color amber = Convert255(255,191,0);
		private static Color yellow = Convert255(255,255,0);
		private static Color lime = Convert255(191,255,0);
		private static Color chartreuse = Convert255(127,255,0);
		private static Color green = Convert255(0,255,0);
		private static Color sea = Convert255(0,255,127);
		private static Color turquoise = Convert255(0,255,191);
		private static Color cyan = Convert255(0,255,255);
		private static Color sky = Convert255(0,191,255);
		private static Color azure = Convert255(0,127,255);
		private static Color blue = Convert255(0,0,255);
		private static Color han = Convert255(63,0,255);
		private static Color violet = Convert255(127,0,255);
		private static Color purple = Convert255(191,0,255);
		private static Color fuchsia = Convert255(255,0,255);
		private static Color magenta = Convert255(255,0,191);
		private static Color pink = Convert255(255,0,127);
		private static Color crimson = Convert255(255,0,63);

		// Dark colors
		private static Color dark_red = Convert255(191,0,0);
		private static Color dark_flame = Convert255(191,47,0);
		private static Color dark_orange = Convert255(191,95,0);
		private static Color dark_amber = Convert255(191,143,0);
		private static Color dark_yellow = Convert255(191,191,0);
		private static Color dark_lime = Convert255(143,191,0);
		private static Color dark_chartreuse = Convert255(95,191,0);
		private static Color dark_green = Convert255(0,191,0);
		private static Color dark_sea = Convert255(0,191,95);
		private static Color dark_turquoise = Convert255(0,191,143);
		private static Color dark_cyan = Convert255(0,191,191);
		private static Color dark_sky = Convert255(0,143,191);
		private static Color dark_azure = Convert255(0,95,191);
		private static Color dark_blue = Convert255(0,0,191);
		private static Color dark_han = Convert255(47,0,191);
		private static Color dark_violet = Convert255(95,0,191);
		private static Color dark_purple = Convert255(143,0,191);
		private static Color dark_fuchsia = Convert255(191,0,191);
		private static Color dark_magenta = Convert255(191,0,143);
		private static Color dark_pink = Convert255(191,0,95);
		private static Color dark_crimson = Convert255(191,0,47);

		// Darker colors
		private static Color darker_red = Convert255(127,0,0);
		private static Color darker_flame = Convert255(127,31,0);
		private static Color darker_orange = Convert255(127,63,0);
		private static Color darker_amber = Convert255(127,95,0);
		private static Color darker_yellow = Convert255(127,127,0);
		private static Color darker_lime = Convert255(95,127,0);
		private static Color darker_chartreuse = Convert255(63,127,0);
		private static Color darker_green = Convert255(0,127,0);
		private static Color darker_sea = Convert255(0,127,63);
		private static Color darker_turquoise = Convert255(0,127,95);
		private static Color darker_cyan = Convert255(0,127,127);
		private static Color darker_sky = Convert255(0,95,127);
		private static Color darker_azure = Convert255(0,63,127);
		private static Color darker_blue = Convert255(0,0,127);
		private static Color darker_han = Convert255(31,0,127);
		private static Color darker_violet = Convert255(63,0,127);
		private static Color darker_purple = Convert255(95,0,127);
		private static Color darker_fuchsia = Convert255(127,0,127);
		private static Color darker_magenta = Convert255(127,0,95);
		private static Color darker_pink = Convert255(127,0,63);
		private static Color darker_crimson = Convert255(127,0,31);

		// Darkest colors
		private static Color darkest_red = Convert255(63,0,0);
		private static Color darkest_flame = Convert255(63,15,0);
		private static Color darkest_orange = Convert255(63,31,0);
		private static Color darkest_amber = Convert255(63,47,0);
		private static Color darkest_yellow = Convert255(63,63,0);
		private static Color darkest_lime = Convert255(47,63,0);
		private static Color darkest_chartreuse = Convert255(31,63,0);
		private static Color darkest_green = Convert255(0,63,0);
		private static Color darkest_sea = Convert255(0,63,31);
		private static Color darkest_turquoise = Convert255(0,63,47);
		private static Color darkest_cyan = Convert255(0,63,63);
		private static Color darkest_sky = Convert255(0,47,63);
		private static Color darkest_azure = Convert255(0,31,63);
		private static Color darkest_blue = Convert255(0,0,63);
		private static Color darkest_han = Convert255(15,0,63);
		private static Color darkest_violet = Convert255(31,0,63);
		private static Color darkest_purple = Convert255(47,0,63);
		private static Color darkest_fuchsia = Convert255(63,0,63);
		private static Color darkest_magenta = Convert255(63,0,47);
		private static Color darkest_pink = Convert255(63,0,31);
		private static Color darkest_crimson = Convert255(63,0,15);

		// Light colors
		private static Color light_red = Convert255(255,114,114);
		private static Color light_flame = Convert255(255,149,114);
		private static Color light_orange = Convert255(255,184,114);
		private static Color light_amber = Convert255(255,219,114);
		private static Color light_yellow = Convert255(255,255,114);
		private static Color light_lime = Convert255(219,255,114);
		private static Color light_chartreuse = Convert255(184,255,114);
		private static Color light_green = Convert255(114,255,114);
		private static Color light_sea = Convert255(114,255,184);
		private static Color light_turquoise = Convert255(114,255,219);
		private static Color light_cyan = Convert255(114,255,255);
		private static Color light_sky = Convert255(114,219,255);
		private static Color light_azure = Convert255(114,184,255);
		private static Color light_blue = Convert255(114,114,255);
		private static Color light_han = Convert255(149,114,255);
		private static Color light_violet = Convert255(184,114,255);
		private static Color light_purple = Convert255(219,114,255);
		private static Color light_fuchsia = Convert255(255,114,255);
		private static Color light_magenta = Convert255(255,114,219);
		private static Color light_pink = Convert255(255,114,184);
		private static Color light_crimson = Convert255(255,114,149);

		//Lighter colors
		private static Color lighter_red = Convert255(255,165,165);
		private static Color lighter_flame = Convert255(255,188,165);
		private static Color lighter_orange = Convert255(255,210,165);
		private static Color lighter_amber = Convert255(255,232,165);
		private static Color lighter_yellow = Convert255(255,255,165);
		private static Color lighter_lime = Convert255(232,255,165);
		private static Color lighter_chartreuse = Convert255(210,255,165);
		private static Color lighter_green = Convert255(165,255,165);
		private static Color lighter_sea = Convert255(165,255,210);
		private static Color lighter_turquoise = Convert255(165,255,232);
		private static Color lighter_cyan = Convert255(165,255,255);
		private static Color lighter_sky = Convert255(165,232,255);
		private static Color lighter_azure = Convert255(165,210,255);
		private static Color lighter_blue = Convert255(165,165,255);
		private static Color lighter_han = Convert255(188,165,255);
		private static Color lighter_violet = Convert255(210,165,255);
		private static Color lighter_purple = Convert255(232,165,255);
		private static Color lighter_fuchsia = Convert255(255,165,255);
		private static Color lighter_magenta = Convert255(255,165,232);
		private static Color lighter_pink = Convert255(255,165,210);
		private static Color lighter_crimson = Convert255(255,165,188);

		// Lightest colors
		private static Color lightest_red = Convert255(255,191,191);
		private static Color lightest_flame = Convert255(255,207,191);
		private static Color lightest_orange = Convert255(255,223,191);
		private static Color lightest_amber = Convert255(255,239,191);
		private static Color lightest_yellow = Convert255(255,255,191);
		private static Color lightest_lime = Convert255(239,255,191);
		private static Color lightest_chartreuse = Convert255(223,255,191);
		private static Color lightest_green = Convert255(191,255,191);
		private static Color lightest_sea = Convert255(191,255,223);
		private static Color lightest_turquoise = Convert255(191,255,239);
		private static Color lightest_cyan = Convert255(191,255,255);
		private static Color lightest_sky = Convert255(191,239,255);
		private static Color lightest_azure = Convert255(191,223,255);
		private static Color lightest_blue = Convert255(191,191,255);
		private static Color lightest_han = Convert255(207,191,255);
		private static Color lightest_violet = Convert255(223,191,255);
		private static Color lightest_purple = Convert255(239,191,255);
		private static Color lightest_fuchsia = Convert255(255,191,255);
		private static Color lightest_magenta = Convert255(255,191,239);
		private static Color lightest_pink = Convert255(255,191,223);
		private static Color lightest_crimson = Convert255(255,191,207);

		// Desaturated colors
		private static Color desaturated_red = Convert255(127,63,63);
		private static Color desaturated_flame = Convert255(127,79,63);
		private static Color desaturated_orange = Convert255(127,95,63);
		private static Color desaturated_amber = Convert255(127,111,63);
		private static Color desaturated_yellow = Convert255(127,127,63);
		private static Color desaturated_lime = Convert255(111,127,63);
		private static Color desaturated_chartreuse = Convert255(95,127,63);
		private static Color desaturated_green = Convert255(63,127,63);
		private static Color desaturated_sea = Convert255(63,127,95);
		private static Color desaturated_turquoise = Convert255(63,127,111);
		private static Color desaturated_cyan = Convert255(63,127,127);
		private static Color desaturated_sky = Convert255(63,111,127);
		private static Color desaturated_azure = Convert255(63,95,127);
		private static Color desaturated_blue = Convert255(63,63,127);
		private static Color desaturated_han = Convert255(79,63,127);
		private static Color desaturated_violet = Convert255(95,63,127);
		private static Color desaturated_purple = Convert255(111,63,127);
		private static Color desaturated_fuchsia = Convert255(127,63,127);
		private static Color desaturated_magenta = Convert255(127,63,111);
		private static Color desaturated_pink = Convert255(127,63,95);
		private static Color desaturated_crimson = Convert255(127,63,79);

		// Metallic
		private static Color brass = Convert255(191,151,96);
		private static Color copper = Convert255(197,136,124);
		private static Color gold = Convert255(229,191,0);
		private static Color silver = Convert255(203,203,203);

		// Miscellaneous
		private static Color celadon = Convert255(172,255,175);
		private static Color peach = Convert255(255,159,127);
        #endregion

        /// <summary>
        /// Converts a color from 0-255 to 0-1
        /// </summary>
        /// <param name="r">The amount of red (0-255)</param>
        /// <param name="g">The amount of green (0-255)</param>
        /// <param name="b">The amount of blue (0-255)</param>
        /// <returns>Color created from inputs</returns>
        public static Color Convert255(float r, float g, float b)
		{
			r = Mathf.Clamp(r, 0f, MAX_RGBVal);
			g = Mathf.Clamp(g, 0f, MAX_RGBVal);
			b = Mathf.Clamp(b, 0f, MAX_RGBVal);
			return new Color((r / MAX_RGBVal), (g / MAX_RGBVal), (b / MAX_RGBVal));
		}

		/// <summary>
		/// Converts a color from 0-255 to 0-1
		/// </summary>
		/// <param name="r">The amount of red (0-255)</param>
		/// <param name="g">The amount of green (0-255)</param>
		/// <param name="b">The amount of blue (0-255)</param>
		/// <param name="a">The amount of alpha (0-255)</param>
		/// <returns>Color created from inputs</returns>
		public static Color Convert255(float r, float g, float b, float a)
		{
			r = Mathf.Clamp(r, 0f, MAX_RGBVal);
			g = Mathf.Clamp(g, 0f, MAX_RGBVal);
			b = Mathf.Clamp(b, 0f, MAX_RGBVal);
			a = Mathf.Clamp(a, 0f, MAX_RGBVal);
			return new Color((r / MAX_RGBVal), (g / MAX_RGBVal), (b / MAX_RGBVal), (a / MAX_RGBVal));
		}

        /// <summary>
        /// Gets a color from a property name in CustomColor
        /// </summary>
        /// <param name="uObject">The CustomColor object to pull the property from</param>
        /// <param name="propertyName">The name of the property/color to return</param>
        /// <returns></returns>
        public static Color ColorFromProperty(CustomColor uObject, string propertyName)
        {
            PropertyInfo property = typeof(CustomColor).GetProperty(propertyName, typeof(Color));
            Color color = black;
            if (property != null)
            {
                color = (Color)(property.GetValue(uObject, null));
            }
            else
            {
                Debug.LogError("There is no property named " + propertyName + " in CustomColor. Returning Black by default.");
                color =  black;
            }
            return color;
        }

        #region C# Properties
        // Default colors
        // Grey levels
        public static Color Black { get { return black; } }
        public static Color Darkest_Grey {  get { return darkest_grey; } }
        public static Color Darker_Grey { get { return darker_grey; } }
        public static Color Dark_Grey { get { return dark_grey; } }
        public static Color Grey { get { return grey; } }
        public static Color Light_Grey { get { return light_grey; } }
        public static Color Lighter_Grey { get { return lighter_grey; } }
        public static Color Lightest_Grey { get { return lightest_grey; } }
        public static Color Darkest_Gray { get { return darkest_gray; } }
        public static Color Darker_Gray { get { return darker_gray; } }
        public static Color Dark_Gray { get { return dark_gray; } }
        public static Color Gray { get { return gray; } }
        public static Color Light_Gray { get { return light_gray; } }
        public static Color Lighter_Gray { get { return lighter_gray; } }
        public static Color Lightest_Gray { get { return lightest_gray; } }
        public static Color White { get { return white; } }

        // Sepia
        public static Color Darkest_Sepia { get { return darkest_sepia; } }
        public static Color Darker_Sepia { get { return darker_sepia; } }
        public static Color Dark_Sepia { get { return dark_sepia; } }
        public static Color Sepia { get { return sepia; } }
        public static Color Light_Sepia { get { return light_sepia; } }
        public static Color Lighter_Sepia { get { return lighter_sepia; } }
        public static Color Lightest_Sepia { get { return lightest_sepia; } }

        // Standard colors
        public static Color Red { get { return red; } }
        public static Color Flame { get { return flame; } }
        public static Color Orange { get { return orange; } }
        public static Color Amber { get { return amber; } }
        public static Color Yellow { get { return yellow; } }
        public static Color Lime { get { return lime; } }
        public static Color Chartreuse { get { return chartreuse; } }
        public static Color Green { get { return green; } }
        public static Color Sea { get { return sea; } }
        public static Color Turquoise { get { return turquoise; } }
        public static Color Cyan { get { return cyan; } }
        public static Color Sky { get { return sky; } }
        public static Color Azure { get { return azure; } }
        public static Color Blue { get { return blue; } }
        public static Color Han { get { return han; } }
        public static Color Violet { get { return violet; } }
        public static Color Purple { get { return purple; } }
        public static Color Fuchsia { get { return fuchsia; } }
        public static Color Magenta { get { return magenta; } }
        public static Color Pink { get { return pink; } }
        public static Color Crimson { get { return crimson; } }

        // Dark colors
        public static Color Dark_Red { get { return dark_red; } }
        public static Color Dark_Flame { get { return dark_flame; } }
        public static Color Dark_Orange { get { return dark_orange; } }
        public static Color Dark_Amber { get { return dark_amber; } }
        public static Color Dark_Yellow { get { return dark_yellow; } }
        public static Color Dark_Lime { get { return dark_lime; } }
        public static Color Dark_Chartreuse { get { return dark_chartreuse; } }
        public static Color Dark_Green { get { return dark_green; } }
        public static Color Dark_Sea { get { return dark_sea; } }
        public static Color Dark_Turquoise { get { return dark_turquoise; } }
        public static Color Dark_Cyan { get { return dark_cyan; } }
        public static Color Dark_Sky { get { return dark_sky; } }
        public static Color Dark_Azure { get { return dark_azure; } }
        public static Color Dark_Blue { get { return dark_blue; } }
        public static Color Dark_Han { get { return dark_han; } }
        public static Color Dark_Violet { get { return dark_violet; } }
        public static Color Dark_Purple { get { return dark_purple; } }
        public static Color Dark_Fuchsia { get { return dark_fuchsia; } }
        public static Color Dark_Magenta { get { return dark_magenta; } }
        public static Color Dark_Pink { get { return dark_pink; } }
        public static Color Dark_Crimson { get { return dark_crimson; } }

        // Darker colors
        public static Color Darker_Red { get { return darker_red; } }
        public static Color Darker_Flame { get { return darker_flame; } }
        public static Color Darker_Orange { get { return darker_orange; } }
        public static Color Darker_Amber { get { return darker_amber; } }
        public static Color Darker_Yellow { get { return darker_yellow; } }
        public static Color Darker_Lime { get { return darker_lime; } }
        public static Color Darker_Chartreuse { get { return darker_chartreuse; } }
        public static Color Darker_Green { get { return darker_green; } }
        public static Color Darker_Sea { get { return darker_sea; } }
        public static Color Darker_Turquoise { get { return darker_turquoise; } }
        public static Color Darker_Cyan { get { return darker_cyan; } }
        public static Color Darker_Sky { get { return darker_sky; } }
        public static Color Darker_Azure { get { return darker_azure; } }
        public static Color Darker_Blue { get { return darker_blue; } }
        public static Color Darker_Han { get { return darker_han; } }
        public static Color Darker_Violet { get { return darker_violet; } }
        public static Color Darker_Purple { get { return darker_purple; } }
        public static Color Darker_Fuchsia { get { return darker_fuchsia; } }
        public static Color Darker_Magenta { get { return darker_magenta; } }
        public static Color Darker_Pink { get { return darker_pink; } }
        public static Color Darker_Crimson { get { return darker_crimson; } }

        // Darkest colors
        public static Color Darkest_Red { get { return darkest_red; } }
        public static Color Darkest_Flame { get { return darkest_flame; } }
        public static Color Darkest_Orange { get { return darkest_orange; } }
        public static Color Darkest_Amber { get { return darkest_amber; } }
        public static Color Darkest_Yellow { get { return darkest_yellow; } }
        public static Color Darkest_Lime { get { return darkest_lime; } }
        public static Color Darkest_Chartreuse { get { return darkest_chartreuse; } }
        public static Color Darkest_Green { get { return darkest_green; } }
        public static Color Darkest_Sea { get { return darkest_sea; } }
        public static Color Darkest_Turquoise { get { return darkest_turquoise; } }
        public static Color Darkest_Cyan { get { return darkest_cyan; } }
        public static Color Darkest_Sky { get { return darkest_sky; } }
        public static Color Darkest_Azure { get { return darkest_azure; } }
        public static Color Darkest_Blue { get { return darkest_blue; } }
        public static Color Darkest_Han { get { return darkest_han; } }
        public static Color Darkest_Violet { get { return darkest_violet; } }
        public static Color Darkest_Purple { get { return darkest_purple; } }
        public static Color Darkest_Fuchsia { get { return darkest_fuchsia; } }
        public static Color Darkest_Magenta { get { return darkest_magenta; } }
        public static Color Darkest_Pink { get { return darkest_pink; } }
        public static Color Darkest_Crimson { get { return darkest_crimson; } }

        // Light colors
        public static Color Light_Red { get { return light_red; } }
        public static Color Light_Flame { get { return light_flame; } }
        public static Color Light_Orange { get { return light_orange; } }
        public static Color Light_Amber { get { return light_amber; } }
        public static Color Light_Yellow { get { return light_yellow; } }
        public static Color Light_Lime { get { return light_lime; } }
        public static Color Light_Chartreuse { get { return light_chartreuse; } }
        public static Color Light_Green { get { return light_green; } }
        public static Color Light_Sea { get { return light_sea; } }
        public static Color Light_Turquoise { get { return light_turquoise; } }
        public static Color Light_Cyan { get { return light_cyan; } }
        public static Color Light_Sky { get { return light_sky; } }
        public static Color Light_Azure { get { return light_azure; } }
        public static Color Light_Blue { get { return light_blue; } }
        public static Color Light_Han { get { return light_han; } }
        public static Color Light_Violet { get { return light_violet; } }
        public static Color Light_Purple { get { return light_purple; } }
        public static Color Light_Fuchsia { get { return light_fuchsia; } }
        public static Color Light_Magenta { get { return light_magenta; } }
        public static Color Light_Pink { get { return light_pink; } }
        public static Color Light_Crimson { get { return light_crimson; } }

        //Lighter colors
        public static Color Lighter_Red { get { return lighter_red; } }
        public static Color Lighter_Flame { get { return lighter_flame; } }
        public static Color Lighter_Orange { get { return lighter_orange; } }
        public static Color Lighter_Amber { get { return lighter_amber; } }
        public static Color Lighter_Yellow { get { return lighter_yellow; } }
        public static Color Lighter_Lime { get { return lighter_lime; } }
        public static Color Lighter_Chartreuse { get { return lighter_chartreuse; } }
        public static Color Lighter_Green { get { return lighter_green; } }
        public static Color Lighter_Sea { get { return lighter_sea; } }
        public static Color Lighter_Turquoise { get { return lighter_turquoise; } }
        public static Color Lighter_Cyan { get { return lighter_cyan; } }
        public static Color Lighter_Sky { get { return lighter_sky; } }
        public static Color Lighter_Azure { get { return lighter_azure; } }
        public static Color Lighter_Blue { get { return lighter_blue; } }
        public static Color Lighter_Han { get { return lighter_han; } }
        public static Color Lighter_Violet { get { return lighter_violet; } }
        public static Color Lighter_Purple { get { return lighter_purple; } }
        public static Color Lighter_Fuchsia { get { return lighter_fuchsia; } }
        public static Color Lighter_Magenta { get { return lighter_magenta; } }
        public static Color Lighter_Pink { get { return lighter_pink; } }
        public static Color Lighter_Crimson { get { return lighter_crimson; } }

        // Lightest colors
        public static Color Lightest_Red { get { return lightest_red; } }
        public static Color Lightest_Flame { get { return lightest_flame; } }
        public static Color Lightest_Orange { get { return lightest_orange; } }
        public static Color Lightest_Amber { get { return lightest_amber; } }
        public static Color Lightest_Yellow { get { return lightest_yellow; } }
        public static Color Lightest_Lime { get { return lightest_lime; } }
        public static Color Lightest_Chartreuse { get { return lightest_chartreuse; } }
        public static Color Lightest_Green { get { return lightest_green; } }
        public static Color Lightest_Sea { get { return lightest_sea; } }
        public static Color Lightest_Turquoise { get { return lightest_turquoise; } }
        public static Color Lightest_Cyan { get { return lightest_cyan; } }
        public static Color Lightest_Sky { get { return lightest_sky; } }
        public static Color Lightest_Azure { get { return lightest_azure; } }
        public static Color Lightest_Blue { get { return lightest_blue; } }
        public static Color Lightest_Han { get { return lightest_han; } }
        public static Color Lightest_Violet { get { return lightest_violet; } }
        public static Color Lightest_Purple { get { return lightest_purple; } }
        public static Color Lightest_Fuchsia { get { return lightest_fuchsia; } }
        public static Color Lightest_Magenta { get { return lightest_magenta; } }
        public static Color Lightest_Pink { get { return lightest_pink; } }
        public static Color Lightest_Crimson { get { return lightest_crimson; } }

        // Desaturated colors
        public static Color Desaturated_Red { get { return desaturated_red; } }
        public static Color Desaturated_Flame { get { return desaturated_flame; } }
        public static Color Desaturated_Orange { get { return desaturated_orange; } }
        public static Color Desaturated_Amber { get { return desaturated_amber; } }
        public static Color Desaturated_Yellow { get { return desaturated_yellow; } }
        public static Color Desaturated_Lime { get { return desaturated_lime; } }
        public static Color Desaturated_Chartreuse { get { return desaturated_chartreuse; } }
        public static Color Desaturated_Green { get { return desaturated_green; } }
        public static Color Desaturated_Sea { get { return desaturated_sea; } }
        public static Color Desaturated_Turquoise { get { return desaturated_turquoise; } }
        public static Color Desaturated_Cyan { get { return desaturated_cyan; } }
        public static Color Desaturated_Sky { get { return desaturated_sky; } }
        public static Color Desaturated_Azure { get { return desaturated_azure; } }
        public static Color Desaturated_Blue { get { return desaturated_blue; } }
        public static Color Desaturated_Han { get { return desaturated_han; } }
        public static Color Desaturated_Violet { get { return desaturated_violet; } }
        public static Color Desaturated_Purple { get { return desaturated_purple; } }
        public static Color Desaturated_Fuchsia { get { return desaturated_fuchsia; } }
        public static Color Desaturated_Magenta { get { return desaturated_magenta; } }
        public static Color Desaturated_Pink { get { return desaturated_pink; } }
        public static Color Desaturated_Crimson { get { return desaturated_crimson; } }

        // metallic
        public static Color Brass { get { return brass; } }
        public static Color Copper { get { return copper; } }
        public static Color Gold { get { return gold; } }
        public static Color Silver { get { return silver; } }

        // miscellaneous
        public static Color Celadon { get { return celadon; } }
        public static Color Peach { get { return peach; } }
        #endregion
    }
}