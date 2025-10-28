using Préda_naplo;
using System;
using System.Collections.Generic;
using System.IO;

namespace Préda_naplo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fajl = "felhasznalok.txt";

           
            if (!File.Exists(fajl))
            {
                File.Create(fajl).Close();
            }

            List<Felhasznalo> felhasznalok = Felhasznalo.BetoltFajlbol(fajl);

            Bejelentkezes bejelentkezes = new Bejelentkezes(felhasznalok);
            Regisztracio regisztracio = new Regisztracio(felhasznalok, fajl);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ONLINE NAPLÓ RENDSZER ===");
                Console.WriteLine("[1] Bejelentkezés");
                Console.WriteLine("[2] Regisztráció");
                Console.WriteLine("[0] Kilépés");
                Console.Write("\nVálasztás: ");
                string valasztas = Console.ReadLine();

                if (valasztas == "1")
                {
                    bool siker = bejelentkezes.Belepes();
                    if (siker)
                    {
                        Console.WriteLine($"\n✅ Üdvözöllek, {bejelentkezes.AktualisFelhasznalo.Nev}!");
                        Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                        Console.ReadLine();
                    }
                }
                else if (valasztas == "2")
                {
                    regisztracio.UjFelhasznalo();
                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "0")
                {
                    Console.WriteLine("Kilépés...");
                    break;
                }
                else
                {
                    Console.WriteLine("Érvénytelen választás!");
                    Console.ReadLine();
                }
            }
        }
    }
}
