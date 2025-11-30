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
                Console.WriteLine("Csak tanarok rogzithetnek hianyzasokat!");
                return;
            }

            Console.WriteLine("HIANYZAS ROGZITESE");

            Console.Write("Diák felhasznaloneve: ");
            string diakFelh = Console.ReadLine();

            var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == diakFelh && f.Szerepkor == "Diák");
            if (diak == null)
            {
                Console.WriteLine("Nem talalhato ilyen diak!");
                return;
            }

            Console.WriteLine("Diak: " + diak.Nev + " (" + diak.Osztaly + ")");

            DateTime datum;
            while (true)
            {
                Console.Write("Datum (eééé.hh.nn, uresen hagyva ma): ");
                string datumInput = Console.ReadLine();

                if (string.IsNullOrEmpty(datumInput))
                {
                    datum = DateTime.Now;
                    break;
                }

                try
                {
                    datum = DateTime.Parse(datumInput);
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Hibas datum formatum! Hasznald az eééé.hh.nn formatumot, vagy hagyd uresen.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hiba a datum feldolgozasakor: " + ex.Message);
                }
            }

            Console.Write("Ora/tantargy: ");
            string ora = Console.ReadLine();

            Console.Write("Biztosan hianyzas rogzitesz " + diak.Nev + " szamara? (i/n): ");
            string valasz = Console.ReadLine().ToLower();

            if (valasz == "i")
            {
                Hianyzas uj = new Hianyzas(diakFelh, tanar.Felhasznalonev, datum, ora, false);
                hianyzasok.Add(uj);
                File.AppendAllText(fajlEleres, uj.ToFileFormat() + Environment.NewLine);
                Console.WriteLine("Hianyzas rogzitve: " + diak.Nev + " - " + datum.ToString("yyyy.MM.dd") + " - " + ora);
            }
            else
            {
                Console.WriteLine("Hianyzas rogzitese megszakítva.");
            }
        }

        public void HianyzasIgazolasa(Felhasznalo felhasznalo)
        {
            if (felhasznalo.Szerepkor != "Tanár" && felhasznalo.Szerepkor != "Igazgató")
            {
                Console.WriteLine("Nincs jogosultsagod hianyzasokat igazolni!");
                return;
            }

            Console.WriteLine("HIANYZAS IGAZOLASA");

            var igazolatlanHianyzasok = hianyzasok.Where(h => !h.Igazolt).ToList();
            if (!igazolatlanHianyzasok.Any())
            {
                Console.WriteLine("Nincsenek igazolatlan hianyzasok.");
                return;
            }

            for (int i = 0; i < igazolatlanHianyzasok.Count; i++)
            {
                var hianyzas = igazolatlanHianyzasok[i];
                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == hianyzas.DiakFelhasznalonev);
                string diakNev = diak?.Nev ?? hianyzas.DiakFelhasznalonev;

                Console.WriteLine("[" + (i + 1) + "] " + diakNev + " - " + hianyzas.Datum.ToString("yyyy.MM.dd") + " - " + hianyzas.Ora);
            }

            Console.Write("Valassz hianyzas a sorszamaval (0 - megsem): ");
            if (int.TryParse(Console.ReadLine(), out int valasztas) && valasztas > 0 && valasztas <= igazolatlanHianyzasok.Count)
            {
                var kivalasztott = igazolatlanHianyzasok[valasztas - 1];
                kivalasztott.Igazolt = true;
                FrissitFajl();
                Console.WriteLine("Hianyzas igazolva!");
            }
        }

        public void HianyzasokListazasa(Felhasznalo felhasznalo)
        {
            Console.WriteLine("HIANYZASOK - " + felhasznalo.Nev.ToUpper());

            List<Hianyzas> megjelenitendoHianyzasok;

            if (felhasznalo.Szerepkor == "Tanár" || felhasznalo.Szerepkor == "Igazgató" || felhasznalo.Szerepkor == "Adminisztrátor")
            {
                megjelenitendoHianyzasok = hianyzasok;
            }
            else if (felhasznalo.Szerepkor == "Szülő")
            {
                megjelenitendoHianyzasok = GetGyermekHianyzasai(felhasznalo);
            }
            else
            {
                megjelenitendoHianyzasok = hianyzasok.Where(h => h.DiakFelhasznalonev == felhasznalo.Felhasznalonev).ToList();
            }

            if (!megjelenitendoHianyzasok.Any())
            {
                Console.WriteLine("Nincsenek hianyzasok.");
                return;
            }

            foreach (var hianyzas in megjelenitendoHianyzasok.OrderBy(h => h.DiakFelhasznalonev).ThenBy(h => h.Datum))
            {
                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == hianyzas.DiakFelhasznalonev);
                string diakNev = diak?.Nev ?? hianyzas.DiakFelhasznalonev;

                Console.WriteLine(diakNev + " - " + hianyzas.Datum.ToString("yyyy.MM.dd") + " - " + hianyzas.Ora + " - " + (hianyzas.Igazolt ? "Igazolva" : "Igazolatlan"));
            }
        }

        private List<Hianyzas> GetGyermekHianyzasai(Felhasznalo szulo)
        {
            var gyermekHianyzasok = new List<Hianyzas>();

            if (szuloGyerekKapcsolat.TryGetValue(szulo.Felhasznalonev, out string gyermekFelhasznalonev))
            {
                gyermekHianyzasok = hianyzasok.Where(h => h.DiakFelhasznalonev == gyermekFelhasznalonev).ToList();

                var gyermek = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == gyermekFelhasznalonev);
                if (gyermek != null)
                {
                    Console.WriteLine("Gyermeked: " + gyermek.Nev + " (" + gyermek.Osztaly + ")");
                }
            }
            else
            {
                Console.WriteLine("Nincs gyermeked hozzarendelve a fiokodhoz!");
            }

            return gyermekHianyzasok;
        }

        public int GetHianyzasokSzama(string diakFelhasznalonev)
        {
            return hianyzasok.Count(h => h.DiakFelhasznalonev == diakFelhasznalonev);
        }

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

        public void SzuloHianyzasIgazolasa(Felhasznalo szulo)
        {
            if (szulo.Szerepkor != "Szülő")
            {
                Console.WriteLine("Csak szulok hasznalhatjak ezt a funkciot!");
                return;
            }

            Console.WriteLine("HIANYZAS IGAZOLASA (SZULOI)");

            if (!szuloGyerekKapcsolat.TryGetValue(szulo.Felhasznalonev, out string gyerekFelhasznalonev))
            {
                Console.WriteLine("Nincs gyermeked hozzarendelve a fiokodhoz!");
                return;
            }

            var gyerek = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == gyerekFelhasznalonev);
            if (gyerek == null)
            {
                Console.WriteLine("A hozzarendelt gyermek nem talalhato!");
                return;
            }

            Console.WriteLine("Gyermeked: " + gyerek.Nev + " (" + gyerekFelhasznalonev + ")");

            var igazolatlanHianyzasok = hianyzasok
                .Where(h => h.DiakFelhasznalonev == gyerekFelhasznalonev && !h.Igazolt)
                .ToList();

            if (!igazolatlanHianyzasok.Any())
            {
                Console.WriteLine("Nincsenek igazolatlan hianyzasok.");
                return;
            }

            Console.WriteLine("Igazolatlan hianyzasok:");
            for (int i = 0; i < igazolatlanHianyzasok.Count; i++)
            {
                var hianyzas = igazolatlanHianyzasok[i];
                Console.WriteLine("[" + (i + 1) + "] " + hianyzas.Datum.ToString("yyyy.MM.dd") + " - " + hianyzas.Ora);
            }

            Console.Write("Valassz hianyzas a sorszamaval (0 - megsem): ");
            if (int.TryParse(Console.ReadLine(), out int valasztas) && valasztas > 0 && valasztas <= igazolatlanHianyzasok.Count)
            {
                var kivalasztott = igazolatlanHianyzasok[valasztas - 1];
                kivalasztott.Igazolt = true;
                FrissitFajl();
                Console.WriteLine("Hianyzas sikeresen igazolva!");
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
            return DiakFelhasznalonev + ";" + TanarFelhasznalonev + ";" + Datum.ToString("yyyy.MM.dd") + ";" + Ora + ";" + Igazolt;
        }

        public static Hianyzas FromFileFormat(string sor)
        {
            var adatok = sor.Split(';');
            return new Hianyzas(adatok[0], adatok[1], DateTime.Parse(adatok[2]), adatok[3], bool.Parse(adatok[4]));
        }
    }
}