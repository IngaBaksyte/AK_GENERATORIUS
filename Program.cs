using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Diagnostics.Metrics;
using System.Xml.Linq;
using System.ComponentModel.Design;
using System.Reflection;

namespace KODU_GENERATORIUS
{
    // Klasė aprašanti .json failo laukus
    class Person
    { 
        public string? Vardas { get; set; }
        public string? Pavarde { get; set; }
        public string? Lytis { get; set; }
        public string? Metai { get; set; }
        public string? Menuo { get; set; }
        public string? Diena { get; set; }
        public string? AK { get; set; }
    }
    // Klasė .json failo laukų tikrinimui
    class Check
    {
        // Metodas tikrina, ar string'as tuščias, ar jame yra skaičiai
        public void checkName(ref string name, ref bool go)
        {
            if (string.IsNullOrEmpty(name) || name.Any(char.IsDigit))
            {
                name += " - NOT VALID";
                go = false;
            }
        }

        // Metodas tikrina lytį
        public void checkGender(ref string gender, ref bool go)
        {

            checkName(ref gender, ref go);
            if (go && gender.Equals("vyras", StringComparison.OrdinalIgnoreCase))
            {
                gender = "Vyras";
            }
            else if (go && gender.Equals("moteris", StringComparison.OrdinalIgnoreCase))
            {
                gender = "Moteris";
            }
            else
            {
                go = false;
            }
        }

        // Metodas tikrina metus
        public void checkYear(ref string year, ref bool go)
        {
            if (string.IsNullOrEmpty(year) || !int.TryParse(year.ToString(), out int yearInt) || yearInt < 1800 || yearInt > 2099)
            {
                year += " - NOT VALID";
                go = false;
            }
        }

        // Metodas tikrina mėnesį
        public void checkMonth(ref string month, ref bool go)
        {
            if (string.IsNullOrEmpty(month) || !int.TryParse(month.ToString(), out int monthInt) || monthInt < 1 || monthInt > 12)
            {
                month += " - NOT VALID";
                go = false;
            }
        }

        // Metodas tikrina dieną pagal metus ir mėnesį
        public void checkDay(ref string day, ref string year, ref string month, ref bool yearGo, ref bool monthGo, ref bool go)
        {
            if (string.IsNullOrEmpty(day) || !int.TryParse(day.ToString(), out int dayInt) || dayInt < 1 || dayInt > 31)
            {
                day += " - NOT VALID";
                go = false;
            }
            else if (yearGo && monthGo)
            {
                string dateString = year + "-" + month + "-" + day;
                DateTime date;
                if (!DateTime.TryParse(dateString, out date))
                {
                    day += " - NOT VALID";
                    go = false;
                }
            }
        }
    }
    // Klasė asmens kodo skaičiams nustatyti
    class Create
    {
        // Nustato AK 1-7 skaitmenis (ABCDEFG)
        public string createIdABCDEFG(string gender, string year, string month, string day)
        {
            int.TryParse(year.ToString(), out int yearInt);
            int.TryParse(month.ToString(), out int monthInt);
            int.TryParse(day.ToString(), out int dayInt);
            string id1234567 = "";
            switch (gender)
            {
                case "Vyras":
                    if (yearInt >= 1800 && yearInt <= 1899)
                    {
                        id1234567 = "1";
                    }
                    else if (yearInt >= 1900 && yearInt <= 1999)
                    {
                        id1234567 = "3";
                    }
                    else if (yearInt >= 2000 && yearInt <= 2099)
                    {
                        id1234567 = "5";

                    }
                    break;
                case "Moteris":
                    if (yearInt >= 1800 && yearInt <= 1899)
                    {
                        id1234567 = "2";
                    }
                    else if (yearInt >= 1900 && yearInt <= 1999)
                    {
                        id1234567 = "4";
                    }
                    else if (yearInt >= 2000 && yearInt <= 2099)
                    {
                        id1234567 = "6";
                    }
                    break;
            }
            id1234567 += yearInt.ToString().Substring(2) + monthInt.ToString("D2") + dayInt.ToString("D2");
            return id1234567;
        }
        // Generuoja atsitiktini skaičių nuo 100 iki 999. AK 8-10 skaitmenys (HIJ).
        public string createIdHIJ()
        {
            Random rnd = new Random();
            int randNum = rnd.Next(100, 1000);
            string id8910 = Convert.ToString(randNum);
            return id8910;
        }

        // Nustato AK paskutinį 11 skaitmenį (K).
        public string createIdK(string ABCDEFG, string HIJ)
        {
            string id = ABCDEFG + HIJ;
            int[] weights = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1 };
            int sum = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                sum += weights[i] * int.Parse(id[i].ToString());
            }
            int remainder = sum % 11;
            if (remainder != 10)
            {
                string id11 = Convert.ToString(remainder);
                return id11;
            }
            else
            {
                int[] newWeights = { 3, 4, 5, 6, 7, 8, 9, 1, 2, 3 };
                sum = 0;
                for (int i = 0; i < newWeights.Length; i++)
                {
                    sum += newWeights[i] * int.Parse(id[i].ToString());
                }
                remainder = sum % 11;
                if (remainder != 10)
                {
                    string id11 = Convert.ToString(remainder);
                    return id11;
                }
                else
                {
                    string id11 = Convert.ToString(0);
                    return id11;
                }
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            // Pradedamas darbas
            try
            {
                // Išvalomas paruoštų duomenų .json failas nuo senos info
                File.WriteAllText("paruostiDuomenys.json", string.Empty);

                // Skaitomi duomenys iš .json failo į List struktūrą
                string jsonContent = File.ReadAllText("pradiniaiDuomenys.json");
                List<Person> data = JsonConvert.DeserializeObject<List<Person>>(jsonContent);
                
                for (int i = 0; i < data.Count; i++)
                {
                    string vardas = data[i].Vardas;
                    string pavarde = data[i].Pavarde;
                    string lytis = data[i].Lytis;
                    string metai = data[i].Metai;
                    string menuo = data[i].Menuo;
                    string diena = data[i].Diena;
                    string? ak = null;

                    Check personCheck = new Check();
                    bool vardasPirmyn = true;
                    personCheck.checkName(ref vardas, ref vardasPirmyn);
                    bool pavardePirmyn = true;
                    personCheck.checkName(ref pavarde, ref pavardePirmyn);
                    bool lytisPirmyn = true;
                    personCheck.checkGender(ref lytis, ref lytisPirmyn);
                    bool metaiPirmyn = true;
                    personCheck.checkYear(ref metai, ref metaiPirmyn);
                    bool menuoPirmyn = true;
                    personCheck.checkMonth(ref menuo, ref menuoPirmyn);
                    bool dienaPirmyn = true;
                    personCheck.checkDay(ref diena, ref metai, ref menuo, ref metaiPirmyn, ref menuoPirmyn, ref dienaPirmyn);

                    if (vardasPirmyn && pavardePirmyn && lytisPirmyn && metaiPirmyn && menuoPirmyn && dienaPirmyn)
                    {
                        Create createPersonId = new Create();
                        ak = createPersonId.createIdABCDEFG(lytis, metai, menuo, diena);
                        string atsitiktinisSk = createPersonId.createIdHIJ();
                        ak += atsitiktinisSk + createPersonId.createIdK(ak, atsitiktinisSk);
                    }
                    else
                        ak = null;

                    var myPerson = new Person { Vardas = vardas, Pavarde = pavarde, Lytis = lytis, Metai = metai, Menuo = menuo, Diena = diena, AK = ak };
                    string outputJson = JsonConvert.SerializeObject(myPerson, Formatting.Indented);
                    File.AppendAllText("paruostiDuomenys.json", outputJson + Environment.NewLine);
                }
                Console.WriteLine("DUOMENYS SU ASMENS KODU YRA IŠVESTI FAILE: KODU_GENERATORIUS\\bin\\Debug\\net6.0\\paruostiDuomenys.json");
            }
            // jei neteisinga json struktūra, darbas stabdomas ir išvedamas klaidos pranešimas
            catch //(Exception e)
            {
                //Console.Error.WriteLine(e.ToString());
                Console.WriteLine($"PATIKRINKITE, AR FAILAS KODU_GENERATORIUS\\bin\\Debug\\net6.0\\pradiniaiDuomenys.json ATITINKA JSON STRUKTŪRĄ IR YRA TEISINGAI UŽPILDYTAS (kableliai, skliaustai, tuščios reikšmės ir pan.).");
            }
        }
    }
}