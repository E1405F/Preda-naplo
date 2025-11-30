using System;

namespace Préda_naplo
{
    internal class Diak : Felhasznalo
    {
        public Diak(string felhasznalonev, string jelszo, string nev, string iskolaNev, string osztaly)
            : base(felhasznalonev, jelszo, nev, iskolaNev, "Diák", osztaly) { }

        public override void FoMenu(JegyManager jegyManager, KozlemenyManager kozlemenyManager, HianyzasManager hianyzasManager)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("FOMENU - " + Nev + " (" + Szerepkor + ")");
                if (!string.IsNullOrEmpty(Osztaly))
                {
                    Console.WriteLine("Osztaly: " + Osztaly);
                }
                Console.WriteLine("[1] Jegyek megtekintese");
                Console.WriteLine("[2] Fellebbezés benyújtása");
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
                    jegyManager.FellebbezesBenyujtasa(this);
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
    }
}