using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Runtime.Remoting.Lifetime;

namespace Lekser
{
    internal class Program
    {
        static List<lekse> lekser = new List<lekse>(); //Definerer listen globalt slik at dne kan brukes av alle metoder uten å måtte bli matet som et parameter
        static string directoryPath; //Samme med stien til filens directory
        static void Main(string[] args)
        {
            DefinerFil();
            HentFraJson();
            int option = 0;
            while (option != 7)
            {
                try
                {
                    Console.WriteLine("\n\nLekse-app\n----------------------------");
                    Console.WriteLine("1: Se alle lekser\n2: Legg til lekse\n3: Fjern lekse\n4: Søk etter tittel\n5: Søk etter index\n6: Søk etter frist\n7: Avslutt\n\n");
                    option = Convert.ToInt32(Console.ReadLine());
                    switch (option)
                    {
                        case 1:
                            SkrivUtListe();
                            break;
                        case 2:
                            LeggTilLekse();
                            break;
                        case 3:
                            FjernLekse();
                            break;
                        case 4:
                            SearchByTittel();
                            break;
                        case 5:
                            SearchByIndex();
                            break;
                        case 6:
                            SearchByFrist();
                            break;
                        case 7:
                            break;
                        default: //Kjøres hvis det oppgis et tall utenfor alternativene
                            Console.WriteLine("Ugyldig alternativ");
                            break;
                            
                    }
                }
                catch //Kjører hvis det ikke oppgis et tall
                {
                    Console.WriteLine("Uglydig alternativ");
                }
            }
            SkrivTilJson();
        }
        static void DefinerFil()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory; //Henter lokasjonen hvor filen kjøres (directory\bin\debug)
            DirectoryInfo directoryInfo = new DirectoryInfo(currentPath); //Gjør dette om til en directoryinfo variabel
            DirectoryInfo parentDirectory = directoryInfo.Parent.Parent; //Beveger seg opp to steg fordi når filen kjører så kjører den i directory\bin\debug
            directoryPath = (string)parentDirectory.FullName + "\\lekser.json"; //Lagrer stien til lekser.json som er lagret i samme mappe som program.cs
        }
        static void HentFraJson()
        {
            using (StreamReader file = new StreamReader(directoryPath)) //Åpner filen slik at vi kan lese den, men lukker filen igjen når vi er ferdig med den
            {
                string fil = file.ReadToEnd(); //Leser hele json filen
                try
                {
                    lekser = JsonSerializer.Deserialize<List<lekse>>(fil); //Deserialiserer json filen og lagrer den i den globale listen
                }
                catch //Kjører hvis det har oppstått en feil og programmet ikke kunne hente informasjonen fra filen
                {
                    Console.WriteLine("Kunne ikke hente fra fil\n");
                }
            }

        }
        static void SkrivTilJson()
        {
            var options = new JsonSerializerOptions //Lager en liste med alternativer som kan brukes til å bestemme hvordan dataen skal skrives til filen
            {
                WriteIndented = true //Denne passer på at alt ikke havner på en lang linje
            };
            string json = JsonSerializer.Serialize(lekser, options); //Gjør om lekser til en Serialisert string som kan skrives til en json
            File.WriteAllText(directoryPath, json); //Overskriver json filen med den nye dataen som skal lagres
        }
        static void LeggTilLekse()
        {
            Console.WriteLine("Hva er tittelen på oppgaven?");
            string tittel = Console.ReadLine();
            Console.WriteLine("Hva er datofristen på oppgaven? (dd.mm)");
            string frist = Console.ReadLine();
            Console.WriteLine("Hva er oppgaven?");
            string beskrivelse = Console.ReadLine();
            lekser.Add(new lekse(tittel,frist,beskrivelse)); //Bygger ett lekse objekt og legger det til i dne globale listen
        }
        static void FjernLekse()
        {
            int option;
            Console.WriteLine("Hva er index-en til leksen du ønsker å fjerne? (-1 for å slette alle)");
            while (true)
            {
                try
                {
                    option = Convert.ToInt32(Console.ReadLine());
                    if (option == -1)
                    {
                        lekser.Clear(); //Fjerner alle objektene i listen
                    } else
                    {
                        lekser.RemoveAt(option); //Fjerner ett spesifikt index fra listen
                    }
                }
                catch //Kjører hvis brukeren ikke oppgir et gyldig index
                {
                    Console.WriteLine("Ugyldig alternativ");
                }
            }
        }
        static void SkrivUt(lekse i) //Skriver ut dataen til et lekse objekt + index-en til objektet i dne globale listen
        {
            Console.WriteLine($"Tittel: {i.tittel}\nFrist: {i.frist}\nBeskrivelse: {i.beskrivelse}\nIndex: {lekser.IndexOf(i)}\n");
        }
        static void SkrivUtListe()
        {
            if (lekser.Count == 0) //Sjekker om listen er tom
            {
                Console.WriteLine("Fant ingen lekser");
            } else
            {
                foreach (lekse i in lekser) //Kjører for hvert objekt i listen
                {
                    SkrivUt(i); //Kaller på SkrivUt for å skrive ut informasjonen om objektet
                }
            }
        }
        static void SearchByIndex()
        {
            int index;
            Console.WriteLine($"Hvilken lekse ønsker du å vite om? (0 - {lekser.Count() - 1})");
            try
            {
                index = Convert.ToInt32(Console.ReadLine());
                SkrivUt(lekser[index]); //Kaller på SkrivUt for å skrive ut 
            } catch
            {
                Console.WriteLine("Vennligst oppgi et gyldig index");
            }
        }
        static void SearchByTittel()
        {
            int temp = 0;
            Console.WriteLine("Hva er tittelen til leksen du søker etter?");
            string tittel = Console.ReadLine().ToLower();
            foreach (lekse i in lekser)
            {            
                if (i.tittel.ToLower().Contains(tittel))
                {
                    SkrivUt(i);
                    temp++;
                }
                Console.WriteLine($"Fant {temp} lekse(r) med den tittelen");
            }
        }
        static void SearchByFrist()
        {
            int temp = 0;
            Console.WriteLine("Hva er datoen på fristen til leksen du søker etter? (dd.mm)");
            string dato = Console.ReadLine();
            foreach (lekse i in lekser)
            {
                if (i.frist == dato)
                {
                    SkrivUt(i);
                    temp++;
                }
                Console.WriteLine($"Fant {temp} lekse(r) med frist til {dato}");
            }
        }
    }
    public class lekse
    {
        public string tittel { get; set; }
        public string frist { get; set; }
        public string beskrivelse { get; set; }

        public lekse(string Tittel, string Frist, string Beskrivelse)
        {
            this.tittel = Tittel;
            this.frist = Frist;
            this.beskrivelse = Beskrivelse;
        }
    }
}
