using System;
using System.Linq;

namespace Préda_naplo
{
    internal class Igazgato : Felhasznalo
    {
        public Igazgato(string felhasznalonev, string jelszo, string nev, string iskolaNev)
            : base(felhasznalonev, jelszo, nev, iskolaNev, "Igazgató", "") { }

        public override void FoMenu(JegyManager jegyManager, KozlemenyManager kozlemenyManager, HianyzasManager hianyzasManager)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"IGAZGATÓI MENÜ - {Nev}");
                Console.WriteLine("[1] Összes jegy megtekintése");
                Console.WriteLine("[2] Statisztikák");
                Console.WriteLine("[3] Közlemény küldése");
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
                    StatisztikakMegjelenitese(jegyManager, hianyzasManager);
                    Console.WriteLine("\nNyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "3")
                {
                    kozlemenyManager.UjKozlemeny(this);
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

        private void StatisztikakMegjelenitese(JegyManager jegyManager, HianyzasManager hianyzasManager)
        {
            Console.WriteLine("\n=== STATISZTIKÁK ===");

            // Osztályátlagok számítása és megjelenítése
            Console.WriteLine("\n📊 OSZTÁLYÁTLAGOK:");
            var osztalyAtlagok = jegyManager.GetOsztalyAtlagok();

            if (osztalyAtlagok.Any())
            {
                foreach (var atlag in osztalyAtlagok)
                {
                    Console.WriteLine($"  {atlag.Key} osztály: {atlag.Value:F2}");
                }
            }
            else
            {
                Console.WriteLine("  Nincs elérhető adat osztályátlagokhoz.");
            }

            // Hiányzási statisztikák
            Console.WriteLine("\n📈 HIÁNYZÁSI STATISZTIKÁK:");
            var hianyzasStatisztikak = hianyzasManager.GetHianyzasStatisztikak();

            if (hianyzasStatisztikak.Any())
            {
                foreach (var stat in hianyzasStatisztikak)
                {
                    Console.WriteLine($"  {stat.Key}: {stat.Value} hiányzás");
                }
            }
            else
            {
                Console.WriteLine("  Nincs elérhető adat hiányzásokhoz.");
            }

            // Diákok listája átlaggal és hiányzással
            Console.WriteLine("\n👨‍🎓 DIÁKOK ÁTLAGAI ÉS HIÁNYZÁSAI:");
            var diakStatisztikak = jegyManager.GetDiakStatisztikak(hianyzasManager);

            if (diakStatisztikak.Any())
            {
                foreach (var diak in diakStatisztikak)
                {
                    Console.WriteLine($"  {diak.Key} - Átlag: {diak.Value.Atlag:F2}, Hiányzások: {diak.Value.Hianyzasok}");
                }
            }
            else
            {
                Console.WriteLine("  Nincs elérhető adat diákokhoz.");
            }
        }
    }
}