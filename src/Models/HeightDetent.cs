using Maui.BindableProperty.Generator.Core;

namespace The49.Maui.BottomSheet;

[ContentProperty(nameof(Height))]
public partial class HeightDetent : Detent
{
#pragma warning disable CS0169
    [AutoBindable]
    readonly double height;
#pragma warning restore CS0169
    public override double GetHeight(BottomSheet page, double maxSheetHeight)
    {
        return Height;
    }
}
