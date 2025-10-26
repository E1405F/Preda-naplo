using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Préda_naplo
{
    internal class Felhasznalo
    {
        public string Felhasznalonev { get; private set; }
        private string jelszo;
        public string Nev { get; private set; }
        public bool Letiltva { get; private set; }
        private int hibasProbalkozasok;

        public Felhasznalo(string felhasznalonev, string jelszo, string nev)
        {
            Felhasznalonev = felhasznalonev;
            this.jelszo = jelszo;
            Nev = nev;
            hibasProbalkozasok = 0;
            Letiltva = false;
        }

        public bool EllenorizJelszo(string megadott)
        {
            if (Letiltva) return false;

            if (megadott == jelszo)
            {
                hibasProbalkozasok = 0;
                return true;
            }
            else
            {
                hibasProbalkozasok++;
                if (hibasProbalkozasok >= 3)
                {
                    Letiltva = true;
                    Console.WriteLine("⚠️ A fiók letiltásra került 3 hibás próbálkozás után.");
                }
                return false;
            }
        }

        public override string ToString()
        {
            return $"{Nev} ({Felhasznalonev}) - {(Letiltva ? "Letiltva" : "Aktív")}";
        }
    }
}
