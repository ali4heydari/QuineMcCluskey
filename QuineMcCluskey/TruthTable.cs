using System.Collections.Generic;
using System.Text;

namespace QuineMcCluskey
{
    class TruthTable
    {
        public string[] Titles;
        public List<List<LogicState>> InputStates;
        public List<LogicState> OutputStates;

        public TruthTable(string[] titles, List<List<LogicState>> inputStates, List<LogicState> outputStates)
        {
            Titles = titles;
            InputStates = inputStates;
            OutputStates = outputStates;
        }

        public void SetInputStates(List<List<LogicState>> newStates)
        {
            InputStates = newStates;
        }

        public void SetOutputStates(List<LogicState> newStates)
        {
            OutputStates = newStates;
        }

        public void SetOutputStatesToTrue()
        {
            int count = InputStates.Count;
            OutputStates = new List<LogicState>();
            for (int i = 0; i < count; i++) OutputStates.Add(LogicState.True);
        }
    }
}