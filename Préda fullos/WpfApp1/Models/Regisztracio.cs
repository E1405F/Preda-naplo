using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfApp1.Models
{
    internal class Regisztracio
    {
        private List<Felhasznalo> felhasznalok;
        private string fajlEleres;
        private Dictionary<string, string> szuloGyerekKapcsolat;
        private string szuloGyerekFajl;
        private string titkosKod;

        public Regisztracio(List<Felhasznalo> felhasznalok, string fajlEleres, Dictionary<string, string> szuloGyerekKapcsolat, string szuloGyerekFajl, string titkosKod)
        {
            this.felhasznalok = felhasznalok;
            this.fajlEleres = fajlEleres;
            this.szuloGyerekKapcsolat = szuloGyerekKapcsolat;
            this.szuloGyerekFajl = szuloGyerekFajl;
            this.titkosKod = titkosKod;
        }

        // ========== WPF VERZIÓ (9 paraméter) ==========
        public bool UjFelhasznalo(string felhasznalonev, string jelszo, string nev, string iskolaNev,
            string szerepkor, string osztaly, string gyerekFelhasznalonev,
            List<string> tantargyak = null, List<string> tanitottOsztalyok = null)
        {
            if (felhasznalok.Any(f => f.Felhasznalonev == felhasznalonev))
            {
                return false;
            }

            Felhasznalo uj;

            switch (szerepkor)
            {
                case "Diák":
                    var diak = new Diak(felhasznalonev, jelszo, nev, iskolaNev, osztaly);
                    if (tantargyak != null)
                        diak.Tantargyak = tantargyak;
                    uj = diak;
                    break;
                case "Tanár":
                    var tanar = new Tanar(felhasznalonev, jelszo, nev, iskolaNev, osztaly);
                    if (tantargyak != null)
                        tanar.Tantargyak = tantargyak;
                    if (tanitottOsztalyok != null)
                        tanar.TanitottOsztalyok = tanitottOsztalyok;
                    uj = tanar;
                    break;
                case "Szülő":
                    uj = new Szulo(felhasznalonev, jelszo, nev, iskolaNev);
                    break;
                case "Igazgató":
                    uj = new Igazgato(felhasznalonev, jelszo, nev, iskolaNev);
                    break;
                case "Adminisztrátor":
                    uj = new Admin(felhasznalonev, jelszo, nev, iskolaNev);
                    break;
                default:
                    uj = new Felhasznalo(felhasznalonev, jelszo, nev, iskolaNev, szerepkor, osztaly);
                    break;
            }

            felhasznalok.Add(uj);

            // Fájlba mentés
            if ((szerepkor == "Tanár" || szerepkor == "Diák") && tantargyak != null)
            {
                string tantargyakString = string.Join(",", tantargyak);
                string hatodikMezo;
                if (szerepkor == "Tanár")
                {
                    hatodikMezo = string.Join(",", tanitottOsztalyok ?? new List<string>());
                }
                else // Diák
                {
                    hatodikMezo = osztaly; // a diák osztálya (pl. "10.A")
                }
                File.AppendAllText(fajlEleres, $"{felhasznalonev};{jelszo};{nev};{iskolaNev};{szerepkor};{hatodikMezo};{tantargyakString}" + Environment.NewLine);
            }
            else
            {
                File.AppendAllText(fajlEleres, uj.ToFileFormat() + Environment.NewLine);
            }

            if (szerepkor == "Szülő" && !string.IsNullOrEmpty(gyerekFelhasznalonev))
            {
                szuloGyerekKapcsolat[felhasznalonev] = gyerekFelhasznalonev;
                File.AppendAllText(szuloGyerekFajl, $"{felhasznalonev};{gyerekFelhasznalonev}" + Environment.NewLine);
            }

            return true;
        }

        // ========== KONZOLOS VERZIÓ (a régi menük miatt) ==========
        public void UjFelhasznalo()
        {
            Console.WriteLine("\n=== ÚJ FELHASZNÁLÓ REGISZTRÁLÁSA ===");

            string felh = BeolvasKotelezoMezo("Felhasználónév: ");
            if (felhasznalok.Any(f => f.Felhasznalonev == felh))
            {
                Console.WriteLine("❌ Ez a felhasználónév már létezik!");
                return;
            }

            string jelszo = BeolvasKotelezoMezo("Jelszó: ", true);
            string nev = BeolvasKotelezoMezo("Teljes név: ");
            string iskola = BeolvasKotelezoMezo("Iskola neve: ");

            string osztaly = "";
            string szerep = "";
            List<string> valasztottTantargyak = new List<string>();        // átnevezve
            List<string> valasztottTanitottOsztalyok = new List<string>(); // átnevezve
            bool helyes = false;

            do
            {
                Console.WriteLine("\nVálassz szerepkört:");
                Console.WriteLine("1 - Diák");
                Console.WriteLine("2 - Tanár");
                Console.WriteLine("3 - Szülő");
                Console.WriteLine("4 - Igazgató");
                Console.WriteLine("5 - Adminisztrátor");
                Console.Write("Szerepkör száma: ");
                string szerepValasz = Console.ReadLine();

                switch (szerepValasz)
                {
                    case "1":
                        szerep = "Diák";
                        osztaly = BeolvasOsztaly();
                        valasztottTantargyak = BeolvasTantargyak("diák");
                        helyes = true;
                        UjDiakSzuloKapcsolatLetrehozasa(felh);
                        break;
                    case "2":
                        szerep = "Tanár";
                        if (!EllenorizTitkosKod())
                            return;
                        osztaly = BeolvasOsztalyOpcionalis();
                        valasztottTantargyak = BeolvasTantargyak("tanár");
                        valasztottTanitottOsztalyok = BeolvasTanitottOsztalyok();
                        helyes = true;
                        break;
                    case "3":
                        szerep = "Szülő";
                        helyes = true;
                        break;
                    case "4":
                        szerep = "Igazgató";
                        if (!EllenorizTitkosKod())
                            return;
                        helyes = true;
                        break;
                    case "5":
                        szerep = "Adminisztrátor";
                        if (!EllenorizTitkosKod())
                            return;
                        helyes = true;
                        break;
                    default:
                        Console.WriteLine("❌ Érvénytelen választás, próbáld újra!");
                        break;
                }
            } while (!helyes);

            Felhasznalo uj;
            string gyerekFelhasznalonev = "";

            switch (szerep)
            {
                case "Diák":
                    var diak = new Diak(felh, jelszo, nev, iskola, osztaly);
                    diak.Tantargyak = valasztottTantargyak;
                    uj = diak;
                    break;
                case "Tanár":
                    var tanar = new Tanar(felh, jelszo, nev, iskola, osztaly);
                    tanar.Tantargyak = valasztottTantargyak;
                    tanar.TanitottOsztalyok = valasztottTanitottOsztalyok;
                    uj = tanar;
                    break;
                case "Szülő":
                    gyerekFelhasznalonev = BeolvasGyerekFelhasznalonev();
                    if (string.IsNullOrEmpty(gyerekFelhasznalonev))
                        return;
                    uj = new Szulo(felh, jelszo, nev, iskola);
                    break;
                case "Igazgató":
                    uj = new Igazgato(felh, jelszo, nev, iskola);
                    break;
                case "Adminisztrátor":
                    uj = new Admin(felh, jelszo, nev, iskola);
                    break;
                default:
                    uj = new Felhasznalo(felh, jelszo, nev, iskola, szerep, osztaly);
                    break;
            }

            felhasznalok.Add(uj);

            // Mentés a fájlba
            if ((szerep == "Tanár" || szerep == "Diák") && valasztottTantargyak.Any())
            {
                string tantargyakString = string.Join(",", valasztottTantargyak);
                string osztalyokString = string.Join(",", valasztottTanitottOsztalyok);
                File.AppendAllText(fajlEleres, $"{felh};{jelszo};{nev};{iskola};{szerep};{osztalyokString};{tantargyakString}" + Environment.NewLine);
            }
            else
            {
                File.AppendAllText(fajlEleres, uj.ToFileFormat() + Environment.NewLine);
            }

            if (szerep == "Szülő" && !string.IsNullOrEmpty(gyerekFelhasznalonev))
            {
                szuloGyerekKapcsolat[felh] = gyerekFelhasznalonev;
                File.AppendAllText(szuloGyerekFajl, $"{felh};{gyerekFelhasznalonev}" + Environment.NewLine);
            }

            Console.WriteLine($"\n✅ Sikeres regisztráció, üdvözlünk {nev} ({szerep}) a {iskola} iskolából!" +
                (string.IsNullOrEmpty(osztaly) ? "" : $" Osztály: {osztaly}"));
            if ((szerep == "Tanár" || szerep == "Diák") && valasztottTantargyak.Any())
            {
                Console.WriteLine($"Tantárgyak: {string.Join(", ", valasztottTantargyak)}");
            }
            if (szerep == "Tanár" && valasztottTanitottOsztalyok.Any())
            {
                Console.WriteLine($"Tanított osztályok: {string.Join(", ", valasztottTanitottOsztalyok)}");
            }

            Console.WriteLine("Nyomj ENTER-t a visszalépéshez...");
            Console.ReadLine();
        }

        // ---------- Segédmetódusok (változatlanok) ----------
        private List<string> BeolvasTantargyak(string szerepkor)
        {
            List<string> osszesTantargy = new List<string>
            {
                "Matematika",
                "Magyar nyelv és irodalom",
                "Állampolgári ismeretek",
                "Történelem",
                "Angol",
                "Szakmai Angol",
                "Német",
                "Testnevelés",
                "Asztali alkalmazások fejlesztése",
                "IKT projektmunka",
                "Szoftver tesztelés",
                "Webprogramozás"
            };

            List<string> kivalasztott = new List<string>();
            Console.WriteLine($"\n=== TANTÁRGYAK KIVÁLASZTÁSA ({szerepkor.ToUpper()}) ===");
            Console.WriteLine("Válaszd ki a tantárgyakat (0 végzés):");

            for (int i = 0; i < osszesTantargy.Count; i++)
                Console.WriteLine($"[{i + 1}] {osszesTantargy[i]}");

            while (true)
            {
                Console.Write("\nVálasztás száma (0 a befejezéshez): ");
                if (int.TryParse(Console.ReadLine(), out int valasztas))
                {
                    if (valasztas == 0) break;
                    if (valasztas >= 1 && valasztas <= osszesTantargy.Count)
                    {
                        string valasztott = osszesTantargy[valasztas - 1];
                        if (!kivalasztott.Contains(valasztott))
                        {
                            kivalasztott.Add(valasztott);
                            Console.WriteLine($"✅ {valasztott} hozzáadva");
                        }
                        else
                            Console.WriteLine($"⚠️ {valasztott} már hozzá lett adva");
                    }
                    else
                        Console.WriteLine("❌ Érvénytelen választás!");
                }
            }
            return kivalasztott;
        }

        private List<string> BeolvasTanitottOsztalyok()
        {
            List<string> osszesOsztaly = new List<string> { "9.A", "9.B", "10.A", "10.B", "11.A", "11.B", "12.EC" };
            List<string> kivalasztott = new List<string>();
            Console.WriteLine("\n=== TANÍTOTT OSZTÁLYOK KIVÁLASZTÁSA ===");
            Console.WriteLine("Válaszd ki az osztályokat (0 végzés):");
            for (int i = 0; i < osszesOsztaly.Count; i++)
                Console.WriteLine($"[{i + 1}] {osszesOsztaly[i]}");

            while (true)
            {
                Console.Write("\nVálasztás száma (0 a befejezéshez): ");
                if (int.TryParse(Console.ReadLine(), out int valasztas))
                {
                    if (valasztas == 0) break;
                    if (valasztas >= 1 && valasztas <= osszesOsztaly.Count)
                    {
                        string valasztott = osszesOsztaly[valasztas - 1];
                        if (!kivalasztott.Contains(valasztott))
                        {
                            kivalasztott.Add(valasztott);
                            Console.WriteLine($"✅ {valasztott} hozzáadva");
                        }
                        else
                            Console.WriteLine($"⚠️ {valasztott} már hozzá lett adva");
                    }
                    else
                        Console.WriteLine("❌ Érvénytelen választás!");
                }
            }
            return kivalasztott;
        }

        private string BeolvasKotelezoMezo(string uzenet, bool jelszo = false)
        {
            string ertek;
            do
            {
                Console.Write(uzenet);
                if (jelszo)
                    ertek = ReadPassword();
                else
                    ertek = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(ertek))
                    Console.WriteLine("❌ Ez a mező kötelező! Adj meg egy értéket.");
            } while (string.IsNullOrWhiteSpace(ertek));
            return ertek;
        }

        private string BeolvasOsztaly()
        {
            string osztaly;
            do
            {
                Console.Write("Osztály (pl. 8.A, 12.B): ");
                osztaly = Console.ReadLine();
                if (!EllenorizOsztalyFormatum(osztaly))
                    Console.WriteLine("❌ Érvénytelen osztály formátum! Használd: 8.A, 9.B, 12.C");
            } while (!EllenorizOsztalyFormatum(osztaly));
            return osztaly;
        }

        private string BeolvasOsztalyOpcionalis()
        {
            Console.Write("Osztály (opcionális, pl. 10.A osztályfőnök): ");
            string osztaly = Console.ReadLine();
            if (!string.IsNullOrEmpty(osztaly) && !EllenorizOsztalyFormatum(osztaly))
            {
                Console.WriteLine("❌ Érvénytelen osztály formátum!");
                return BeolvasOsztalyOpcionalis();
            }
            return osztaly;
        }

        private bool EllenorizOsztalyFormatum(string osztaly)
        {
            if (string.IsNullOrWhiteSpace(osztaly)) return false;
            if (osztaly.Length < 3 || osztaly.Length > 4) return false;
            if (!osztaly.Contains('.')) return false;
            var reszek = osztaly.Split('.');
            if (reszek.Length != 2) return false;
            if (!int.TryParse(reszek[0], out int evfolyam)) return false;
            if (evfolyam < 8 || evfolyam > 12) return false;
            if (reszek[1].Length != 1 || !char.IsLetter(reszek[1][0])) return false;
            return true;
        }

        private bool EllenorizTitkosKod()
        {
            Console.Write("Add meg a titkos kódot: ");
            string kod = ReadPassword();
            if (kod != titkosKod)
            {
                Console.WriteLine("❌ Hibás titkos kód! Regisztráció megszakítva.");
                return false;
            }
            return true;
        }

        private string BeolvasGyerekFelhasznalonev()
        {
            Console.WriteLine("\n=== GYERMEK HOZZÁRENDELÉSE ===");
            var diakok = felhasznalok.Where(f => f.Szerepkor == "Diák").ToList();
            if (!diakok.Any())
            {
                Console.WriteLine("❌ Nincsenek regisztrált diákok! Először a gyermeket kell regisztrálni.");
                return null;
            }
            Console.WriteLine("\nElérhető diákok:");
            for (int i = 0; i < diakok.Count; i++)
            {
                var diak = diakok[i];
                Console.WriteLine($"[{i + 1}] {diak.Nev} ({diak.Felhasznalonev}) - {diak.Osztaly}");
            }
            Console.Write("\nVálassz diákot a sorszámával: ");
            if (int.TryParse(Console.ReadLine(), out int valasztas) && valasztas > 0 && valasztas <= diakok.Count)
            {
                var kivalasztottDiak = diakok[valasztas - 1];
                if (szuloGyerekKapcsolat.ContainsValue(kivalasztottDiak.Felhasznalonev))
                {
                    var meglovoSzulo = szuloGyerekKapcsolat.FirstOrDefault(x => x.Value == kivalasztottDiak.Felhasznalonev).Key;
                    Console.WriteLine($"⚠️ Ez a diák már hozzá van rendelve egy szülőhöz: {meglovoSzulo}");
                    Console.Write("Biztosan át szeretnéd írni a kapcsolatot? (i/n): ");
                    string valasz = Console.ReadLine().ToLower();
                    if (valasz != "i") return null;
                    var regiKapcsolat = szuloGyerekKapcsolat.FirstOrDefault(x => x.Value == kivalasztottDiak.Felhasznalonev);
                    if (!string.IsNullOrEmpty(regiKapcsolat.Key))
                        szuloGyerekKapcsolat.Remove(regiKapcsolat.Key);
                }
                return kivalasztottDiak.Felhasznalonev;
            }
            else
            {
                Console.WriteLine("❌ Érvénytelen választás!");
                return null;
            }
        }

        private string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return password;
        }

        private void UjDiakSzuloKapcsolatLetrehozasa(string diakFelhasznalonev)
        {
            var szulok = felhasznalok.Where(f => f.Szerepkor == "Szülő").ToList();
            if (szulok.Any())
            {
                Console.WriteLine("\n💡 Van regisztrált szülő. Szeretnéd hozzárendelni ehhez a diákhoz?");
                Console.Write("Szülő felhasználóneve (üresen hagyva, ha nem): ");
                string szuloFelh = Console.ReadLine();
                if (!string.IsNullOrEmpty(szuloFelh))
                {
                    var szulo = szulok.FirstOrDefault(f => f.Felhasznalonev == szuloFelh);
                    if (szulo != null)
                    {
                        szuloGyerekKapcsolat[szuloFelh] = diakFelhasznalonev;
                        File.AppendAllText(szuloGyerekFajl, $"{szuloFelh};{diakFelhasznalonev}" + Environment.NewLine);
                        Console.WriteLine($"✅ {szulo.Nev} szülő hozzárendelve a diákhoz!");
                    }
                    else
                        Console.WriteLine("❌ Nem található ilyen szülő!");
                }
            }
        }
    }
}