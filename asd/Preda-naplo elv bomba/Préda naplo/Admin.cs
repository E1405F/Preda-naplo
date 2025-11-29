using System;

namespace Préda_naplo
{
    internal class Admin : Felhasznalo
    {
        public Admin(string felhasznalonev, string jelszo, string nev, string iskolaNev)
            : base(felhasznalonev, jelszo, nev, iskolaNev, "Adminisztrátor", "") { }

        public override void FoMenu(JegyManager jegyManager)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== ADMIN MENÜ - {Nev} ===");
                Console.WriteLine("[1] Összes jegy megtekintése");
                Console.WriteLine("[2] Rendszerbeállítások");
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
                    Console.WriteLine("\n⚙️ Rendszerbeállítások:");
                    Console.WriteLine("Ez a funkció hamarosan elérhető...");
                    Console.WriteLine("\nNyomj ENTER-t a folytatáshoz...");
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