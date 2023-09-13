using CommunityToolkit.Maui.Core.Platform;
using System.Collections.ObjectModel;

namespace The49.Maui.BottomSheet.Sample;

public partial class ChatPage : ContentPage
{

	ObservableCollection<string> _messages = new ObservableCollection<string>();

	public ObservableCollection<string> Messages
	{
		get => _messages;
		set
		{
			_messages = value;
			OnPropertyChanged();
		}
	}
	public ChatPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		sheet.ShowAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		sheet.DismissAsync();
    }

    void Button_Clicked(object sender, EventArgs e)
    {
		Messages.Add(editor.Text);
		editor.Text = "";
		editor.Unfocus();
		KeyboardExtensions.HideKeyboardAsync(editor, CancellationToken.None);
    }
}