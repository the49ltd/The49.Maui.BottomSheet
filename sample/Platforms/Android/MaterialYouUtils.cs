using Android.Content.Res;
using Android.Content;
using Google.Android.Material.Color;

namespace The49.Maui.BottomSheet;

public class MaterialYouUtils
{
    static ResourceDictionary systemDict = new ResourceDictionary();
    static string[] names = new string[]
    {
        "ColorPrimary",
        "ColorOnPrimary",
        "ColorPrimaryContainer",
        "ColorOnPrimaryContainer",
        "ColorSecondary",
        "ColorOnSecondary",
        "ColorSecondaryContainer",
        "ColorOnSecondaryContainer",
        "ColorTertiary",
        "ColorOnTertiary",
        "ColorTertiaryContainer",
        "ColorOnTertiaryContainer",
        "ColorError",
        "ColorErrorContainer",
        "ColorOnError",
        "ColorOnErrorContainer",
        "ColorBackground",
        "ColorOnBackground",
        "ColorSurface",
        "ColorOnSurface",
        "ColorSurfaceVariant",
        "ColorOnSurfaceVariant",
        "ColorOutline",
        "ColorOnSurfaceInverse",
        "ColorSurfaceInverse",
        "ColorPrimaryInverse",
    };
    public static void InitSystemResourceDictionary()
    {
        App.Current.Resources.MergedDictionaries.Add(systemDict);
        UpdateSystemResourceDictionary();
    }
    public static void UpdateSystemResourceDictionary()
    {
        Context dynamicColorContext = DynamicColors.WrapContextIfAvailable(Platform.CurrentActivity, BottomSheet.Resource.Style.ThemeOverlay_Material3_DynamicColors_Light);

        int[] attrsToResolve = {
            BottomSheet.Resource.Attribute.colorPrimary,
            BottomSheet.Resource.Attribute.colorOnPrimary,
            BottomSheet.Resource.Attribute.colorPrimaryContainer,
            BottomSheet.Resource.Attribute.colorOnPrimaryContainer,
            BottomSheet.Resource.Attribute.colorSecondary,
            BottomSheet.Resource.Attribute.colorOnSecondary,
            BottomSheet.Resource.Attribute.colorSecondaryContainer,
            BottomSheet.Resource.Attribute.colorOnSecondaryContainer,
            BottomSheet.Resource.Attribute.colorTertiary,
            BottomSheet.Resource.Attribute.colorOnTertiary,
            BottomSheet.Resource.Attribute.colorTertiaryContainer,
            BottomSheet.Resource.Attribute.colorOnTertiaryContainer,
            BottomSheet.Resource.Attribute.colorError,
            BottomSheet.Resource.Attribute.colorErrorContainer,
            BottomSheet.Resource.Attribute.colorOnError,
            BottomSheet.Resource.Attribute.colorOnErrorContainer,
            Android.Resource.Attribute.ColorBackground,
            BottomSheet.Resource.Attribute.colorOnBackground,
            BottomSheet.Resource.Attribute.colorSurface,
            BottomSheet.Resource.Attribute.colorOnSurface,
            BottomSheet.Resource.Attribute.colorSurfaceVariant,
            BottomSheet.Resource.Attribute.colorOnSurfaceVariant,
            BottomSheet.Resource.Attribute.colorOutline,
            BottomSheet.Resource.Attribute.colorOnSurfaceInverse,
            BottomSheet.Resource.Attribute.colorSurfaceInverse,
            BottomSheet.Resource.Attribute.colorPrimaryInverse,
        };

        // now resolve them
        TypedArray ta = dynamicColorContext.ObtainStyledAttributes(attrsToResolve);

        for (var i = 0; i < names.Length; i++)
        {
            systemDict[names[i]] = FromAndroid(ta.GetColor(i, 0));
        }

        Console.WriteLine(((Color)systemDict["ColorBackground"]).ToHex());

        ta.Recycle();
    }

    static Color FromAndroid(int acolor)
    {
        var color = new Android.Graphics.Color(acolor);
        return Color.FromRgba(color.R, color.G, color.B, color.A);
    }
}
