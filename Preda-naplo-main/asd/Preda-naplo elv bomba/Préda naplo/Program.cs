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
            string kozlemenyekFajl = "kozlemenyek.txt";
            string hianyzasokFajl = "hianyzasok.txt";

            // Fájlok létrehozása ha nem léteznek
            if (!File.Exists(felhasznalokFajl)) File.Create(felhasznalokFajl).Close();
            if (!File.Exists(jegyekFajl)) File.Create(jegyekFajl).Close();
            if (!File.Exists(kozlemenyekFajl)) File.Create(kozlemenyekFajl).Close();
            if (!File.Exists(hianyzasokFajl)) File.Create(hianyzasokFajl).Close();

            List<Felhasznalo> felhasznalok = Felhasznalo.BetoltFajlbol(felhasznalokFajl);

            Bejelentkezes bejelentkezes = new Bejelentkezes(felhasznalok);
            Regisztracio regisztracio = new Regisztracio(felhasznalok, felhasznalokFajl);
            JegyManager jegyManager = new JegyManager(jegyekFajl, felhasznalok);
            KozlemenyManager kozlemenyManager = new KozlemenyManager(kozlemenyekFajl);
            HianyzasManager hianyzasManager = new HianyzasManager(hianyzasokFajl, felhasznalok);

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
                    if (siker && bejelentkezes.AktualisFelhasznalo != null) // ✅ DUPLA ELLENŐRZÉS
                    {
                        Console.WriteLine($"\n✅ Üdvözöllek, {bejelentkezes.AktualisFelhasznalo.Nev}!");
                        Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                        Console.ReadLine();

                        bejelentkezes.AktualisFelhasznalo.FoMenu(jegyManager, kozlemenyManager, hianyzasManager);
                    }
                    else
                    {
                        Console.WriteLine("❌ Bejelentkezés sikertelen!");
                        Console.ReadLine();
                    }
                }
                else if (valasztas == "2")
                {
                    regisztracio.UjFelhasznalo();

                    // ✅ ÚJ: FRISSÍTJÜK A FELHASZNÁLÓK LISTÁJÁT
                    felhasznalok = Felhasznalo.BetoltFajlbol(felhasznalokFajl);
                    bejelentkezes = new Bejelentkezes(felhasznalok); // ✅ Új bejelentkezés objektum

                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
            }
        }
    }
}