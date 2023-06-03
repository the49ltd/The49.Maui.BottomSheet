namespace The49.Maui.BottomSheet.DemoPages;


public partial class SimpleScrollViewPage: BottomSheet
{
    public List<string> Items { get; }

    public SimpleScrollViewPage()
    {
        Items = Enumerable.Range(0, 50).Select(i => $"Item #{i}").ToList();

        InitializeComponent();
    }
}
