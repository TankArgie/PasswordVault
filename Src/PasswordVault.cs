using System;
using System.Reflection;
using System.Text;

namespace Managers
{
    class PasswordVault : Data
    {
        private readonly string _securityName = "CSharpPasswordVault";
        private string _tempSecurityHash = null;
        private bool _creating = false;
        private bool _loggedin = false;
        private bool _modified = false;
        private readonly string _prefix;
        private string SecurityHash => Environment.GetEnvironmentVariable(_securityName);
        
        public PasswordVault(string prefix) => _prefix = prefix;

        public void Start()
        {
            if ((SecurityHash != null) && !_modified)
                _tempSecurityHash = SecurityHash;
            Console.Clear();
            Login();
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("================== Password Vault ====================");

            base.AddCommandDescription("To create an account", $"{_prefix} create");
            base.AddCommandDescription("To remove a specific account", $"{_prefix} remove <position>");
            base.AddCommandDescription("To remove all accounts", $"{_prefix} remove all");
            base.AddCommandDescription("To clear the console", $"{_prefix} clear");
            base.AddCommandDescription("To terminate the console", $"{_prefix} exit");
            base.AddCommandDescription("To show the accounts", $"{_prefix} list");
            base.AddCommandDescription("To change the password of your vault", $"{_prefix} changepass");
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("======================================================");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                if (!_creating) Console.Write("Command: ");
                Console.ForegroundColor = ConsoleColor.Cyan;

                string[] args = Console.ReadLine().Split(" ");
                if (args[0].ToLower() != _prefix) continue;
                if (Command(args)) Environment.Exit(0);
            }
        }

        private void Login()
        {
            if (_loggedin) return;
            if (_tempSecurityHash == null)
            {
                Register();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("================== Password Vault ====================");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Please enter the valid password before to proceed.");
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("======================================================");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Password: ");
                Console.ForegroundColor = ConsoleColor.Cyan;

                string input = Console.ReadLine();
                if (base.GetHash(input) == _tempSecurityHash) break;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Incorrect password!\n");
            }
            _loggedin = true;
        }

        public void Register()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("================== Password Vault ====================");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Please enter a password for ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Password Vault\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("that you can remember.");
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("======================================================");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Password: ");
            Console.ForegroundColor = ConsoleColor.Cyan;

            string input = Console.ReadLine();
            string hash = base.GetHash(input);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nPlease press ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("ENTER ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("to proceed or press ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("ESC ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("to cancel.");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Note: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("You have to restart your device to save the password.");

            while (true)
            {
                ConsoleKey inputKey = Console.ReadKey().Key;
                if (inputKey == ConsoleKey.Enter)
                {
                    Environment.SetEnvironmentVariable(_securityName, hash,
                        EnvironmentVariableTarget.User);
                    _tempSecurityHash = hash;
                    break;
                }
                else if (inputKey == ConsoleKey.Escape)
                {
                    Console.Clear();
                    Register();
                }
            }
            Start();
        }

        private bool Command(string[] args)
        {
            if (args.Length == 1) return false;
            switch (args[1].ToLower())
            {
                case "exit":
                    return true;
                case "create":
                    Create();
                    break;
                case "clear":
                    Start();
                    break;
                case "list":
                    base.GetListData();
                    break;
                case "remove":
                {
                    if (base._accounts == null)
                    {
                        InvalidInputMessage("Please load the accounts first.");
                        return false;
                    }
                    if (args.Length != 3)
                    {
                        InvalidInputMessage();
                        return false;
                    }

                    if (args[2].ToLower() == "all") args[2] = "0";
                    if (!int.TryParse(args[2], out int pos))
                    {
                        InvalidInputMessage();
                        return false;
                    }

                    Remove(pos);
                    break;
                }
                case "changepass":
                    ChangePassword();
                    break;
            }
            
            return false;
        }

        private void ChangePassword()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nNew password: ");
            Console.ForegroundColor = ConsoleColor.Cyan;

            string input = Console.ReadLine();
            string hash = base.GetHash(input);
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nPlease press ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("ENTER ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("to confirm or press ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("ESC ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("to cancel.");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Note: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("You have to restart your device to save the password.");
            
            while (true)
            {
                ConsoleKey inputKey = Console.ReadKey().Key;
                if (inputKey == ConsoleKey.Enter)
                {
                    Environment.SetEnvironmentVariable(_securityName, hash,
                        EnvironmentVariableTarget.User);
                    _tempSecurityHash = hash;
                    _loggedin = false;
                    _modified = true;
                    break;
                }
                else if (inputKey == ConsoleKey.Escape) break;
            }
            Start();
        }

        private void InvalidInputMessage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid input!\n");
        }
        private void InvalidInputMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{message}\n");
        }

        private void Remove(int pos)
        {
            if (pos == 0)
            {
                base.RemoveAllData();
                return;
            }
            Account account = base._accounts?.Find(acc => acc.Position == pos);
            base.RemoveData(account);
        }

        private void Create()
        {
            _creating = true;
            var account = new Account();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nYou may start now.");
            foreach (PropertyInfo prop in account.GetType().GetProperties())
            {
                Console.ForegroundColor = ConsoleColor.White;
                switch (prop.Name)
                {
                    case "Platform":
                    {
                        Console.Write("Platform: ");
                        break;
                    }
                    case "UsernameOrEmail":
                    {
                        Console.Write("Username/Email: ");
                        break;
                    }
                    case "Password":
                    {
                        Console.Write("Password: ");
                        break;
                    }
                }
                Console.ForegroundColor = ConsoleColor.Cyan;

                if (prop.Name is "Position" or "FilePath") continue;

                double timestamp = DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
                byte[] timestampBytes = UTF8Encoding.UTF8.GetBytes(timestamp.ToString());
                string timestampBase64 = Convert.ToBase64String(timestampBytes);
                
                string input = Console.ReadLine();
                byte[] inputBytes = UTF8Encoding.UTF8.GetBytes(input);
                string inputBase64 = Convert.ToBase64String(inputBytes);

                byte[] lengthBytes = UTF8Encoding.UTF8.GetBytes(input.Length.ToString());
                string lengthBase64 = Convert.ToBase64String(lengthBytes);

                prop.SetValue(account, $"{timestampBase64}.{inputBase64}.{lengthBase64}");
            }
            base.CreateData(account);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nAdded to the vault!\n");
            Console.ForegroundColor = ConsoleColor.White;
            _creating = false;
        }
    }
}