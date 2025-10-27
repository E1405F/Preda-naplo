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
            Console.Clear();
            Console.WriteLine("=== Bejelentkezés ===");
            Console.WriteLine();

            Console.Write("Felhasználónév: ");
            string felh = Console.ReadLine();

            Felhasznalo talalat = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == felh);

            if (talalat == null)
            {
                Console.WriteLine("❌ Nincs ilyen felhasználó!");
                Console.WriteLine("Nyomj egy gombot a visszalépéshez...");
                Console.ReadKey();
                return false;
            }

            if (talalat.Letiltva)
            {
                Console.WriteLine("🚫 Ez a fiók le van tiltva!");
                Console.WriteLine("Nyomj egy gombot a visszalépéshez...");
                Console.ReadKey();
                return false;
            }

            Console.Write("Jelszó: ");
            string jelszo = ReadPassword();

            if (talalat.EllenorizJelszo(jelszo))
            {
                AktualisFelhasznalo = talalat;
                return true;
            }
            else
            {
                Console.WriteLine("❌ Hibás jelszó!");
                Console.WriteLine("Nyomj egy gombot a visszalépéshez...");
                Console.ReadKey();
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