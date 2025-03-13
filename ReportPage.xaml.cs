using System.Collections.ObjectModel;
using MokinVuokrausApp.luokat;
using MySqlConnector;
using System.Reflection.Metadata;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;


namespace MokinVuokrausApp;

public partial class ReportPage : ContentPage
{
  //Luodaan yhteys listviewiin
  ListView LaskuList;
  private ObservableCollection<Lasku> lasku;
  public ObservableCollection<Lasku> Lasku
  {
    get { return lasku; }
    set
    {
      //Asetetaan arvo, jos ei ole
      if (lasku != value)
      {
        lasku = value;
        OnPropertyChanged(nameof(Lasku));
        //Päivitetään lista, jos muutoksia
        foreach (var laskut in lasku)
        {
          OnPropertyChanged(nameof(laskut.Lasku_id));
          OnPropertyChanged(nameof(laskut.Varaus));
          OnPropertyChanged(nameof(laskut.Summa));
          OnPropertyChanged(nameof(laskut.ALV));
          OnPropertyChanged(nameof(laskut.Maksettu));
        }

      }
    }

  }
  public ReportPage()
  {
    InitializeComponent();
    Lasku = new ObservableCollection<Lasku>();
    BindingContext = this;
    //Ladataan laskut tietokannasta
    LoadInvoicesFromDatabase();
    LaskuList = new ListView();
    LaskuList.ItemSelected += LaskuList_ItemSelected;
  }
  private async void LoadInvoicesFromDatabase()
  {
    try
    {
      DataBaseConnector dbc = new DataBaseConnector();
      var conn = dbc._getConnection();
      await conn.OpenAsync();
      //Tehdään hakuehto
      string query = "SELECT * FROM lasku";

      using (MySqlCommand command = new MySqlCommand(query, conn))
      {
        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
        {
          while (await reader.ReadAsync())
          {
            Varaus varaus = new Varaus();
            varaus.Varaus_id = reader.GetInt32("varaus_id");
            bool maksettu = reader.GetBoolean("maksettu");
            string maksettutext = maksettu ? "Maksettu" : "Erääntynyt";
            Lasku.Add(new Lasku
            {
              Lasku_id = reader.GetInt32("lasku_id"),
              Varaus = varaus,
              Summa = reader.GetDouble("Summa"),
              ALV = reader.GetDouble("ALV"),
              MaksettuText = maksettutext,
            }
            );
          }
        }
      }
    }
    catch (MySqlException ex)
    {
      await DisplayAlert("Virhe", ex.Message, "OK");
    }
  }
  //Tehdään boolean listan järjestelyyn 
  private bool IsAscending = true;
  private void SortByVaraus()
  {
    //Järjestellään lista varauksen mukaan 
    if (IsAscending)
    {
      Lasku = new ObservableCollection<Lasku>(Lasku.OrderBy(l => l.Varaus));
    }
    else
    {
      Lasku = new ObservableCollection<Lasku>(Lasku.OrderByDescending(l => l.Varaus));
    }
    IsAscending = !IsAscending;
  }
  void VarausButton_Clicked(System.Object sender, System.EventArgs e)
  {
    SortByVaraus();
  }
  private void SortByLaskuID()
  {
    if (IsAscending)
    {
      Lasku = new ObservableCollection<Lasku>(Lasku.OrderBy(l => l.Lasku_id));
    }
    else
    {
      Lasku = new ObservableCollection<Lasku>(Lasku.OrderByDescending(l => l.Lasku_id));
    }
    IsAscending = !IsAscending;
  }
  void LaskuIDButton_Clicked(System.Object sender, System.EventArgs e)
  {
    SortByLaskuID();
  }
  private void SortByMaksettu()
  {
    if (IsAscending)
    {
      Lasku = new ObservableCollection<Lasku>(Lasku.OrderBy(l => l.Maksettu));
    }
    else
    {
      Lasku = new ObservableCollection<Lasku>(Lasku.OrderByDescending(l => l.Maksettu));
    }
    IsAscending = !IsAscending;
  }
  void MaksettuButton_Clicked(System.Object sender, System.EventArgs e)
  {
    SortByMaksettu();
  }

  async void LaskuList_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
  {
    if (e.SelectedItem != null)
    {
      Lasku selected = (Lasku)e.SelectedItem;
      DataBaseConnector dbc = new DataBaseConnector();
      try
      {
        var conn = dbc._getConnection();
        conn.Open();
        string query = "SELECT * FROM lasku WHERE lasku_id = @lasku_id;";
        using (MySqlCommand command = new MySqlCommand(query, conn))
        {
          command.Parameters.AddWithValue("@lasku_id", selected.Lasku_id);
          using (MySqlDataReader reader = command.ExecuteReader())
          {
            if (reader.Read())
            {
              // Kirjoitetaan data
              string filePath = "MokinVuokrausApp/MokinVuokrausApp/lasku/Testi.txt";
              using (StreamWriter sw = new StreamWriter(filePath))
              {
                sw.WriteLine("Lasku ID: " + selected.Lasku_id);
                sw.WriteLine("Varaus ID: " + selected.Varaus.Varaus_id);
                sw.WriteLine("Laskun Summa: " + selected.Summa + " €");
                sw.WriteLine("Lasku ALV: " + selected.ALV + "%");
                sw.WriteLine("Maksettutext: " + selected.MaksettuText);
              }

              await DisplayAlert("Onnistui", "Lasku löytyi", "OK");
            }
            else
            {
              await DisplayAlert("Virhe", "Laskua ei löytynyt", "OK");
            }
          }
        }
      }
      catch (MySqlException ex)
      {
        await DisplayAlert("Virhe", ex.Message, "OK");
      }
    }
  }
  public void ShowInvoicebtn_Clicked(System.Object sender, System.EventArgs e)
  {
    try
    {
      string filePath = "/Users/reettatiihonen/Projects/Ohjelmistotuotanto-1-4/MokinVuokrausApp/MokinVuokrausApp/Invoices/Testi.txt";

      // Luetaan data
      string fileContent;
      using (StreamReader sr = new StreamReader(filePath))
      {
        fileContent = sr.ReadToEnd();
      }

      // Näytetään data
      DisplayAlert("Tiedoston sisältö", fileContent, "OK");
    }
    catch (Exception ex)
    {
      Console.WriteLine("Exception: " + ex.Message);
    }
  }
}