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
                Console.WriteLine("ADMIN MENU - " + Nev);
                Console.WriteLine("[1] Osszes jegy megtekintese");
                Console.WriteLine("[2] Felhasznalok kezelese");
                Console.WriteLine("[3] Kozlemeny kuldese");
                Console.WriteLine("[0] Kijelentkezes");
                Console.Write("Valasztas: ");
                string valasztas = Console.ReadLine();

                if (valasztas == "1")
                {
                    jegyManager.JegyekListazasa(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "2")
                {
                    FelhasznalokKezelese(jegyManager.GetFelhasznalok());
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "3")
                {
                    kozlemenyManager.UjKozlemeny(this);
                    Console.WriteLine("Nyomj ENTER-t a folytatashoz...");
                    Console.ReadLine();
                }
                else if (valasztas == "0")
                {
                    Console.WriteLine("Kijelentkezes...");
                    break;
                }
                else
                {
                    Console.WriteLine("Ervenytelen valasztas!");
                    Console.ReadLine();
                }
            }
        }

        private void FelhasznalokKezelese(List<Felhasznalo> felhasznalok)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("FELHASZNALOK KEZELESE");

                if (!felhasznalok.Any())
                {
                    Console.WriteLine("Nincsenek felhasznalok.");
                    Console.WriteLine("Nyomj ENTER-t a visszalepeshez...");
                    Console.ReadLine();
                    return;
                }

                for (int i = 0; i < felhasznalok.Count; i++)
                {
                    var felhasznalo = felhasznalok[i];
                    string status = felhasznalo.Letiltva ? "Letiltva" : "Aktiv";
                    string osztalyInfo = string.IsNullOrEmpty(felhasznalo.Osztaly) ? "" : " - " + felhasznalo.Osztaly;

                    Console.WriteLine("[" + (i + 1) + "] " + felhasznalo.Nev + " (" + felhasznalo.Felhasznalonev + ") - " + felhasznalo.Szerepkor + osztalyInfo + " - " + status);
                }

                Console.Write("Valassz felhasznalot (0 - vissza): ");
                if (int.TryParse(Console.ReadLine(), out int valasztas) && valasztas > 0 && valasztas <= felhasznalok.Count)
                {
                    var kivalasztott = felhasznalok[valasztas - 1];
                    if (!FelhasznaloMuveletek(kivalasztott, felhasznalok))
                    {
                        break;
                    }
                }
                else if (valasztas == 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Ervenytelen valasztas!");
                    Console.ReadLine();
                }
            }
        }

        private bool FelhasznaloMuveletek(Felhasznalo felhasznalo, List<Felhasznalo> felhasznalok)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(felhasznalo.Nev + " MUVELETEK");
                Console.WriteLine("Felhasznalonev: " + felhasznalo.Felhasznalonev);
                Console.WriteLine("Nev: " + felhasznalo.Nev);
                Console.WriteLine("Szerepkor: " + felhasznalo.Szerepkor);
                Console.WriteLine("Iskola: " + felhasznalo.IskolaNev);
                if (!string.IsNullOrEmpty(felhasznalo.Osztaly))
                    Console.WriteLine("Osztaly: " + felhasznalo.Osztaly);
                Console.WriteLine("Statusz: " + (felhasznalo.Letiltva ? "Letiltva" : "Aktiv"));

                Console.WriteLine("[1] Letiltas/feloldas");
                Console.WriteLine("[2] Jelszó megvaltoztatasa");
                Console.WriteLine("[3] Nev modositasa");
                Console.WriteLine("[4] Fiok torlese");
                Console.WriteLine("[0] Vissza a felhasznalok listajahoz");
                Console.Write("Valasztas: ");

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
                            return false;
                        break;
                    case "0":
                        return true;
                    default:
                        Console.WriteLine("Ervenytelen valasztas!");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void FelhasznaloLetiltas(Felhasznalo felhasznalo)
        {
            if (felhasznalo.Felhasznalonev == this.Felhasznalonev)
            {
                Console.WriteLine("Nem tilthatod le a sajat fiokodat!");
                Console.ReadLine();
                return;
            }

            felhasznalo.Letiltva = !felhasznalo.Letiltva;
            Console.WriteLine("Felhasznalo " + (felhasznalo.Letiltva ? "letiltva" : "feloldva"));
            Console.ReadLine();
        }

        private void JelszoMegvaltoztatasa(Felhasznalo felhasznalo)
        {
            Console.Write("Uj jelszo: ");
            string ujJelszo = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(ujJelszo))
            {
                Console.WriteLine("A jelszo nem lehet ures!");
                Console.ReadLine();
                return;
            }

            if (felhasznalo.JelszoMegvaltoztatasa(ujJelszo))
            {
                Console.WriteLine("Jelszo sikeresen megvaltoztatva!");
            }
            else
            {
                Console.WriteLine("A jelszo megvaltoztatasa sikertelen!");
            }
            Console.ReadLine();
        }

        private void NevModositasa(Felhasznalo felhasznalo)
        {
            Console.Write("Uj nev: ");
            string ujNev = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(ujNev))
            {
                Console.WriteLine("A nev nem lehet ures!");
                Console.ReadLine();
                return;
            }

            string regiNev = felhasznalo.Nev;
            felhasznalo.Nev = ujNev;
            Console.WriteLine("Nev megvaltoztatva: " + regiNev + " -> " + ujNev);
            Console.ReadLine();
        }

        private bool FiokTorlese(Felhasznalo felhasznalo, List<Felhasznalo> felhasznalok)
        {
            if (felhasznalo.Felhasznalonev == this.Felhasznalonev)
            {
                Console.WriteLine("Nem torolheted a sajat fiokodat!");
                Console.ReadLine();
                return false;
            }

            Console.Write("Biztosan torlod a(z) " + felhasznalo.Nev + " felhasznalot? (i/n): ");
            string valasz = Console.ReadLine().ToLower();

            if (valasz == "i")
            {
                felhasznalok.Remove(felhasznalo);
                Console.WriteLine("Felhasznalo sikeresen torolve!");
                Console.ReadLine();
                return true;
            }

            Console.WriteLine("Torles megszakítva.");
            Console.ReadLine();
            return false;
        }
    }
}