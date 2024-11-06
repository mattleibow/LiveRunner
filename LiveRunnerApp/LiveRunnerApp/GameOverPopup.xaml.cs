using CommunityToolkit.Maui.Views;

namespace LiveRunnerApp;

public partial class GameOverPopup : Popup
{
	public GameOverPopup()
	{
		InitializeComponent();
	}

    private async void OnMainMenuClicked(object sender, EventArgs e)
    {
        await CloseAsync();
        await Shell.Current.GoToAsync("///menu");

        //await Shell.Current.GoToAsync("/game");
    }
}