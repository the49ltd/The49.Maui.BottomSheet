using The49.Maui.BottomSheet.Sample.DemoPages;

namespace The49.Maui.BottomSheet.Sample;

public partial class ModalPage : ContentPage
{
	public ModalPage()
	{
		InitializeComponent();
	}

    void Button_Clicked(object sender, EventArgs e)
    {
		var s = new SimplePage();

		s.Detents = new List<Detent>
		{
			new FullscreenDetent(),
			new ContentDetent(),
		};

		s.ShowAsync(Window);
    }
}