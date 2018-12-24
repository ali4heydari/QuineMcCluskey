using System;
using System.Linq;

namespace QuineMcCluskey
{
    public class Minterm : IEquatable<Minterm>
    {
        public string Number { get; set; }
        public string BinaryCode { get; set; }
        public int NumberofOnes { get; set; }

        public Minterm(string number)
        {
            Number = number;
            BinaryCode = ToBinaryCode(number);
            NumberofOnes = GetOneCountAtBinaryCode(ToBinaryCode(number));
        }

        private static int GetOneCountAtBinaryCode(string binaryCode)
        {
            return binaryCode.Count(t => t == '1');
        }

        private static string ToBinaryCode(string number)
        {
            int numberBinary = int.Parse(number);
            int a = 0;
            for (int i = 0; numberBinary > 0; i++)
            {
                a += numberBinary % 2 * (int) Math.Pow(10, i);
                numberBinary /= 2;
            }

            return a.ToString();
        }


        public bool Equals(Minterm other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Number, other.Number);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Minterm) obj);
        }

        public override int GetHashCode()
        {
            return Number != null ? Number.GetHashCode() : 0;
        }
    }
}