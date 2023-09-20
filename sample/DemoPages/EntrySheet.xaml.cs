namespace The49.Maui.BottomSheet.Sample.DemoPages;

public partial class EntrySheet
{
	public EntrySheet()
	{
		InitializeComponent();
		Entry.SizeChanged += (s, e) =>
		{
			Content.InvalidateMeasureNonVirtual(Microsoft.Maui.Controls.Internals.InvalidationTrigger.MeasureChanged);
		};
	}

	public new void Focus()
	{
		Entry.Focus();
	}
}