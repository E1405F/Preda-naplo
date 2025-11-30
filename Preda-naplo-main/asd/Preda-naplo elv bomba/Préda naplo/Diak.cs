using System;

namespace Préda_naplo
{
    internal class Diak : Felhasznalo
    {
        public Diak(string felhasznalonev, string jelszo, string nev, string iskolaNev, string osztaly)
            : base(felhasznalonev, jelszo, nev, iskolaNev, "Diák", osztaly) { }


        // Add hozzá a Felhasznalo osztály Diák számára speciális menüjéhez:
        public override void FoMenu(JegyManager jegyManager, KozlemenyManager kozlemenyManager, HianyzasManager hianyzasManager)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== FŐMENÜ - {Nev} ({Szerepkor}) ===");
                if (!string.IsNullOrEmpty(Osztaly))
                {
                    Console.WriteLine($"Osztály: {Osztaly}");
                }
                Console.WriteLine("[1] Jegyek megtekintése");
                Console.WriteLine("[2] Fellebbezés benyújtása"); // ÚJ
                Console.WriteLine("[0] Kijelentkezés");
                Console.Write("\nVálasztás: ");
                string valasztas = Console.ReadLine();

                if (valasztas == "1")
                {
                    jegyManager.JegyekListazasa(this);
                    Console.WriteLine("\nNyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "2") // ÚJ
                {
                    jegyManager.FellebbezesBenyujtasa(this);
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