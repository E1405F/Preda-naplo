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
                Console.WriteLine("TANARI MENU - " + Nev);
                if (!string.IsNullOrEmpty(Osztaly))
                {
                    Console.WriteLine("Osztaly: " + Osztaly);
                }
                Console.WriteLine("[1] Jegyek megtekintese");
                Console.WriteLine("[2] Uj jegy rogzitese");
                Console.WriteLine("[3] Jegy iras osztalynak");
                Console.WriteLine("[4] Sulyozott jegy rogzitese");
                Console.WriteLine("[5] Hianyzas rogzitese");
                Console.WriteLine("[6] Hianyzasok listazasa");
                Console.WriteLine("[7] Hianyzas igazolasa");
                Console.WriteLine("[8] Fellebbezések kezelese");
                Console.WriteLine("[9] Diak lezarasa");
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
                    jegyManager.UjJegy(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "3")
                {
                    jegyManager.JegyIrasOsztalynak(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "4")
                {
                    jegyManager.UjJegySullyal(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "5")
                {
                    hianyzasManager.HianyzasRogzitese(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "6")
                {
                    hianyzasManager.HianyzasokListazasa(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "7")
                {
                    hianyzasManager.HianyzasIgazolasa(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "8")
                {
                    jegyManager.FellebbezesekKezelese(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "9")
                {
                    DiakLezarasa();
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

        private void DiakLezarasa()
        {
            Console.WriteLine("DIAK LEZARASA");
            Console.WriteLine("Ez a funkcio lehetove teszi az ev vegi jegyek lezarasat.");
            Console.WriteLine("A lezaras utan a jegyek mar nem modosithatok.");
            Console.Write("Biztosan le szeretned zarni az osztalyzatokat? (i/n): ");

            string valasz = Console.ReadLine().ToLower();
            if (valasz == "i")
            {
                Console.WriteLine("Osztalyzatok sikeresen lezarva!");
            }
            else
            {
                Console.WriteLine("Lezaras megszakítva.");
            }
        }
    }
}