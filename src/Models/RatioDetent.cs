using Maui.BindableProperty.Generator.Core;

namespace The49.Maui.BottomSheet;

[ContentProperty(nameof(Ratio))]
public partial class RatioDetent : Detent
{
    [AutoBindable]
    readonly float ratio;
    public override double GetHeight(BottomSheetPage page, double maxSheetHeight)
    {
        return maxSheetHeight * Ratio;
    }
}
