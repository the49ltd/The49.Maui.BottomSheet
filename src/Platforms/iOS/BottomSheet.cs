using UIKit;

namespace The49.Maui.BottomSheet;

public partial class BottomSheet
{
    public BottomSheetViewController Controller { get; set; }

    // Can't get the sheet max height with large and medium detents
    // custom detents are not supported on iOS 15
    // can't use largestUndimmedIdentifier or selected detent with custom detents on iOS 16
    // So I guess we'll just have to calculate the sheet height ourselves then
    // This number was found by getting the full screen height, subtracting the sheet's UIView height and the top inset
    // This seems to be the spacing iOS leaves at the top of the screen when a sheet is fullscreen
    // TODO: Check if this is the same number for fullscreen sheets opened on top of another sheet
    const int SheetTopSpacing = 10;

    // Cache the calculated detents as iOS likes to ask for detents often
    internal readonly IDictionary<int, float> CachedDetents = new Dictionary<int, float>();

    double _maximumDetentValue = -1;

    partial void CalculateTallestDetent(double heightConstraint)
    {
        if (_maximumDetentValue == -1)
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var topPadding = window?.SafeAreaInsets.Top ?? 0;
            _maximumDetentValue = Window.Height - topPadding - SheetTopSpacing;
        }
        _tallestDetent = GetEnabledDetents().Select(d => d.GetHeight(this, _maximumDetentValue)).Max();
    }
}
