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
                Console.WriteLine("SZULOI MENU - " + Nev);

                var gyermekInfo = GetGyermekInfo();
                if (!string.IsNullOrEmpty(gyermekInfo))
                {
                    Console.WriteLine(gyermekInfo);
                }

                Console.WriteLine("[1] Gyermekem jegyei");
                Console.WriteLine("[2] Gyermekem hianyzasai");
                Console.WriteLine("[3] Hianyzas igazolasa");
                Console.WriteLine("[4] Kozlemenyek");
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
                    hianyzasManager.HianyzasokListazasa(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "3")
                {
                    hianyzasManager.SzuloHianyzasIgazolasa(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "4")
                {
                    kozlemenyManager.KozlemenyekListazasa(this);
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

        private string GetGyermekInfo()
        {
            return "A gyermek adatai betoltes alatt...";
        }
    }
}