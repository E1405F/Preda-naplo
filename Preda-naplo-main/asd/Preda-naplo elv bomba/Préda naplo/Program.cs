using Préda_naplo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Préda_naplo
{
    internal class Program
    {
        private const string TITKOS_KOD = "Bturbo";

        static void Main(string[] args)
        {
            string felhasznalokFajl = "felhasznalok.txt";
            string jegyekFajl = "jegyek.txt";
            string kozlemenyekFajl = "kozlemenyek.txt";
            string hianyzasokFajl = "hianyzasok.txt";
            string szuloGyerekKapcsolatFajl = "szulogyerek.txt";

            // Fájlok létrehozása ha nem léteznek
            if (!File.Exists(felhasznalokFajl)) File.Create(felhasznalokFajl).Close();
            if (!File.Exists(jegyekFajl)) File.Create(jegyekFajl).Close();
            if (!File.Exists(kozlemenyekFajl)) File.Create(kozlemenyekFajl).Close();
            if (!File.Exists(hianyzasokFajl)) File.Create(hianyzasokFajl).Close();
            if (!File.Exists(szuloGyerekKapcsolatFajl)) File.Create(szuloGyerekKapcsolatFajl).Close();

            List<Felhasznalo> felhasznalok = Felhasznalo.BetoltFajlbol(felhasznalokFajl);
            Dictionary<string, string> szuloGyerekKapcsolat = BetoltSzuloGyerekKapcsolatot(szuloGyerekKapcsolatFajl);

            Bejelentkezes bejelentkezes = new Bejelentkezes(felhasznalok);
            Regisztracio regisztracio = new Regisztracio(felhasznalok, felhasznalokFajl, szuloGyerekKapcsolat, szuloGyerekKapcsolatFajl, TITKOS_KOD);
            JegyManager jegyManager = new JegyManager(jegyekFajl, felhasznalok);
            KozlemenyManager kozlemenyManager = new KozlemenyManager(kozlemenyekFajl, felhasznalok);
            HianyzasManager hianyzasManager = new HianyzasManager(hianyzasokFajl, felhasznalok, szuloGyerekKapcsolat);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ONLINE NAPLÓ RENDSZER ===");
                Console.WriteLine("[1] Bejelentkezés");
                Console.WriteLine("[2] Regisztráció");
                Console.WriteLine("[3] Elfelejtett jelszó");
                Console.WriteLine("[0] Kilépés");
                Console.Write("\nVálasztás: ");
                string valasztas = Console.ReadLine();

                if (valasztas == "1")
                {
                    bool siker = bejelentkezes.Belepes();
                    if (siker && bejelentkezes.AktualisFelhasznalo != null)
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

                    // Frissítjük a felhasználók listáját
                    felhasznalok = Felhasznalo.BetoltFajlbol(felhasznalokFajl);
                    szuloGyerekKapcsolat = BetoltSzuloGyerekKapcsolatot(szuloGyerekKapcsolatFajl);
                    bejelentkezes = new Bejelentkezes(felhasznalok);
                    hianyzasManager = new HianyzasManager(hianyzasokFajl, felhasznalok, szuloGyerekKapcsolat);

                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "3")
                {
                    ElfelejtettJelszo(felhasznalok, felhasznalokFajl);
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

        private static Dictionary<string, string> BetoltSzuloGyerekKapcsolatot(string fajlEleres)
        {
            var kapcsolat = new Dictionary<string, string>();
            if (!File.Exists(fajlEleres)) return kapcsolat;

            foreach (var sor in File.ReadAllLines(fajlEleres))
            {
                if (string.IsNullOrWhiteSpace(sor)) continue;
                var adatok = sor.Split(';');
                if (adatok.Length >= 2)
                {
                    kapcsolat[adatok[0]] = adatok[1]; // szulo -> gyerek
                }
            }
            return kapcsolat;
        }

        private static void ElfelejtettJelszo(List<Felhasznalo> felhasznalok, string fajlEleres)
        {
            Console.Clear();
            Console.WriteLine("\n=== ELFELEJTETT JELSZÓ ===");
            Console.Write("Felhasználónév: ");
            string felhasznalonev = Console.ReadLine();

            var felhasznalo = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == felhasznalonev);

            if (felhasznalo == null)
            {
                Console.WriteLine("❌ Nem található ilyen felhasznaló!");
                Console.WriteLine("Nyomj ENTER-t a visszalépéshez...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"\nFelhasználó: {felhasznalo.Nev}");
            Console.WriteLine($"Szerepkör: {felhasznalo.Szerepkor}");
            Console.WriteLine($"Iskola: {felhasznalo.IskolaNev}");

            Console.Write("\nÚj jelszó: ");
            string ujJelszo = ReadPassword();

            if (string.IsNullOrWhiteSpace(ujJelszo))
            {
                Console.WriteLine("❌ A jelszó nem lehet üres!");
                Console.WriteLine("Nyomj ENTER-t a visszalépéshez...");
                Console.ReadLine();
                return;
            }

            if (felhasznalo.JelszoMegvaltoztatasa(ujJelszo))
            {
                FrissitFelhasznalokFajlt(felhasznalok, fajlEleres);
                Console.WriteLine("✅ Jelszó sikeresen megváltoztatva!");
            }
            else
            {
                Console.WriteLine("❌ A jelszó megváltoztatása sikertelen!");
            }

            Console.WriteLine("Nyomj ENTER-t a visszalépéshez...");
            Console.ReadLine();
        }

        private static string ReadPassword()
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

        private static void FrissitFelhasznalokFajlt(List<Felhasznalo> felhasznalok, string fajlEleres)
        {
            try
            {
                var sorok = felhasznalok.Select(f => f.ToFileFormat());
                File.WriteAllLines(fajlEleres, sorok);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Hiba a fájl frissítésekor: {ex.Message}");
            }
        }
    }
}