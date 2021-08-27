using System;

using Managers;

namespace MainProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            new PasswordVault(prefix: "pv").Start();
        }
    }
}
