using System.Collections.Generic;
using System.Linq;

namespace QuineMcCluskey
{
    public class TruthTable
    {
        public List<Minterm>[] TruthTabales;
        private readonly int _maxNumber;

        public TruthTable(int groupsNumber)
        {
            TruthTabales = new List<Minterm>[groupsNumber];
            _maxNumber = groupsNumber;
        }

        public TruthTable()
        {
        }

        public void GroupLists(List<Minterm> minterms)
        {
            int maxNumberOfOne = _maxNumber;
            List<Minterm>[] mintermGroups = new List<Minterm>[maxNumberOfOne + 1];
            mintermGroups[0] = new List<Minterm>();
            foreach (Minterm minterm in minterms)
                if (minterm.NumberofOnes == 0)
                    mintermGroups[0].Add(minterm);

            for (int i = 1; i <= maxNumberOfOne; i++)
            {
                mintermGroups[i] = new List<Minterm>();
                foreach (Minterm minterm in minterms)
                    if (minterm.NumberofOnes == i)
                        mintermGroups[i].Add(minterm);
            }

            TruthTabales = mintermGroups;
        }

        public void SortGroupList()
        {
            List<string> binaryCodesUnOrdinate = TruthTabales.Select(truthTable => truthTable[0].BinaryCode).ToList();

            List<string> binaryCodes = SortList(binaryCodesUnOrdinate);
            List<Minterm>[] newMinterms = new List<Minterm>[binaryCodes.Count];
            
            for (int i = 0; i < binaryCodes.Count; i++) newMinterms[i] = new List<Minterm>();
            for (int i = 0; i < binaryCodes.Count; i++)
                foreach (List<Minterm> truthTables in TruthTabales)
                foreach (Minterm minterm in truthTables)
                    if (binaryCodes[i] == minterm.BinaryCode)
                        if (!newMinterms[i].Contains(minterm))
                            newMinterms[i].Add(minterm);

            for (int i = 0; i < newMinterms.Length; i++) newMinterms[i] = SortList(newMinterms[i]);
            TruthTabales = new List<Minterm>[newMinterms.Length];
            TruthTabales = newMinterms;
        }

        public static List<string> SortList(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            for (int j = i + 1; j < list.Count; j++)
                if (list[i] == list[j])
                    list[j] = " ";

            return list.Where(t => t != " ").ToList();
        }

        public static List<Minterm> SortList(List<Minterm> list)
        {
            List<Minterm> result = new List<Minterm>();
            for (int i = 0; i < list.Count; i++)
            for (int j = i + 1; j < list.Count; j++)
                if (list[i].Number == list[j].Number)
                    list[j].Number = " ";

            foreach (Minterm minterm in list)
                if (minterm.Number != " ")
                    result.Add(minterm);

            return result;
        }
    }
}