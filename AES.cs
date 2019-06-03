﻿using System;

namespace AESv2
{
    class AES
    {
        private int BLOCK_SIZE = 4;
        private int KEY_SIZE = 4;
        private int TOTAL_ROUNDS = 10;

        private byte[] key;
        private byte[,] keySchedule;

        private byte[,] state;

        private int round = 0;

        public AES(byte[] keyBytes)
        {
            key = new byte[KEY_SIZE * 4];
            keyBytes.CopyTo(key, 0);
            KeyExpansion();
        }

        public byte[] perform(byte[] input)
        {
            state = new byte[4, BLOCK_SIZE];
            for (int i = 0; i < (4 * BLOCK_SIZE); ++i)
            {
                state[i % 4, i / 4] = input[i];
            }
            Print("Texto Simples");
            PrintState();

            AddRoundKey(0);

            for (round = 1; round <= (TOTAL_ROUNDS - 1); ++round)
            {
                SubBytes();
                ShiftRows();
                MixColumns();
                AddRoundKey(round);
            }

            SubBytes();
            ShiftRows();
            AddRoundKey(TOTAL_ROUNDS);

            byte[] output = new byte[16];
            for (int i = 0; i < (4 * BLOCK_SIZE); ++i)
            {
                output[i] = state[i % 4, i / 4];
            }

            return output;
        }

        private void KeyExpansion()
        {

            Print("KeyExpansion");

            keySchedule = new byte[BLOCK_SIZE * (TOTAL_ROUNDS + 1), 4];  // 4 columns of bytes corresponds to a word

            for (int row = 0; row < KEY_SIZE; ++row)
            {
                keySchedule[row, 0] = key[4 * row];
                keySchedule[row, 1] = key[4 * row + 1];
                keySchedule[row, 2] = key[4 * row + 2];
                keySchedule[row, 3] = key[4 * row + 3];
            }

            byte[] temp = new byte[4];

            for (int row = KEY_SIZE; row < BLOCK_SIZE * (TOTAL_ROUNDS + 1); ++row)
            {
                temp[0] = this.keySchedule[row - 1, 0]; temp[1] = this.keySchedule[row - 1, 1];
                temp[2] = this.keySchedule[row - 1, 2]; temp[3] = this.keySchedule[row - 1, 3];

                if (row % KEY_SIZE == 0)
                {
                    temp = SubWord(RotWord(temp));

                    temp[0] = (byte)(temp[0] ^ (int)RCon.Get(row / KEY_SIZE, 0));
                    temp[1] = (byte)(temp[1] ^ (int)RCon.Get(row / KEY_SIZE, 1));
                    temp[2] = (byte)(temp[2] ^ (int)RCon.Get(row / KEY_SIZE, 2));
                    temp[3] = (byte)(temp[3] ^ (int)RCon.Get(row / KEY_SIZE, 3));
                }
                else if (KEY_SIZE > 6 && (row % KEY_SIZE == 4))
                {
                    temp = SubWord(temp);
                }

                keySchedule[row, 0] = (byte)(keySchedule[row - KEY_SIZE, 0] ^ temp[0]);
                keySchedule[row, 1] = (byte)(keySchedule[row - KEY_SIZE, 1] ^ temp[1]);
                keySchedule[row, 2] = (byte)(keySchedule[row - KEY_SIZE, 2] ^ temp[2]);
                keySchedule[row, 3] = (byte)(keySchedule[row - KEY_SIZE, 3] ^ temp[3]);

            }
        }

        private void AddRoundKey(int round)
        {
            Print("AddRoundKey");

            for (int row = 0; row < 4; ++row)
            {
                for (int column = 0; column < 4; ++column)
                {
                    state[row, column] = (byte)(state[row, column] ^ keySchedule[(round * 4) + column, row]);
                }
            }
            PrintState();
        }

        private void SubBytes()
        {
            Print("SubBytes");

            for (int row = 0; row < 4; ++row)
            {
                for (int column = 0; column < 4; ++column)
                {
                    state[row, column] = SBox.Get((this.GetLeftTerm(state[row, column])), (this.GetRightTerm(state[row, column])));
                }
            }

            PrintState();
        }

        private void ShiftRows()
        {
            Print("ShiftRows");

            byte[,] temp = new byte[4, 4];
            for (int row = 0; row < 4; ++row)
            {
                for (int column = 0; column < 4; ++column)
                {
                    temp[row, column] = this.state[row, column];
                }
            }
            for (int row = 1; row < 4; ++row)
            {
                for (int column = 0; column < 4; ++column)
                {
                    this.state[row, column] = temp[row, (column + row) % BLOCK_SIZE];
                }
            }

            PrintState();
        }

        private void MixColumns()
        {
            Print("MixColumns");

            byte[,] temp = new byte[4, 4];
            for (int row = 0; row < 4; ++row)
            {
                for (int column = 0; column < 4; ++column)
                {
                    temp[row, column] = state[row, column];
                }
            }

            for (int column = 0; column < 4; ++column)
            {
                state[0, column] = (byte)(gfmultby02(temp[0, column]) ^ gfmultby03(temp[1, column]) ^
                                           gfmultby01(temp[2, column]) ^ gfmultby01(temp[3, column]));
                state[1, column] = (byte)(gfmultby01(temp[0, column]) ^ gfmultby02(temp[1, column]) ^
                                           gfmultby03(temp[2, column]) ^ gfmultby01(temp[3, column]));
                state[2, column] = (byte)(gfmultby01(temp[0, column]) ^ gfmultby01(temp[1, column]) ^
                                           gfmultby02(temp[2, column]) ^ gfmultby03(temp[3, column]));
                state[3, column] = (byte)(gfmultby03(temp[0, column]) ^ gfmultby01(temp[1, column]) ^
                                           gfmultby01(temp[2, column]) ^ gfmultby02(temp[3, column]));
            }

            PrintState();
        }

        private byte[] SubWord(byte[] word)
        {
            byte[] result = new byte[4];
            result[0] = SBox.Get(this.GetLeftTerm(word[0]), this.GetRightTerm(word[0]));
            result[1] = SBox.Get(this.GetLeftTerm(word[1]), this.GetRightTerm(word[1]));
            result[2] = SBox.Get(this.GetLeftTerm(word[2]), this.GetRightTerm(word[2]));
            result[3] = SBox.Get(this.GetLeftTerm(word[3]), this.GetRightTerm(word[3]));
            return result;
        }

        private byte[] RotWord(byte[] word)
        {
            byte[] result = new byte[4];
            result[0] = word[1];
            result[1] = word[2];
            result[2] = word[3];
            result[3] = word[0];
            return result;
        }
        private byte GetLeftTerm(byte b)
        {
            return (byte)(b >> 4);
        }
        private byte GetRightTerm(byte b)
        {
            return (byte)(b & 0x0f);
        }
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
