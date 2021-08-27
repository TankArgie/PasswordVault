using System.Security.Cryptography;
using System.Text;

namespace Managers
{
    class Security
    {
        public string GetHash(string content)
        {
            var stringBuilder = new StringBuilder();

            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(UTF8Encoding.UTF8.GetBytes(content));

            foreach (byte res in bytes)
                stringBuilder.Append(res.ToString("x2"));
            return stringBuilder.ToString();
        }
    }    
}