﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Préda_naplo
{
    internal class Regisztracio
    {
        private List<Felhasznalo> felhasznalok;
        private string fajlEleres;

        public Regisztracio(List<Felhasznalo> felhasznalok, string fajlEleres)
        {
            this.felhasznalok = felhasznalok;
            this.fajlEleres = fajlEleres;
        }

        public void UjFelhasznalo()
        {
            Console.WriteLine("\n=== ÚJ FELHASZNÁLÓ REGISZTRÁLÁSA ===");

           
            Console.Write("Felhasználónév: ");
            string felh = Console.ReadLine();

            if (felhasznalok.Any(f => f.Felhasznalonev == felh))
            {
                Console.WriteLine("❌ Ez a felhasználónév már létezik!");
                return;
            }

           
            Console.Write("Jelszó: ");
            string jelszo = ReadPassword();

          
            Console.Write("Teljes név: ");
            string nev = Console.ReadLine();

            
            Console.Write("Iskola neve: ");
            string iskola = Console.ReadLine();

           
            string szerep = "";
            bool helyes = false;
            do
            {
                Console.WriteLine("\nVálassz szerepkört:");
                Console.WriteLine("1 - Diák");
                Console.WriteLine("2 - Tanár");
                Console.WriteLine("3 - Szülő");
                Console.WriteLine("4 - Igazgató");
                Console.WriteLine("5 - Adminisztrátor");
                Console.Write("Szerepkör száma: ");
                string szerepValasz = Console.ReadLine();

                switch (szerepValasz)
                {
                    case "1":
                        szerep = "Diák";
                        helyes = true;
                        break;
                    case "2":
                        szerep = "Tanár";
                        helyes = true;
                        break;
                    case "3":
                        szerep = "Szülő";
                        helyes = true;
                        break;
                    case "4":
                        szerep = "Igazgató";
                        helyes = true;
                        break;
                    case "5":
                        szerep = "Adminisztrátor";
                        helyes = true;
                        break;
                    default:
                        Console.WriteLine("❌ Érvénytelen választás, próbáld újra!");
                        break;
                }

            } while (!helyes);

            
            Felhasznalo uj = new Felhasznalo(felh, jelszo, nev, iskola, szerep);
            felhasznalok.Add(uj);

            
            File.AppendAllText(fajlEleres, uj.ToFileFormat() + Environment.NewLine);

            Console.WriteLine($"\n✅ Sikeres regisztráció, üdvözlünk {nev} ({szerep}) a {iskola} iskolából!");
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
