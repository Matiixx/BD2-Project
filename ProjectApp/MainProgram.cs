using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAPI;
using System.IO;

namespace ProjectApp
{
    class MainProgram
    {

        static bool isLoggedIn = false;

        static void Main(string[] args)
        {
            string connectionString = "Data Source=WINSERV01;Initial Catalog=ProjectTest;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False";
            ProjectFunctions pf = new ProjectFunctions(connectionString);


            while (true)
            {
                Console.Clear();

                int option;
                if (isLoggedIn == false)
                {

                    Console.Write(@"1 - Zaloguj sie
2 - Zarejestruj sie
0 - Wyjdz z programu
");
                    try
                    {
                        option = int.Parse(Console.ReadLine());
                    }
                    catch (FormatException)
                    {
                        continue;
                    }

                    switch (option)
                    {
                        case 1:
                            Console.Clear();
                            Console.Write("Login: ");
                            string login = Console.ReadLine();
                            Console.Write("Haslo: ");
                            string password = Console.ReadLine();
                            try
                            {
                                if (pf.loginUser(login, password))
                                {
                                    isLoggedIn = true;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.Clear();
                                Console.WriteLine(e.Message);
                                Console.ReadLine();
                            }
                            continue;
                        case 2:
                            Console.Clear();
                            Console.Write("Login: ");
                            login = Console.ReadLine();
                            Console.Write("Haslo: ");
                            password = Console.ReadLine();
                            try
                            {
                                pf.createUser(login, password);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Console.ReadLine();
                            }

                            continue;
                        case 0:
                            Environment.Exit(0);
                            break;

                    }
                }
                else
                {
                    Console.Write(@"1 - Wypisz swoje dokumenty
2 - Dodaj nowy dokument
3 - Szukaj dokumenty po nazwie
4 - Szukaj dokumenty z tesktem
0 - Wyjdz z programu
");
                    try
                    {
                        option = int.Parse(Console.ReadLine());
                    }
                    catch (FormatException)
                    {
                        continue;
                    }

                    switch (option)
                    {
                        case 1:
                            Console.Clear();
                            var res = pf.getUserDocuments();
                            foreach (var r in res)
                            {
                                Console.WriteLine("[" + r.id + "]");
                                Console.WriteLine("Nazwa dokumentu: " + r.name);
                                Console.WriteLine("===============");
                                Console.WriteLine(r.document);
                                Console.WriteLine("---------------");
                            }
                            if (res.Count == 0)
                            {
                                Console.WriteLine(@"Brak dokumentow
0 - Powrot");
                            }
                            else
                            {
                                Console.WriteLine(@"[id] - Akcje z dokumentem
0 - Powrot");
                            }

                            try
                            {
                                option = int.Parse(Console.ReadLine());
                            }
                            catch (FormatException)
                            {
                                continue;
                            }
                            if (option == 0) break;
                            else if (!res.Where(item => item.id == option).Any())
                            {
                                break;
                            }
                            else
                            {
                                var elem = res.Where(item => item.id == option).First();
                                Console.Clear();
                                Console.WriteLine("[" + elem.id + "]");
                                Console.WriteLine("Nazwa dokumentu: " + elem.name);
                                Console.WriteLine("===============");
                                Console.WriteLine(elem.document);
                                Console.WriteLine("---------------");
                                Console.WriteLine(@"1 - Usun dokument
2 - Zmien nazwe dokumentu
0 - Powrot");
                                try
                                {
                                    option = int.Parse(Console.ReadLine());
                                }
                                catch (FormatException)
                                {
                                    continue;
                                }
                                switch (option)
                                {
                                    case 1:
                                        try
                                        {
                                            pf.deleteDocumentWithId(elem.id);
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.Message);
                                            Console.ReadLine();
                                        }
                                        break;

                                    case 2:
                                        Console.Clear();
                                        Console.WriteLine("Stara nazwa dokumentu: " + elem.name);
                                        Console.Write("Wpisz nowa nazwe dokumentu: ");
                                        string newName = Console.ReadLine();
                                        try
                                        {
                                            pf.updateNameOfDocument(elem.id, newName);
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.Message);
                                            Console.ReadLine();
                                        }
                                        break;
                                }
                                break;
                            }
                        case 2:
                            Console.Clear();
                            Console.Write("Nazwa dokumentu: ");
                            string name = Console.ReadLine();
                            StringBuilder sb = new StringBuilder();
                            Console.Write("Zawartosc dokumentu: ");
                            bool previousEnter = false;
                            while (true)
                            {
                                ConsoleKeyInfo keyInfo = Console.ReadKey();
                                if (keyInfo.Key == ConsoleKey.Enter)
                                {
                                    if (previousEnter)
                                    {
                                        break;
                                    }
                                    previousEnter = true;
                                    Console.Write('\n');
                                    sb.Append('\n');
                                }
                                else
                                {
                                    previousEnter = false;
                                    sb.Append(keyInfo.KeyChar);
                                }
                            }
                            try
                            {
                                pf.createClobObjectFromString(sb.ToString(), name);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Console.ReadLine();
                            }
                            break;

                        case 3:
                            Console.Clear();
                            Console.Write("Szkuana nazwa dokumentu: ");
                            string searchName = Console.ReadLine();
                            res = pf.searchDocumentByName(searchName);
                            foreach (var r in res)
                            {
                                Console.WriteLine("[" + r.id + "]");
                                Console.WriteLine("Nazwa dokumentu: " + r.name);
                                Console.WriteLine("===============");
                                Console.WriteLine(r.document);
                                Console.WriteLine("---------------");
                            }
                            if (res.Count == 0)
                            {
                                Console.WriteLine(@"Brak dokumentow
0 - Powrot");
                            }
                            else
                            {
                                Console.WriteLine(@"[id] - Akcje z dokumentem
0 - Powrot");
                            }

                            try
                            {
                                option = int.Parse(Console.ReadLine());
                            }
                            catch (FormatException)
                            {
                                continue;
                            }
                            if (option == 0) break;
                            else if (!res.Where(item => item.id == option).Any())
                            {
                                break;
                            }
                            else
                            {
                                var elem = res.Where(item => item.id == option).First();
                                Console.Clear();
                                Console.WriteLine("[" + elem.id + "]");
                                Console.WriteLine("Nazwa dokumentu: " + elem.name);
                                Console.WriteLine("===============");
                                Console.WriteLine(elem.document);
                                Console.WriteLine("---------------");
                                Console.WriteLine(@"1 - Usun dokument
2 - Zmien nazwe dokumentu
0 - Powrot");
                                try
                                {
                                    option = int.Parse(Console.ReadLine());
                                }
                                catch (FormatException)
                                {
                                    continue;
                                }
                                switch (option)
                                {
                                    case 1:
                                        try
                                        {
                                            pf.deleteDocumentWithId(elem.id);
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.Message);
                                            Console.ReadLine();
                                        }
                                        break;

                                    case 2:
                                        Console.Clear();
                                        Console.WriteLine("Stara nazwa dokumentu: " + elem.name);
                                        Console.Write("Wpisz nowa nazwe dokumentu: ");
                                        string newName = Console.ReadLine();
                                        try
                                        {
                                            pf.updateNameOfDocument(elem.id, newName);
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.Message);
                                            Console.ReadLine();
                                        }
                                        break;
                                }
                                break;
                            }

                        case 4:
                            Console.Clear();
                            Console.Write("Szkuana fraza w dokumencie: ");
                            string searchText = Console.ReadLine();
                            res = pf.searchDocumentsByText(searchText);
                            foreach (var r in res)
                            {
                                Console.WriteLine("[" + r.id + "]");
                                Console.WriteLine("Nazwa dokumentu: " + r.name);
                                Console.WriteLine("===============");
                                Console.WriteLine(r.document);
                                Console.WriteLine("---------------");
                            }
                            if (res.Count == 0)
                            {
                                Console.WriteLine(@"Brak dokumentow
0 - Powrot");
                            }
                            else
                            {
                                Console.WriteLine(@"[id] - Akcje z dokumentem
0 - Powrot");
                            }

                            try
                            {
                                option = int.Parse(Console.ReadLine());
                            }
                            catch (FormatException)
                            {
                                continue;
                            }
                            if (option == 0) break;
                            else if (!res.Where(item => item.id == option).Any())
                            {
                                break;
                            }
                            else
                            {
                                var elem = res.Where(item => item.id == option).First();
                                Console.Clear();
                                Console.WriteLine("[" + elem.id + "]");
                                Console.WriteLine("Nazwa dokumentu: " + elem.name);
                                Console.WriteLine("===============");
                                Console.WriteLine(elem.document);
                                Console.WriteLine("---------------");
                                Console.WriteLine(@"1 - Usun dokument
2 - Zmien nazwe dokumentu
0 - Powrot");
                                try
                                {
                                    option = int.Parse(Console.ReadLine());
                                }
                                catch (FormatException)
                                {
                                    continue;
                                }
                                switch (option)
                                {
                                    case 1:
                                        try
                                        {
                                            pf.deleteDocumentWithId(elem.id);
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.Message);
                                            Console.ReadLine();
                                        }
                                        break;

                                    case 2:
                                        Console.Clear();
                                        Console.WriteLine("Stara nazwa dokumentu: " + elem.name);
                                        Console.Write("Wpisz nowa nazwe dokumentu: ");
                                        string newName = Console.ReadLine();
                                        try
                                        {
                                            pf.updateNameOfDocument(elem.id, newName);
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.Message);
                                            Console.ReadLine();
                                        }
                                        break;
                                }
                                break;
                            }

                        case 0:
                            Environment.Exit(0);
                            break;
                    }
                }
            }
        }

        static void generateStringToFile(string filepath, int numOfStrings)
        {
            using (StreamWriter writer = new StreamWriter(filepath, true))
            {
                Random random = new Random();

                for (int i = 0; i < numOfStrings; i++)
                {
                    string randomString = generateString(random, 100);
                    writer.Write(randomString);
                }
            }
        }

        static string generateString(Random random, int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789\n";

            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(stringChars);
        }
    }
}
