using AView = Android.Views.View;
using Google.Android.Material.BottomSheet;

namespace The49.Maui.BottomSheet;

public class BottomSheetCallback : BottomSheetBehavior.BottomSheetCallback
{
    readonly BottomSheet _page;

    public event EventHandler StateChanged;
    public BottomSheetCallback(BottomSheet page) : base()
    {
        _page = page;
    }
    public override void OnSlide(AView bottomSheet, float newState)
    {}

    public override void OnStateChanged(AView view, int newState)
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
