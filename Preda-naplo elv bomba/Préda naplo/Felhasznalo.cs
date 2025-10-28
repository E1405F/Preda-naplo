using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Préda_naplo
{
    internal class Felhasznalo
    {
        public string Felhasznalonev { get; private set; }
        private string jelszo;
        public string Nev { get; private set; }
        public string IskolaNev { get; private set; }
        public string Szerepkor { get; private set; }
        public bool Letiltva { get; private set; }
        private int hibasProbalkozasok;

        public Felhasznalo(string felhasznalonev, string jelszo, string nev, string iskolaNev, string szerepkor)
        {
            Felhasznalonev = felhasznalonev;
            this.jelszo = jelszo;
            Nev = nev;
            IskolaNev = iskolaNev;
            Szerepkor = szerepkor;
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
            return $"{Nev} ({Felhasznalonev}) - {Szerepkor} @ {IskolaNev} - {(Letiltva ? "Letiltva" : "Aktív")}";
        }

        public string ToFileFormat()
        {
            
            return $"{Felhasznalonev};{jelszo};{Nev};{IskolaNev};{Szerepkor}";
        }

        public static List<Felhasznalo> BetoltFajlbol(string fajl)
        {
            List<Felhasznalo> lista = new List<Felhasznalo>();

            foreach (var sor in File.ReadAllLines(fajl))
            {
                if (string.IsNullOrWhiteSpace(sor)) continue;
                var adatok = sor.Split(';');

               
                if (adatok.Length == 5)
                {
                    lista.Add(new Felhasznalo(adatok[0], adatok[1], adatok[2], adatok[3], adatok[4]));
                }
                else if (adatok.Length == 3)
                {
                    lista.Add(new Felhasznalo(adatok[0], adatok[1], adatok[2], "Ismeretlen iskola", "Ismeretlen szerepkör"));
                }
            }

            return lista;
        }
    }
}
