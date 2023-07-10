using UIKit;

namespace The49.Maui.BottomSheet;

public partial class BottomSheet
{
    public BottomSheetViewController Controller { get; set; }

    // Cache the calculated detents as iOS likes to ask for detents often
    internal readonly IDictionary<int, float> CachedDetents = new Dictionary<int, float>();

    
}
