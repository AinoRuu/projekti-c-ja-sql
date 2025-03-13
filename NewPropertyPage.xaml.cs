using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Internals;
using MokinVuokrausApp.luokat;
namespace MokinVuokrausApp;




public partial class NewPropertyPage : ContentPage
{
    private readonly UudenvarauskenHallinta uusivaraushallinta;
    public ObservableCollection<string> Mokit{get; set;} = new ObservableCollection<string>();
    public ObservableCollection<Varaus> Varaus {get; set;}

    Varaus valittuVaraus;
    
    public NewPropertyPage()
    {
        InitializeComponent();
        BindingContext = this;
        uusivaraushallinta = new UudenvarauskenHallinta();
        haeJaTaytaAluePicker();
        haeJaTaytaAsiakaspicker();
        LoadRentingInfo();
        muokkaus_nappi.IsVisible = false;
    }
    //varauksen luonnin napin tapahtumankäsittelijä.
    public void TeeVaraus_Clicked(object sender, EventArgs e)
    {
        //tarkistetaan onko entryt tyhjiä
        if (string.IsNullOrWhiteSpace(EtuNimi_kentta.Text) || string.IsNullOrWhiteSpace(SukuNimi_kentta.Text) ||
            string.IsNullOrWhiteSpace(Osoite_kentta.Text) || string.IsNullOrWhiteSpace(Postinumero_kentta.Text) ||
            string.IsNullOrWhiteSpace(Email_kentta.Text) || string.IsNullOrWhiteSpace(Puhelinnumero_kentta.Text))
            {
                //jos entryt oli thjiä annetaan virhe ilmoitus
                DisplayAlert("virhe", "täytä kaikki kentät!","OK" );
                return;
            }
        //tallennetaan pickerin valinta muuttujaan
        string mokinnimi = mokkipicker.SelectedItem?.ToString();
        //jos valinta tyhjä, valitetaan
        if(string.IsNullOrEmpty(mokinnimi))
        {
            DisplayAlert("Virhe", "Valitse mökki!", "OK");
            return;
        }
        //tallennellaan valittu asiakas muuuttjaan pickeristä
        string valittuAsiakas = Asiakaspicker.SelectedItem?.ToString();
        //muuttuja asiakasID:lle
        int asiakasID;
        //tarkistellaan onko valittuasiakas null tai tyhjä. eli onko pickerillä valittu asiakas vai ei.
        if(string.IsNullOrEmpty(valittuAsiakas))
        {   
            //jos tyhjä luodaan uusi asiakas
            Posti uusiPostinumero = new Posti(Postinumero_kentta.Text);
            Asiakas uusiAsiakas = new Asiakas
            {
                Etunimi = EtuNimi_kentta.Text,
                Sukunimi = SukuNimi_kentta.Text,
                Osoite = Osoite_kentta.Text,
                Postinumero = uusiPostinumero,
                Sahkoposti = Email_kentta.Text,
                PuhelinNumero = Puhelinnumero_kentta.Text
            };
           
            //kutsutaan metodia jolla tallennetaan luotu asiakas   
            uusivaraushallinta.TallennaAsiakas(uusiAsiakas);

            //haetaan yllä luodun asiakkaan id varaustavarten
            //kutusutaan metodia joka ottaa parametreiksi etu ja sukunimen, jonka perusteella hakee ID:n
            asiakasID = uusivaraushallinta.HaeAsiakasIDNimella(EtuNimi_kentta.Text,SukuNimi_kentta.Text);

            //jos funktio palauttaa asetetun oletusarvon, annetaan virheilmotus
            if(asiakasID == -1)
            {
                DisplayAlert("virhe","asiakasta ei voitu lisätä","OK");
                return;
            }
        }
        //jos ollaan pickerillä valittu tietokannassa oleva asiakas
        else
        {
            //erotellaan id merkkijonosta joka haetaan pickeriin tietokannasta
            //etsii sulkujen indeksit, jotka erottelevat id:n ja asiakastiedot
            int alku = valittuAsiakas.IndexOf("(ID: ");
            int loppu = valittuAsiakas.IndexOf(")");

            // Tarkista että indeksit ovat kelvollisia
            if (alku != -1 && loppu != -1 && alku < loppu)
            {
                // Erottele ID merkkijonosta
                string asiakasIDstring = valittuAsiakas.Substring(alku + "(ID:".Length, loppu - alku - "(ID:".Length);
            
                if (!int.TryParse(asiakasIDstring, out asiakasID))
                {
                    DisplayAlert("virhe","Virheellinen asiakas ID", "ok");
                    return;
                }                   
            }
            else
            {
                DisplayAlert("virhe","Virheellinen asiakastieto", "ok");
                return;
            }
        }
        //haetaan valitun mökin ID varausta varten
        int mokinID = uusivaraushallinta.HaeMokkiIDNimella(mokinnimi);
        //jos palautuu oletusarvo, annetaan virhe
        if (mokinID == -1)
        {
            DisplayAlert("Virhe", "Mökkiä ei löytynyt!", "OK");
            return;
        }
        // luodaan uusi varaus öliö
        Varaus varaus = new Varaus
        {
            Asiakas = new Asiakas{Asiakas_id = asiakasID},
            Mokki = new Mokki{Mokki_id = mokinID},
            Varattu_pvm = DateTime.Now,
            Vahvistus_pvm = DateTime.Now,
            Varattu_AlkuPvm = alkupvm.Date,
            Varattu_LoppuPvm = loppupvm.Date
        };
        //tallennetaan varaus tietokantaan kutsumalla LuoUusiVaraus metodia joka ottaa olion parametrina
        uusivaraushallinta.LuoUusiVaraus(varaus);
        //ilmoitetaan onnistuneesta varauksen luonnista
        DisplayAlert("Onnistui", "Varaus luotu!", "OK");
        Tyhjenna_kentat();
        LoadRentingInfo();
    }


    //funktio jolla täytetään picker
    private void haeJaTaytaAluePicker()
    {
        //tallennellaan haealueet metodin palauttama lista var tyyppiseen muuttujaan. 
        //kääntäjä tietää että palautetaan lista, joten voidaan käyttää var:ia ja näin säästää kaikkien aikaa.  
        var alueet = uusivaraushallinta.HaeAlueet();

        //loopataan lista läpi ja lisäillään aluepickeriin. 
        foreach(var alue in alueet)
        {
            Aluepicker.Items.Add(alue);
        }
    }
    //funktio jolla täytetään asiakaspicker

    //tässä tehdään täysin sama kun yllä. 
   private void haeJaTaytaAsiakaspicker()
    {
        var asiakkaat = uusivaraushallinta.HaeasiakasTiedot();

        foreach(var asiakas in asiakkaat )
        {
            Asiakaspicker.Items.Add(asiakas);
        }
    }
    // funktio jolla saadaan alue pickerin valinnan perusteella haettua mökit mökki pickeriin 
    // aluepickerin tapahtuman käsittelijä. 
    private void Aluepicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        //tallennetaan aluepickerin valinta muuttujaa.  
        string valittuAlue = Aluepicker.SelectedItem?.ToString();
        //tyhjennetään mökki kokoelma varalta ennen kuin täytetään. 
        Mokit.Clear();
        //haetaan mökkien nimet tietokannasta valitun alueen perusteella
        if(!string.IsNullOrEmpty(valittuAlue))
        {   
            //haetaan alueen id valitun alueen perusteella
            int alueID = uusivaraushallinta.HaeAlueIdNimella(valittuAlue);

            //haetaan mökit tietokannasta
            var mokit = uusivaraushallinta.HaeMokit(alueID);
            //loopataan mökki kokoelmaan josta ne bingataan pickeriin
            foreach (var mokki in mokit)
            {
                Mokit.Add(mokki);
            }
           
        }
    }

    //funktio jolla haetaan asiakkaan tiedot pickerin valinnan perusteella ja täytetään entryihin.
    //koska asiakastiedot tulevat pickeriin yhdistettynä merkkijonona, joudutaan merkkijonosta pilkkomaan id 
    //erilleen jotta saadaan välitettyä id asiakkaan haku funktiolle

    private void Asiakaspicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string valittuAsiakas = Asiakaspicker.SelectedItem?.ToString();

        if (!string.IsNullOrEmpty(valittuAsiakas))
        {
            //etsitään sulkujen indeksit, jotka erotttavat ID.n ja asiakasttiedot
            int alku = valittuAsiakas.IndexOf("(ID: ");
            int loppu = valittuAsiakas.LastIndexOf(")");

            //tarkistetaan että indexit on kelvollisia
            if(alku != -1 && loppu != -1 && alku < loppu)
            {
                //eroteraan id merkkijonosta
                string asiakasIDString = valittuAsiakas.Substring(alku + "(ID:".Length, loppu - alku - "(ID:".Length);
                int asiakasID;
                if(int.TryParse(asiakasIDString, out asiakasID))
                {
                    //kutsutaan metodia joka hakee asiakastiedot id:nperusteella
                    Asiakas asiakas = uusivaraushallinta.HaeAsiakasTiedot(asiakasID);

                    //tarkastellaan että asiakas ei ole tyhjä
                    if(asiakas != null)
                    {
                        //asetetaam asiakastiedot entryihin
                        EtuNimi_kentta.Text = asiakas.Etunimi;
                        SukuNimi_kentta.Text = asiakas.Sukunimi;
                        Osoite_kentta.Text = asiakas.Osoite;
                        Posti posti = asiakas.Postinumero;
                        Postinumero_kentta.Text = posti.Postinumero;
                        Puhelinnumero_kentta.Text = asiakas.PuhelinNumero;
                        Email_kentta.Text = asiakas.Sahkoposti;
                    }
                    else
                    {
                        DisplayAlert("virhe", "asiakasta ei löytynyt.", "ok");
                    }
                }
                else
                {
                    DisplayAlert("virhe", "virheellinen asaikas ID.", "ok");
                }
            }
            else
            {
                DisplayAlert("virhe", "virheellinen asiakastieto.", "ok");
            }
        }
    }
    //tyhjennä napin tapahtumankäsittelijä. kutuss funktioa joka tyhjentää kentät
    public void Tyhjenna_Clicked(Object sender, EventArgs e)
    {
        Tyhjenna_kentat();
    }

    // funktio joka tyhjentää kentät. kutustaan tyhjennys napin tapahtumankäsittelijässä ja varauksen luonnin lopussa. 
    public void Tyhjenna_kentat()
    {
        EtuNimi_kentta.Text = string.Empty;
        SukuNimi_kentta.Text = string.Empty;
        Osoite_kentta.Text = string.Empty;
        Postinumero_kentta.Text = string.Empty;
        Puhelinnumero_kentta.Text = string.Empty;
        Email_kentta.Text = string.Empty;
        Mokit.Clear();
        Aluepicker.SelectedIndex = -1;
        Asiakaspicker.SelectedIndex = -1;
        alkupvm.Date = DateTime.Now;
        loppupvm.Date = DateTime.Now;
        asiakastext.Text = "Asiakas tiedot";
        mokkitext.Text = "Mökin tiedot";
        varaus_nappi.IsVisible = true;
        muokkaus_nappi.IsVisible = false;  
        
    }

    //funktio jolla haetaan varaukset taulukkoon. luodaan varaustyyppinen lista, haetaan siihen tiedot haevarauksettietokannast funktiolla
    //asetetaan palautetut varaukset kokoelmaan ja päivitetään listview,.
    public async void LoadRentingInfo()
    {
        List<Varaus> varaukset = await uusivaraushallinta.HaeVarauksetTietokannasta();
        Varaus = new ObservableCollection<Varaus>(varaukset);
        RentingInfo.ItemsSource = Varaus;
    }

    //varauksen valinnan tapahtumankäsittelijä. painamalla varausta, voidaan valita muokataanko varausta vai poistetaanko varaus.
    public async void NaytaValikko(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
    {
       if (e.SelectedItem is Varaus valittu)
       {
            
            string valinta = await DisplayActionSheet("Haluatko muokata vai poistaa?", "Peruuta", null, "Muokkaa", "Poista");
            switch (valinta)
            {
                case "Muokkaa":
                    Varaus haettuVaraus = await uusivaraushallinta.HaeVarausTiedot(valittu.Varaus_id);
                    valittuVaraus = haettuVaraus;
                    if (haettuVaraus != null)
                    {
                        Mokit.Clear();

                        EtuNimi_kentta.Text = haettuVaraus.Asiakas.Etunimi;
                        SukuNimi_kentta.Text = haettuVaraus.Asiakas.Sukunimi;
                        Osoite_kentta.Text = haettuVaraus.Asiakas.Osoite;
                        Posti posti = haettuVaraus.Asiakas.Postinumero;
                        Postinumero_kentta.Text = posti.Postinumero;
                        Puhelinnumero_kentta.Text = haettuVaraus.Asiakas.PuhelinNumero;
                        Email_kentta.Text = haettuVaraus.Asiakas.Sahkoposti;                 
                        alkupvm.Date = haettuVaraus.Varattu_AlkuPvm;
                        loppupvm.Date = haettuVaraus.Varattu_LoppuPvm;
                        

                        
                        string valittuAlue = haettuVaraus.Mokki.Alue.Nimi;
                        int alueIndex = Aluepicker.Items.IndexOf(valittuAlue);
                        if (alueIndex != -1)
                        {
                            Aluepicker.SelectedIndex = alueIndex;
                        }

                        // Päivitä MokkiPicker valitun alueen perusteella
                        if (haettuVaraus.Mokki.Alue != null)
                        {
                            int alueID = haettuVaraus.Mokki.Alue.Alue_Id;
                            var mokit = uusivaraushallinta.HaeMokit(alueID);
                            foreach (var mokki in mokit)
                            {
                                Mokit.Add(mokki);
                            }

                            // Tarkista, että varauksella on määrätty mökki
                            string valittuMokki = haettuVaraus.Mokki.Nimi;
                            int mokkiIndex = Mokit.IndexOf(valittuMokki);
                            if (mokkiIndex != -1)
                            {
                                mokkipicker.SelectedIndex = mokkiIndex;
                            }
                        } 
                        asiakastext.Text = "Muokkaa Asiakas";
                        mokkitext.Text = "Muokkaa Mökit";  
                        varaus_nappi.IsVisible = false;
                        muokkaus_nappi.IsVisible = true;    
                    }
                    break;

                case "Poista":
                        await uusivaraushallinta.PoistaVaraus(valittu.Varaus_id);
                        // poistetaan varaus myös kokoelmasta
                        Varaus.Remove(valittuVaraus);
                        LoadRentingInfo();
                    break;

                default:
                    // Peruuta
                    break;
            }
       }
       else
       {
           await DisplayAlert("Virhe","ei voitu hakea varausta tietokannasta","ok");
       }
        
           
    }

    //muokkaa napin event handler
    public async void muokkaa_nappi_Clicked(object sender, EventArgs e)
    {
        Asiakas muokattuAsiakas = new Asiakas
        {
            Asiakas_id = valittuVaraus.Asiakas.Asiakas_id,
            Etunimi = EtuNimi_kentta.Text,
            Sukunimi = SukuNimi_kentta.Text,
            Osoite = Osoite_kentta.Text,
            Postinumero = new Posti(Postinumero_kentta.Text),
            Sahkoposti = Email_kentta.Text,
            PuhelinNumero = Puhelinnumero_kentta.Text
        };
        
        Varaus muokattuVaraus = new Varaus
        {
           Varaus_id = valittuVaraus.Varaus_id,
           Asiakas = muokattuAsiakas,
           Varattu_AlkuPvm = alkupvm.Date,
           Varattu_LoppuPvm = loppupvm.Date,
           Varattu_pvm = DateTime.Now,
           Vahvistus_pvm = DateTime.Now
        };


        if(muutokset(valittuVaraus, muokattuVaraus ))
        {          
            await uusivaraushallinta.PaivitaVaraus(muokattuVaraus);
            await DisplayAlert("Onnistui", "Varaus päivitetty onnistuneesti.", "OK");
        }
        else
        {
           await DisplayAlert("virhe","ei muutoksia","ok");
        } 
    }

    private bool muutokset(Varaus valittuVaraus, Varaus muokattuVaraus)
    {
         // Tarkistetaan, onko muutoksia tapahtunut etunimessä, sukunimessä, osoitteessa jne.
    bool onMuutoksia = false;

    if (valittuVaraus.Asiakas.Etunimi != muokattuVaraus.Asiakas.Etunimi ||
        valittuVaraus.Asiakas.Sukunimi != muokattuVaraus.Asiakas.Sukunimi ||
        valittuVaraus.Asiakas.Osoite != muokattuVaraus.Asiakas.Osoite ||
        valittuVaraus.Asiakas.Postinumero.ToString() != muokattuVaraus.Asiakas.Postinumero.ToString() ||
        valittuVaraus.Asiakas.PuhelinNumero != muokattuVaraus.Asiakas.PuhelinNumero ||
        valittuVaraus.Asiakas.Sahkoposti != muokattuVaraus.Asiakas.Sahkoposti ||
        valittuVaraus.Varattu_AlkuPvm != muokattuVaraus.Varattu_AlkuPvm ||
        valittuVaraus.Varattu_LoppuPvm != muokattuVaraus.Varattu_LoppuPvm ||
        valittuVaraus.Mokki.Nimi != muokattuVaraus.Mokki.Nimi ||
        valittuVaraus.Mokki.Alue.Nimi != muokattuVaraus.Mokki.Alue.Nimi)
    {
        onMuutoksia = true;
    }

    return onMuutoksia;

    }
}