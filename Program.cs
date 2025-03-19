using System.Globalization;


class SemestralniPrace // Není potřeba, ale při psaní kódu se hodí znát počet volání metod, který se jinak nezobrazuje
{
    static int Hodiny()
    {
        var datum = DateTime.Now;
        int h = datum.Hour;
        return h;
    }

    static int Minuty()
    {
        var datum = DateTime.Now;
        int m = datum.Minute;
        return m;
    }

    static void Hlavicka()
    {
        Console.WriteLine("********** Semestralni Prace **********");
        Console.WriteLine("************* Jizdni rady *************");
        Console.WriteLine("************* Adam Jurica *************");
    }

    static int DenVTydnu()
    {
        var datum = DateTime.Now;
        int d = (int)datum.DayOfWeek;
        return d;
    }

    static void VypisCas(int h, int m, string text)
    {
        if (h < 10 && m < 10)
            Console.Write($"\n{text} 0{h}:0{m}");
        else if (m < 10)
            Console.Write($"\n{text} {h}:0{m}");
        else if (h < 10)
            Console.Write($"\n{text} 0{h}:{m}");
        else
            Console.Write($"\n{text} {h}:{m}");
    }

    static void VypisOdjezdy(string soubor, int n, int A, int B, bool smer)
    {
        try // Soubory by nemusely být nalezeny
        {
            using (StreamReader file = new($@"odjezdy\{soubor}"))
            {
                int hodiny;
                int minuty;
                int i = 0;
                bool dalsiden = false;
                bool uspech = false;
                int[] pridat_smer_fgn = { 0, 1, 2, 3, 4, 5, 6, 9 };
                int[] pridat_smer_zoo = { 0, 1, 2, 3, 4, 5, 6, 8 };
                int prijezd_h = 0, prijezd_m = 0;
                int rozdil;

                if (A - B > 0) // počítání doby jízdy mezi zadanými stanicemi
                    rozdil = pridat_smer_zoo[A - 1] - pridat_smer_zoo[B - 1];
                else 
                    rozdil = pridat_smer_fgn[B - 1] - pridat_smer_fgn[A - 1];

                while (!file.EndOfStream)
                {
                    string line = file.ReadLine(); // Čtení řádku ze souboru
                    if (line.Contains('-'))
                        dalsiden = true;

                    // podaří se přečíst čas v požadovaném formátu
                    if (DateTime.TryParseExact(line, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime))
                    {
                        if (smer) parsedTime = parsedTime.AddMinutes(pridat_smer_zoo[8 - A]);
                        else parsedTime = parsedTime.AddMinutes(pridat_smer_fgn[A - 1]);

                        hodiny = parsedTime.Hour;
                        minuty = parsedTime.Minute;

                        if (((Hodiny() > hodiny) || (Hodiny() == hodiny && Minuty() > minuty)) && dalsiden == false) continue;
                        else
                        {
                            VypisCas(hodiny, minuty, "Odjezd:  ");

                            //počítání doby příjezdu pro další implemetaci
                            parsedTime = parsedTime.AddMinutes(rozdil);
                            prijezd_h = parsedTime.Hour;
                            prijezd_m = parsedTime.Minute;

                            uspech = true;
                        }

                    }
                    else // pokud na řádku není čas, je tam identifikátor linky
                    {
                        if (line.Contains('a') && uspech == true)
                            Console.Write("\t\t\t\tLinka 2");
                        else if (line.Contains('b') && uspech == true)
                            Console.Write("\t\t\t\tLinka 3");
                        else
                            continue;


                        if (prijezd_h != 0 && prijezd_m != 0)
                        {
                            VypisCas(prijezd_h, prijezd_m, "Prijezd: ");
                            Console.Write("\n------\n");
                        }
                        uspech = false;
                        i++;
                    }
                    if (i == n) break;
                }
            }

        }
        catch 
        {
            Console.WriteLine("Chyba pri nacteni souboru s odjezdy!");
        }
    }
    

    static int NactiCeleCislo(string zadani, string neuspech)
    {
        int cislo = 0;
        Console.Write(zadani);
        while (!int.TryParse(Console.ReadLine(), out cislo) || cislo == 0 || cislo < 0)
            Console.Write(neuspech);
        return cislo;
    }


    static void Main()
    {
        string opakovani = "a";
        while (opakovani == "a")
        {
            Console.Clear();
            Hlavicka();
            VypisCas(Hodiny(), Minuty(), "Prave je:");
            int A, B;

            Console.Write("\n\nStanice:\n1 - LIDOVE SADY, ZOO\n2 - RIEGROVA\n3 - BOTANICKA ZAHRADA\n4 - MUZEUM\n5 - PRUMYSLOVA SKOLA\n6 - UL. 5. KVETNA\n7 - SALDOVO NAMESTI\n8 - FUGNEROVA");
            while (true)
            {
                A = NactiCeleCislo("\n\nZadejte počáteční stanici: ", "Zřejmě jste nezadali kladnou celočíselnou nenulovou hodnotu.\nZadejte znovu počáteční stanici: ");
                B = NactiCeleCislo("\nZadejte cílovou stanici: ", "Zřejmě jste nezadali kladnou celočíselnou nenulovou hodnotu.\nZadejte znovu cílovou stanici: ");
                if (A != B && A < 9 && B < 9)
                    break;
                else
                    Console.WriteLine("Chyba, zkuste to znovu!");
            }
            int n = NactiCeleCislo("\nPočet nejbližších načtených spojů: ", "Zřejmě jste nezadali kladnou celočíselnou nenulovou hodnotu.\nZadejte znovu počet načtených spojů: ");

            Console.Write("\n=========================================\n\n");
            bool smer;
            if (A > B)
                smer = true;
            else
                smer = false;
            Console.ForegroundColor = ConsoleColor.Yellow;
            switch (DenVTydnu())
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    if (smer)
                        VypisOdjezdy("linka2+3_po-ct_smer_zoo.txt", n, A, B, smer);
                    else
                        VypisOdjezdy("linka2+3_po-ct_smer_fgn.txt", n, A, B, smer);
                    break;
                case 5:
                    if (smer)
                        VypisOdjezdy("linka2+3_pa_smer_zoo.txt", n, A, B, smer);
                    else
                        VypisOdjezdy("linka2+3_pa_smer_fgn.txt", n, A, B, smer);
                    break;
                case 6:
                    if (smer)
                        VypisOdjezdy("linka2+3_so_smer_zoo.txt", n, A, B, smer);
                    else
                        VypisOdjezdy("linka2+3_so_smer_fgn.txt", n, A, B, smer);
                    break;
                case 7:
                    if (smer)
                        VypisOdjezdy("linka2+3_ne_smer_zoo.txt", n, A, B, smer);
                    else
                        VypisOdjezdy("linka2+3_ne_smer_fgn.txt", n, A, B, smer);
                    break;
                default:
                    Console.WriteLine("Nepodarilo se nacist den v tydnu");
                    break;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n\nPro opakovani programu zadejte a: ");
            opakovani = Console.ReadLine();
        }
    }
}
