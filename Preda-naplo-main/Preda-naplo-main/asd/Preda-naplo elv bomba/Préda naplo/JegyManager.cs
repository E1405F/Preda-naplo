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
        private Dictionary<string, string> szuloGyerekKapcsolat;

        public JegyManager(string fajlEleres, List<Felhasznalo> felhasznalok, Dictionary<string, string> szuloGyerekKapcsolat)
        {
            this.fajlEleres = fajlEleres;
            this.felhasznalok = felhasznalok;
            this.szuloGyerekKapcsolat = szuloGyerekKapcsolat;
            jegyek = new List<Jegy>();
            BetoltFajlbol();
        }

        public List<Felhasznalo> GetFelhasznalok()
        {
            return felhasznalok;
        }

        public Dictionary<string, double> GetOsztalyAtlagok()
        {
            var osztalyAtlagok = new Dictionary<string, double>();

            var diakokOsztalySzerint = jegyek
                .Where(j => felhasznalok.Any(f => f.Felhasznalonev == j.DiakFelhasznalonev && f.Szerepkor == "Diák"))
                .GroupBy(j => {
                    var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == j.DiakFelhasznalonev && f.Szerepkor == "Diák");
                    return diak?.Osztaly ?? "Ismeretlen";
                })
                .Where(g => g.Key != "Ismeretlen");

            foreach (var osztalyGroup in diakokOsztalySzerint)
            {
                if (osztalyGroup.Any())
                {
                    double sumErtekSullyal = osztalyGroup.Sum(j => j.Ertek * j.Suly);
                    double sumSuly = osztalyGroup.Sum(j => j.Suly);
                    double atlag = sumSuly > 0 ? sumErtekSullyal / sumSuly : 0;
                    osztalyAtlagok.Add(osztalyGroup.Key, atlag);
                }
            }

            return osztalyAtlagok;
        }

        public Dictionary<string, (double Atlag, int Hianyzasok)> GetDiakStatisztikak(HianyzasManager hianyzasManager)
        {
            var statisztikak = new Dictionary<string, (double, int)>();

            var diakok = felhasznalok.Where(f => f.Szerepkor == "Diák");

            foreach (var diak in diakok)
            {
                var diakJegyei = jegyek.Where(j => j.DiakFelhasznalonev == diak.Felhasznalonev);

                double atlag = 0;
                if (diakJegyei.Any())
                {
                    double sumErtekSullyal = diakJegyei.Sum(j => j.Ertek * j.Suly);
                    double sumSuly = diakJegyei.Sum(j => j.Suly);
                    atlag = sumSuly > 0 ? sumErtekSullyal / sumSuly : 0;
                }

                int hianyzasok = hianyzasManager.GetHianyzasokSzama(diak.Felhasznalonev);

                string diakInfo = $"{diak.Nev} ({diak.Osztaly})";
                statisztikak.Add(diakInfo, (atlag, hianyzasok));
            }

            return statisztikak;
        }

        public void UjJegy(Felhasznalo tanar)
        {
            if (tanar.Szerepkor != "Tanár")
            {
                Console.WriteLine("Csak tanárok írhatnak be jegyeket!");
                return;
            }

            Console.WriteLine("UJ JEGY ROGZITESE");

            Console.Write("Diák felhasználóneve: ");
            string diakFelh = Console.ReadLine();

            var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == diakFelh && f.Szerepkor == "Diák");
            if (diak == null)
            {
                Console.WriteLine("Nem található ilyen diák!");
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
                    Console.WriteLine("Ervenytelen jegy! Csak 1-5 kozotti ertek lehet.");
                }
            }

            Console.Write("Megjegyzés (opcionális): ");
            string megjegyzes = Console.ReadLine();

            Jegy ujJegy = new Jegy(diakFelh, tanar.Felhasznalonev, tantargy, ertek, DateTime.Now, megjegyzes);
            jegyek.Add(ujJegy);
            File.AppendAllText(fajlEleres, ujJegy.ToFileFormat() + Environment.NewLine);

            Console.WriteLine("Jegy sikeresen rogzitve: " + diak.Nev + " - " + tantargy + " - " + ertek);
        }

        public void JegyIrasOsztalynak(Felhasznalo tanar)
        {
            if (tanar.Szerepkor != "Tanár")
            {
                Console.WriteLine("Csak tanárok írhatnak be jegyeket!");
                return;
            }

            Console.WriteLine("JEGY IRAS OSZTALYNAK");

            Console.Write("Osztály (pl. 10.A): ");
            string osztaly = Console.ReadLine();

            var osztalyDiakjai = felhasznalok
                .Where(f => f.Szerepkor == "Diák" && f.Osztaly == osztaly)
                .ToList();

            if (!osztalyDiakjai.Any())
            {
                Console.WriteLine("Nem talalhato diak a(z) " + osztaly + " osztalyban!");
                return;
            }

            Console.WriteLine(osztaly + " osztaly diakjai:");
            foreach (var diak in osztalyDiakjai)
            {
                Console.WriteLine("  - " + diak.Nev + " (" + diak.Felhasznalonev + ")");
            }

            Console.Write("Tantárgy: ");
            string tantargy = Console.ReadLine();

            Console.WriteLine("Most jegyeket irhatsz a " + osztaly + " osztaly diakjainak...");

            foreach (var diak in osztalyDiakjai)
            {
                Console.WriteLine("--- " + diak.Nev + " ---");

                Console.Write("Jegy (1-5, 0 ha nem kap jegyet): ");
                if (int.TryParse(Console.ReadLine(), out int ertek))
                {
                    if (ertek >= 1 && ertek <= 5)
                    {
                        Console.Write("Megjegyzés (opcionális): ");
                        string megjegyzes = Console.ReadLine();

                        Jegy ujJegy = new Jegy(diak.Felhasznalonev, tanar.Felhasznalonev, tantargy, ertek, DateTime.Now, megjegyzes);
                        jegyek.Add(ujJegy);
                        File.AppendAllText(fajlEleres, ujJegy.ToFileFormat() + Environment.NewLine);
                        Console.WriteLine("Jegy rogzitve: " + ertek);
                    }
                    else if (ertek == 0)
                    {
                        Console.WriteLine("Nem kap jegyet.");
                    }
                    else
                    {
                        Console.WriteLine("Ervenytelen jegy, kihagyva.");
                    }
                }
                else
                {
                    Console.WriteLine("Ervenytelen bemenet, kihagyva.");
                }
            }

            Console.WriteLine("Jegyiras befejezve a(z) " + osztaly + " osztaly szamara!");
        }

        public void JegyekListazasa(Felhasznalo felhasznalo)
        {
            Console.WriteLine("JEGYEK - " + felhasznalo.Nev.ToUpper());

            List<Jegy> megjelenitendoJegyek;

            if (felhasznalo.Szerepkor == "Tanár" || felhasznalo.Szerepkor == "Igazgató" || felhasznalo.Szerepkor == "Adminisztrátor")
            {
                megjelenitendoJegyek = jegyek;
            }
            else if (felhasznalo.Szerepkor == "Szülő")
            {
                megjelenitendoJegyek = GetGyermekJegyei(felhasznalo);
            }
            else
            {
                megjelenitendoJegyek = jegyek.Where(j => j.DiakFelhasznalonev == felhasznalo.Felhasznalonev).ToList();
            }

            if (!megjelenitendoJegyek.Any())
            {
                Console.WriteLine("Nincsenek jegyek a megjeleniteshez.");
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

                Console.WriteLine("Diak: " + diakNev + " (" + diakGroup.Key + ") - Osztaly: " + diakOsztaly);
                Console.WriteLine(new string('-', 50));

                var jegyekTantargySzerint = diakGroup.GroupBy(j => j.Tantargy);

                foreach (var tantargyGroup in jegyekTantargySzerint)
                {
                    Console.WriteLine(tantargyGroup.Key + ":");
                    foreach (var jegy in tantargyGroup.OrderBy(j => j.Datum))
                    {
                        Console.WriteLine("  - " + jegy);
                    }

                    if (tantargyGroup.Any())
                    {
                        double sumErtekSullyal = tantargyGroup.Sum(j => j.Ertek * j.Suly);
                        double sumSuly = tantargyGroup.Sum(j => j.Suly);
                        double atlag = sumSuly > 0 ? sumErtekSullyal / sumSuly : 0;
                        Console.WriteLine("  Sulyozott atlag: " + atlag.ToString("F2"));
                    }
                }

                if (diakGroup.Any())
                {
                    double sumErtekSullyal = diakGroup.Sum(j => j.Ertek * j.Suly);
                    double sumSuly = diakGroup.Sum(j => j.Suly);
                    double atlag = sumSuly > 0 ? sumErtekSullyal / sumSuly : 0;
                    Console.WriteLine("OSSZESITETT SULYOZOTT ATLAG: " + atlag.ToString("F2"));
                }
            }
        }

        private List<Jegy> GetGyermekJegyei(Felhasznalo szulo)
        {
            var gyermekJegyei = new List<Jegy>();

            if (szuloGyerekKapcsolat.TryGetValue(szulo.Felhasznalonev, out string gyermekFelhasznalonev))
            {
                gyermekJegyei = jegyek.Where(j => j.DiakFelhasznalonev == gyermekFelhasznalonev).ToList();

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

            return gyermekJegyei;
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

                    
                    double suly = 1.0;
                    if (adatok.Length > 6 && !string.IsNullOrEmpty(adatok[6]))
                    {
                        
                        string sulyString = adatok[6].Replace('.', ',');
                        suly = double.Parse(sulyString);
                    }

                    bool fellebbezes = adatok.Length > 7 ? bool.Parse(adatok[7]) : false;
                    string fellebbezesIndoklas = adatok.Length > 8 ? adatok[8] : "";
                    bool fellebbezesElbiralva = adatok.Length > 9 ? bool.Parse(adatok[9]) : false;

                    jegyek.Add(new Jegy(diakFelh, tanarFelh, tantargy, ertek, datum, megjegyzes, suly, fellebbezes, fellebbezesIndoklas, fellebbezesElbiralva));
                }
            }
        }

        public void UjJegySullyal(Felhasznalo tanar)
        {
            if (tanar.Szerepkor != "Tanár")
            {
                Console.WriteLine("Csak tanárok írhatnak be jegyeket!");
                return;
            }

            Console.WriteLine("UJ JEGY ROGZITESE (SULYOZOTT)");

            Console.Write("Diák felhasználóneve: ");
            string diakFelh = Console.ReadLine();

            var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == diakFelh && f.Szerepkor == "Diák");
            if (diak == null)
            {
                Console.WriteLine("Nem található ilyen diák!");
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
                    Console.WriteLine("Ervenytelen jegy! Csak 1-5 kozotti ertek lehet.");
                }
            }

            double suly = 1.0;
            bool ervenyesSuly = false;
            while (!ervenyesSuly)
            {
                Console.WriteLine("Jegy súlyozása:");
                Console.WriteLine("1 - Normál (100%)");
                Console.WriteLine("2 - Fontos (150%)");
                Console.WriteLine("3 - Nagyon fontos (200%)");
                Console.WriteLine("4 - Különösen fontos (300%)");
                Console.WriteLine("5 - Extra fontos (400%)");
                Console.Write("Valasztas: ");

                string sulyValasztas = Console.ReadLine();
                switch (sulyValasztas)
                {
                    case "1": suly = 1.0; ervenyesSuly = true; break;
                    case "2": suly = 1.5; ervenyesSuly = true; break;
                    case "3": suly = 2.0; ervenyesSuly = true; break;
                    case "4": suly = 3.0; ervenyesSuly = true; break;
                    case "5": suly = 4.0; ervenyesSuly = true; break;
                    default: Console.WriteLine("Ervenytelen valasztas!"); break;
                }
            }

            Console.Write("Megjegyzés (opcionális): ");
            string megjegyzes = Console.ReadLine();

            Jegy ujJegy = new Jegy(diakFelh, tanar.Felhasznalonev, tantargy, ertek, DateTime.Now, megjegyzes, suly);
            jegyek.Add(ujJegy);
            File.AppendAllText(fajlEleres, ujJegy.ToFileFormat() + Environment.NewLine);

            Console.WriteLine("Jegy sikeresen rogzitve: " + diak.Nev + " - " + tantargy + " - " + ertek + " (" + suly * 100 + "%)");
        }

        public void FellebbezesBenyujtasa(Felhasznalo diak)
        {
            if (diak.Szerepkor != "Diák")
            {
                Console.WriteLine("Csak diákok nyújthatnak be fellebbezést!");
                return;
            }

            Console.WriteLine("FELLEBEZES BENYUJTASA");

            var diakJegyei = jegyek.Where(j => j.DiakFelhasznalonev == diak.Felhasznalonev && !j.Fellebbezes).ToList();

            if (!diakJegyei.Any())
            {
                Console.WriteLine("Nincsenek fellebbezesre jogosult jegyeid.");
                return;
            }

            Console.WriteLine("Jegyeid:");
            for (int i = 0; i < diakJegyei.Count; i++)
            {
                var jegy = diakJegyei[i];
                Console.WriteLine("[" + (i + 1) + "] " + jegy);
            }

            Console.Write("Valassz jegyet a sorszamaval (0 - megsem): ");
            if (int.TryParse(Console.ReadLine(), out int valasztas) && valasztas > 0 && valasztas <= diakJegyei.Count)
            {
                var kivalasztottJegy = diakJegyei[valasztas - 1];

                Console.Write("Fellebbezés indoklása: ");
                string indoklas = Console.ReadLine();

                kivalasztottJegy.Fellebbezes = true;
                kivalasztottJegy.FellebbezesIndoklas = indoklas;
                kivalasztottJegy.FellebbezesElbiralva = false;

                FrissitFajl();
                Console.WriteLine("Fellebbezés sikeresen benyújtva!");
            }
        }

        public void FellebbezesekKezelese(Felhasznalo tanar)
        {
            if (tanar.Szerepkor != "Tanár" && tanar.Szerepkor != "Igazgató")
            {
                Console.WriteLine("Nincs jogosultságod fellebbezéseket kezelni!");
                return;
            }

            Console.WriteLine("FELLEBEZESEK KEZELESE");

            var fellebbezesek = jegyek.Where(j => j.Fellebbezes && !j.FellebbezesElbiralva).ToList();

            if (!fellebbezesek.Any())
            {
                Console.WriteLine("Nincsenek fuggoben levo fellebbezések.");
                return;
            }

            foreach (var jegy in fellebbezesek)
            {
                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == jegy.DiakFelhasznalonev);
                string diakNev = diak?.Nev ?? jegy.DiakFelhasznalonev;

                Console.WriteLine("--- " + diakNev + " ---");
                Console.WriteLine("Jegy: " + jegy.Tantargy + " - " + jegy.Ertek + " (" + jegy.Datum.ToString("yyyy.MM.dd") + ")");
                Console.WriteLine("Indoklás: " + jegy.FellebbezesIndoklas);
                Console.WriteLine("Tanár: " + jegy.TanarFelhasznalonev);

                Console.Write("Elfogadod a fellebbezést? (i/n): ");
                string valasz = Console.ReadLine().ToLower();

                if (valasz == "i")
                {
                    Console.Write("Új jegy érték (1-5): ");
                    if (int.TryParse(Console.ReadLine(), out int ujErtek) && ujErtek >= 1 && ujErtek <= 5)
                    {
                        jegy.Ertek = ujErtek;
                        jegy.FellebbezesElbiralva = true;
                        Console.WriteLine("Fellebbezés elfogadva, jegy módosítva!");
                    }
                }
                else
                {
                    jegy.FellebbezesElbiralva = true;
                    Console.WriteLine("Fellebbezés elutasítva.");
                }
            }

            FrissitFajl();
        }

        private void FrissitFajl()
        {
            File.WriteAllLines(fajlEleres, jegyek.Select(j => j.ToFileFormat()));
        }
    }
}