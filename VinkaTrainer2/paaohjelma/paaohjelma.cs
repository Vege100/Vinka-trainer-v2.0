using FontStashSharp;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace paaohjelma
{
    /// @author: Verneri Kilpeläinen ja Samuel Koljonen 
    /// @version: 2.0 Beta
    /// <summary>
    /// Vinka Trainer
    /// </summary>

    public class paaohjelma : PhysicsGame
    {
        PhysicsObject pelaaja;
        IntMeter pisteLaskuri;
        IntMeter rahalaskuri;
        int raha = 30;
        int elama = 3;
        int telama = 3;
        double ohjusnopeus = 2;
        EasyHighScore toplista = new();

        
        /// <summary>
        /// Alustaa ohjelman
        /// </summary>
        public override void Begin()
        {
            IsFullScreen = true;
            Valikko();
            Luotaustavinka();
            Soitavapaata();
        }

        
        /// <summary>
        /// Aloitusvalikko, joka sisältää liikkumisen eri valikkojen välillä
        /// </summary>
        void Valikko()
        {

            MultiSelectWindow alkuValikko = new MultiSelectWindow("", "Start", "Points", "Shop", "Quit");
            Luolaskuri(1);
            Add(alkuValikko);
            alkuValikko.AddItemHandler(0, Start);
            alkuValikko.AddItemHandler(1, delegate { Points(false); });
            alkuValikko.AddItemHandler(2, Shop);
            alkuValikko.AddItemHandler(3, Exit);
        }


        /// <summary>
        /// Kesken pelin valittava pause, joka avautuu 'm' näppäimestä
        /// myös luovutus mahdollinen
        /// </summary>
        void Pausevalikko()
        {
            MultiSelectWindow pausevalikko = new MultiSelectWindow("", "Continue", "Giveup");
            Pausettaa();
            Add(pausevalikko);
            pausevalikko.AddItemHandler(0, Pausettaa);
            pausevalikko.AddItemHandler(1, Gameover);

        }


        /// <summary>
        /// ApuAliOhjelma Pauselle, jotta moottorin ääni ei sekoa.
        /// </summary>
        void Pausettaa()
        {
            if (MediaPlayer.IsMuted) Soitamoottoria();
            else MediaPlayer.Volume = 0;
            Pause();
        }


        /// <summary>
        /// Soittaa moottoriääntä
        /// </summary>
        void Soitamoottoria()
        {
            MediaPlayer.Play("motor");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.2;
        }


        /// <summary>
        /// Kutsutaan kun peli aloitetaan
        /// </summary>
        void Start()
        {   
            Luokentta();
            Asetaohjaimet();
            Luolaskuri(0);
            Timer.SingleShot(1,AmmuOhjus);
            Timer.CreateAndStart(2, Ammukolikko);
        }


        /// <summary>
        /// Easyhighscore sakkaa kuten vinka, joten lisäsimme yhden valikon ja siihen varmistuksen
        /// jotta taustaa ei tarvitse luoda uudestaan joka kerta
        /// </summary>
        /// <param name="a">bool asetuksille</param>
        void Points(bool a)
        {
            if (a) ClearAll();
            MultiSelectWindow pisteet = new MultiSelectWindow("", "Are you sure you want to see points?", "I'm afraid");
            Add(pisteet);
            pisteet.AddItemHandler(0, Lista);
            pisteet.AddItemHandler(1, Valikko);
            if (a) Luotaustavinka();
            
        }


        /// <summary>
        /// Näyttää parhaat pisteet! :)
        /// </summary>
        void Lista()
        {
            ClearAll();
            toplista.Show();
            toplista.HighScoreWindow.Closed += delegate { Points(true); };
        }


        /// <summary>
        /// Kauppavalikko, josta voidaan siirtyä aloitukseen tai ostaa elämiä lisää
        /// </summary>
        void Shop()
        {
            MultiSelectWindow shop = new MultiSelectWindow("", "Back", "1 Life 4 10 coins");
            Add(shop);
            shop.AddItemHandler(0, Valikko);
            shop.AddItemHandler(1, Elamaa);
        }


        /// <summary>
        /// toteuttaa kaupassa ostetun elämän lisäyksen
        /// </summary>
        void Elamaa()
        {
            if (rahalaskuri >= 10)
            {
             telama += 1;
             rahalaskuri.AddValue(-10);
             raha -= 10;
            }
            Shop();
        }


        /// <summary>
        /// Soittaa täysin CopyRIghtfree musiikkia! :)
        /// </summary>
        void Soitavapaata()
        {
            MediaPlayer.Play("copyrightfree");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5;
        }


        /// <summary>
        /// Etsii sään Getmetar sivulta ja toistaiseksi oletuksena EFJY
        /// </summary>
        /// <returns>Palauttaa karsitun html, joka sisältää vain metar osuuden</returns>
        public static string[] Etsisaa()
        {
            string[] mika = new string[3];
            string saa = Lataanetista("https://www.getmetar.com/EFJY");
            int a = saa.IndexOf("> EFJY ");
            int b = saa.IndexOf(" Q");
            saa = saa.Substring(a, b - a);
            mika[0] = Etsipilvet(saa);
            mika[1] = Etsisade(saa);
            return mika;
        }


        /// <summary>
        /// Etsii tuodusta stringistä merkkejä pilvistä
        /// </summary>
        /// <param name="saa">Metar string</param>
        /// <returns>Palauttaa onko pilviä vai ei merkkijonona</returns>
        public static string Etsipilvet(string saa)
        {
            string pilvi = "";
            string[] pilvet = { "SKC", "FEW", "SCT", "BKN", "OVC"};
            foreach (string s in pilvet)
            {
                if (saa.Contains(s))
                {
                    pilvi = s;
                }
            }
            return pilvi;
        }


        /// <summary>
        /// Etsii tuodusta stringistä merkkejä Sateesta
        /// </summary>
        /// <param name="saa">Metar string</param>
        /// <returns>Palauttaa onko sadetta vai ei merkkijonona</returns>
        public static string Etsisade(string saa)
        {
            string sade = "";
            string[] sateet = { "DZ", "GR", "GS", "IC", "PL", "RA", "SG", "SN" };
            foreach (string s in sateet)
            {
                if (saa.Contains(s))
                {
                    sade = "RA";
                }
            }
            return sade;
        }


        /// <summary>
        /// Luo taustan Valikoille
        /// </summary>
        void Luotaustavinka()
        {
            GameObject vinkatausta = new(Screen.Width, Screen.Height);
            vinkatausta.Image = LoadImage("vinkataustoitta");
            Add(vinkatausta);
            Level.Background.CreateGradient(Color.White, Color.Blue);
            LuoPallo(3000);
        }


        /// <summary>
        /// Kääntää olioita.
        /// </summary>
        /// <param name="a">Mitä käännetään</param>
        /// <param name="b">kerroin millä käännetään</param>
        void Kaannapalloa(GameObject a,double b)
        {
            a.Angle += Angle.FromDegrees(-0.1*b);
        }


        /// <summary>
        /// Etsii auringonkulman netistä ja karsii HTML
        /// pelkän kulman ja palauttaa sen double arvona
        /// Oletuksena jyväskylä.
        /// </summary>
        /// <returns>auringonkulma*10</returns>
        public static double Etsiaika()
        {
            string kulma = Lataanetista("https://www.timeanddate.com/sun/finland/jyvaskyla");
            int a = kulma.IndexOf("sunalt");
            kulma = kulma.Substring(a + 7, 5);
            string[] numero = kulma.Split(',');
            double akulma = Convert.ToDouble(numero[0] + numero[1][0]);
            return akulma;
        }


        /// <summary>
        /// Lataa netistä annetun sivun ja palauttaa 
        /// sivun stringinä
        /// </summary>
        /// <param name="osoite"></param>
        /// <returns>Sivu stringinä</returns>
        public static string Lataanetista(string osoite)
        {
            WebClient client = new WebClient();
            string lataus = client.DownloadString(osoite);
            return lataus;
        }


        /// <summary>
        /// Luo sään ja kutsuu aliohjelmia, jotka karsivat netistä halutut sääilmiöt metarista.
        /// </summary>
        void Luosaa()
        {
            string[] saa = Etsisaa();
            if (saa[0].Length != 0)
            {
                GameObject pilvi = new(Screen.Width, Screen.Height);
                pilvi.Image = LoadImage(saa[0]);
                Add(pilvi,1);
            }
            if (saa[1].Length != 0)
            {
                GameObject sato = new(Screen.Width, Screen.Height);
                Image[] sade = LoadImages("sade1", "sade2", "sade3");
                sato.Animation = new Animation(sade);
                sato.Animation.FPS = 10;
                sato.Animation.Start();
                Add(sato,-1);
            }
        }


        /// <summary>
        /// Kutsutaan mikäli pitää luoda öinen tausta
        /// </summary>
        void Piirrataivas()
        {
            Level.Background.CreateStars(1000);
            GameObject tummennus = new(Screen.Width, Screen.Height);
            tummennus.Color = new Color(0, 0, 0, 135);
            Add(tummennus, 2);
        }


        /// <summary>
        /// Kutsuu aliohjelmia, joissa määritellään sää, pelaaja, ääni ja lisää collisionhandlerit.
        /// </summary>
        void Luokentta()
        {
            ClearAll();
            Luosaa();
            double auringonkulma = Etsiaika();
            if (auringonkulma < 0) 
            Piirrataivas();
            Soitamoottoria();
            LuoPallo(15000);
            pelaaja = LuoPelaaja();
            AddCollisionHandler(pelaaja, "ohjus", Pelaajatormasi);
            AddCollisionHandler(pelaaja, "kolikko", Pisteita);
            ohjusnopeus = 2;
            elama = telama;

            Level.Height = Screen.Height;
            Level.Width = Screen.Width;



        }


        /// <summary>
        /// Luopallon halutulla koolla
        /// Toistaiseksi peli vaatii vain kaksi palloa, joten parametreinä ei viedä muuta kuin koko
        /// ohjelma luo sopivan kuvan pallolle koon mukaan.
        /// </summary>
        /// <param name="koko">halkaisija</param>
        void LuoPallo(int koko)
        {
            GameObject pallo = new(koko, koko);
            pallo.Shape = Shape.Circle;
            if (koko == 3000) pallo.Image = LoadImage("pallo");
            else pallo.Image = LoadImage("pelipallo");
            if (koko == 3000) pallo.Y = -koko / 2.5;
            else pallo.Y = -koko / 2.05;
            Add(pallo, -2);
            double suunta;
            if (koko == 3000) suunta = 1;
            else suunta = -0.1;
            Timer ajastin = new Timer();
            ajastin.Interval = 0.01;
            ajastin.Timeout += delegate { Kaannapalloa(pallo,suunta); };
            ajastin.Start();
        }


        /// <summary>
        /// kutsutaan collisionhandleristä, 
        /// antaa pelaajalle kolikon ja soittaa Ilmasta musiikkia! :)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        void Pisteita(PhysicsObject a, PhysicsObject b)
        {
            rahalaskuri.AddValue(1);
            SoundEffect klink = LoadSoundEffect("Ilmane");
            klink.Play();
            b.Destroy();
        }


        /// <summary>
        /// törmäyskäsittelijä, kun pelaaja törmää
        /// tuhoaa törmättävän olion ja vie pelaajalta elämää
        /// </summary>
        /// <param name="pelaaja">pelaaja</param>
        /// <param name="ohjus">törmättävä olio</param>
        void Pelaajatormasi(PhysicsObject pelaaja, PhysicsObject ohjus)
        {
            elama -= 1;
            Explosion rajahdys = new Explosion(ohjus.Width);
            rajahdys.Position = (ohjus.Position);
            Add(rajahdys);
            ohjus.Destroy();
            if (elama <= 0)
            {
                Explosion pommi = new Explosion(ohjus.Width);
                pommi.Position = (pelaaja.Position);
                Add(pommi);
                Add(rajahdys);
                pelaaja.Destroy();
                Timer.SingleShot(2, Gameover);
            }


        }


        /// <summary>
        /// Kutsutaan kun pelaaja lopettaa pelin tai elämät loppuvat
        /// putsaa pöydän ja näyttää ennätykset
        /// </summary>
        void Gameover()
        {
            int loppupisteet = pisteLaskuri.Value;
            raha = rahalaskuri;
            ClearAll();
            Soitavapaata();
            toplista.EnterAndShow(loppupisteet);
            toplista.HighScoreWindow.Closed += Valikkoohjaus;
        }


        /// <summary>
        /// Highscore vaatii Clearall() poistumisen jälkeen, joten
        /// aliohjelma putsaa pöydät ja kutsuu taustan ja valikon
        /// </summary>
        /// <param name="sender">Ikkuna.</param>
        public void Valikkoohjaus(Window sender)
        {
            ClearAll();
            Luotaustavinka();
            Valikko();
        }


        /// <summary>
        /// luo pelaajan ja palauttaa sen jotta sille
        /// voidaan luoda törmäyskäsittelijöitä
        /// </summary>
        /// <returns></returns>
        PhysicsObject LuoPelaaja()
        {
            Image vinka = LoadImage("vinkafreimi");
            PhysicsObject pelaaja = PhysicsObject.CreateStaticObject(333, 120);
            pelaaja.Shape = Shape.FromImage(vinka);
            pelaaja.Y = 0;
            pelaaja.Tag = "pelaaja";
            pelaaja.X = Level.Left + 150;
            pelaaja.Image = LoadImage("vinka3");
            Add(pelaaja,0);
            Timer.CreateAndStart(0.01, Paivita);
            return pelaaja;

        }


        /// <summary>
        /// Ampuu ohjuksen random y arvolla ja kutsuu itseään uudestaan.
        /// jokaisella kutsulla muutetaan arvoja, joilla manipuloidaan mm. ohjuksen nopeutta ja kutsun tiheyttä
        /// </summary>
        void AmmuOhjus()
        {
            PhysicsObject ohjus;
            ohjus = new PhysicsObject(105, 25);
            int nopeus = -400;
            int y = RandomGen.NextInt(-500, 500);
            ohjus.Y = y;
            ohjus.X = Level.Right;

            int kuva = RandomGen.NextInt(0, 7);
            string[] t = { "ohjus1", "ohjus2", "ohjus3", "ohjus4", "ohjus5", "ohjus6", "ohjus7" };
            Image ohjuskuva = LoadImage(t[kuva]);
            ohjus.Shape = Shape.FromImage(ohjuskuva);
            ohjus.Image = ohjuskuva;

            ohjus.Tag = "ohjus";
            ohjus.LifetimeLeft = new (50000000); // ei tarvi käsitellä törmäyksiä, kun ohjukset eivät ole hengissä liian kauan.
            pisteLaskuri.Value += 1;
            if (ohjusnopeus > 0.4) ohjusnopeus = (ohjusnopeus * 0.97);
            else nopeus = pisteLaskuri.Value * -7;
            ohjus.Velocity = new Vector(nopeus, 0);
            Add(ohjus);
            Timer.SingleShot(ohjusnopeus, AmmuOhjus);

        }


        /// <summary>
        /// ampuu kolikoita
        /// </summary>
        void Ammukolikko()
        {

            int tod = RandomGen.NextInt(0, 10);
            if (tod < 5) return;
            PhysicsObject kolikko;
            kolikko = new PhysicsObject(50, 50);
            kolikko.Shape = Shape.Circle;
            int y = RandomGen.NextInt(-500, 500);
            kolikko.Y = y;
            kolikko.Tag = "kolikko";
            kolikko.X = Level.Right;
            kolikko.Image = LoadImage("kolikko");
            kolikko.Velocity = new Vector(-400, 0);
            Add(kolikko);
        }


        /// <summary>
        /// Luolaskureita näyttöön, erilaskurit numeroitu.
        /// lisäksii kutsuu visuaalista apua
        /// </summary>
        /// <param name="a">mikä laskuri luodaan</param>
        void Luolaskuri(int a)
        {
            
            pisteLaskuri = new IntMeter(0); 
            rahalaskuri = new IntMeter(raha); // vakiona raha, johon arvo tallennetaan, sillä ClearAll komento tyhjentää intmeterit.
            for (int i = 0+a; i < 2; i++)
            {

                Label pisteNaytto = new Label();
                if (i == 0) pisteNaytto.X = Screen.Right - 100;
                else pisteNaytto.X = Screen.Left + 100;
                pisteNaytto.Y = Screen.Top - 100;
                pisteNaytto.TextColor = Color.Black;
                pisteNaytto.Color = Color.White;
                if (i == 1) pisteNaytto.BindTo(rahalaskuri);
                else pisteNaytto.BindTo(pisteLaskuri);
                pisteNaytto.Font = Font.DefaultBold;
                pisteNaytto.Font.Size = 70;
                pisteNaytto.TextColor = Color.White;
                pisteNaytto.Color = new Color(0, 0, 0, 0);
                Add(pisteNaytto);
            }
            Laskurikolikko();

        }


        /// <summary>
        /// Luo laskurille visuaalista ilmettä.
        /// </summary>
        void Laskurikolikko()
        {
            GameObject kolikko = new(60, 60);
            kolikko.Y = Screen.Top - 105;
            kolikko.X = Screen.Left + 160;
            kolikko.Image = LoadImage("kolikko");
            Add(kolikko, 3);
        }


        /// <summary>
        /// asettaa ohjaimet.
        /// </summary>
        void Asetaohjaimet()
        {
            Vector nopeus = new Vector(0, 500);
            

            Keyboard.Listen(Key.W, ButtonState.Down, AsetaNopeus, "Kaarto ylös", pelaaja, nopeus);
            Keyboard.Listen(Key.W, ButtonState.Released, AsetaNopeus, null, pelaaja, Vector.Zero);
            Keyboard.Listen(Key.S, ButtonState.Down, AsetaNopeus, "Kaarto alas", pelaaja, -nopeus);
            Keyboard.Listen(Key.S, ButtonState.Released, AsetaNopeus, null, pelaaja, Vector.Zero);

            Keyboard.Listen(Key.Up, ButtonState.Down, AsetaNopeus, "Kaarto ylös", pelaaja, nopeus);
            Keyboard.Listen(Key.Up, ButtonState.Released, AsetaNopeus, null, pelaaja, Vector.Zero);
            Keyboard.Listen(Key.Down, ButtonState.Down, AsetaNopeus, "Kaarto alas", pelaaja, -nopeus);
            Keyboard.Listen(Key.Down, ButtonState.Released, AsetaNopeus, null, pelaaja, Vector.Zero);

            Keyboard.Listen(Key.M, ButtonState.Pressed, Pausevalikko, "pause");
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, Pausevalikko, "pause");
        }


        /// <summary>
        /// Asettaa pelaajan nopeuden ja pysäyttää sen, jotta ei mennä kentästä ulos
        /// </summary>
        /// <param name="kone"></param>
        /// <param name="nopeus"></param>
        void AsetaNopeus(PhysicsObject kone, Vector nopeus)
        {
            if ((nopeus.Y < 0) && (kone.Y < Level.Bottom+50))
            {
                kone.Velocity = Vector.Zero;
                return;
            }
            if ((nopeus.Y > 0) && (kone.Y > Level.Top-50))
            {
                kone.Velocity = Vector.Zero;
                return;
            }

            kone.Velocity = nopeus;
            if (nopeus.Y > 0) Kaannapelaajaa(kone, -2);
            if (nopeus.Y < 0) Kaannapelaajaa(kone, 2);
        }


        /// <summary>
        /// kääntää pelaajaa ohjattaessa
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        void Kaannapelaajaa(PhysicsObject a, double b)
        {
            a.Angle += Angle.FromDegrees(-1*b);
        }


        /// <summary>
        /// Trimmi KUnnoSSA! (Y) :)
        /// </summary>
        void Paivita()
        {
            if (pelaaja.Angle > Angle.FromDegrees(0)) pelaaja.Angle += Angle.FromDegrees(-0.7);
            if (pelaaja.Angle < Angle.FromDegrees(0)) pelaaja.Angle += Angle.FromDegrees(0.7);
        }


    }
        
}