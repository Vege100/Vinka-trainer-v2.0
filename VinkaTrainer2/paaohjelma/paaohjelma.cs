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
        public override void Begin()
        {
            Luokentta();
            Asetaohjaimet();

            Timer.CreateAndStart(2, AmmuOhjus);
            PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        }

        void Luokentta()
        {
            Pelaaja = LuoPelaaja();
            Level.CreateBorders();
            Background tausta = new Background(1000, 1000);


        }
        PhysicsObject LuoPelaaja()
        {
            PhysicsObject Pelaaja = PhysicsObject.CreateStaticObject(100, 40);
            Pelaaja.Shape = Shape.Rectangle;
            Pelaaja.Y = 0;
            Pelaaja.X = Level.Left + 20;
            Pelaaja.Image = LoadImage("vinka1");
            Add(Pelaaja);
            return Pelaaja;

        }
        void AmmuOhjus()
        {
            PhysicsObject ohjus = new PhysicsObject(100,20);
            ohjus.Shape = Shape.Rectangle;
            int y = RandomGen.NextInt(-500, 500);
            ohjus.Y = y;
            ohjus.X = Level.Right;
            string[] t = {"ohjus1", "ohjus2", "ohjus3", "ohjus4", "ohjus5", "ohjus6", "ohjus7" };
            int kuva = RandomGen.NextInt(0, 7);
            ohjus.Image = LoadImage(t[kuva]);
            ohjus.Velocity = new Vector(ohjusnopeus, 0);
            ohjusnopeus = ohjusnopeus*2;
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