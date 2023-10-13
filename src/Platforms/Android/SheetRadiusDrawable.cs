using Android.Graphics.Drawables;

namespace The49.Maui.BottomSheet;

internal class SheetRadiusDrawable: GradientDrawable
{
    public SheetRadiusDrawable(): base()
    {
        
    }

    internal void SetCornerRadius(int radius)
    {
        SetCornerRadii(new float[] { radius, radius, radius, radius, 0, 0, 0, 0 });
    }
}
