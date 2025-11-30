using System;
using System.Collections.Generic;
using System.Linq;

namespace Préda_naplo
{
    internal class Bejelentkezes
    {
        private List<Felhasznalo> felhasznalok;
        public Felhasznalo AktualisFelhasznalo { get; private set; }

        public Bejelentkezes(List<Felhasznalo> felhasznalok)
        {
            this.felhasznalok = felhasznalok;
        }

        public bool Belepes()
        {
            Console.Write("Felhasznalonev: ");
            string felh = Console.ReadLine();

            Felhasznalo talalat = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == felh);

            if (talalat == null)
            {
                Console.WriteLine("Nincs ilyen felhasznalo!");
                AktualisFelhasznalo = null;
                return false;
            }

            if (talalat.Letiltva)
            {
                Console.WriteLine("Ez a fiok le van tiltva!");
                AktualisFelhasznalo = null;
                return false;
            }

            Console.Write("Jelszo: ");
            string jelszo = ReadPassword();

            if (talalat.EllenorizJelszo(jelszo))
            {
                AktualisFelhasznalo = talalat;

                Console.WriteLine("Sikeres bejelentkezes!");
                Console.WriteLine("Nev: " + talalat.Nev);
                Console.WriteLine("Szerepkor: " + talalat.Szerepkor);
                Console.WriteLine("Iskola: " + talalat.IskolaNev);

                return true;
            }
            else
            {
                Console.WriteLine("Hibas jelszo!");
                AktualisFelhasznalo = null;
                return false;
            }
        }

        private string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }
}