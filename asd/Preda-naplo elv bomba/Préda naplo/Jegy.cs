using System;

namespace Préda_naplo
{
    internal class Jegy
    {
        public string DiakFelhasznalonev { get; set; }
        public string TanarFelhasznalonev { get; set; }
        public string Tantargy { get; set; }
        public int Ertek { get; set; }
        public DateTime Datum { get; set; }
        public string Megjegyzes { get; set; }

        public Jegy(string diakFelhasznalonev, string tanarFelhasznalonev, string tantargy, int ertek, DateTime datum, string megjegyzes = "")
        {
            DiakFelhasznalonev = diakFelhasznalonev;
            TanarFelhasznalonev = tanarFelhasznalonev;
            Tantargy = tantargy;
            Ertek = ertek;
            Datum = datum;
            Megjegyzes = megjegyzes;
        }

        public override string ToString()
        {
            return $"{Tantargy} - {Ertek} ({Datum:yyyy.MM.dd}) - {TanarFelhasznalonev}" +
                   (string.IsNullOrEmpty(Megjegyzes) ? "" : $" - {Megjegyzes}");
        }

        public string ToFileFormat()
        {
            return $"{DiakFelhasznalonev};{TanarFelhasznalonev};{Tantargy};{Ertek};{Datum:yyyy.MM.dd};{Megjegyzes}";
        }
    }
}