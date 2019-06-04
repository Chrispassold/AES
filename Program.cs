using System;
using System.IO;

namespace AESv2
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileInput = "./texto.txt";
            var simpleTextBytes = File.ReadAllBytes(fileInput);
            Console.WriteLine("Lendo arquivo: " + fileInput);

            Console.WriteLine("Deseja informar os (B) bytes ou um (A) arquivo: ");
            var option = Console.ReadLine();

            byte[] keyBytes = new byte[16];
            if (option.ToLower().Equals("b"))
            {
                Console.WriteLine("Informe os bytes separados por virgula");
                var bytes = Console.ReadLine();

                var splitBytes = bytes.Split(",");

                if (splitBytes.Length > 16 || splitBytes.Length < 16)
                    throw new InvalidDataException("Chave deve ter 16 bytes");

                for (int i = 0; i < splitBytes.Length; i++)
                {
                    keyBytes[i] = Convert.ToByte(splitBytes[i]);
                }

            }
            else
            {
                var keyInput = "./chave.txt";
                keyBytes = File.ReadAllBytes(keyInput);
                Console.WriteLine("Arquivo chave: " + keyInput);
                if (keyBytes.Length > 16 || keyBytes.Length < 16)
                    throw new InvalidDataException("Chave deve ter 16 bytes");

            }

            Console.WriteLine("Informe o caminho de destino da cifragem: ");
            var distInput = Console.ReadLine();

            if (string.IsNullOrEmpty(distInput))
                distInput = "C:/www/FURB/desenv-sistemas-seguros/AESv2/cifrado.txt";

            var aes = new AES(keyBytes);
            byte[] output = aes.perform(simpleTextBytes);

            File.WriteAllBytes(distInput, output);
            Console.WriteLine("Arquivo cifrado: " + distInput);

            Console.WriteLine("END!");
            Console.Read();//*/
        }
    }
}
