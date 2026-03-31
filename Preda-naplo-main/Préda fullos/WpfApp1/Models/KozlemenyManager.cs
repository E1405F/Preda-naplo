using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        // Konzolos verzió (a régi menük miatt)
        public void UjKozlemeny(Felhasznalo felado)
        {
            Console.WriteLine("\n=== ÚJ KÖZLEMÉNY ===");
            Console.Write("Cím: ");
            string cim = Console.ReadLine();
            Console.Write("Tartalom: ");
            string tartalom = Console.ReadLine();
            UjKozlemeny(felado, cim, tartalom, "MINDENKI");
        }

        // WPF verzió – egy közleményt mentünk, nem felhasználónként
        public void UjKozlemeny(Felhasznalo felado, string cim, string tartalom, string celcsoport = "MINDENKI")
        {
            if (felado.Szerepkor != "Tanár" && felado.Szerepkor != "Igazgató" && felado.Szerepkor != "Adminisztrátor")
            {
                System.Windows.MessageBox.Show("❌ Nincs jogosultságod közlemény küldésére!", "Hiba",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            Kozlemeny uj = new Kozlemeny(felado.Felhasznalonev, cim, tartalom, DateTime.Now, celcsoport);
            kozlemenyek.Add(uj);
            FrissitFajl();
        }

        // Visszaadja a felhasználónak szánt közleményeket (célcsoport alapján)
        public List<Kozlemeny> GetKozlemenyekByUser(Felhasznalo user)
        {
            return kozlemenyek
                .Where(k => CelcsoportbaTartozik(user, k.CelzettFelhasznalonev))
                .OrderByDescending(k => k.Datum)
                .ToList();
        }

        // Megjelenítés a konzolban (a régi menük miatt)
        public void KozlemenyekListazasa(Felhasznalo felhasznalo)
        {
            Console.WriteLine("\n=== KÖZLEMÉNYEK ===");

            var felhasznaloKozlemenyei = GetKozlemenyekByUser(felhasznalo);

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
        
        // Segédmetódus: eldönti, hogy a felhasználó tartozik-e a célcsoportba
        private bool CelcsoportbaTartozik(Felhasznalo user, string celcsoport)
        {
            if (celcsoport == "MINDENKI") return true;
            if (celcsoport == "DIAKOK") return user.Szerepkor == "Diák";
            if (celcsoport == "TANAROK") return user.Szerepkor == "Tanár";
            if (celcsoport == "IGAZGATO") return user.Szerepkor == "Igazgató";
            if (celcsoport == "ADMIN") return user.Szerepkor == "Adminisztrátor";
            if (celcsoport.StartsWith("OSZTALY:"))
            {
                string osztaly = celcsoport.Substring(8);
                return user.Szerepkor == "Diák" && user.Osztaly == osztaly;
            }
            // Régi formátum: konkrét felhasználónév
            return user.Felhasznalonev == celcsoport;
        }

        // Fájl betöltése
        private void BetoltFajlbol()
        {
            if (!File.Exists(fajlEleres)) return;

            foreach (var sor in File.ReadAllLines(fajlEleres))
            {
                if (string.IsNullOrWhiteSpace(sor)) continue;
                kozlemenyek.Add(Kozlemeny.FromFileFormat(sor));
            }
        }

        // Fájl frissítése (felülírás)
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
        public string CelzettFelhasznalonev { get; set; }  // Most a célcsoport kódját tárolja

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