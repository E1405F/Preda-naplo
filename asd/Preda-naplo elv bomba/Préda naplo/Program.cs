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
            string felhasznalokFajl = "felhasznalok.txt";
            string jegyekFajl = "jegyek.txt";

            if (!File.Exists(felhasznalokFajl))
            {
                File.Create(felhasznalokFajl).Close();
            }
            if (!File.Exists(jegyekFajl))
            {
                File.Create(jegyekFajl).Close();
            }

            List<Felhasznalo> felhasznalok = Felhasznalo.BetoltFajlbol(felhasznalokFajl);

            Bejelentkezes bejelentkezes = new Bejelentkezes(felhasznalok);
            Regisztracio regisztracio = new Regisztracio(felhasznalok, felhasznalokFajl);
            JegyManager jegyManager = new JegyManager(jegyekFajl, felhasznalok);  // Átadjuk a felhasználókat

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

                        bejelentkezes.AktualisFelhasznalo.FoMenu(jegyManager);
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