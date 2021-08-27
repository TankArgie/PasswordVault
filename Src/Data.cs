using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Managers
{
    class Data : Security
    {
        protected List<Account> _accounts = null;

        protected void AddCommandDescription(string description, string commandName)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{description}: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(commandName);
        }

        protected void CreateData(Account account)
        {
            string name = account.Platform.Split(".").FirstOrDefault();
            string content = $"{account.Platform}\n{account.UsernameOrEmail}\n{account.Password}";
            File.WriteAllText($"./Src/Data/{name}.txt", content);
        }

        protected void RemoveData(Account account)
        {
            if ((account == null) || !File.Exists(account.FilePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("That account does not exists or it is already removed.\n");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Successfully removed!\n");
            File.Delete(account.FilePath);
        }
        protected void RemoveAllData()
        {
            foreach (string file in Directory.GetFiles("./Src/Data").Where(f => f.EndsWith(".txt")))
                File.Delete(file);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All data successfully removed!\n");
        }

        protected void GetListData()
        {
            _accounts = new List<Account>();
            int count = 0;
            foreach (string file in Directory.GetFiles("./Src/Data").Where(f => f.EndsWith(".txt")))
            {
                count++;
                string[] contents = File.ReadAllText(file).Split("\n");
                var acc = new Account() { Position = count };

                foreach (string content in contents)
                {
                    string[] arr = content.Split(".");
                    byte[] bytes = Convert.FromBase64String(arr[1]);
                    string decoded = Encoding.UTF8.GetString(bytes);
                    
                    if (acc.Platform == null)
                        acc.Platform = decoded;
                    else if (acc.UsernameOrEmail == null)
                        acc.UsernameOrEmail = decoded;
                    else
                        acc.Password = decoded;
                    acc.FilePath = file;
                }
                _accounts.Add(acc);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n===================== Accounts =======================");
            if (_accounts.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Empty");
            }
            foreach (Account acc in _accounts)
            {
                AddCommandDescription("Position", acc.Position.ToString());
                AddCommandDescription("Platform", acc.Platform);
                AddCommandDescription("Username/Email", acc.UsernameOrEmail);
                AddCommandDescription("Password", acc.Password);
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("======================================================\n");
        }
    }
}