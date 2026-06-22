using Capa_Negocio;
using System;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] hash = EncryptionHelper.Encrypt("admin123");
            string base64 = Convert.ToBase64String(hash);
            Console.WriteLine(base64);
            Console.ReadKey();
        }
    }
}