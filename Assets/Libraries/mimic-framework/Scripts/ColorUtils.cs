using UnityEngine;
using UnityEngine.UI;

namespace Mimic {

    public static class ColorUtils {

        //
        // Summary:
        //     Cyan. RGBA is (0, 1, 1, 0).
        public static readonly Color cyanTransparent = new Color(0,1,1,0);
        //
        // Summary:
        //     English spelling for gray. RGBA is the same (0.5, 0.5, 0.5, 0).
        public static readonly Color greyTransparent = new Color(0.5f,0.5f,0.5f,0);
        //
        // Summary:
        //     Gray. RGBA is (0.5, 0.5, 0.5, 0).
        public static readonly Color grayTransparent = new Color(0.5f, 0.5f, 0.5f, 0);
        //
        // Summary:
        //     Magenta. RGBA is (1, 0, 1, 0).
        public static readonly Color magentaTransparent = new Color(1,0,1,0);
        //
        // Summary:
        //     Solid red. RGBA is (1, 0, 0, 0).
        public static readonly Color redTransparent = new Color(1,0,0,0);
        //
        // Summary:
        //     Yellow. RGBA is (1, 0.92, 0.016, 0), but the color is nice to look at!
        public static readonly Color yellowTransparent = new Color(1, 0.92f, 0.016f, 0);
        //
        // Summary:
        //     Solid white. RGBA is (1, 1, 1, 0).
        public static readonly Color whiteTransparent = new Color(1,1,1,0);
        //
        // Summary:
        //     Solid green. RGBA is (0, 1, 0, 0).
        public static readonly Color greenTransparent = new Color(0,1,0,0);
        //
        // Summary:
        //     Solid blue. RGBA is (0, 0, 1, 0).
        public static readonly Color blueTransparent = new Color(0,0,1,0);

        public static Color RandomColor => new Color(Random.value, Random.value, Random.value);

        public static Color GetColorWithAlpha(this Color color, float alpha) {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static void SetAlpha(this Graphic graphic, float alpha) {
            Color color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }
}