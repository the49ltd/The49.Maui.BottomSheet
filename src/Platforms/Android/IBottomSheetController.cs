using Google.Android.Material.BottomSheet;

namespace The49.Maui.BottomSheet;

public interface IBottomSheetController
{
    void Show();
    void Layout();

    void UpdateBackground();
    void Dismiss();
    BottomSheetBehavior Behavior { get; }
}
