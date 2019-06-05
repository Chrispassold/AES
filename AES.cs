using System;

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
        private byte[][] matrixMix = new byte[4][] {
            new byte[] { 2, 3, 1, 1 },
            new byte[] { 1, 2, 3, 1 },
            new byte[] { 1, 1, 2, 3 },
            new byte[] { 3, 1, 1, 2 }
        };

        private int round = 0;

        public AES(byte[] keyBytes)
        {
            key = new byte[KEY_SIZE * 4];
            keyBytes.CopyTo(key, 0);
            KeyExpansion();
        }

        public byte[] perform(byte[] input)
        {
            // byte[] result = new byte[input.Length * 16];
            // for (int c = 0; c < input.Length; c++)
            // {
            //     byte[] bytes = new byte[4 * BLOCK_SIZE];
            //     for (int i = 0; i < (4 * BLOCK_SIZE); ++i)
            //     {
            //         bytes[i] = input[i];
            //     }
            //     byte[] perform = performBlock(bytes);

            //     perform.CopyTo(result, 16 * c);
            // }

            // int padding = 16 - input.Length % 16;
            // for (int i = 0; i < padding; i++)
            // {
            //     result[input.Length + i] = Convert.ToByte(padding);
            // }
            byte[] result = performBlock(input);

            Print("Result");
            Print(result);

            return result;
        }

        private byte[] performBlock(byte[] input)
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
                    temp[row, column] = (byte)(
                        VerifyTableEAndTableL(state[0, column], matrixMix[row][0]) ^
                        VerifyTableEAndTableL(state[1, column], matrixMix[row][1]) ^
                        VerifyTableEAndTableL(state[2, column], matrixMix[row][2]) ^
                        VerifyTableEAndTableL(state[3, column], matrixMix[row][3])
                    );
                }
            }
            state = temp;

            PrintState();
        }
        private int VerifyTableEAndTableL(byte value_1, byte value_2)
        {
            if (value_1.Equals(0) || value_2.Equals(0))
            {
                return 0;
            }
            if (value_1.Equals(1))
            {
                return value_2;
            }
            if (value_2.Equals(1))
            {
                return value_1;
            }
            var tableE = TableE.table;
            var tableL = TableL.table;

            var l1 = tableL.Get(this.GetLeftTerm(value_1), this.GetRightTerm(value_1));
            var l2 = tableL.Get(this.GetLeftTerm(value_2), this.GetRightTerm(value_2));
            var tableLResult = (byte) (ValidateLength(l1 + l2));

            return tableE.Get(this.GetLeftTerm(tableLResult), this.GetRightTerm(tableLResult));
        }
        private int ValidateLength(int value)
        {
            return value > 255 ? value - 255 : value;
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

        public void Print(string message)
        {
            Console.WriteLine("**************" + message + " - " + round + "**************");
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

        public void Print(byte[] bytes)
        {
            if (bytes == null)
                return;

            for (var i = 0; i < 16; i += 4)
            {
                for (var j = 0; j < BLOCK_SIZE; j++)
                {
                    var hex = bytes[i + j].ToString("X2");
                    Console.Write("0x" + $"{hex} ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}
