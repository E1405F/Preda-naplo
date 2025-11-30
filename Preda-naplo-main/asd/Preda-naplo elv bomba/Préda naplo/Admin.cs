using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Préda_naplo
{
    internal class Admin : Felhasznalo
    {
        public Admin(string felhasznalonev, string jelszo, string nev, string iskolaNev)
            : base(felhasznalonev, jelszo, nev, iskolaNev, "Adminisztrátor", "") { }

        public override void FoMenu(JegyManager jegyManager, KozlemenyManager kozlemenyManager, HianyzasManager hianyzasManager)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"ADMIN MENÜ - {Nev}");
                Console.WriteLine("[1] Összes jegy megtekintése");
                Console.WriteLine("[2] Felhasználók kezelése");
                Console.WriteLine("[3] Közlemény küldése");
                Console.WriteLine("[0] Kijelentkezés");
                Console.Write("\nVálasztás: ");
                string valasztas = Console.ReadLine();

                if (valasztas == "1")
                {
                    jegyManager.JegyekListazasa(this);
                    Console.WriteLine("\nNyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "2")
                {
                    FelhasznalokKezelese(jegyManager.GetFelhasznalok());
                    Console.WriteLine("\nNyomj ENTER-t a folytatáshoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "3")
                {
                    kozlemenyManager.UjKozlemeny(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatáshoz...");
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

        private void FelhasznalokKezelese(List<Felhasznalo> felhasznalok)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\nFELHASZNÁLÓK KEZELÉSE");
                  
                if (!felhasznalok.Any())
                {
                    Console.WriteLine("Nincsenek felhasználók.");
                    Console.WriteLine("Nyomj ENTER-t a visszalépéshez...");
                    Console.ReadLine();
                    return;
                }

                for (int i = 0; i < felhasznalok.Count; i++)
                {
                    var felhasznalo = felhasznalok[i];
                    string status = felhasznalo.Letiltva ? "🚫 Letiltva" : "✅ Aktív";
                    string osztalyInfo = string.IsNullOrEmpty(felhasznalo.Osztaly) ? "" : $" - {felhasznalo.Osztaly}";

                    Console.WriteLine($"[{i + 1}] {felhasznalo.Nev} ({felhasznalo.Felhasznalonev}) - {felhasznalo.Szerepkor}{osztalyInfo} - {status}");
                }

                Console.Write("\nVálassz felhasználót (0 - vissza): ");
                if (int.TryParse(Console.ReadLine(), out int valasztas) && valasztas > 0 && valasztas <= felhasznalok.Count)
                {
                    var kivalasztott = felhasznalok[valasztas - 1];
                    if (!FelhasznaloMuveletek(kivalasztott, felhasznalok))
                    {
                        break; // Ha törölték a felhasználót, visszalépünk
                    }
                }
                else if (valasztas == 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("❌ Érvénytelen választás!");
                    Console.ReadLine();
                }
            }
        }

        private bool FelhasznaloMuveletek(Felhasznalo felhasznalo, List<Felhasznalo> felhasznalok)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\n=== {felhasznalo.Nev} MŰVELETEK ===");
                Console.WriteLine($"Felhasználónév: {felhasznalo.Felhasznalonev}");
                Console.WriteLine($"Név: {felhasznalo.Nev}");
                Console.WriteLine($"Szerepkör: {felhasznalo.Szerepkor}");
                Console.WriteLine($"Iskola: {felhasznalo.IskolaNev}");
                if (!string.IsNullOrEmpty(felhasznalo.Osztaly))
                    Console.WriteLine($"Osztály: {felhasznalo.Osztaly}");
                Console.WriteLine($"Státusz: {(felhasznalo.Letiltva ? "🚫 Letiltva" : "✅ Aktív")}");

                Console.WriteLine("\n[1] Letiltás/feloldás");
                Console.WriteLine("[2] Jelszó megváltoztatása");
                Console.WriteLine("[3] Név módosítása");
                Console.WriteLine("[4] Fiók törlése");
                Console.WriteLine("[0] Vissza a felhasználók listájához");
                Console.Write("Választás: ");

                string valasztas = Console.ReadLine();
                switch (valasztas)
                {
                    case "1":
                        FelhasznaloLetiltas(felhasznalo);
                        break;
                    case "2":
                        JelszoMegvaltoztatasa(felhasznalo);
                        break;
                    case "3":
                        NevModositasa(felhasznalo);
                        break;
                    case "4":
                        if (FiokTorlese(felhasznalo, felhasznalok))
                            return false; // Visszalépünk a listába, mert töröltünk
                        break;
                    case "0":
                        return true; // Vissza a listához
                    default:
                        Console.WriteLine("❌ Érvénytelen választás!");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void FelhasznaloLetiltas(Felhasznalo felhasznalo)
        {
            if (felhasznalo.Felhasznalonev == this.Felhasznalonev)
            {
                Console.WriteLine("❌ Nem tilthatod le a saját fiókodat!");
                Console.ReadLine();
                return;
            }

            felhasznalo.Letiltva = !felhasznalo.Letiltva;
            Console.WriteLine($"✅ Felhasználó {(felhasznalo.Letiltva ? "letiltva" : "feloldva")}");
            Console.ReadLine();
        }

        private void JelszoMegvaltoztatasa(Felhasznalo felhasznalo)
        {
            Console.Write("Új jelszó: ");
            string ujJelszo = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(ujJelszo))
            {
                Console.WriteLine("❌ A jelszó nem lehet üres!");
                Console.ReadLine();
                return;
            }

            if (felhasznalo.JelszoMegvaltoztatasa(ujJelszo))
            {
                Console.WriteLine("✅ Jelszó sikeresen megváltoztatva!");
            }
            else
            {
                Console.WriteLine("❌ A jelszó megváltoztatása sikertelen!");
            }
            Console.ReadLine();
        }

        private void NevModositasa(Felhasznalo felhasznalo)
        {
            Console.Write("Új név: ");
            string ujNev = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(ujNev))
            {
                Console.WriteLine("❌ A név nem lehet üres!");
                Console.ReadLine();
                return;
            }

            string regiNev = felhasznalo.Nev;
            felhasznalo.Nev = ujNev;
            Console.WriteLine($"✅ Név megváltoztatva: {regiNev} -> {ujNev}");
            Console.ReadLine();
        }

        private bool FiokTorlese(Felhasznalo felhasznalo, List<Felhasznalo> felhasznalok)
        {
            if (felhasznalo.Felhasznalonev == this.Felhasznalonev)
            {
                Console.WriteLine("❌ Nem törölheted a saját fiókodat!");
                Console.ReadLine();
                return false;
            }

            Console.Write($"Biztosan törlöd a(z) {felhasznalo.Nev} felhasználót? (i/n): ");
            string valasz = Console.ReadLine().ToLower();

            if (valasz == "i")
            {
                felhasznalok.Remove(felhasznalo);
                Console.WriteLine("✅ Felhasználó sikeresen törölve!");
                Console.ReadLine();
                return true;
            }

            Console.WriteLine("❌ Törlés megszakítva.");
            Console.ReadLine();
            return false;
        }
    }
}