using Android.Animation;
using Android.Content;
using Android.Graphics.Drawables;
using AView = Android.Views.View;

namespace The49.Maui.BottomSheet;
internal class BottomSheetBackdrop : AView
{
    public BottomSheetBackdrop(Context context) : base(context)
    {
        Clickable = true;
        Background = new ColorDrawable(Android.Graphics.Color.Black);
        Alpha = .5f;
    }

    public void AnimateIn()
    {
        var alphaAnimator = ObjectAnimator.OfFloat(this, "alpha", 0f, .5f);

        alphaAnimator.SetDuration(Context.Resources.GetInteger(Resource.Integer.bottom_sheet_slide_duration));

        alphaAnimator.Start();
    }

    public void AnimateOut()
    {
        var alphaAnimator = ObjectAnimator.OfFloat(this, "alpha", .5f, 0f);

        alphaAnimator.SetDuration(Context.Resources.GetInteger(Resource.Integer.bottom_sheet_slide_duration));

        alphaAnimator.Start();
    }
}

