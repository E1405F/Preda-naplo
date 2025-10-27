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
            // Felhasználók listája
            List<Felhasznalo> felhasznalok = new List<Felhasznalo>();

            // Regisztrációs osztály inicializálása
            Regisztracio regisztracio = new Regisztracio(felhasznalok);

            // Felhasználók betöltése fájlból
            regisztracio.FelhasznalokBetolteseFajlbol();

            Bejelentkezes bejelentkezes = new Bejelentkezes(felhasznalok);

            bool kilepes = false;

            while (!kilepes)
            {
                Console.Clear();
                Console.WriteLine("=== Préda Napló ===");
                Console.WriteLine();
                Console.WriteLine("1. Bejelentkezés");
                Console.WriteLine("2. Regisztráció");
                Console.WriteLine("3. Kilépés");
                Console.WriteLine();
                Console.Write("Válassz egy menüpontot: ");

                string valasztas = Console.ReadLine();

                switch (valasztas)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("=== Bejelentkezés ===");
                        Console.WriteLine();

                        bool sikeres = bejelentkezes.Belepes();

                        if (sikeres)
                        {
                            Console.WriteLine("\n✅ Sikeres bejelentkezés!");
                            Console.WriteLine($"Üdvözöllek, {bejelentkezes.AktualisFelhasznalo.Nev}!");

                            // Itt jönne a fő alkalmazás funkció
                            Console.WriteLine("\nNyomj egy gombot a folytatáshoz...");
                            Console.ReadKey();
                        }
                        else
                        {
                            // Automatikusan visszadob a főmenübe
                        }
                        break;

                    case "2":
                        bool regSikeres = regisztracio.Regisztralas();
                        // Ha sikertelen a regisztráció, automatikusan visszadob a főmenübe
                        break;

                    case "3":
                        kilepes = true;
                        Console.WriteLine("Viszlát!");
                        break;

                    default:
                        Console.WriteLine("❌ Érvénytelen választás!");
                        Console.WriteLine("Nyomj egy gombot a folytatáshoz...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}