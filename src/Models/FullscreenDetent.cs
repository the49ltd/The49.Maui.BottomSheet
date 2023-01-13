namespace The49.Maui.BottomSheet;

public partial class FullscreenDetent : Detent
{
    public override double GetHeight(BottomSheetPage page, double maxSheetHeight)
    {
        return maxSheetHeight;
    }
}
