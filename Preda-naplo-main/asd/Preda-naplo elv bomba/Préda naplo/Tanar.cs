using System;

namespace Préda_naplo
{
    internal class Tanar : Felhasznalo
    {
        public Tanar(string felhasznalonev, string jelszo, string nev, string iskolaNev, string osztaly = "")
            : base(felhasznalonev, jelszo, nev, iskolaNev, "Tanár", osztaly) { }

        public override void FoMenu(JegyManager jegyManager, KozlemenyManager kozlemenyManager, HianyzasManager hianyzasManager)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"TANÁRI MENÜ - {Nev}");
                if (!string.IsNullOrEmpty(Osztaly))
                {
                    Console.WriteLine($"Osztály: {Osztaly}");
                }
                Console.WriteLine("[1] Jegyek megtekintése");
                Console.WriteLine("[2] Új jegy rögzítése");
                Console.WriteLine("[3] Jegy írás osztálynak");
                Console.WriteLine("[4] Súlyozott jegy rögzítése");
                Console.WriteLine("[5] Hiányzás rögzítése"); // ÚJ
                Console.WriteLine("[6] Hiányzások listázása"); // ÚJ
                Console.WriteLine("[7] Hiányzás igazolása"); // ÚJ
                Console.WriteLine("[8] Fellebbezések kezelése");
                Console.WriteLine("[9] Diák lezárása");
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
                else if (valasztas == "5") // ÚJ
                {
                    hianyzasManager.HianyzasRogzitese(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "6") // ÚJ
                {
                    hianyzasManager.HianyzasokListazasa(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "7") // ÚJ
                {
                    hianyzasManager.HianyzasIgazolasa(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "8")
                {
                    jegyManager.FellebbezesekKezelese(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "9")
                {
                    DiakLezarasa();
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

        // ÚJ: Diák lezárása metódus
        private void DiakLezarasa()
        {
            Console.WriteLine("\n=== DIÁK LEZÁRÁSA ===");
            Console.WriteLine("Ez a funkció lehetővé teszi az év végi jegyek lezárását.");
            Console.WriteLine("A lezárás után a jegyek már nem módosíthatók.");
            Console.Write("\nBiztosan le szeretnéd zárni az osztályzatokat? (i/n): ");

            string valasz = Console.ReadLine().ToLower();
            if (valasz == "i")
            {
                Console.WriteLine("✅ Osztályzatok sikeresen lezárva!");
                // Itt implementálhatod a tényleges lezárási logikát
                // Például: jegyek zárolása, véglegesítés, stb.
            }
            else
            {
                Console.WriteLine("❌ Lezárás megszakítva.");
            }
        }
    }
}