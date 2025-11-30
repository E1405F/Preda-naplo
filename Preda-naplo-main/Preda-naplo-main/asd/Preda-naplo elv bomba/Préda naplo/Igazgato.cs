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
                Console.WriteLine("IGAZGATOI MENU - " + Nev);
                Console.WriteLine("[1] Osszes jegy megtekintese");
                Console.WriteLine("[2] Statisztikak");
                Console.WriteLine("[3] Kozlemeny kuldese");
                Console.WriteLine("[0] Kijelentkezes");
                Console.Write("Valasztas: ");
                string valasztas = Console.ReadLine();

                if (valasztas == "1")
                {
                    jegyManager.JegyekListazasa(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "2")
                {
                    StatisztikakMegjelenitese(jegyManager, hianyzasManager);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "3")
                {
                    kozlemenyManager.UjKozlemeny(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "0")
                {
                    Console.WriteLine("Kijelentkezes...");
                    break;
                }
                else
                {
                    Console.WriteLine("Ervenytelen valasztas!");
                    Console.ReadLine();
                }
            }
        }

        private void StatisztikakMegjelenitese(JegyManager jegyManager, HianyzasManager hianyzasManager)
        {
            Console.WriteLine("STATISZTIKAK");

            Console.WriteLine("OSZTALYATLAGOK:");
            var osztalyAtlagok = jegyManager.GetOsztalyAtlagok();

            if (osztalyAtlagok.Any())
            {
                foreach (var atlag in osztalyAtlagok)
                {
                    Console.WriteLine("  " + atlag.Key + " osztaly: " + atlag.Value.ToString("F2"));
                }
            }
            else
            {
                Console.WriteLine("  Nincs elerheto adat osztalyatlagokhoz.");
            }

            Console.WriteLine("HIANYZASI STATISZTIKAK:");
            var hianyzasStatisztikak = hianyzasManager.GetHianyzasStatisztikak();

            if (hianyzasStatisztikak.Any())
            {
                foreach (var stat in hianyzasStatisztikak)
                {
                    Console.WriteLine("  " + stat.Key + ": " + stat.Value + " hianyzas");
                }
            }
            else
            {
                Console.WriteLine("  Nincs elerheto adat hianyzasokhoz.");
            }

            Console.WriteLine("DIAKOK ATLAGAI ES HIANYZASAI:");
            var diakStatisztikak = jegyManager.GetDiakStatisztikak(hianyzasManager);

            if (diakStatisztikak.Any())
            {
                foreach (var diak in diakStatisztikak)
                {
                    Console.WriteLine("  " + diak.Key + " - Atlag: " + diak.Value.Atlag.ToString("F2") + ", Hianyzasok: " + diak.Value.Hianyzasok);
                }
            }
            else
            {
                Console.WriteLine("  Nincs elerheto adat diakokhoz.");
            }
        }
    }
}