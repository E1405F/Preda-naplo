using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfApp1.Models
{
    internal class Felhasznalo
    {
        public string Felhasznalonev { get; protected set; }
        private string jelszo;
        public string Nev { get; set; }
        public string IskolaNev { get; protected set; }
        public string Szerepkor { get; protected set; }
        public string Osztaly { get; protected set; }
        public bool Letiltva { get; set; }
        private int hibasProbalkozasok;

        protected string GetJelszo() => jelszo;

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

        public virtual string ToFileFormat()
        {
            return $"{Felhasznalonev};{jelszo};{Nev};{IskolaNev};{Szerepkor};{Osztaly}";
        }

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

        public static List<Felhasznalo> BetoltFajlbol(string fajl)
        {
            List<Felhasznalo> lista = new List<Felhasznalo>();

            if (!File.Exists(fajl))
            {
                System.Diagnostics.Debug.WriteLine($"Fájl nem található: {fajl}");
                return lista;
            }

            foreach (var sor in File.ReadAllLines(fajl))
            {
                if (string.IsNullOrWhiteSpace(sor)) continue;
                var adatok = sor.Split(';');

                System.Diagnostics.Debug.WriteLine($"Feldolgozás: {sor}");
                System.Diagnostics.Debug.WriteLine($"Mezők száma: {adatok.Length}");

                if (adatok.Length >= 5)
                {
                    string osztaly = adatok.Length > 5 ? adatok[5] : "";
                    List<string> tantargyakList = new List<string>();        // átnevezve
                    List<string> tanitottOsztalyokList = new List<string>(); // tanított osztályok

                    // DEBUG: Kiírjuk a szerepkört
                    System.Diagnostics.Debug.WriteLine($"Szerepkör: {adatok[4]}");

                    // Ha tanár vagy diák, és van tantárgy mező
                    if ((adatok[4] == "Tanár" || adatok[4] == "Diák") && adatok.Length > 6 && !string.IsNullOrEmpty(adatok[6]))
                    {
                        tantargyakList = adatok[6].Split(',').ToList();
                        System.Diagnostics.Debug.WriteLine($"Tantárgyak betöltve: {string.Join(", ", tantargyakList)}");
                    }

                    // Tanár esetén a tanított osztályok betöltése (6. mező, ha van)
                    if (adatok[4] == "Tanár" && adatok.Length > 5 && !string.IsNullOrEmpty(adatok[5]))
                    {
                        // A 6. mező a tanított osztályok listája (mert a 0-4 után az 5. index a 6. mező)
                        // Figyelem: adatok[5] a 6. mező (0-index). De mivel a tanár esetén a sorban:
                        // 0: felhasználónév, 1: jelszó, 2: név, 3: iskola, 4: szerep, 5: tanított osztályok, 6: tantárgyak
                        string tanitottOsztalyokString = adatok[5];
                        if (!string.IsNullOrEmpty(tanitottOsztalyokString))
                        {
                            tanitottOsztalyokList = tanitottOsztalyokString.Split(',').ToList();
                            System.Diagnostics.Debug.WriteLine($"Tanított osztályok betöltve: {string.Join(", ", tanitottOsztalyokList)}");
                        }
                    }

                    Felhasznalo felhasznalo;
                    switch (adatok[4])
                    {
                        case "Tanár":
                            // Tanár konstruktor: felhasználónév, jelszó, név, iskola, osztály, tantárgyak, tanított osztályok
                            felhasznalo = new Tanar(adatok[0], adatok[1], adatok[2], adatok[3], osztaly, tantargyakList, tanitottOsztalyokList);
                            break;
                        case "Diák":
                            // Diák konstruktor: felhasználónév, jelszó, név, iskola, osztály, tantárgyak
                            felhasznalo = new Diak(adatok[0], adatok[1], adatok[2], adatok[3], osztaly, tantargyakList);
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
    }
}