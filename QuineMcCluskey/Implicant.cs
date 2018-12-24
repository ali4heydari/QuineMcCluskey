using System;
using System.Collections.Generic;

namespace QuineMcCluskey
{
    public class Implicant : IEquatable<Implicant>
    {
        public List<Minterm> Minterms { get; set; }
        public string BinaryCode { get; }
        public bool Status { get; set; }

        public Implicant(List<Minterm> minterms)
        {
            Minterms = minterms;
            Status = true;
            BinaryCode = minterms[0].BinaryCode;
        }

        public override string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < BinaryCode.Length; i++)
            {
                string innerResult = string.Empty;
                switch (BinaryCode[i])
                {
                    case '1':
                        innerResult += ((char) (65 + i)).ToString();
                        break;
                    case '0':
                        innerResult += $"{'~' + ((char) (65 + i)).ToString()}";
                        break;
                }

                result += innerResult;
            }

            return result + "+";
        }

        public bool Equals(Implicant other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || string.Equals(BinaryCode, other.BinaryCode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Implicant) obj);
        }

        public override int GetHashCode()
        {
            return BinaryCode != null ? BinaryCode.GetHashCode() : 0;
        }
    }
}