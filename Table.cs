using System;

namespace AESv2
{
public class Table
  {
      public byte[,] table = new byte[16, 16];

      public Table(String sboxString) {
        string[] sboxArray = sboxString.Split(" ");
        for (var i = 0; i < 16; i++)
        {
            for (var j = 0; j < 16; j++)
            {
                table[i, j] = ToByte(sboxArray[(i * 16) + j]);
            }
        }
      }

      public byte Get(byte leftTerm, byte rightTerm)
      {
          return table[leftTerm, rightTerm];
      }

      private byte ToByte(String bte)
      {
          return Convert.ToByte(Convert.ToInt32(bte, 16));
      }
  }
}