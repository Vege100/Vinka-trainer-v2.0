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
        public override void Begin()
        {
            LuoKentta();
            Asetaohjaimet();
            

            PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        }

        public void Luokentta()
        {
            Pelaaja = LuoPelaaja();
            Level.CreateBorders();
            Background tausta = new Background(1000, 1000);
            tausta.Image = LoadImage("kuva");
            LuoPisteet();


        }
        PhysicsObject LuoPelaaja()
        {
            PhysicsObject Pelaaja = PhysicsObject.CreateStaticObject(100, 40);
            Pelaaja.Shape = Shape.Rectangle;
            Pelaaja.Y = 0;
            Pelaaja.X = Level.Left + 20;
            Pelaaja.Image = LoadImage("vinka");
            Add(Pelaaja);
            return Pelaaja;

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