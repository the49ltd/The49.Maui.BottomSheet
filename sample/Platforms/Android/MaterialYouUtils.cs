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
        Context dynamicColorContext = DynamicColors.WrapContextIfAvailable(Platform.CurrentActivity, The49.Maui.BottomSheet.Resource.Style.ThemeOverlay_Material3_DynamicColors_Light);

        int[] attrsToResolve = {
            The49.Maui.BottomSheet.Resource.Attribute.colorPrimary,
            The49.Maui.BottomSheet.Resource.Attribute.colorOnPrimary,
            The49.Maui.BottomSheet.Resource.Attribute.colorPrimaryContainer,
            The49.Maui.BottomSheet.Resource.Attribute.colorOnPrimaryContainer,
            The49.Maui.BottomSheet.Resource.Attribute.colorSecondary,
            The49.Maui.BottomSheet.Resource.Attribute.colorOnSecondary,
            The49.Maui.BottomSheet.Resource.Attribute.colorSecondaryContainer,
            The49.Maui.BottomSheet.Resource.Attribute.colorOnSecondaryContainer,
            The49.Maui.BottomSheet.Resource.Attribute.colorTertiary,
            The49.Maui.BottomSheet.Resource.Attribute.colorOnTertiary,
            The49.Maui.BottomSheet.Resource.Attribute.colorTertiaryContainer,
            The49.Maui.BottomSheet.Resource.Attribute.colorOnTertiaryContainer,
            The49.Maui.BottomSheet.Resource.Attribute.colorError,
            The49.Maui.BottomSheet.Resource.Attribute.colorErrorContainer,
            The49.Maui.BottomSheet.Resource.Attribute.colorOnError,
            The49.Maui.BottomSheet.Resource.Attribute.colorOnErrorContainer,
            Android.Resource.Attribute.ColorBackground,
            The49.Maui.BottomSheet.Resource.Attribute.colorOnBackground,
            The49.Maui.BottomSheet.Resource.Attribute.colorSurface,
            The49.Maui.BottomSheet.Resource.Attribute.colorOnSurface,
            The49.Maui.BottomSheet.Resource.Attribute.colorSurfaceVariant,
            The49.Maui.BottomSheet.Resource.Attribute.colorOnSurfaceVariant,
            The49.Maui.BottomSheet.Resource.Attribute.colorOutline,
            The49.Maui.BottomSheet.Resource.Attribute.colorOnSurfaceInverse,
            The49.Maui.BottomSheet.Resource.Attribute.colorSurfaceInverse,
            The49.Maui.BottomSheet.Resource.Attribute.colorPrimaryInverse,
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
