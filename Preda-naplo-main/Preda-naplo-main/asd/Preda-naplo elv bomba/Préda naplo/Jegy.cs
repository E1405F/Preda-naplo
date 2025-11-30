using System;

namespace Préda_naplo
{
    internal class Jegy
    {
        public string DiakFelhasznalonev { get; set; }
        public string TanarFelhasznalonev { get; set; }
        public string Tantargy { get; set; }
        public int Ertek { get; set; }
        public double Suly { get; set; }
        public DateTime Datum { get; set; }
        public string Megjegyzes { get; set; }
        public bool Fellebbezes { get; set; }
        public string FellebbezesIndoklas { get; set; }
        public bool FellebbezesElbiralva { get; set; }

        public Jegy(string diakFelhasznalonev, string tanarFelhasznalonev, string tantargy, int ertek,
                   DateTime datum, string megjegyzes = "", double suly = 1.0,
                   bool fellebbezes = false, string fellebbezesIndoklas = "", bool fellebbezesElbiralva = false)
        {
            DiakFelhasznalonev = diakFelhasznalonev;
            TanarFelhasznalonev = tanarFelhasznalonev;
            Tantargy = tantargy;
            Ertek = ertek;
            Suly = suly;
            Datum = datum;
            Megjegyzes = megjegyzes;
            Fellebbezes = fellebbezes;
            FellebbezesIndoklas = fellebbezesIndoklas;
            FellebbezesElbiralva = fellebbezesElbiralva;
        }

        public override string ToString()
        {
            string sulyInfo = Suly != 1.0 ? " (" + Suly * 100 + "%)" : "";
            string fellebbezesInfo = Fellebbezes ?
                (FellebbezesElbiralva ? " [Fellebbezés elbírálva]" : " [Fellebbezés benyújtva]") : "";

            return Tantargy + " - " + Ertek + sulyInfo + " (" + Datum.ToString("yyyy.MM.dd") + ") - " + TanarFelhasznalonev +
                   (string.IsNullOrEmpty(Megjegyzes) ? "" : " - " + Megjegyzes) + fellebbezesInfo;
        }

        public string ToFileFormat()
        {
            return DiakFelhasznalonev + ";" + TanarFelhasznalonev + ";" + Tantargy + ";" + Ertek + ";" + Datum.ToString("yyyy.MM.dd") + ";" + Megjegyzes + ";" + Suly + ";" + Fellebbezes + ";" + FellebbezesIndoklas + ";" + FellebbezesElbiralva;
        }
    }
}