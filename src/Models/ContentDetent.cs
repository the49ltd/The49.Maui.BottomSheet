namespace The49.Maui.BottomSheet;

public partial class ContentDetent : Detent
{
    public override double GetHeight(BottomSheet page, double maxSheetHeight)
    {
        if (page.Content is null)
        {
            return maxSheetHeight;
        }
        if (page.Content is ScrollView sv)
        {
            var s = sv.Content.Measure(page.Window.Width - page.Padding.HorizontalThickness, maxSheetHeight);
        }
        var r = page.Content.Measure(page.Window.Width - page.Padding.HorizontalThickness, maxSheetHeight);

        return Math.Min(maxSheetHeight, r.Request.Height + page.Padding.VerticalThickness);
    }
}
