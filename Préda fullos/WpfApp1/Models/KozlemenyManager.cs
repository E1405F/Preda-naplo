using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfApp1.Models
{
    internal class KozlemenyManager
    {
        private List<Kozlemeny> kozlemenyek;
        private string fajlEleres;
        private List<Felhasznalo> felhasznalok;

        public KozlemenyManager(string fajlEleres, List<Felhasznalo> felhasznalok)
        {
            this.fajlEleres = fajlEleres;
            this.felhasznalok = felhasznalok;
            kozlemenyek = new List<Kozlemeny>();
            BetoltFajlbol();
        }

        public void UjKozlemeny(Felhasznalo felado)
        {
            if (felado.Szerepkor != "Tanár" && felado.Szerepkor != "Igazgató" && felado.Szerepkor != "Adminisztrátor")
            {
                Console.WriteLine("❌ Csak tanárok, igazgatók és adminisztrátorok írhatnak közleményeket!");
                return;
            }

            Console.WriteLine("\n=== ÚJ KÖZLEMÉNY ===");
            Console.Write("Cím: ");
            string cim = Console.ReadLine();
            Console.Write("Tartalom: ");
            string tartalom = Console.ReadLine();

            // Közlemény mentése minden felhasználó számára
            foreach (var felhasznalo in felhasznalok)
            {
                Kozlemeny uj = new Kozlemeny(felado.Felhasznalonev, cim, tartalom, DateTime.Now, felhasznalo.Felhasznalonev);
                kozlemenyek.Add(uj);
                File.AppendAllText(fajlEleres, uj.ToFileFormat() + Environment.NewLine);
            }

            Console.WriteLine($"✅ Közlemény sikeresen elküldve {felhasznalok.Count} felhasználónak!");
        }

        // New overload used by WPF UI
        public void UjKozlemeny(Felhasznalo felado, string cim, string tartalom)
        {
            if (felado.Szerepkor != "Tanár" && felado.Szerepkor != "Igazgató" && felado.Szerepkor != "Adminisztrátor")
            {
                System.Windows.MessageBox.Show("❌ Nincs jogosultságod közlemény küldésére!", "Hiba",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            foreach (var felhasznalo in felhasznalok)
            {
                Kozlemeny uj = new Kozlemeny(felado.Felhasznalonev, cim, tartalom, DateTime.Now, felhasznalo.Felhasznalonev);
                kozlemenyek.Add(uj);
            }

            FrissitFajl();
        }

        public void KozlemenyekListazasa(Felhasznalo felhasznalo)
        {
            Console.WriteLine("\n=== KÖZLEMÉNYEK ===");

            var felhasznaloKozlemenyei = kozlemenyek
                .Where(k => k.CelzettFelhasznalonev == felhasznalo.Felhasznalonev || k.CelzettFelhasznalonev == "MINDENKI")
                .OrderByDescending(k => k.Datum)
                .ToList();

            if (!felhasznaloKozlemenyei.Any())
            {
                Console.WriteLine("Nincsenek közlemények.");
                return;
            }

            foreach (var kozlemeny in felhasznaloKozlemenyei)
            {
                Console.WriteLine(kozlemeny);
                Console.WriteLine(new string('-', 40));
            }
        }

        private void BetoltFajlbol()
        {
            if (!File.Exists(fajlEleres)) return;

            foreach (var sor in File.ReadAllLines(fajlEleres))
            {
                if (string.IsNullOrWhiteSpace(sor)) continue;
                kozlemenyek.Add(Kozlemeny.FromFileFormat(sor));
            }
        }

        // New helper used by WPF UI
        public List<Kozlemeny> GetKozlemenyekByUser(string felhasznalonev)
        {
            return kozlemenyek
                .Where(k => k.CelzettFelhasznalonev == felhasznalonev || k.CelzettFelhasznalonev == "MINDENKI")
                .OrderByDescending(k => k.Datum)
                .ToList();
        }

        private void FrissitFajl()
        {
            File.WriteAllLines(fajlEleres, kozlemenyek.Select(k => k.ToFileFormat()));
        }
    }

    internal class Kozlemeny
    {
        public string FeladoFelhasznalonev { get; set; }
        public string Cim { get; set; }
        public string Tartalom { get; set; }
        public DateTime Datum { get; set; }
        public string CelzettFelhasznalonev { get; set; }

        public Kozlemeny(string felado, string cim, string tartalom, DateTime datum, string celzettFelhasznalonev = "MINDENKI")
        {
            FeladoFelhasznalonev = felado;
            Cim = cim;
            Tartalom = tartalom;
            Datum = datum;
            CelzettFelhasznalonev = celzettFelhasznalonev;
        }

        public override string ToString()
        {
            return $"{Cim}\n{Tartalom}\n\nFeladó: {FeladoFelhasznalonev} - {Datum:yyyy.MM.dd HH:mm}";
        }

        public string ToFileFormat()
        {
            return $"{FeladoFelhasznalonev};{Cim};{Tartalom};{Datum:yyyy.MM.dd HH:mm};{CelzettFelhasznalonev}";
        }

        public static Kozlemeny FromFileFormat(string sor)
        {
            var adatok = sor.Split(';');
            var kozlemeny = new Kozlemeny(adatok[0], adatok[1], adatok[2], DateTime.Parse(adatok[3]));
            if (adatok.Length > 4)
            {
                kozlemeny.CelzettFelhasznalonev = adatok[4];
            }
            return kozlemeny;
        }


    }

}