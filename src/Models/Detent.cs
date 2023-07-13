using Maui.BindableProperty.Generator.Core;

namespace The49.Maui.BottomSheet;

public abstract partial class Detent : BindableObject
{
#pragma warning disable CS0169
    [AutoBindable(DefaultValue = "true")]
    readonly bool isEnabled;

    [AutoBindable]
    readonly bool isDefault;
#pragma warning restore CS0169
    public abstract double GetHeight(BottomSheet page, double maxSheetHeight);
}
