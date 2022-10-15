using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

namespace paaohjelma
{
    public class paaohjelma : PhysicsGame
    {
        PhysicsObject Pelaaja;
        double ohjusnopeus = -100;
        PhysicsObject ohjus;
        public override void Begin()
        {
            Valikko();

            Keyboard.Listen(Key.Escape, ButtonState.Pressed, Valikko,"kakka");
        }
        void Valikko()
        {
            MultiSelectWindow alkuValikko = new MultiSelectWindow("kakka", "Start", "Points", "Quit");
            Add(alkuValikko);


            alkuValikko.AddItemHandler(0, Start);
            alkuValikko.AddItemHandler(1, Points);
            alkuValikko.AddItemHandler(2, Exit);
        }
        void Start()
        {
            Luokentta();
            Asetaohjaimet();
            Timer.CreateAndStart(2, AmmuOhjus);
        }
        void Points()
        {
            
        }

        void Luokentta()
        {
            Pelaaja = LuoPelaaja();
            Image tausta = LoadImage("ohjukset");
            Level.Background.CreateStars(1000);


        }
        PhysicsObject LuoPelaaja()
        {
            PhysicsObject Pelaaja = PhysicsObject.CreateStaticObject(200,140);
            Pelaaja.Shape = Shape.Rectangle;
            Pelaaja.Y = 0;
            Pelaaja.X = Level.Left + 150;
            Pelaaja.Image = LoadImage("vinka1");
            Add(Pelaaja);
            return Pelaaja;

        }

        void AmmuOhjus()
        {
            ohjus = new PhysicsObject(210,50);
            ohjus.Shape = Shape.Rectangle;
            int y = RandomGen.NextInt(-500, 500);

            ohjus.Y = y;
            ohjus.X = Level.Right;
            int kuva = RandomGen.NextInt(0, 7);
            string[] t = {"ohjus1", "ohjus2", "ohjus3", "ohjus4", "ohjus5", "ohjus6", "ohjus7" };
            ohjus.Image = LoadImage(t[kuva]);

            ohjus.Velocity = new Vector(ohjusnopeus, 0);
            ohjusnopeus = ohjusnopeus*1.1;
            Add(ohjus);

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