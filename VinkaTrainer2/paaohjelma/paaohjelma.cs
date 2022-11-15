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
        
        
        public override void Begin()
        {
            IsFullScreen = true;
            Valikko();
            Luotaustavinka();
            Soitavapaata();
        }
        

        void Valikko()
        {

            MultiSelectWindow alkuValikko = new MultiSelectWindow("VinkaTrainer2", "Start", "Points", "Shop", "Quit");
            Luolaskuri(1);
            Add(alkuValikko);
            alkuValikko.AddItemHandler(0, Start);
            alkuValikko.AddItemHandler(1, Points);
            alkuValikko.AddItemHandler(2, Shop);
            alkuValikko.AddItemHandler(3, Exit);
        }


        void Pausevalikko()
        {
            MultiSelectWindow pausevalikko = new MultiSelectWindow("", "Continue", "Giveup");
            Pausettaa();
            Add(pausevalikko);
            pausevalikko.AddItemHandler(0, Pausettaa);
            pausevalikko.AddItemHandler(1, Gameover);

        }


        void Pausettaa()
        {
            if (MediaPlayer.IsMuted) Soitamoottoria();
            else MediaPlayer.Volume = 0;
            Pause();
        }


        void Soitamoottoria()
        {
            MediaPlayer.Play("motor");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.2;
        }


        void Start()
        {   
            Luokentta();
            Asetaohjaimet();
            Luolaskuri(0);
            Timer.SingleShot(1,AmmuOhjus);
            Timer.CreateAndStart(2, Ammukolikko);
        }


        void Points()
        {
            toplista.Show();
            toplista.HighScoreWindow.Closed += Valikkoohjaus;
        }


        void Shop()
        {
            MultiSelectWindow shop = new MultiSelectWindow("", "Back", "1 Life 4 10 coins");
            Add(shop);
            shop.AddItemHandler(0, Valikko);
            shop.AddItemHandler(1, Elamaa);
        }


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


        void Soitavapaata()
        {
            MediaPlayer.Play("copyrightfree");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5;
        }


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


        void Luotaustavinka()
        {
            GameObject vinkatausta = new(Screen.Width, Screen.Height);
            vinkatausta.Image = LoadImage("vinkataustoitta");
            Add(vinkatausta);
            Level.Background.CreateGradient(Color.White, Color.Blue);
            LuoPallo(3000);
        }


        void Kaannapalloa(GameObject a,double b)
        {
            a.Angle += Angle.FromDegrees(-0.1*b);
        }


        public static double Etsiaika()
        {
            string kulma = Lataanetista("https://www.timeanddate.com/sun/finland/jyvaskyla");
            int a = kulma.IndexOf("sunalt");
            kulma = kulma.Substring(a + 7, 5);
            string[] numero = kulma.Split(',');
            double akulma = Convert.ToDouble(numero[0] + numero[1][0]);
            return akulma;
        }


        public static string Lataanetista(string osoite)
        {
            WebClient client = new WebClient();
            string lataus = client.DownloadString(osoite);
            return lataus;
        }


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


        void Piirrataivas(double ak)
        {
            Level.Background.CreateStars(1000);
            GameObject tummennus = new(Screen.Width, Screen.Height);
            tummennus.Color = new Color(0, 0, 0, 135);
            Add(tummennus, 2);
        }


        
        void Luokentta()
        {
            ClearAll();
            Luosaa();
            double auringonkulma = Etsiaika();
            if (auringonkulma < 0) 
            Piirrataivas(auringonkulma);
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


        void Pisteita(PhysicsObject a, PhysicsObject b)
        {
            rahalaskuri.AddValue(1);
            b.Destroy();
        }


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

        void Gameover()
        {
            int loppupisteet = pisteLaskuri.Value;
            raha = rahalaskuri;
            ClearAll();
            Luotaustavinka();
            Soitavapaata();
            toplista.EnterAndShow(loppupisteet);
            toplista.HighScoreWindow.Closed += Valikkoohjaus;
        }


        public void Valikkoohjaus(Window sender)
        {
            Valikko();
        }


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
            return pelaaja;

        }


        void AmmuOhjus()
        {
            PhysicsObject ohjus;
            ohjus = new PhysicsObject(105, 25);
            int y = RandomGen.NextInt(-500, 500);
            ohjus.Y = y;
            int nopeus = -400;
            ohjus.X = Level.Right;
            int kuva = RandomGen.NextInt(0, 7);
            string[] t = { "ohjus1", "ohjus2", "ohjus3", "ohjus4", "ohjus5", "ohjus6", "ohjus7" };
            Image ohjuskuva = LoadImage(t[kuva]);
            ohjus.Shape = Shape.FromImage(ohjuskuva);
            ohjus.Image = ohjuskuva;
            ohjus.Tag = "ohjus";
            ohjus.LifetimeLeft = new (50000000);
            pisteLaskuri.Value += 1;
            if (ohjusnopeus > 0.4) ohjusnopeus = (ohjusnopeus * 0.97);
            else nopeus = pisteLaskuri.Value * -7;
            ohjus.Velocity = new Vector(nopeus, 0);
            Add(ohjus);
            Timer.SingleShot(ohjusnopeus, AmmuOhjus);

        }


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


        void Luolaskuri(int a)
        {
            pisteLaskuri = new IntMeter(0);
            rahalaskuri = new IntMeter(raha);
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
            Add(pisteNaytto);
            }
        }


        void Asetaohjaimet()
        {
            Vector nopeusYlos = new Vector(0, 500);
            Vector nopeusAlas = new Vector(0, -500);

            Keyboard.Listen(Key.W, ButtonState.Down, AsetaNopeus, "Kaarto ylös", pelaaja, nopeusYlos);
            Keyboard.Listen(Key.W, ButtonState.Released, AsetaNopeus, null, pelaaja, Vector.Zero);
            Keyboard.Listen(Key.S, ButtonState.Down, AsetaNopeus, "Kaarto alas", pelaaja, nopeusAlas);
            Keyboard.Listen(Key.S, ButtonState.Released, AsetaNopeus, null, pelaaja, Vector.Zero);

            Keyboard.Listen(Key.Up, ButtonState.Down, AsetaNopeus, "Kaarto ylös", pelaaja, nopeusYlos);
            Keyboard.Listen(Key.Up, ButtonState.Released, AsetaNopeus, null, pelaaja, Vector.Zero);
            Keyboard.Listen(Key.Down, ButtonState.Down, AsetaNopeus, "Kaarto alas", pelaaja, nopeusAlas);
            Keyboard.Listen(Key.Down, ButtonState.Released, AsetaNopeus, null, pelaaja, Vector.Zero);

            Keyboard.Listen(Key.M, ButtonState.Pressed, Pausevalikko, "pause");
        }


        void AsetaNopeus(PhysicsObject kone, Vector nopeus)
        {
            if ((nopeus.Y < 0) && (kone.Bottom < Level.Bottom))
            {
                kone.Velocity = Vector.Zero;
                return;
            }
            if ((nopeus.Y > 0) && (kone.Top > Level.Top))
            {
                kone.Velocity = Vector.Zero;
                return;
            }

            kone.Velocity = nopeus;
            if (nopeus.Y > 0) kone.Angle = Angle.FromDegrees(15);
            if (nopeus.Y < 0) kone.Angle = Angle.FromDegrees(-15);
            if (nopeus.Y == 0) kone.Angle = Angle.FromDegrees(0);
        }


    }
        
}