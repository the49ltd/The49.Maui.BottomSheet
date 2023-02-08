namespace The49.Maui.BottomSheet;

public partial class ContentDetent : Detent
{
    public override double GetHeight(BottomSheet page, double maxSheetHeight)
    {
        if (page.Height == -1)
        {
            var r = page.Measure(page.Window.Width, maxSheetHeight);

            return r.Request.Height;
        }

        return page.Height;
    }
}
