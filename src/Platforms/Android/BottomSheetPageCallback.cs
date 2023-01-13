using AView = Android.Views.View;
using Google.Android.Material.BottomSheet;

namespace The49.Maui.BottomSheet;

public class BottomSheetPageCallback : BottomSheetBehavior.BottomSheetCallback
{
    readonly BottomSheetPage _page;

    public event EventHandler StateChanged;
    public BottomSheetPageCallback(BottomSheetPage page) : base()
    {
        _page = page;
    }
    public override void OnSlide(AView bottomSheet, float newState) {}

    public override void OnStateChanged(AView view, int newState)
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
