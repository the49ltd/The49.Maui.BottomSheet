using System.Collections.ObjectModel;
using System.Windows.Input;

namespace The49.Maui.BottomSheet.DemoPages;

public class ListAction
{
	public string Title { get; set; }
	public ICommand Command { get; set; }
}

public partial class SimplePage : BottomSheetPage
{
	public ObservableCollection<ListAction> Actions => new()
	{
		new ListAction
		{
			Title = "Share",
			Command = new Command(() => { }),
		},
		new ListAction
        {
            Title = "Copy",
            Command = new Command(() => { }),
        },
        new ListAction
        {
            Title = "Open in browser",
            Command = new Command(() => { }),
        },
        new ListAction
        {
            Title = "Dismiss",
            Command = new Command(Dismiss),
        }
    };
	public SimplePage()
	{
		InitializeComponent();
	}

    public VisualElement Divider => divider;
}