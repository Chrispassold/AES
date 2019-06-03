using System;

namespace AESv2
{
    class AES
    {
        private int Nb = 4;   // block size always = 4 words = 16 bytes = 128 bits for AES
        private int Nk = 4;   // key size = 4 words = 16 bytes = 128 bits
        private int Nr = 10;  // rounds for algorithm = 10

        private byte[] key;
        private byte[,] keySchedule;

        private byte[,] state;

        private int round = 0;

        public AES(byte[] keyBytes)
        {
            key = new byte[Nk * 4];  // 16, 24, 32 bytes
            keyBytes.CopyTo(key, 0);
            KeyExpansion();
        }

        public byte[] perform(byte[] input)
        {
            state = new byte[4, Nb];  // always [4,4]
            for (int i = 0; i < (4 * Nb); ++i)
            {
                state[i % 4, i / 4] = input[i];
            }
            Print("Texto Simples");
            PrintState();

            AddRoundKey(0);

            for (round = 1; round <= (Nr - 1); ++round)  // main round loop
            {
                SubBytes();
                ShiftRows();
                MixColumns();
                AddRoundKey(round);
            }  // main round loop

            SubBytes();
            ShiftRows();
            AddRoundKey(Nr);

            byte[] output = new byte[16];
            for (int i = 0; i < (4 * Nb); ++i)
            {
                output[i] = state[i % 4, i / 4];
            }

            return output;
        }

        private void KeyExpansion()
        {

            Print("KeyExpansion");

            keySchedule = new byte[Nb * (Nr + 1), 4];  // 4 columns of bytes corresponds to a word

            for (int row = 0; row < Nk; ++row)
            {
                keySchedule[row, 0] = key[4 * row];
                keySchedule[row, 1] = key[4 * row + 1];
                keySchedule[row, 2] = key[4 * row + 2];
                keySchedule[row, 3] = key[4 * row + 3];
            }

            byte[] temp = new byte[4];

            for (int row = Nk; row < Nb * (Nr + 1); ++row)
            {
                temp[0] = this.keySchedule[row - 1, 0]; temp[1] = this.keySchedule[row - 1, 1];
                temp[2] = this.keySchedule[row - 1, 2]; temp[3] = this.keySchedule[row - 1, 3];

                if (row % Nk == 0)
                {
                    temp = SubWord(RotWord(temp));

                    temp[0] = (byte)(temp[0] ^ (int)RCon.Get(row / Nk, 0));
                    temp[1] = (byte)(temp[1] ^ (int)RCon.Get(row / Nk, 1));
                    temp[2] = (byte)(temp[2] ^ (int)RCon.Get(row / Nk, 2));
                    temp[3] = (byte)(temp[3] ^ (int)RCon.Get(row / Nk, 3));
                }
                else if (Nk > 6 && (row % Nk == 4))
                {
                    temp = SubWord(temp);
                }

                keySchedule[row, 0] = (byte)(keySchedule[row - Nk, 0] ^ temp[0]);
                keySchedule[row, 1] = (byte)(keySchedule[row - Nk, 1] ^ temp[1]);
                keySchedule[row, 2] = (byte)(keySchedule[row - Nk, 2] ^ temp[2]);
                keySchedule[row, 3] = (byte)(keySchedule[row - Nk, 3] ^ temp[3]);

            }  // for loop
        }  // KeyExpansion()

        private void AddRoundKey(int round)
        {
            Print("AddRoundKey");

            for (int r = 0; r < 4; ++r)
            {
                for (int c = 0; c < 4; ++c)
                {
                    state[r, c] = (byte)(state[r, c] ^ keySchedule[(round * 4) + c, r]);
                }
            }

            PrintState();

        }  // AddRoundKey()

        private void SubBytes()
        {
            Print("SubBytes");

            for (int r = 0; r < 4; ++r)
            {
                for (int c = 0; c < 4; ++c)
                {
                    state[r, c] = SBox.Get((state[r, c] >> 4), (state[r, c] & 0x0f));
                }
            }

            PrintState();
        }  // SubBytes

        private void ShiftRows()
        {
            Print("ShiftRows");

            byte[,] temp = new byte[4, 4];
            for (int r = 0; r < 4; ++r)  // copy State into temp[]
            {
                for (int c = 0; c < 4; ++c)
                {
                    temp[r, c] = this.state[r, c];
                }
            }

            for (int r = 1; r < 4; ++r)  // shift temp into State
            {
                for (int c = 0; c < 4; ++c)
                {
                    this.state[r, c] = temp[r, (c + r) % Nb];
                }
            }

            PrintState();
        }  // ShiftRows()

        private void MixColumns()
        {
            Print("MixColumns");

            byte[,] temp = new byte[4, 4];
            for (int r = 0; r < 4; ++r)  // copy State into temp[]
            {
                for (int c = 0; c < 4; ++c)
                {
                    temp[r, c] = state[r, c];
                }
            }

            for (int c = 0; c < 4; ++c)
            {
                state[0, c] = (byte)(gfmultby02(temp[0, c]) ^ gfmultby03(temp[1, c]) ^
                                           gfmultby01(temp[2, c]) ^ gfmultby01(temp[3, c]));
                state[1, c] = (byte)(gfmultby01(temp[0, c]) ^ gfmultby02(temp[1, c]) ^
                                           gfmultby03(temp[2, c]) ^ gfmultby01(temp[3, c]));
                state[2, c] = (byte)(gfmultby01(temp[0, c]) ^ gfmultby01(temp[1, c]) ^
                                           gfmultby02(temp[2, c]) ^ gfmultby03(temp[3, c]));
                state[3, c] = (byte)(gfmultby03(temp[0, c]) ^ gfmultby01(temp[1, c]) ^
                                           gfmultby01(temp[2, c]) ^ gfmultby02(temp[3, c]));
            }

            PrintState();
        }  // MixColumns

        private byte[] SubWord(byte[] word)
        {
            byte[] result = new byte[4];
            result[0] = SBox.Get(word[0] >> 4, word[0] & 0x0f);
            result[1] = SBox.Get(word[1] >> 4, word[1] & 0x0f);
            result[2] = SBox.Get(word[2] >> 4, word[2] & 0x0f);
            result[3] = SBox.Get(word[3] >> 4, word[3] & 0x0f);
            return result;
        } //SubWord

        private byte[] RotWord(byte[] word)
        {
            byte[] result = new byte[4];
            result[0] = word[1];
            result[1] = word[2];
            result[2] = word[3];
            result[3] = word[0];
            return result;
        } //RotWord

        private static byte gfmultby01(byte b)
        {
            return b;
        }

        private static byte gfmultby02(byte b)
        {
            if (b < 0x80)
                return (byte)(b << 1);
            else
                return (byte)(b << 1 ^ 0x1b);
        }

        private static byte gfmultby03(byte b)
        {
            return (byte)(gfmultby02(b) ^ b);
        }

        private static byte gfmultby09(byte b)
        {
            return (byte)(gfmultby02(gfmultby02(gfmultby02(b))) ^
                           b);
        }

        private static byte gfmultby0b(byte b)
        {
            return (byte)(gfmultby02(gfmultby02(gfmultby02(b))) ^
                           gfmultby02(b) ^
                           b);
        }

        private static byte gfmultby0d(byte b)
        {
            return (byte)(gfmultby02(gfmultby02(gfmultby02(b))) ^
                           gfmultby02(gfmultby02(b)) ^
                           b);
        }

        private static byte gfmultby0e(byte b)
        {
            return (byte)(gfmultby02(gfmultby02(gfmultby02(b))) ^
                           gfmultby02(gfmultby02(b)) ^
                           gfmultby02(b));
        }

        public void Print(string message)
        {
            Console.WriteLine("**************"+message+ " - "+ round + "**************");
            Console.WriteLine();
        }
        public void PrintState()
        {
            if (state == null)
                return;

            for (var i = 0; i < state.Length; i++)
            {
                var column = i % 4;
                var hex = state[i / 4, column].ToString("X2");
                if (column == 3)
                {
                    Console.WriteLine("0x" + hex);
                }
                else
                {
                    Console.Write("0x" + $"{hex} ");
                }
            }

            Console.WriteLine();
        }
    }
}
