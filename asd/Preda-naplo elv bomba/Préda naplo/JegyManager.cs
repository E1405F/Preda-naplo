using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Préda_naplo
{
    internal class JegyManager
    {
        private List<Jegy> jegyek;
        private string fajlEleres;
        private List<Felhasznalo> felhasznalok;

        public JegyManager(string fajlEleres, List<Felhasznalo> felhasznalok)
        {
            this.fajlEleres = fajlEleres;
            this.felhasznalok = felhasznalok;
            jegyek = new List<Jegy>();
            BetoltFajlbol();
        }

        public void UjJegy(Felhasznalo tanar)
        {
            if (tanar.Szerepkor != "Tanár")
            {
                Console.WriteLine("❌ Csak tanárok írhatnak be jegyeket!");
                return;
            }

            Console.WriteLine("\n=== ÚJ JEGY RÖGZÍTÉSE ===");

            // Diák kiválasztása
            Console.Write("Diák felhasználóneve: ");
            string diakFelh = Console.ReadLine();

            // Ellenőrizzük, hogy létezik-e a diák
            var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == diakFelh && f.Szerepkor == "Diák");
            if (diak == null)
            {
                Console.WriteLine("❌ Nem található ilyen diák!");
                return;
            }

            Console.Write("Tantárgy: ");
            string tantargy = Console.ReadLine();

            int ertek = 0;
            bool ervenyesJegy = false;
            while (!ervenyesJegy)
            {
                Console.Write("Jegy (1-5): ");
                if (int.TryParse(Console.ReadLine(), out ertek) && ertek >= 1 && ertek <= 5)
                {
                    ervenyesJegy = true;
                }
                else
                {
                    Console.WriteLine("❌ Érvénytelen jegy! Csak 1-5 közötti érték lehet.");
                }
            }

            Console.Write("Megjegyzés (opcionális): ");
            string megjegyzes = Console.ReadLine();

            Jegy ujJegy = new Jegy(diakFelh, tanar.Felhasznalonev, tantargy, ertek, DateTime.Now, megjegyzes);
            jegyek.Add(ujJegy);
            File.AppendAllText(fajlEleres, ujJegy.ToFileFormat() + Environment.NewLine);

            Console.WriteLine($"\n✅ Jegy sikeresen rögzítve: {diak.Nev} - {tantargy} - {ertek}");
        }

        public void JegyIrasOsztalynak(Felhasznalo tanar)
        {
            if (tanar.Szerepkor != "Tanár")
            {
                Console.WriteLine("❌ Csak tanárok írhatnak be jegyeket!");
                return;
            }

            Console.WriteLine("\n=== JEGY ÍRÁS OSZTÁLYNAK ===");

            Console.Write("Osztály (pl. 10.A): ");
            string osztaly = Console.ReadLine();

            // Diákok lekérése az osztályból
            var osztalyDiakjai = felhasznalok
                .Where(f => f.Szerepkor == "Diák" && f.Osztaly == osztaly)
                .ToList();

            if (!osztalyDiakjai.Any())
            {
                Console.WriteLine($"❌ Nem található diák a(z) {osztaly} osztályban!");
                return;
            }

            Console.WriteLine($"\n📋 {osztaly} osztály diákjai:");
            foreach (var diak in osztalyDiakjai)
            {
                Console.WriteLine($"  - {diak.Nev} ({diak.Felhasznalonev})");
            }

            Console.Write("\nTantárgy: ");
            string tantargy = Console.ReadLine();

            Console.WriteLine($"\n💡 Most jegyeket írhatsz a {osztaly} osztály diákjainak...");

            foreach (var diak in osztalyDiakjai)
            {
                Console.WriteLine($"\n--- {diak.Nev} ---");

                Console.Write($"Jegy (1-5, 0 ha nem kap jegyet): ");
                if (int.TryParse(Console.ReadLine(), out int ertek))
                {
                    if (ertek >= 1 && ertek <= 5)
                    {
                        Console.Write("Megjegyzés (opcionális): ");
                        string megjegyzes = Console.ReadLine();

                        Jegy ujJegy = new Jegy(diak.Felhasznalonev, tanar.Felhasznalonev, tantargy, ertek, DateTime.Now, megjegyzes);
                        jegyek.Add(ujJegy);
                        File.AppendAllText(fajlEleres, ujJegy.ToFileFormat() + Environment.NewLine);
                        Console.WriteLine($"✅ Jegy rögzítve: {ertek}");
                    }
                    else if (ertek == 0)
                    {
                        Console.WriteLine("ℹ️  Nem kap jegyet.");
                    }
                    else
                    {
                        Console.WriteLine("❌ Érvénytelen jegy, kihagyva.");
                    }
                }
                else
                {
                    Console.WriteLine("❌ Érvénytelen bemenet, kihagyva.");
                }
            }

            Console.WriteLine($"\n✅ Jegyírás befejezve a(z) {osztaly} osztály számára!");
        }

        public void JegyekListazasa(Felhasznalo felhasznalo)
        {
            Console.WriteLine($"\n=== JEGYEK - {felhasznalo.Nev.ToUpper()} ===");

            List<Jegy> megjelenitendoJegyek;

            if (felhasznalo.Szerepkor == "Tanár" || felhasznalo.Szerepkor == "Igazgató" || felhasznalo.Szerepkor == "Adminisztrátor")
            {
                megjelenitendoJegyek = jegyek;
            }
            else
            {
                megjelenitendoJegyek = jegyek.Where(j => j.DiakFelhasznalonev == felhasznalo.Felhasznalonev).ToList();
            }

            if (!megjelenitendoJegyek.Any())
            {
                Console.WriteLine("Nincsenek jegyek a megjelenítéshez.");
                return;
            }

            var jegyekDiakSzerint = megjelenitendoJegyek
                .GroupBy(j => j.DiakFelhasznalonev)
                .OrderBy(g => g.Key);

            foreach (var diakGroup in jegyekDiakSzerint)
            {
                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == diakGroup.Key);
                string diakNev = diak != null ? diak.Nev : diakGroup.Key;
                string diakOsztaly = diak != null ? diak.Osztaly : "Ismeretlen";

                Console.WriteLine($"\n📚 Diák: {diakNev} ({diakGroup.Key}) - Osztály: {diakOsztaly}");
                Console.WriteLine(new string('-', 50));

                var jegyekTantargySzerint = diakGroup.GroupBy(j => j.Tantargy);

                foreach (var tantargyGroup in jegyekTantargySzerint)
                {
                    Console.WriteLine($"\n{tantargyGroup.Key}:");
                    foreach (var jegy in tantargyGroup.OrderBy(j => j.Datum))
                    {
                        Console.WriteLine($"  - {jegy}");
                    }

                    var atlag = tantargyGroup.Average(j => j.Ertek);
                    Console.WriteLine($"  Átlag: {atlag:F2}");
                }
            }
        }

        private void BetoltFajlbol()
        {
            if (!File.Exists(fajlEleres))
            {
                File.Create(fajlEleres).Close();
                return;
            }

            foreach (var sor in File.ReadAllLines(fajlEleres))
            {
                if (string.IsNullOrWhiteSpace(sor)) continue;

                var adatok = sor.Split(';');
                if (adatok.Length >= 5)
                {
                    string diakFelh = adatok[0];
                    string tanarFelh = adatok[1];
                    string tantargy = adatok[2];
                    int ertek = int.Parse(adatok[3]);
                    DateTime datum = DateTime.Parse(adatok[4]);
                    string megjegyzes = adatok.Length > 5 ? adatok[5] : "";

                    jegyek.Add(new Jegy(diakFelh, tanarFelh, tantargy, ertek, datum, megjegyzes));
                }
            }
        }
    }
}