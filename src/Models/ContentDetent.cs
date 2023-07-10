namespace The49.Maui.BottomSheet;

public partial class ContentDetent : Detent
{
    public override double GetHeight(BottomSheet page, double maxSheetHeight)
    {
        if (page.Content is null)
        {
            return maxSheetHeight;
        }
        var r = page.Content.Measure(page.Window.Width - page.Padding.HorizontalThickness, maxSheetHeight);

        return r.Request.Height + page.Padding.VerticalThickness;
    }
}
