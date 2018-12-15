using System;
using System.Collections.Generic;

namespace QuineMcCluskey
{
    class PrimeImplicant
    {
        public readonly List<int> AffectedRows;
        public readonly List<LogicState> TruthTableRow;

        public PrimeImplicant(List<LogicState> truthTableRow)
        {
            this.TruthTableRow = truthTableRow;
            this.AffectedRows = GetAffectedRowsFromTruthTableRow(truthTableRow);
        }

        private List<int> GetAffectedRowsFromTruthTableRow(List<LogicState> truthTableRow)
        {
            List<int> affectedRows = GetPossibleRowsForTruthTable(truthTableRow.Count);
            for (int i = 0; i < affectedRows.Count; i++)
                if (!AndConjunction(IntToRow(affectedRows[i], truthTableRow.Count), truthTableRow))
                {
                    affectedRows.RemoveAt(i);
                    i--;
                }

            return affectedRows;
        }

        private List<int> GetPossibleRowsForTruthTable(int columns)
        {
            List<int> rows = new List<int>();
            for (int i = 0; i < Math.Pow(2, columns); i++) rows.Add(i);
            return rows;
        }

        private List<LogicState> IntToRow(int x, int columns)
        {
            List<LogicState> row = new List<LogicState>(columns);
            for (int i = columns - 1; i >= 0; i--) row.Add((x & (1 << i)) == 0 ? LogicState.False : LogicState.True);
            return row;
        }

        private bool AndConjunction(List<LogicState> a, List<LogicState> b)
        {
            if (a.Count != b.Count) throw new ArgumentException();

            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] == b[i] || a[i] == LogicState.DontCare || b[i] == LogicState.DontCare) continue;

                return false;
            }

            return true;
        }
    }
}