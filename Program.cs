using System;
using System.IO;

namespace AESv2
{
    class Program
    {
        static void Main(string[] args)
        {
            var simpleTextBytes = File.ReadAllBytes("./texto.txt");
            var keyBytes = File.ReadAllBytes("./chave.txt");

            var aes = new AES(keyBytes);
            aes.perform(simpleTextBytes);


            Console.WriteLine("END!");
            Console.Read();
        }
    }
}
