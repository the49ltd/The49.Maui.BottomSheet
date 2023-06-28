namespace The49.Maui.BottomSheet;

public partial class BottomSheet
{
    public BottomSheetController Controller { get; set; }

    partial void CalculateTallestDetent(double heightConstraint)
    {
        if (Controller is null)
        {
            return;
        }
        if (Controller._heights is null)
        {
            Controller.CalculateHeights(heightConstraint);
        }
        _tallestDetent = Controller._heights.Values.Max();
    }

}
