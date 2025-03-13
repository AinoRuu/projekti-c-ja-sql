namespace MokinVuokrausApp.luokat
{
    using MySqlConnector;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    

    public class UudenvarauskenHallinta
{
    private readonly DataBaseConnector DBcon;
    
    public UudenvarauskenHallinta()
    {
        DBcon = new DataBaseConnector();
    }
//-------------------------------------------------------------------------------------------
// Author: Atro Ojalehto 
//-------------------------------------------------------------------------------------------
// luokka sivu, jonne on survottu kaikki sql haku ja tallennus funktiot. 
// useiden funktioiden rakenne on sama, jossa avataan tietokantayhteys, 
// annetaan kysely ja tallennetaan tai palautetaan tietokantaan/tietokannasta
//-------------------------------------------------------------------------------------------

//funktio joka luo uuden varauksen tietokantaan. ottaa vastaan varaus tyyppisen parametrin
//suoritetaan teevaraus napin tapahtuman käsittelijässä.(TeeVaraus_Clicked)
    public void LuoUusiVaraus(Varaus varaus)
    {
        try
        {
            using (MySqlConnection con = DBcon._getConnection())
            {
                con.Open();
                
                //luodaan kysely
                string query = $"INSERT INTO varaus(asiakas_id, mokki_id, varattu_pvm, vahvistus_pvm, varattu_alkupvm, varattu_loppupvm)"+
                                $"VALUES (@asiakas_id, @mokki_id, @varattu_pvm, @vahvistus_pvm, @varattu_alkupvm, @varattu_loppupvm)";
                //luodaan uusi instanssi mysqlcommadista ja annetaan parametriksi kysely ja avaaja
                MySqlCommand cmd = new MySqlCommand(query, con);
                //syötetään tiedot
                cmd.Parameters.AddWithValue("@asiakas_id", varaus.Asiakas.Asiakas_id);
                cmd.Parameters.AddWithValue("@mokki_id", varaus.Mokki.Mokki_id);
                cmd.Parameters.AddWithValue("@varattu_pvm", varaus.Varattu_pvm);
                cmd.Parameters.AddWithValue("@vahvistus_pvm", varaus.Vahvistus_pvm);
                cmd.Parameters.AddWithValue("@varattu_alkupvm", varaus.Varattu_AlkuPvm);
                cmd.Parameters.AddWithValue("@varattu_loppupvm", varaus.Varattu_LoppuPvm);
                //suoritetaaan tietokannan muokkaus
                cmd.ExecuteNonQuery();
            }
        }catch(Exception ex){Console.WriteLine("Ei voitu luoda uutta varausta: " + ex);}
        
    }
//funktio joka tallentaa asiakkaan. ottaa vastaan asiakas tyyppisen parametrin
    public void TallennaAsiakas(Asiakas asiakas)
    {
        try
        {
            using(MySqlConnection con = DBcon._getConnection())
            {   
                //avataan yhteys tietokantaan
                con.Open();
                //asetetaan kysely
                string query = $"INSERT INTO asiakas (etunimi, sukunimi, lahiosoite, postinro, email, puhelinnro) "+
                $"VALUES(@etunimi, @sukunimi, @lahiosoite, @postinro, @email, @puhelinnro)";
                //luodaan uusi instanssi mysqlcommadista ja annetaan parametriksi kysely ja avaaja
                MySqlCommand cmd = new MySqlCommand(query, con);
                //syötetään tiedot
                cmd.Parameters.AddWithValue("@etunimi", asiakas.Etunimi);
                cmd.Parameters.AddWithValue("@sukunimi", asiakas.Sukunimi);
                cmd.Parameters.AddWithValue("@lahiosoite", asiakas.Osoite);
                cmd.Parameters.AddWithValue("@postinro", asiakas.Postinumero.Postinumero);
                cmd.Parameters.AddWithValue("@email", asiakas.Sahkoposti);
                cmd.Parameters.AddWithValue("@puhelinnro", asiakas.PuhelinNumero);
                //suoritetaaan tietokannan muokkaus
                cmd.ExecuteNonQuery();
            }
            //virhe jos ei onnistu
        }catch(Exception ex){Console.WriteLine("Ei voitu luoda uutta asiakasta: " + ex);}
    }


    //hakee alueet tietokannasta ja palauttaa alueet listana.
    //tätä käytetään aluepickerin täyttämiseen.
    public List<string> HaeAlueet()
    {
        List<string> alueet = new List<string>();
        try
        {
            using(MySqlConnection con = DBcon._getConnection())
            {
                con.Open();
                string query = "SELECT nimi FROM alue";

                MySqlCommand cmd = new MySqlCommand(query, con);
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string aluenimi = reader["nimi"].ToString();
                            alueet.Add(aluenimi);
                        }
                    }
                }
            }
        }catch(Exception ex){Console.WriteLine("Ei voitu luoda uutta asiakasta: " + ex);}
        
        return alueet;
    }

    // funktio joka hakee alueen id:n nimen perusteella ja palauttaa idn.
    //käytettään Aluepicker_SelectedIndexChanged tapahtuman käsittelijässä. 

     public int HaeAlueIdNimella(string alueNimi)
    {
        
        int alueId = -1; //oletusarvo jos ID:tä ei löydy
        try
        {
            using(MySqlConnection con = DBcon._getConnection())
            {
                con.Open();
                string query = "SELECT alue_id FROM alue WHERE nimi = @nimi";

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nimi", alueNimi);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    alueId = Convert.ToInt32(result);
                } 
            }
        }catch(Exception ex){Console.WriteLine("Ei voitu luoda uutta asiakasta: " + ex);}
        

        return alueId;
    }
    //funktio jolla haetaan mökit alue_idllä. ottaa vastaan alue id integerinä joka saadaan 
    //Aluepicker_SelectedIndexChanged tapahtuman käsittelijässä. palauttaa mökit id:n perusteella tietokannasta 
    public List<string> HaeMokit(int alueId)
    {
        List<string> mokit = new List<string>();

        try
        {
             using(MySqlConnection con = DBcon._getConnection())
             {
                con.Open();
                string query = @"SELECT mokkinimi FROM mokki where alue_id = @alue_id";

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@alue_id", alueId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string mokinNimi = reader["mokkinimi"].ToString();
                        mokit.Add(mokinNimi);
                    }
                }
             }
        }catch(Exception ex){Console.WriteLine("Ei voitu luoda uutta asiakasta: " + ex);}
       
        return mokit;
    }

//haetaan tällä asiakkaat tietokannasta jotta saadan ne listattua asiakaspickeriin
//ei ota argumentteja palauttaa listan asiakkaista
    public List<String> HaeasiakasTiedot()
    {
        List<string> asiakasTiedot = new List<string>();
        try
        {
              using(MySqlConnection con = DBcon._getConnection())
            {
                con.Open();
                string query = "SELECT asiakas_id, etunimi, sukunimi FROM asiakas";

                MySqlCommand cmd = new MySqlCommand(query, con);
                
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        int asiakasID = Convert.ToInt32(rdr["asiakas_id"]);
                        string etunimi = rdr["etunimi"].ToString();
                        string sukunimi = rdr["sukunimi"].ToString();

                        string asiakasTieto = $"{etunimi} {sukunimi} (ID: {asiakasID})";

                        asiakasTiedot.Add(asiakasTieto);
                    }
                } 
            };
        }catch(Exception ex){Console.WriteLine("Ei voitu luoda uutta asiakasta: " + ex);}
        
        return asiakasTiedot;
    }
    //funktio joka hakee asiakastiedot, luo uuden asiakas olion ja palauttaa sen. 
    //ottaa vastaan asiakas ID:n joka erotellaan Asiakaspicker_SelectedIndexChanged tapahtumankäsittelijässä

     public Asiakas HaeAsiakasTiedot(int asiakasID)
     {
        Asiakas asiakas = null;
        try{
            using (MySqlConnection con = DBcon._getConnection())
            {
                con.Open();
                string query = "SELECT * FROM asiakas WHERE asiakas_id = @asiakasID";

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@asiakasID", asiakasID);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        asiakas = new Asiakas
                        {
                            Asiakas_id = Convert.ToInt32(rdr["asiakas_id"]),
                            Etunimi = rdr["etunimi"].ToString(),
                            Sukunimi = rdr["sukunimi"].ToString(),
                            Osoite = rdr["lahiosoite"].ToString(),
                            Postinumero = new Posti(rdr["postinro"].ToString()), 
                            Sahkoposti = rdr["email"].ToString(),
                            PuhelinNumero = rdr["puhelinnro"].ToString()
                        };
                    }
                } 
            }
        }catch(Exception ex){Console.WriteLine("Ei voitu luoda uutta asiakasta: " + ex);}
       
        return asiakas;
     }    

   
    //funktio asiakas ID hakua varten. käytetään varauksen luonnissa TeeVaraus_Clicked tapahtumankäsittelijässä. 
    public int HaeAsiakasIDNimella(string etunimi, string sukunimi)
    {
        int asiakasID = -1; //oletus arvo jos idtä ei löydy
        try
        {
            using (MySqlConnection con = DBcon._getConnection())
            {
                con.Open();
                string query = "SELECT asiakas_id FROM asiakas WHERE etunimi = @etunimi AND sukunimi = @sukunimi";

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@etunimi", etunimi);
                cmd.Parameters.AddWithValue("@sukunimi", sukunimi);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    asiakasID = Convert.ToInt32(result);
                }
            }
        }catch(Exception ex){Console.WriteLine("Ei voitu luoda uutta asiakasta: " + ex);}
       
        return asiakasID;
    }
    //haetaan valitun mökin id nimellä. käytetään varauksen luonnissa TeeVaraus_Clicked tapahtuman käsittelijässä.
    public int HaeMokkiIDNimella(string mokinNimi)
    {
        int mokinID = -1;//oletus arvo jos idtä ei löydy
        try
        {
            using (MySqlConnection con = DBcon._getConnection())
            {
                con.Open();
                string query = "SELECT mokki_id FROM mokki WHERE mokkinimi = @mokinNimi";

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@mokinNimi", mokinNimi);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    mokinID = Convert.ToInt32(result);
                }
            }
        }catch(Exception ex){Console.WriteLine("Ei voitu luoda uutta asiakasta: " + ex);}
       
        return mokinID;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //tästä alaspäin on funktiot varausten hakemiselle, muokkaamiselle ja poistamiselle. sama olisi tehdä uudelle sivulle, mutta meni jo.                                                                                       //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------//


    // funktio jolla haetaan varaukset tietokannasta ja palautetaan ne listana. 
    public async Task<List<Varaus>> HaeVarauksetTietokannasta()
    {
        List<Varaus> varaukset = new List<Varaus>();

        try
        {
            using (MySqlConnection conn = DBcon._getConnection())
            {
                await conn.OpenAsync();
                string query = "SELECT v.varaus_id, a.sukunimi AS asiakkaan_sukunimi, m.mokkinimi AS mökin_nimi, v.varattu_alkupvm, v.varattu_loppupvm FROM varaus v JOIN asiakas a ON v.asiakas_id = a.asiakas_id JOIN mokki m ON v.mokki_id = m.mokki_id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while(await rdr.ReadAsync())
                        {
                            Asiakas asiakas = new Asiakas();
                            asiakas.Sukunimi = rdr.GetString("asiakkaan_sukunimi");
                            Mokki mokki = new Mokki();
                            mokki.Nimi = rdr.GetString("mökin_nimi");

                            varaukset.Add(new Varaus
                            {
                                Varaus_id = rdr.GetInt32("varaus_id"),
                                Asiakas = asiakas,
                                Mokki = mokki,
                                Varattu_AlkuPvm = rdr.GetDateTime("varattu_alkupvm"),
                                Varattu_LoppuPvm = rdr.GetDateTime("varattu_loppupvm")
                            });
                        }
                    }
                }
            }
        }catch(MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
        return varaukset;
    }
    //funktio jolla voidaan hakea varaus ID:n perusteella
    public async Task<Varaus> HaeVarausTiedot(int varausId)
    {
        Varaus varaus = null;

        try
        {
            using (MySqlConnection conn = DBcon._getConnection())
            {
                await conn.OpenAsync();
                string query = @"
                                SELECT 
                                    v.varaus_id, 
                                    a.etunimi, a.sukunimi, a.lahiosoite, a.postinro, a.puhelinnro, a.email,
                                    p.toimipaikka, 
                                    al.nimi AS alue_nimi, 
                                    m.mokkinimi, m.katuosoite, m.mokki_id, 
                                    v.varattu_pvm, v.vahvistus_pvm, v.varattu_alkupvm, v.varattu_loppupvm 
                                FROM 
                                    varaus v 
                                    JOIN asiakas a ON v.asiakas_id = a.asiakas_id 
                                    JOIN posti p ON a.postinro = p.postinro 
                                    JOIN mokki m ON v.mokki_id = m.mokki_id 
                                    JOIN alue al ON m.alue_id = al.alue_id 
                                WHERE 
                                    v.varaus_id = @varausId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@varausId", varausId);

                    using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        if (await rdr.ReadAsync())
                        {
                            Posti postinro = new Posti
                            {
                                Postinumero = rdr.GetString("postinro"),
                                Postitoimipaikka = rdr.GetString("toimipaikka")
                            };

                            Asiakas asiakas = new Asiakas
                            {
                                Etunimi = rdr.GetString("etunimi"),
                                Sukunimi = rdr.GetString("sukunimi"),
                                Osoite = rdr.GetString("lahiosoite"),
                                Postinumero = postinro,
                                PuhelinNumero = rdr.GetString("puhelinnro"),
                                Sahkoposti = rdr.GetString("email")
                            };
                            

                            Alue alue = new Alue
                            {
                                Nimi = rdr.GetString("alue_nimi")
                            };

                            Mokki mokki = new Mokki
                            {
                                Mokki_id = rdr.GetInt32("mokki_id"),
                                Nimi = rdr.GetString("mokkinimi"),
                                Osoite = rdr.GetString("katuosoite"),
                                Alue = alue

                            };

                            varaus = new Varaus
                            {
                                Varaus_id = rdr.GetInt32("varaus_id"),
                                Asiakas = asiakas,                          
                                Mokki = mokki,
                                Varattu_pvm = rdr.GetDateTime("varattu_pvm"),
                                Vahvistus_pvm = rdr.GetDateTime("vahvistus_pvm"),
                                Varattu_AlkuPvm = rdr.GetDateTime("varattu_alkupvm"),
                                Varattu_LoppuPvm = rdr.GetDateTime("varattu_loppupvm")
                            };
                        }
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message + "tassa menipieleen");
        }

        return varaus;
    }
    
    //funktio joka poistaa varauksen id:n perusteella. ottaa vatsaan varauksen id:n jonka perusteell apoistaa tietokannasta.

    public async Task PoistaVaraus(int varausId)
    {
        try
        {
            using (MySqlConnection conn = DBcon._getConnection())
            {
                await conn.OpenAsync();
                string query = "DELETE FROM varaus WHERE varaus_id = @varausID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@varausID", varausId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }catch(MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    //varauksen päivittämisen funktio.
    public async Task PaivitaVaraus(Varaus muokattuVaraus)
    {
        try
        {
            using (MySqlConnection conn = DBcon._getConnection())
            {
                await conn.OpenAsync();
                string query = @"
                            UPDATE varaus 
                            SET 
                                asiakas_id = @asiakas_id,                                  
                                varattu_pvm = @varattu_pvm, 
                                vahvistus_pvm = @vahvistus_pvm, 
                                varattu_alkupvm = @varattu_alkupvm, 
                                varattu_loppupvm = @varattu_loppupvm
                            WHERE varaus_id = @varaus_id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@asiakas_id", muokattuVaraus.Asiakas.Asiakas_id);                 
                    cmd.Parameters.AddWithValue("@varattu_pvm", muokattuVaraus.Varattu_pvm);
                    cmd.Parameters.AddWithValue("@vahvistus_pvm", muokattuVaraus.Vahvistus_pvm);
                    cmd.Parameters.AddWithValue("@varattu_alkupvm", muokattuVaraus.Varattu_AlkuPvm);
                    cmd.Parameters.AddWithValue("@varattu_loppupvm", muokattuVaraus.Varattu_LoppuPvm);
                    cmd.Parameters.AddWithValue("@varaus_id", muokattuVaraus.Varaus_id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }


            await PaivitaAsiakasTietokantaan(muokattuVaraus.Asiakas);
            await PaivitaMokkiTietokantaan(muokattuVaraus.Mokki);

        }catch (MySqlException ex)
        {
            //joku virheilmo tähän taas
            Console.WriteLine(ex.Message + "ei onniostyu");
        }
    }

    //funktio asiakkaan päivittämiseen
    private async Task PaivitaAsiakasTietokantaan(Asiakas asiakas)
    {
        try
        {
            using (MySqlConnection conn = DBcon._getConnection())
            {
                await conn.OpenAsync();
                string query = @"
                                UPDATE asiakas 
                                SET 
                                    etunimi = @etunimi, 
                                    sukunimi = @sukunimi, 
                                    lahiosoite = @lahiosoite, 
                                    postinro = @postinro, 
                                    puhelinnro = @puhelinnro, 
                                    email = @email
                                WHERE asiakas_id = @asiakas_id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@etunimi", asiakas.Etunimi);
                    cmd.Parameters.AddWithValue("@sukunimi", asiakas.Sukunimi);
                    cmd.Parameters.AddWithValue("@lahiosoite", asiakas.Osoite);
                    cmd.Parameters.AddWithValue("@postinro", asiakas.Postinumero.Postinumero);
                    cmd.Parameters.AddWithValue("@puhelinnro", asiakas.PuhelinNumero);
                    cmd.Parameters.AddWithValue("@email", asiakas.Sahkoposti);
                    cmd.Parameters.AddWithValue("@asiakas_id", asiakas.Asiakas_id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    //metodi mökin päivittämiseen.
    private async Task PaivitaMokkiTietokantaan(Mokki mokki)
{
    try
    {
        using (MySqlConnection conn = DBcon._getConnection())
        {
            await conn.OpenAsync();
            string query = @"
                            UPDATE mokki 
                            SET 
                                mokkinimi = @mokkinimi, 
                                katuosoite = @katuosoite, 
                                alue_id = @alue_id
                            WHERE mokki_id = @mokki_id";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@mokkinimi", mokki.Nimi);
                cmd.Parameters.AddWithValue("@katuosoite", mokki.Osoite);
                cmd.Parameters.AddWithValue("@alue_id", mokki.Alue.Alue_Id);
                cmd.Parameters.AddWithValue("@mokki_id", mokki.Mokki_id);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
    catch (MySqlException ex)
    {
        Console.WriteLine(ex.Message);
    }
}

}
}


