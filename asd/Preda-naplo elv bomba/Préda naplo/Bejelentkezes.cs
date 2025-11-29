using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Console.Write("Felhasználónév: ");
            string felh = Console.ReadLine();

            Felhasznalo talalat = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == felh);

            if (talalat == null)
            {
                Console.WriteLine("❌ Nincs ilyen felhasználó!");
                return false;
            }

            if (talalat.Letiltva)
            {
                Console.WriteLine("🚫 Ez a fiók le van tiltva!");
                return false;
            }

            Console.Write("Jelszó: ");
            string jelszo = ReadPassword();

            if (talalat.EllenorizJelszo(jelszo))
            {
                AktualisFelhasznalo = talalat;

                Console.WriteLine($"\n✅ Sikeres bejelentkezés!");
                Console.WriteLine($"Név: {talalat.Nev}");
                Console.WriteLine($"Szerepkör: {talalat.Szerepkor}");
                Console.WriteLine($"Iskola: {talalat.IskolaNev}");

                return true;
            }
            else
            {
                Console.WriteLine("❌ Hibás jelszó!");
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
