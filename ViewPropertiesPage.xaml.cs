using System.Collections.ObjectModel;
using System.Data;
using MokinVuokrausApp.luokat;
using MySqlConnector;

namespace MokinVuokrausApp;


public partial class ViewPropertiesPage : ContentPage
{
  private ObservableCollection<Varaus> varaus;
  public ObservableCollection<Varaus> Varaus
  {
    get { return varaus; }
    set
    {
      //Asetetaan arvo, jos ei ole
      if (varaus != value)
      {
        varaus = value;
        OnPropertyChanged(nameof(Varaus));
        //Päivitetään lista, jos muutoksia
        foreach (var varaukset in varaus)
        {
          OnPropertyChanged(nameof(varaukset.Varaus_id));
          OnPropertyChanged(nameof(varaukset.Asiakas));
          OnPropertyChanged(nameof(varaukset.Mokki));
          OnPropertyChanged(nameof(varaukset.Varattu_pvm));
          OnPropertyChanged(nameof(varaukset.Vahvistus_pvm));
          OnPropertyChanged(nameof(varaukset.Varattu_AlkuPvm));
          OnPropertyChanged(nameof(varaukset.Varattu_LoppuPvm));

        }

      }
    }

  }

  public ViewPropertiesPage()
  {
    InitializeComponent();
    varaus = new ObservableCollection<Varaus>();
    BindingContext = this;
    LoadRentingInfo();

  }

  private async void LoadRentingInfo()
  {
          
    try
    {
      DataBaseConnector dbc = new DataBaseConnector();
      var conn = dbc._getConnection();
      await conn.OpenAsync();


      //hakuehto
      string query = "SELECT * FROM varaus, asiakas,  mokki";

      using (MySqlCommand command = new MySqlCommand(query, conn))
      {

        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
        {

          while (await reader.ReadAsync())
          {
            Asiakas asiakas = new Asiakas();
            asiakas.Sukunimi = reader.GetString("sukunimi");
            Mokki mokki = new Mokki();
            mokki.Mokki_id = reader.GetInt32("mokki_id");

            Varaus.Add(new Varaus
            {
              Varaus_id = reader.GetInt32("varaus_id"),
              Asiakas = asiakas,
              Mokki = mokki,
              Varattu_AlkuPvm = reader.GetDateTime("varattu_alkupvm"),
              Varattu_LoppuPvm = reader.GetDateTime("varattu_loppupvm")
            });
          }
        }
      }
      await conn.CloseAsync();
    }
    catch (MySqlException ex)
    {
      await DisplayAlert("Virhe", ex.Message, "OK");
    }
  }


  async void RentingInfo_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
  {
    if (e.SelectedItem != null)
    {
      Varaus selected = (Varaus)e.SelectedItem;
      DataBaseConnector dbc = new DataBaseConnector();
      try
      {
        var conn1 = dbc._getConnection();
        await conn1.OpenAsync();
        string query = "SELECT * FROM varaus, asiakas, mokki  WHERE varaus_id = @varaus_id;";
        using (MySqlCommand command = new MySqlCommand(query, conn1))
        {
          command.Parameters.AddWithValue("@varaus_id", selected.Varaus_id);
          using (MySqlDataReader reader = command.ExecuteReader())
          {
            if (reader.Read())
            {
              Asiakas asiakas = new Asiakas();
              asiakas.Sukunimi = reader.GetString("sukunimi");
              Mokki mokki = new Mokki();
              mokki.Mokki_id = reader.GetInt32("mokki_id");

              new Varaus
              {
                Varaus_id = reader.GetInt32("varaus_id"),
                Asiakas = asiakas,
                Mokki = mokki,
                Varattu_AlkuPvm = reader.GetDateTime("varattu_alkupvm"),
                Varattu_LoppuPvm = reader.GetDateTime("varattu_loppupvm"),
              };

              string result = await DisplayActionSheet("Haluatko muokata vai poistaa varauksen?", "Peruuta", null, "Muokkaa", "Poista");

              if (result == "Muokkaa") //the user selects to edit the employee's information
              {
                string Editquery = "SELECT * FROM varaus, asiakas, mokki  WHERE varaus_id = @varaus_id;";
                using (MySqlCommand Editcommand = new MySqlCommand(Editquery, conn1))
                {


                  using (MySqlDataReader Editreader = Editcommand.ExecuteReader())
                  {
                    Editcommand.Parameters.AddWithValue("@varaus_id", selected.Varaus_id);
                    if (reader.Read())
                    {
                      Asiakas Editasiakas = new Asiakas();
                      asiakas.Sukunimi = reader.GetString("sukunimi");
                      Mokki Editmokki = new Mokki();
                      mokki.Mokki_id = reader.GetInt32("mokki_id");

                      new Varaus
                      {
                        Varaus_id = reader.GetInt32("varaus_id"),
                        Asiakas = asiakas,
                        Mokki = mokki,
                        Varattu_AlkuPvm = reader.GetDateTime("varattu_alkupvm"),
                        Varattu_LoppuPvm = reader.GetDateTime("varattu_loppupvm")
                      };



                    }
                  }}}
                    else if (result == "Poista") //the user selects to delete the information
                    {


                      string answer = await DisplayActionSheet("Oletko varma?", "Kyllä", "Ei"); //makes sure the user really wanted to delete the info and that it wasn't a missclick
                      if (answer == "Kyllä") //the user wants to delete the data
                      {

                        try
                        {

                          string deleteQuery = @"
                            DELETE FROM varaus
                            WHERE varaus.varaus_id = @varaus_id;";

                          using (MySqlCommand Deletecommand = new MySqlCommand(deleteQuery, conn1))
                          {
                            Deletecommand.Parameters.AddWithValue("@varaus_id", selected.Varaus_id);
                            Deletecommand.ExecuteNonQuery();
                          }

                          await DisplayAlert("Onnistui", "Tietojen poisto onnistui", "OK");
                        }
                        catch (MySqlException ex)
                        {
                          await DisplayAlert("Virhe", ex.Message, "OK");
                        }
                        finally
                        {
                          await conn1.CloseAsync();
                        }
                      }
                    }
                  }
                }
              }
            }catch (MySqlException ex)
                        {
                          await DisplayAlert("Virhe", ex.Message, "OK");
                        }
                        
          }
        }
      }