using MySqlConnector;

namespace MokinVuokrausApp;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}
	private async void OnDatabaseClicked(object sender, EventArgs e)
	{
		DataBaseConnector dbc = new DataBaseConnector();
		try {
			var conn = dbc._getConnection();
			conn.Open();
			await DisplayAlert("Onnistui", "Tietokanta yhteys aukesi", "OK");
		}
		catch (MySqlException ex)
		{
			await DisplayAlert("Virhe", ex.Message, "OK");
		}
	}
}

