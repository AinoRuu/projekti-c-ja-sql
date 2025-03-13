using System.Collections.ObjectModel;
using System.Data;
using MokinVuokrausApp.luokat;
using MySqlConnector;


namespace MokinVuokrausApp;

public partial class EditPropertiesPage : ContentPage
{
  private ObservableCollection<Mokki> mokit;
  public ObservableCollection<Mokki> Mokkit
  {

    get { return mokit; }
    set
    {
      //Asetetaan arvo, jos ei ole
      if (mokit != value)
      {
        mokit = value;
        OnPropertyChanged(nameof(Mokki));
        //Päivitetään lista, jos muutoksia
        foreach (var m in mokit)
        {
          OnPropertyChanged(nameof(m.Mokki_id));
          OnPropertyChanged(nameof(m.Alue.Alue_Id));
          OnPropertyChanged(nameof(m.Nimi));
          OnPropertyChanged(nameof(m.Kuvaus));
          OnPropertyChanged(nameof(m.Henkilomaara));
          OnPropertyChanged(nameof(m.Hinta));
          OnPropertyChanged(nameof(m.Osoite));
          OnPropertyChanged(nameof(m.Postinumero.Postinumero));
          OnPropertyChanged(nameof(m.Varustelu));


        }

      }
    }
  }



  public EditPropertiesPage()
  {
    InitializeComponent();
    mokit = new ObservableCollection<Mokki>();
    this.BindingContext = this;
    LoadPropertiesFromDatabase();

  }

  private async void LoadPropertiesFromDatabase()
  {
    try
    {
      DataBaseConnector dbc = new DataBaseConnector();
      var conn = dbc._getConnection();
      await conn.OpenAsync();
      string query = "SELECT * FROM mokki JOIN alue ON mokki.alue_id = alue.alue_id JOIN posti ON mokki.postinro = posti.postinro";

      using (MySqlCommand command = new MySqlCommand(query, conn))
      {
        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
        {
          mokit.Clear();
          while (await reader.ReadAsync())
          {
            // Populate the list
            Mokki mokki = new Mokki()
            {
              Mokki_id = reader.GetInt32("mokki_id"),
              Nimi = reader.GetString("mokkinimi"),
              Osoite = reader.GetString("katuosoite"),
              Kuvaus = reader.GetString("kuvaus"),
              Varustelu = reader.GetString("varustelu"),
              Hinta = reader.GetDouble("hinta"),
              Henkilomaara = reader.GetInt32("henkilomaara"),
              Alue = new Alue { Alue_Id = reader.GetInt32("alue_id") },
              Postinumero = new Posti { Postinumero = reader.GetString("postinro") }
            };

            mokit.Add(mokki);
          }
        }
      }
    }
    catch (MySqlException ex)
    {
      await DisplayAlert("Virhe", ex.Message, "OK");
    }
  }






  async void PropertyList_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
  {


    Mokki selected = (Mokki)e.SelectedItem;

    MokinNimi_kentta.Text = selected.Nimi;
    MokinOsoite_kentta.Text = selected.Osoite;
    Mokin_kuvaus.Text = selected.Kuvaus;
    Mokin_varustelu.Text = selected.Varustelu;
    Mokin_hinta.Text = selected.Hinta.ToString();
    Mokin_henkilomaara.Text = selected.Henkilomaara.ToString();
    Mokin_id_kentta.Text = selected.Mokki_id.ToString();
    Mokin_alue.Text = selected.Alue.Alue_Id.ToString();
    Mokin_postinumero.Text = selected.Postinumero.Postinumero;
  }


  private void JarjestaNappi_Clicked(System.Object sender, System.EventArgs e)
  {
    SortByAlue();

  }

  private void TyhjennaNappi_Clicked(System.Object sender, System.EventArgs e)
  {
    TyhjennaKentat();
  }
  private async void PoistaNappi_Clicked(System.Object sender, System.EventArgs e)
  {
    DataBaseConnector dbc = new DataBaseConnector();
    try
    {
      var conn = dbc._getConnection();
      conn.Open();
      string query = "DELETE FROM mokki WHERE mokki_id = @mokki_id";


      using (MySqlCommand command = new MySqlCommand(query, conn))
      {
        command.Parameters.AddWithValue("@mokki_id", Mokin_id_kentta.Text);
        await command.ExecuteNonQueryAsync();
      }

      conn.Close();

      LoadPropertiesFromDatabase();
      TyhjennaKentat();
    }
    catch (MySqlException ex)
    {
      await DisplayAlert("Virhe", ex.Message, "OK");
    }
  }

  private async void TallennaNappi_Clicked(System.Object sender, System.EventArgs e)
  {
    DataBaseConnector dbc = new DataBaseConnector();
    try
    {
      var conn = dbc._getConnection();
      await conn.OpenAsync();


      string query = "INSERT INTO mokki (mokki_id, mokkinimi, varustelu, hinta, katuosoite, henkilomaara, alue_id, postinro, kuvaus) " +
                     "VALUES (@mokki_id, @mokkinimi, @varustelu, @hinta, @katuosoite, @henkilomaara, @alue_id, @postinro, @kuvaus)";

      using (MySqlCommand command = new MySqlCommand(query, conn))
      {
        // Adding parameters with values
        command.Parameters.AddWithValue("@mokki_id", Mokin_id_kentta.Text);
        command.Parameters.AddWithValue("@mokkinimi", MokinNimi_kentta.Text);
        command.Parameters.AddWithValue("@varustelu", Mokin_varustelu.Text);
        command.Parameters.AddWithValue("@hinta", Mokin_hinta.Text);
        command.Parameters.AddWithValue("@katuosoite", MokinOsoite_kentta.Text);
        command.Parameters.AddWithValue("@henkilomaara", Mokin_henkilomaara.Text);
        command.Parameters.AddWithValue("@alue_id", Mokin_alue.Text);
        command.Parameters.AddWithValue("@postinro", Mokin_postinumero.Text);
        command.Parameters.AddWithValue("@kuvaus", Mokin_kuvaus.Text);


        await command.ExecuteNonQueryAsync();

      }

      TyhjennaKentat();
      conn.Close();
      LoadPropertiesFromDatabase(); // Refresh the list

    }
    catch (Exception ex) { Console.WriteLine("Ei voitu tallentaa muutoksia " + ex); }
  }
  private bool IsAscending = true;
  private void SortByAlue()
  {
    //Järjestellään lista alueen mukaan 
    if (IsAscending)
    {
      mokit = new ObservableCollection<Mokki>(mokit.OrderBy(l => l.Alue.Alue_Id));
    }
    else
    {
      mokit = new ObservableCollection<Mokki>(mokit.OrderByDescending(l => l.Alue.Alue_Id));
    }
    IsAscending = !IsAscending;
  }

  private void TyhjennaKentat()
  {
    MokinNimi_kentta.Text = "";
    MokinOsoite_kentta.Text = "";
    Mokin_kuvaus.Text = "";
    Mokin_varustelu.Text = "";
    Mokin_hinta.Text = "";
    Mokin_henkilomaara.Text = "";
    Mokin_id_kentta.Text = "";
    Mokin_alue.Text = "";
    Mokin_postinumero.Text = "";
  }

}