using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Préda_naplo
{
    internal class Felhasznalo
    {
        public string Felhasznalonev { get; protected set; }
        private string jelszo;
        public string Nev { get; set; }  // Legyen public set
        public string IskolaNev { get; protected set; }
        public string Szerepkor { get; protected set; }
        public string Osztaly { get; protected set; }
        public bool Letiltva { get; set; }  // Legyen public set
        private int hibasProbalkozasok;


        public Felhasznalo(string felhasznalonev, string jelszo, string nev, string iskolaNev, string szerepkor, string osztaly = "")
        {
            Felhasznalonev = felhasznalonev;
            this.jelszo = jelszo;
            Nev = nev;
            IskolaNev = iskolaNev;
            Szerepkor = szerepkor;
            Osztaly = osztaly;
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

        public virtual void FoMenu(JegyManager jegyManager, KozlemenyManager kozlemenyManager, HianyzasManager hianyzasManager)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"FŐMENÜ - {Nev} ({Szerepkor})");
                if (!string.IsNullOrEmpty(Osztaly))
                {
                    Console.WriteLine($"Osztály: {Osztaly}");
                }
                Console.WriteLine("[1] Jegyek megtekintése");
                Console.WriteLine("[0] Kijelentkezés");
                Console.Write("\nVálasztás: ");
                string valasztas = Console.ReadLine();

                if (valasztas == "1")
                {
                    jegyManager.JegyekListazasa(this);
                    Console.WriteLine("\nNyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "0")
                {
                    Console.WriteLine("Kijelentkezés...");
                    break;
                }
                else
                {
                    Console.WriteLine("Érvénytelen választás!");
                    Console.ReadLine();
                }
            }
        }

        public override string ToString()
        {
            return $"{Nev} ({Felhasznalonev}) - {Szerepkor} @ {IskolaNev} - {(Letiltva ? "Letiltva" : "Aktív")}";
        }

        public string ToFileFormat()
        {
            return $"{Felhasznalonev};{jelszo};{Nev};{IskolaNev};{Szerepkor};{Osztaly}";
        }

        public static List<Felhasznalo> BetoltFajlbol(string fajl)
        {
            List<Felhasznalo> lista = new List<Felhasznalo>();

            foreach (var sor in File.ReadAllLines(fajl))
            {
                if (string.IsNullOrWhiteSpace(sor)) continue;
                var adatok = sor.Split(';');

                if (adatok.Length >= 5)
                {
                    string osztaly = adatok.Length > 5 ? adatok[5] : "";

                    Felhasznalo felhasznalo;
                    switch (adatok[4])
                    {
                        case "Tanár":
                            felhasznalo = new Tanar(adatok[0], adatok[1], adatok[2], adatok[3], osztaly);
                            break;
                        case "Diák":
                            felhasznalo = new Diak(adatok[0], adatok[1], adatok[2], adatok[3], osztaly);
                            break;
                        case "Szülő":
                            felhasznalo = new Szulo(adatok[0], adatok[1], adatok[2], adatok[3]);
                            break;
                        case "Igazgató":
                            felhasznalo = new Igazgato(adatok[0], adatok[1], adatok[2], adatok[3]);
                            break;
                        case "Adminisztrátor":
                            felhasznalo = new Admin(adatok[0], adatok[1], adatok[2], adatok[3]);
                            break;
                        default:
                            felhasznalo = new Felhasznalo(adatok[0], adatok[1], adatok[2], adatok[3], adatok[4], osztaly);
                            break;
                    }
                    lista.Add(felhasznalo);
                }
            }

            return lista;
        }
        // Add hozzá a Felhasznalo osztályhoz:

        public virtual bool JelszoMegvaltoztatasa(string ujJelszo)
        {
            if (string.IsNullOrWhiteSpace(ujJelszo))
            {
                Console.WriteLine("❌ A jelszó nem lehet üres!");
                return false;
            }

            this.jelszo = ujJelszo;
            return true;
        }
    }
}