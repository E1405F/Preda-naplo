using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Préda_naplo
{
    internal class KozlemenyManager
    {
        private List<Kozlemeny> kozlemenyek;
        private string fajlEleres;

        public KozlemenyManager(string fajlEleres)
        {
            this.fajlEleres = fajlEleres;
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

            Kozlemeny uj = new Kozlemeny(felado.Felhasznalonev, cim, tartalom, DateTime.Now);
            kozlemenyek.Add(uj);
            File.AppendAllText(fajlEleres, uj.ToFileFormat() + Environment.NewLine);

            Console.WriteLine("✅ Közlemény sikeresen közzétéve!");
        }

        public void KozlemenyekListazasa(Felhasznalo felhasznalo)
        {
            Console.WriteLine("\n=== KÖZLEMÉNYEK ===");

            if (!kozlemenyek.Any())
            {
                Console.WriteLine("Nincsenek közlemények.");
                return;
            }

            foreach (var kozlemeny in kozlemenyek.OrderByDescending(k => k.Datum))
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
    }

    internal class Kozlemeny
    {
        public string FeladoFelhasznalonev { get; set; }
        public string Cim { get; set; }
        public string Tartalom { get; set; }
        public DateTime Datum { get; set; }

        public Kozlemeny(string felado, string cim, string tartalom, DateTime datum)
        {
            FeladoFelhasznalonev = felado;
            Cim = cim;
            Tartalom = tartalom;
            Datum = datum;
        }

        public override string ToString()
        {
            return $"{Cim}\n{Tartalom}\n\nFeladó: {FeladoFelhasznalonev} - {Datum:yyyy.MM.dd HH:mm}";
        }

        public string ToFileFormat()
        {
            return $"{FeladoFelhasznalonev};{Cim};{Tartalom};{Datum:yyyy.MM.dd HH:mm}";
        }

        public static Kozlemeny FromFileFormat(string sor)
        {
            var adatok = sor.Split(';');
            return new Kozlemeny(adatok[0], adatok[1], adatok[2], DateTime.Parse(adatok[3]));
        }
    }
}