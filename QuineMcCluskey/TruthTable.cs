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
            this.Titles = titles;
            this.InputStates = inputStates;
            this.OutputStates = outputStates;
        }

        public void SetInputStates(List<List<LogicState>> newStates)
        {
            this.InputStates = newStates;
        }

        public void SetOutputStates(List<LogicState> newStates)
        {
            this.OutputStates = newStates;
        }

        public void SetOutputStatesToTrue()
        {
            int count = InputStates.Count;
            this.OutputStates = new List<LogicState>();
            for (int i = 0; i < count; i++)
            {
                this.OutputStates.Add(LogicState.True);
            }
        }

    }
}
