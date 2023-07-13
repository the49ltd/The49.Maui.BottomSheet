using Maui.BindableProperty.Generator.Core;

namespace The49.Maui.BottomSheet;

[ContentProperty(nameof(Ratio))]
public partial class RatioDetent : Detent
{
#pragma warning disable CS0169
    [AutoBindable]
    readonly float ratio;
#pragma warning restore CS0169
    public override double GetHeight(BottomSheet page, double maxSheetHeight)
    {
        return maxSheetHeight * Ratio;
    }
}
