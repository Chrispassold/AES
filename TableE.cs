using System;

namespace AESv2
{
public class TableE
  {
      public static Table table;
      private static string sboxString =
          "01 03 05 0f 11 33 55 ff 1a 2e 72 96 a1 f8 13 35 " +
          "5f e1 38 48 d8 73 95 a4 f7 02 06 0a 1e 22 66 aa " +
          "e5 34 5c e4 37 59 eb 26 6a be d9 70 90 ab e6 31 " +
          "53 f5 04 0c 14 3c 44 cc 4f d1 68 b8 d3 6e b2 cd " +
          "4c d4 67 a9 e0 3b 4d d7 62 a6 f1 08 18 28 78 88 " +
          "83 9e b9 d0 6b bd dc 7f 81 98 b3 ce 49 db 76 9a " +
          "b5 c4 57 f9 10 30 50 f0 0b 1d 27 69 bb d6 61 a3 " +
          "fe 19 2b 7d 87 92 ad ec 2f 71 93 ae e9 20 60 a0 " +
          "fb 16 3a 4e d2 6d b7 c2 5d e7 32 56 fa 15 3f 41 " +
          "c3 5e e2 3d 47 c9 40 c0 5b ed 2c 74 9c bf da 75 " +
          "9f ba d5 64 ac ef 2a 7e 82 9d bc df 7a 8e 89 80 " +
          "9b b6 c1 58 e8 23 65 af ea 25 6f b1 c8 43 c5 54 " +
          "fc 1f 21 63 a5 f4 07 09 1b 2d 77 99 b0 cb 46 ca " +
          "45 cf 4a de 79 8b 86 91 a8 e3 3e 42 c6 51 f3 0e " +
          "12 36 5a ee 29 7b 8d 8c 8f 8a 85 94 a7 f2 0d 17 " +
          "39 4b dd 7c 84 97 a2 fd 1c 24 6c b4 c7 52 f6 01 ";
      static TableE() {
        TableE.table = new Table(sboxString);
      }
  }
}