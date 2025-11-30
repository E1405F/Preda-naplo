using System;

namespace Préda_naplo
{
    internal class Szulo : Felhasznalo
    {
        public Szulo(string felhasznalonev, string jelszo, string nev, string iskolaNev)
            : base(felhasznalonev, jelszo, nev, iskolaNev, "Szülő", "") { }

        public override void FoMenu(JegyManager jegyManager, KozlemenyManager kozlemenyManager, HianyzasManager hianyzasManager)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== SZÜLŐI MENÜ - {Nev} ===");
                Console.WriteLine("[1] Gyermekem jegyei");
                Console.WriteLine("[2] Hiányzás igazolása");
                Console.WriteLine("[3] Közlemények");
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
                    hianyzasManager.SzuloHianyzasIgazolasa(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "3")
                {
                    kozlemenyManager.KozlemenyekListazasa(this);
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