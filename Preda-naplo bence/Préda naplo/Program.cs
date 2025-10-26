using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Préda_naplo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Előre rögzített felhasználók (példa)
            List<Felhasznalo> felhasznalok = new List<Felhasznalo>()
            {
                new Felhasznalo("kisspisti", "pisti123", "Kiss Pisti"),
                new Felhasznalo("nagymari", "mari2024", "Nagy Mariann"),
                new Felhasznalo("tanarbacsi", "tanar321", "Kovács Béla (tanár)")
            };

            Bejelentkezes bejelentkezes = new Bejelentkezes(felhasznalok);

            Console.WriteLine("=== Préda Napló Bejelentkezés ===");
            Console.WriteLine();

            bool sikeres = bejelentkezes.Belepes();

            if (sikeres)
            {
                Console.WriteLine("\n✅ Sikeres bejelentkezés!");
                Console.WriteLine($"Üdvözöllek, {bejelentkezes.AktualisFelhasznalo.Nev}!");
            }
            else
            {
                Console.WriteLine("\n❌ Sikertelen bejelentkezés – fiók letiltva vagy hibás adatok.");
            }

            Console.ReadKey();
        }
    }
}
