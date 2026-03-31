using System;
using System.Collections.Generic;

namespace WpfApp1.Models
{
    internal class Diak : Felhasznalo
    {
        public List<string> Tantargyak { get; set; } = new List<string>();

        public Diak(string felhasznalonev, string jelszo, string nev, string iskolaNev, string osztaly)
            : base(felhasznalonev, jelszo, nev, iskolaNev, "Diák", osztaly)
        {
            Tantargyak = new List<string>();
        }

        public Diak(string felhasznalonev, string jelszo, string nev, string iskolaNev, string osztaly, List<string> tantargyak)
            : base(felhasznalonev, jelszo, nev, iskolaNev, "Diák", osztaly)
        {
            Tantargyak = tantargyak ?? new List<string>();
        }

        public override string ToFileFormat()
        {
            string tantargyakString = string.Join(",", Tantargyak);
            return $"{Felhasznalonev};{GetJelszo()};{Nev};{IskolaNev};{Szerepkor};{Osztaly};{tantargyakString}";
        }

        public override void FoMenu(JegyManager jegyManager, KozlemenyManager kozlemenyManager, HianyzasManager hianyzasManager)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"DIÁK MENÜ - {Nev} ({Szerepkor})");
                if (!string.IsNullOrEmpty(Osztaly))
                {
                    Console.WriteLine($"Osztály: {Osztaly}");
                }
                Console.WriteLine("[1] Jegyek megtekintése");
                Console.WriteLine("[2] Fellebbezés benyújtása");
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