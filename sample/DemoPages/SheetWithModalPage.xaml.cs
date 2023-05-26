namespace The49.Maui.BottomSheet.Sample.DemoPages;

public partial class SheetWithModalPage
{
	public SheetWithModalPage()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		Shell.Current.GoToAsync(nameof(ModalPage));
    }
}