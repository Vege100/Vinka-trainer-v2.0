using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace paaohjelma
{
    public class paaohjelma : PhysicsGame
    {
        PhysicsObject Pelaaja;
        IntMeter pisteLaskuri;
        int raha = 3;
        int elama = 3;
        public override void Begin()
        {
            Valikko();
        }
        void Valikko()
        {
            ClearAll();
            MultiSelectWindow alkuValikko = new MultiSelectWindow("kakka", "Start", "Points", "Shop", "Quit");
            Add(alkuValikko);


            alkuValikko.AddItemHandler(0, Start);
            alkuValikko.AddItemHandler(1, Points);
            alkuValikko.AddItemHandler(2, Shop);
            alkuValikko.AddItemHandler(3, Exit);
        }
        void Pausevalikko()
        {
            MultiSelectWindow pausevalikko = new MultiSelectWindow("", "Continue", "Giveup");
            Pause();
            Add(pausevalikko);
            pausevalikko.AddItemHandler(0, Continue);
            pausevalikko.AddItemHandler(1, Giveup);

        }
        void Continue()
        {
            Pause();
        }
        void Giveup()
        {
            raha += pisteLaskuri.Value;
            Pause();
            Valikko();
        }
        void Start()
        {
            Luokentta();
            Asetaohjaimet();
            LuoPistelaskuri();
            Timer.CreateAndStart(2, AmmuOhjus);

        }
        void Points()
        {
            MultiSelectWindow points = new MultiSelectWindow("", "Back");
            Add(points);
            points.AddItemHandler(0, Valikko);
        }
        void Shop()
        {
            MultiSelectWindow shop = new MultiSelectWindow("", "Back", "Guns");
            Add(shop);
            shop.AddItemHandler(0, Valikko);
            shop.AddItemHandler(1, Guns);
            Label rahana = new Label(50,20,raha.ToString());
            Add(rahana);
        }
        void Guns()
        {
            ClearAll();
        }
        void Luokentta()
        {
            Pelaaja = LuoPelaaja();
            Image tausta = LoadImage("ohjukset");
            Level.Background.CreateStars(1000);


        }
        PhysicsObject LuoPelaaja()
        {
            Image vinka = LoadImage("vinkaframe3");
            PhysicsObject Pelaaja = PhysicsObject.CreateStaticObject(501, 201);
            Pelaaja.Shape = Shape.FromImage(vinka);
            Pelaaja.Y = 0;
            Pelaaja.X = Level.Left + 150;
            Pelaaja.Color = Color.Red;
            Add(Pelaaja);
            return Pelaaja;

        }

        void AmmuOhjus()
        {
            PhysicsObject ohjus;
            double ohjusnopeus = -100;
            ohjus = new PhysicsObject(210,50);
            int y = RandomGen.NextInt(-500, 500);
            ohjus.Y = y;
            ohjus.X = Level.Right;
            int kuva = RandomGen.NextInt(0, 7);
            string[] t = {"ohjus1", "ohjus2", "ohjus3", "ohjus4", "ohjus5", "ohjus6", "ohjus7" };
            Image ohjuskuva = LoadImage(t[kuva]);
            ohjus.Shape = Shape.FromImage(ohjuskuva);
            ohjus.Image = ohjuskuva;
            pisteLaskuri.Value += 1;
            ohjus.Velocity = new Vector(ohjusnopeus, 0);
            ohjusnopeus = ohjusnopeus*1.1;
            Add(ohjus);

        }
        void LuoPistelaskuri()
        {
            pisteLaskuri = new IntMeter(0);

            Label pisteNaytto = new Label();
            pisteNaytto.X = Screen.Left + 100;
            pisteNaytto.Y = Screen.Top - 100;
            pisteNaytto.TextColor = Color.Black;
            pisteNaytto.Color = Color.White;

            pisteNaytto.BindTo(pisteLaskuri);
            Add(pisteNaytto);
        }
        void Asetaohjaimet()
        {
            Vector nopeusYlos = new Vector(0, 200);
            Vector nopeusAlas = new Vector(0, -200);
            
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
            Keyboard.Listen(Key.W, ButtonState.Down, AsetaNopeus, "Kaarto ylös", Pelaaja, nopeusYlos);
            Keyboard.Listen(Key.W, ButtonState.Released, AsetaNopeus, null, Pelaaja, Vector.Zero);
            Keyboard.Listen(Key.S, ButtonState.Down, AsetaNopeus, "Kaarto alas", Pelaaja, nopeusAlas);
            Keyboard.Listen(Key.S, ButtonState.Released, AsetaNopeus, null, Pelaaja, Vector.Zero);

            Keyboard.Listen(Key.Up, ButtonState.Down, AsetaNopeus, "Kaarto ylös", Pelaaja, nopeusYlos);
            Keyboard.Listen(Key.Up, ButtonState.Released, AsetaNopeus, null, Pelaaja, Vector.Zero);
            Keyboard.Listen(Key.Down, ButtonState.Down, AsetaNopeus, "Kaarto alas", Pelaaja, nopeusAlas);
            Keyboard.Listen(Key.Down, ButtonState.Released, AsetaNopeus, null, Pelaaja, Vector.Zero);

            Keyboard.Listen(Key.M, ButtonState.Pressed, Pausevalikko, "kakka");
        }

        void AsetaNopeus(PhysicsObject maila, Vector nopeus)
        {
            if ((nopeus.Y < 0) && (maila.Bottom < Level.Bottom))
            {
                maila.Velocity = Vector.Zero;
                return;
            }
            if ((nopeus.Y > 0) && (maila.Top > Level.Top))
            {
                maila.Velocity = Vector.Zero;
                return;
            }

            maila.Velocity = nopeus;
        }
    }
}