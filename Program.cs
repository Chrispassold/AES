using System;
using System.IO;

namespace AESv2
{
    class Program
    {
        static void Main(string[] args)
        {
            var simpleTextBytes = File.ReadAllBytes("C:\\www\\FURB\\desenv-sistemas-seguros\\AESv2\\texto.txt");
            var keyBytes = File.ReadAllBytes("C:\\www\\FURB\\desenv-sistemas-seguros\\AESv2\\chave.txt");

            var aes = new AES(keyBytes);
            aes.perform(simpleTextBytes);


            Console.WriteLine("END!");
            Console.Read();
        }
    }
}
