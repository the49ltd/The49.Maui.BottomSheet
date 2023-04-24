namespace The49.Maui.BottomSheet;

public partial class ContentDetent : Detent
{
    public override double GetHeight(BottomSheet page, double maxSheetHeight)
    {
        var r = page.Content.Measure(page.Window.Width, maxSheetHeight);

        return r.Request.Height + page.Padding.VerticalThickness;
    }
}
