namespace The49.Maui.BottomSheet;

public partial class App : Application
{
    public App()
	{
		InitializeComponent();

		MainPage = new NavigationPage(new MainPage());
	}
}
