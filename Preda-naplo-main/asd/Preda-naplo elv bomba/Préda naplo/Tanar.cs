using System;

namespace Préda_naplo
{
    internal class Tanar : Felhasznalo
    {
        public Tanar(string felhasznalonev, string jelszo, string nev, string iskolaNev, string osztaly = "")
            : base(felhasznalonev, jelszo, nev, iskolaNev, "Tanár", osztaly) { }

        // Add hozzá a Tanar osztály FoMenu metódusához:
        public override void FoMenu(JegyManager jegyManager, KozlemenyManager kozlemenyManager, HianyzasManager hianyzasManager)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== TANÁRI MENÜ - {Nev} ===");
                if (!string.IsNullOrEmpty(Osztaly))
                {
                    Console.WriteLine($"Osztály: {Osztaly}");
                }
                Console.WriteLine("[1] Jegyek megtekintése");
                Console.WriteLine("[2] Új jegy rögzítése");
                Console.WriteLine("[3] Jegy írás osztálynak");
                Console.WriteLine("[4] Súlyozott jegy rögzítése");
                Console.WriteLine("[5] Fellebbezések kezelése");
                Console.WriteLine("[0] Kijelentkezés");
                Console.Write("\nVálasztás: ");
                string valasztas = Console.ReadLine();

                if (valasztas == "1")
                {
                    jegyManager.JegyekListazasa(this);
                    Console.WriteLine("\nNyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "2")
                {
                    jegyManager.UjJegy(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "3")
                {
                    jegyManager.JegyIrasOsztalynak(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "4")
                {
                    jegyManager.UjJegySullyal(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "5")
                {
                    jegyManager.FellebbezesekKezelese(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "0")
                {
                    Console.WriteLine("Kijelentkezés...");
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