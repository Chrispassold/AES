using System;
using System.IO;

namespace AESv2
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Informe o arquivo que deseja encripitar ou vazio para o default: ");
            var fileInput = Console.ReadLine();

            if (string.IsNullOrEmpty(fileInput))
                fileInput = "./texto.txt";

            var simpleTextBytes = File.ReadAllBytes(fileInput);


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

                for(int i = 0; i < splitBytes.Length; i++)
                {
                    keyBytes[i] = Convert.ToByte(splitBytes[i]);
                }

            }
            else if(option.ToLower().Equals("a"))
            {
                Console.WriteLine("Informe a key para encriptar ou deixe vazio para o default: ");
                var keyInput = Console.ReadLine();

                if (string.IsNullOrEmpty(keyInput))
                    keyInput = "./chave.txt";

                keyBytes = File.ReadAllBytes(keyInput);

                if (keyBytes.Length > 16 || keyBytes.Length < 16)
                    throw new InvalidDataException("Chave deve ter 16 bytes");
            }

            Console.WriteLine("Informe o caminho de destino da cifragem ou vazio para o default: ");
            var distInput = Console.ReadLine();

            if (string.IsNullOrEmpty(distInput))
                throw new InvalidDataException("Caminho de destino inválido");

            var aes = new AES(keyBytes);
            byte[] output = aes.perform(simpleTextBytes);

            File.WriteAllBytes(distInput, output);

            Console.WriteLine("END!");
            Console.Read();//*/
        }
    }
}
