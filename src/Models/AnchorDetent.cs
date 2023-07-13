using Maui.BindableProperty.Generator.Core;

namespace The49.Maui.BottomSheet;

public partial class AnchorDetent: Detent
{
    double _height = 0;
#pragma warning disable CS0169
    [AutoBindable]
    readonly VisualElement anchor;
#pragma warning restore CS0169
    public override double GetHeight(BottomSheet page, double maxSheetHeight)
    {
        UpdateHeight(page, maxSheetHeight);
        return _height;
    }

    partial void UpdateHeight(BottomSheet page, double maxSheetHeight);
}
