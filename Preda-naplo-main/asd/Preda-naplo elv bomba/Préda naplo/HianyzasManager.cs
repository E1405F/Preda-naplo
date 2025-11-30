using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Préda_naplo
{
    internal class HianyzasManager
    {
        private List<Hianyzas> hianyzasok;
        private string fajlEleres;
        private List<Felhasznalo> felhasznalok;
        private Dictionary<string, string> szuloGyerekKapcsolat;

        public HianyzasManager(string fajlEleres, List<Felhasznalo> felhasznalok, Dictionary<string, string> szuloGyerekKapcsolat)
        {
            this.fajlEleres = fajlEleres;
            this.felhasznalok = felhasznalok;
            this.szuloGyerekKapcsolat = szuloGyerekKapcsolat;
            hianyzasok = new List<Hianyzas>();
            BetoltFajlbol();
        }

        public void HianyzasRogzitese(Felhasznalo tanar)
        {
            if (tanar.Szerepkor != "Tanár")
            {
                Console.WriteLine("❌ Csak tanárok rögzíthetnek hiányzásokat!");
                return;
            }

            Console.WriteLine("\n=== HIÁNYZÁS RÖGZÍTÉSE ===");

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

            Console.WriteLine($"Diák: {diak.Nev} ({diak.Osztaly})");

            Console.Write("Dátum (éééé.hh.nn, üresen hagyva ma): ");
            string datumInput = Console.ReadLine();
            DateTime datum = string.IsNullOrEmpty(datumInput) ? DateTime.Now : DateTime.Parse(datumInput);

            Console.Write("Óra/tantárgy: ");
            string ora = Console.ReadLine();

            Console.Write($"Biztosan hiányzást rögzítesz {diak.Nev} számára? (i/n): ");
            string valasz = Console.ReadLine().ToLower();

            if (valasz == "i")
            {
                Hianyzas uj = new Hianyzas(diakFelh, tanar.Felhasznalonev, datum, ora, false);
                hianyzasok.Add(uj);
                File.AppendAllText(fajlEleres, uj.ToFileFormat() + Environment.NewLine);
                Console.WriteLine($"✅ Hiányzás rögzítve: {diak.Nev} - {datum:yyyy.MM.dd} - {ora}");
            }
            else
            {
                Console.WriteLine("❌ Hiányzás rögzítése megszakítva.");
            }
        }

        public void HianyzasIgazolasa(Felhasznalo felhasznalo)
        {
            if (felhasznalo.Szerepkor != "Tanár" && felhasznalo.Szerepkor != "Igazgató")
            {
                Console.WriteLine("❌ Nincs jogosultságod hiányzásokat igazolni!");
                return;
            }

            Console.WriteLine("\n=== HIÁNYZÁS IGAZOLÁSA ===");

            var igazolatlanHianyzasok = hianyzasok.Where(h => !h.Igazolt).ToList();
            if (!igazolatlanHianyzasok.Any())
            {
                Console.WriteLine("Nincsenek igazolatlan hiányzások.");
                return;
            }

            for (int i = 0; i < igazolatlanHianyzasok.Count; i++)
            {
                var hianyzas = igazolatlanHianyzasok[i];
                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == hianyzas.DiakFelhasznalonev);
                string diakNev = diak?.Nev ?? hianyzas.DiakFelhasznalonev;

                Console.WriteLine($"[{i + 1}] {diakNev} - {hianyzas.Datum:yyyy.MM.dd} - {hianyzas.Ora}");
            }

            Console.Write("\nVálassz hiányzást a sorszámával (0 - mégsem): ");
            if (int.TryParse(Console.ReadLine(), out int valasztas) && valasztas > 0 && valasztas <= igazolatlanHianyzasok.Count)
            {
                var kivalasztott = igazolatlanHianyzasok[valasztas - 1];
                kivalasztott.Igazolt = true;
                FrissitFajl();
                Console.WriteLine("✅ Hiányzás igazolva!");
            }
        }

        public void HianyzasokListazasa(Felhasznalo felhasznalo)
        {
            Console.WriteLine($"\n=== HIÁNYZÁSOK - {felhasznalo.Nev.ToUpper()} ===");

            List<Hianyzas> megjelenitendoHianyzasok;

            if (felhasznalo.Szerepkor == "Tanár" || felhasznalo.Szerepkor == "Igazgató" || felhasznalo.Szerepkor == "Adminisztrátor")
            {
                megjelenitendoHianyzasok = hianyzasok;
            }
            else if (felhasznalo.Szerepkor == "Szülő")
            {
                // Szülő esetén a gyermek hiányzásai
                megjelenitendoHianyzasok = GetGyermekHianyzasai(felhasznalo);
            }
            else
            {
                megjelenitendoHianyzasok = hianyzasok.Where(h => h.DiakFelhasznalonev == felhasznalo.Felhasznalonev).ToList();
            }

            if (!megjelenitendoHianyzasok.Any())
            {
                Console.WriteLine("Nincsenek hiányzások.");
                return;
            }

            foreach (var hianyzas in megjelenitendoHianyzasok.OrderBy(h => h.DiakFelhasznalonev).ThenBy(h => h.Datum))
            {
                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == hianyzas.DiakFelhasznalonev);
                string diakNev = diak?.Nev ?? hianyzas.DiakFelhasznalonev;

                Console.WriteLine($"{diakNev} - {hianyzas.Datum:yyyy.MM.dd} - {hianyzas.Ora} - {(hianyzas.Igazolt ? "✅ Igazolva" : "❌ Igazolatlan")}");
            }
        }

        // ÚJ METÓDUS: Gyermek hiányzásainak lekérése
        private List<Hianyzas> GetGyermekHianyzasai(Felhasznalo szulo)
        {
            var gyermekHianyzasok = new List<Hianyzas>();

            if (szuloGyerekKapcsolat.TryGetValue(szulo.Felhasznalonev, out string gyermekFelhasznalonev))
            {
                gyermekHianyzasok = hianyzasok.Where(h => h.DiakFelhasznalonev == gyermekFelhasznalonev).ToList();

                var gyermek = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == gyermekFelhasznalonev);
                if (gyermek != null)
                {
                    Console.WriteLine($"👨‍👦 Gyermeked: {gyermek.Nev} ({gyermek.Osztaly})");
                }
            }
            else
            {
                Console.WriteLine("❌ Nincs gyermeked hozzárendelve a fiókodhoz!");
            }

            return gyermekHianyzasok;
        }

        // HIÁNYZÓ METÓDUS: Hiányzások számának lekérdezése diák szerint
        public int GetHianyzasokSzama(string diakFelhasznalonev)
        {
            return hianyzasok.Count(h => h.DiakFelhasznalonev == diakFelhasznalonev);
        }

        // HIÁNYZÓ METÓDUS: Hiányzási statisztikák osztályonként
        public Dictionary<string, int> GetHianyzasStatisztikak()
        {
            var statisztikak = new Dictionary<string, int>();

            var hianyzasokOsztalySzerint = hianyzasok
                .GroupBy(h => {
                    var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == h.DiakFelhasznalonev && f.Szerepkor == "Diák");
                    return diak?.Osztaly ?? "Ismeretlen";
                })
                .Where(g => g.Key != "Ismeretlen");

            foreach (var osztalyGroup in hianyzasokOsztalySzerint)
            {
                statisztikak.Add(osztalyGroup.Key, osztalyGroup.Count());
            }

            return statisztikak;
        }

        // HIÁNYZÓ METÓDUS: Szülői hiányzás igazolás
        public void SzuloHianyzasIgazolasa(Felhasznalo szulo)
        {
            if (szulo.Szerepkor != "Szülő")
            {
                Console.WriteLine("❌ Csak szülők használhatják ezt a funkciót!");
                return;
            }

            Console.WriteLine("\n=== HIÁNYZÁS IGAZOLÁSA (SZÜLŐI) ===");

            // Gyermek felhasználónevének lekérése a kapcsolatból
            if (!szuloGyerekKapcsolat.TryGetValue(szulo.Felhasznalonev, out string gyerekFelhasznalonev))
            {
                Console.WriteLine("❌ Nincs gyermeked hozzárendelve a fiókodhoz!");
                return;
            }

            var gyerek = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == gyerekFelhasznalonev);
            if (gyerek == null)
            {
                Console.WriteLine("❌ A hozzárendelt gyermek nem található!");
                return;
            }

            Console.WriteLine($"Gyermeked: {gyerek.Nev} ({gyerekFelhasznalonev})");

            var igazolatlanHianyzasok = hianyzasok
                .Where(h => h.DiakFelhasznalonev == gyerekFelhasznalonev && !h.Igazolt)
                .ToList();

            if (!igazolatlanHianyzasok.Any())
            {
                Console.WriteLine("Nincsenek igazolatlan hiányzások.");
                return;
            }

            Console.WriteLine($"\nIgazolatlan hiányzások:");
            for (int i = 0; i < igazolatlanHianyzasok.Count; i++)
            {
                var hianyzas = igazolatlanHianyzasok[i];
                Console.WriteLine($"[{i + 1}] {hianyzas.Datum:yyyy.MM.dd} - {hianyzas.Ora}");
            }

            Console.Write("\nVálassz hiányzást a sorszámával (0 - mégsem): ");
            if (int.TryParse(Console.ReadLine(), out int valasztas) && valasztas > 0 && valasztas <= igazolatlanHianyzasok.Count)
            {
                var kivalasztott = igazolatlanHianyzasok[valasztas - 1];
                kivalasztott.Igazolt = true;
                FrissitFajl();
                Console.WriteLine("✅ Hiányzás sikeresen igazolva!");
            }
        }

        private void BetoltFajlbol()
        {
            if (!File.Exists(fajlEleres)) return;

            foreach (var sor in File.ReadAllLines(fajlEleres))
            {
                if (string.IsNullOrWhiteSpace(sor)) continue;
                hianyzasok.Add(Hianyzas.FromFileFormat(sor));
            }
        }

        private void FrissitFajl()
        {
            File.WriteAllLines(fajlEleres, hianyzasok.Select(h => h.ToFileFormat()));
        }
    }

    internal class Hianyzas
    {
        public string DiakFelhasznalonev { get; set; }
        public string TanarFelhasznalonev { get; set; }
        public DateTime Datum { get; set; }
        public string Ora { get; set; }
        public bool Igazolt { get; set; }

        public Hianyzas(string diakFelhasznalonev, string tanarFelhasznalonev, DateTime datum, string ora, bool igazolt)
        {
            DiakFelhasznalonev = diakFelhasznalonev;
            TanarFelhasznalonev = tanarFelhasznalonev;
            Datum = datum;
            Ora = ora;
            Igazolt = igazolt;
        }

        public string ToFileFormat()
        {
            return $"{DiakFelhasznalonev};{TanarFelhasznalonev};{Datum:yyyy.MM.dd};{Ora};{Igazolt}";
        }

        public static Hianyzas FromFileFormat(string sor)
        {
            var adatok = sor.Split(';');
            return new Hianyzas(adatok[0], adatok[1], DateTime.Parse(adatok[2]), adatok[3], bool.Parse(adatok[4]));
        }
    }
}