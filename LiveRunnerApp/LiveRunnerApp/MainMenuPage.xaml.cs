namespace LiveRunnerApp;

public partial class MainMenuPage : ContentPage
{
	public MainMenuPage()
	{
		InitializeComponent();
	}

    private async void OnPlayClicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync("/game");
    }
}