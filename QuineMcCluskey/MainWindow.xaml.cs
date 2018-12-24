using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace QuineMcCluskey
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _firstSelect = true;
        private int VariableCount { get; set; }
        private const int TOTAL_MINTERM_COUNT = 64;
        private readonly Button[] _mintermButtons = new Button[TOTAL_MINTERM_COUNT];

        public MainWindow()
        {
            InitializeComponent();
            InitialazeMintermButtons();
            DisableAllMintemButton();
        }

        private void DisableAllMintemButton()
        {
            _mintermButtons?.Select(b => b)
                .ToList()
                .ForEach(button =>
                {
                    button.Content = "D";
                    button.IsEnabled = false;
                });
        }

        private static int GetMintermCount(int variableCount)
        {
            {
                return (int) Math.Pow(2, variableCount);
            }
        }

        private void InitialazeMintermButtons()
        {
            for (int i = 0; i < _mintermButtons.Length; i++) _mintermButtons[i] = (Button) FindName($"BtnMinterm{i}");
        }

        private void BtnMinterm_Click(object sender, RoutedEventArgs e)
        {
            Button mintermButton = (Button) sender;
            switch (mintermButton.Content.ToString())
            {
                case "D":
                    ToggleMintermButtonContent(mintermButton, "1");
                    break;
                case "1":
                    ToggleMintermButtonContent(mintermButton, "0");
                    break;
                case "0":
                    ToggleMintermButtonContent(mintermButton, "D");
                    break;
            }
        }

        private void ToggleMintermButtonContent(Button mintermButton, string newValue)
        {
            Dispatcher.Invoke(() => { mintermButton.Content = newValue; });
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox) sender;
            Label comboBoxLabel = (Label) comboBox.SelectedItem;

            switch (comboBoxLabel.Content)
            {
                case "2":
                    VariableCount = 2;
                    RenderMintermButtons(GetMintermCount(2));
                    break;
                case "3":
                    VariableCount = 3;
                    RenderMintermButtons(GetMintermCount(3));
                    break;
                case "4":
                    VariableCount = 4;
                    RenderMintermButtons(GetMintermCount(4));
                    break;
                case "5":
                    VariableCount = 5;
                    RenderMintermButtons(GetMintermCount(5));
                    break;
                case "6":
                    VariableCount = 6;
                    RenderMintermButtons(GetMintermCount(6));
                    break;
                case "":
                    if (_firstSelect)
                        _firstSelect = false;
                    else
                        DisableAllMintemButton();

                    break;
            }
        }

        private void RenderMintermButtons(int mintermCount)
        {
            if (mintermCount == 4)
            {
                int[] enableMintrms = new[] {0, 1, 4, 5};
                foreach (int i in enableMintrms)
                    _mintermButtons[i].IsEnabled = true;

                for (int i = 0; i < TOTAL_MINTERM_COUNT; i++)
                    if (!enableMintrms.Contains(i))
                        _mintermButtons[i].IsEnabled = false;
            }
            else
            {
                for (int i = 0; i < TOTAL_MINTERM_COUNT; i++) _mintermButtons[i].IsEnabled = i < mintermCount;
            }
        }


        private void BtnSimplify_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxVariableCount.SelectedIndex == 0)
            {
                MessageBox.Show("Error");
                return;
            }

            List<Minterm> minterms = _mintermButtons
                .Where(b =>
                    b.IsEnabled &&
                    (b.Content.Equals("1") ||
                     b.Content.Equals("D")))
                .Select(b =>
                    new Minterm(Regex.Match(b.Name, @"\d+").Value))
                .ToList();

            if (ComboBoxVariableCount.SelectedIndex == 1)
            {
                try
                {
                    Minterm minterm4 = minterms.First(m => m.Number == "4");
                    minterms.Remove(minterm4);
                    minterms.Add(new Minterm("2"));
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    Console.WriteLine(invalidOperationException);
                }

                try
                {
                    Minterm minterm5 = minterms.First(m => m.Number == "5");
                    minterms.Remove(minterm5);
                    minterms.Add(new Minterm("3"));
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    Console.WriteLine(invalidOperationException);
                }
            }

            new Thread(() =>
            {
                List<Minterm> orginMinterms = minterms;

                List<int> numberOfOne = new List<int>();
                foreach (Minterm minterm in minterms)
                    numberOfOne.Add(minterm.NumberofOnes);

                int maxNumberOfOne = numberOfOne.Max();
                foreach (Minterm minterm in minterms)
                    while (minterm.BinaryCode.Length < VariableCount)
                        minterm.BinaryCode = '0' + minterm.BinaryCode;

                TruthTable groupTruthTable = new TruthTable(maxNumberOfOne);
                groupTruthTable.GroupLists(minterms);

                List<List<Implicant>> firstPrimeImplicants = new List<List<Implicant>>();
                List<Implicant> implicants = (from mintermList in groupTruthTable.TruthTabales
                    from minterm in mintermList
                    select new Implicant(new List<Minterm>() {minterm})).ToList();

                firstPrimeImplicants.Add(implicants);
                TruthTable gpTruthTable = groupTruthTable;
                bool x = true;
                while (true)
                {
                    TruthTable gp;
                    if (x)
                    {
                        gp = GroupMintermCreate(gpTruthTable, true);
                        x = false;
                    }
                    else
                    {
                        gp = GroupMintermCreate(gpTruthTable, false);
                    }

                    gp.SortGroupList();
                    if (gp.TruthTabales.Length == 0) break;
                    List<Implicant> implicantsInWhileList = new List<Implicant>();
                    foreach (List<Minterm> mintrms in gp.TruthTabales)
                    {
                        List<Minterm> implicantMinterms = new List<Minterm>();
                        foreach (Minterm minterm in mintrms)
                            implicantMinterms.Add(minterm);

                        Implicant implicant = new Implicant(implicantMinterms);
                        implicantsInWhileList.Add(implicant);
                    }

                    firstPrimeImplicants.Add(implicantsInWhileList);
                    gpTruthTable = gp;
                }


                for (int i = firstPrimeImplicants.Count - 1; i >= 0; i--)
                for (int j = 0; j < firstPrimeImplicants[i].Count; j++)
                    if (firstPrimeImplicants[i][j].Status)
                        for (int k = 0; k < i; k++)
                        for (int l = 0; l < firstPrimeImplicants[k].Count; l++)
                            if (CheckEqual(firstPrimeImplicants[i][j], firstPrimeImplicants[k][l]))
                                firstPrimeImplicants[k][l].Status = false;

                List<Implicant> primeImplicants = new List<Implicant>();
                foreach (List<Implicant> implicantList in firstPrimeImplicants)
                foreach (Implicant implicant in implicantList)
                    if (implicant.Status)
                        primeImplicants.Add(implicant);


                List<Implicant> finalImplicants = new List<Implicant>();
                bool[] mintermsUse = new bool[orginMinterms.Count];
                for (int i = 0; i < mintermsUse.Length; i++) mintermsUse[i] = false;

                bool[,] implicantsTable = new bool[primeImplicants.Count, orginMinterms.Count];
                for (int i = 0; i < orginMinterms.Count; i++)
                for (int j = 0; j < primeImplicants.Count; j++)
                    if (primeImplicants[j].Minterms.Contains(orginMinterms[i]))
                        implicantsTable[j, i] = true;
                    else
                        implicantsTable[j, i] = false;

                while (!CheckMinterUse(mintermsUse))
                {
                    List<int> essentialList = new List<int>();
                    for (int i = 0; i < implicantsTable.GetLength(1); i++)
                        if (CheckEssentialImplicants(implicantsTable, i))
                            essentialList.Add(SendIndexOfEssential(implicantsTable, i));

                    foreach (int essential in essentialList)
                    {
                        for (int j = 0; j < orginMinterms.Count; j++)
                            if (primeImplicants[essential].Minterms.Contains(orginMinterms[j]))
                                mintermsUse[j] = true;
                        finalImplicants.Add(primeImplicants[essential]);
                    }

                    if (essentialList.Count > 0)
                    {
                        implicantsTable = UpdateImplicantsTable(implicantsTable, essentialList);
                        continue;
                    }

                    for (int i = 0; i < implicantsTable.GetLength(1); i++)
                    for (int j = 0; j < implicantsTable.GetLength(1); j++)
                        if (NumberOfDifferences(implicantsTable, i, j, false) == 1)
                            implicantsTable = UpdateImplicantsTable(implicantsTable,
                                NumberOfTrue(implicantsTable, i, false) > NumberOfTrue(implicantsTable, j, false)
                                    ? i
                                    : j, false);


                    for (int i = 0; i < implicantsTable.GetLength(0); i++)
                    for (int j = 0; j < implicantsTable.GetLength(0); j++)
                    {
                        if (i == j) continue;

                        if (NumberOfDifferences(implicantsTable, i, j, true) == 1)
                            implicantsTable = UpdateImplicantsTable(implicantsTable,
                                NumberOfTrue(implicantsTable, i, true) > NumberOfTrue(implicantsTable, j, true) ? j : i,
                                true);
                        else if (NumberOfDifferences(implicantsTable, i, j, true) == 0)
                            implicantsTable = UpdateImplicantsTable(implicantsTable, j, true);
                    }
                }

                List<Implicant> finalImplicantSorted = new List<Implicant>();
                foreach (Implicant implicant in finalImplicants)
                    if (!finalImplicantSorted.Contains(implicant))
                        finalImplicantSorted.Add(implicant);


                string result = "";
                for (int i = 0; i < finalImplicantSorted.Count; i++)
                {
                    result += finalImplicantSorted[i].ToString();
                    if (i % 10 == 0 && i != 0) result += "\n";
                }

                Dispatcher.Invoke(() => { LblOutput.Content = result.Remove(result.Length - 1); });
            }).Start();
        }

        public static TruthTable GroupMintermCreate(TruthTable groupTruthTable, bool x)
        {
            List<List<Minterm>> mintermList1 = new List<List<Minterm>>();
            if (x == false)
                for (int i = 0; i < groupTruthTable.TruthTabales.Length; i++)
                for (int j = 0; j < groupTruthTable.TruthTabales.Length; j++)
                {
                    if (i == j) continue;

                    if (CheckDifferent(groupTruthTable.TruthTabales[i][0].BinaryCode,
                        groupTruthTable.TruthTabales[j][0].BinaryCode))
                        mintermList1.Add(NewGroupMake(groupTruthTable, i, j));
                }

            else
                for (int i = 0; i < groupTruthTable.TruthTabales.Length - 1; i++)
                for (int j = 0; j < groupTruthTable.TruthTabales[i].Count; j++)
                for (int k = 0; k < groupTruthTable.TruthTabales[i + 1].Count; k++)
                    if (CheckDifferent(groupTruthTable.TruthTabales[i][j].BinaryCode,
                        groupTruthTable.TruthTabales[i + 1][k].BinaryCode))
                        mintermList1.Add(NewGroupMake(groupTruthTable, i, j, k));

            TruthTable truthTableGroup2 = new TruthTable(mintermList1.Count);

            for (int i = 0; i < mintermList1.Count; i++) truthTableGroup2.TruthTabales[i] = mintermList1[i];

            return truthTableGroup2;
        }

        public static List<Minterm> NewGroupMake(TruthTable groupTruthTable, int i, int j, int k)
        {
            string result = GroupMake(groupTruthTable.TruthTabales[i][j].BinaryCode,
                groupTruthTable.TruthTabales[i + 1][k].BinaryCode);
            Minterm minterm1 = new Minterm(groupTruthTable.TruthTabales[i][j].Number);
            Minterm minterm2 = new Minterm(groupTruthTable.TruthTabales[i + 1][k].Number);
            minterm1.BinaryCode = result;
            minterm2.BinaryCode = result;
            List<Minterm> mintermsList = new List<Minterm>() {minterm1, minterm2};
            return mintermsList;
        }

        public static List<Minterm> NewGroupMake(TruthTable groupTruthTable, int i, int j)
        {
            string result = GroupMake(groupTruthTable.TruthTabales[i][0].BinaryCode,
                groupTruthTable.TruthTabales[j][0].BinaryCode);
            List<Minterm> mintermsList = new List<Minterm>();
            for (int k = 0; k < groupTruthTable.TruthTabales[i].Count; k++)
            {
                Minterm minterm = new Minterm(groupTruthTable.TruthTabales[i][k].Number) {BinaryCode = result};
                mintermsList.Add(minterm);
            }

            for (int k = 0; k < groupTruthTable.TruthTabales[j].Count; k++)
            {
                Minterm minterm = new Minterm(groupTruthTable.TruthTabales[j][k].Number) {BinaryCode = result};
                mintermsList.Add(minterm);
            }

            return mintermsList;
        }

        public static bool CheckDifferent(string a, string b)
        {
            if (a.Length != b.Length) return false;
            int count = a.Where((t, i) => t != b[i]).Count();
            return count <= 1;
        }

        public static string GroupMake(string a, string b)
        {
            string result = string.Empty;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    result += '-';
                else
                    result += a[i];

            return result;
        }

        public static bool CheckEqual(Implicant gic1, Implicant sic2)
        {
            bool restult = true;
            foreach (Minterm minterm in sic2.Minterms)
                if (!gic1.Minterms.Contains(minterm))
                    restult = false;
            return restult;
        }

        public static bool CheckEssentialImplicants(bool[,] table, int indexJ)
        {
            int sum = 0;
            for (int i = 0; i < table.GetLength(0); i++)
                if (table[i, indexJ])
                    sum++;

            return sum == 1;
        }

        public static int SendIndexOfEssential(bool[,] table, int indexJ)
        {
            int result = -1;
            for (int i = 0; i < table.GetLength(0); i++)
                if (table[i, indexJ])
                    result = i;

            return result;
        }

        public static bool CheckMinterUse(bool[] array)
        {
            return array.All(t => t);
        }

        public static bool[,] UpdateImplicantsTable(bool[,] firstTable, List<int> rowIndexes)
        {
            List<int> columnIndexes = new List<int>();
            foreach (int rIdx in rowIndexes)
                for (int j = 0; j < firstTable.GetLength(1); j++)
                    if (firstTable[rIdx, j])
                    {
                        columnIndexes.Add(j);
                        firstTable[rIdx, j] = false;
                    }

            foreach (int cIdx in columnIndexes)
                for (int j = 0; j < firstTable.GetLength(0); j++)
                    if (firstTable[j, cIdx])
                        firstTable[j, cIdx] = false;

            return firstTable;
        }

        public static bool[,] UpdateImplicantsTable(bool[,] firstTable, int index, bool row)
        {
            if (row)
            {
                for (int i = 0; i < firstTable.GetLength(1); i++) firstTable[index, i] = false;

                return firstTable;
            }

            for (int i = 0; i < firstTable.GetLength(0); i++) firstTable[i, index] = false;

            return firstTable;
        }

        public static int NumberOfDifferences(bool[,] table, int index1, int index2, bool row)
        {
            int sum = 0;
            if (!row) return sum;
            for (int i = 0; i < table.GetLength(1); i++)
                if (table[index1, i] != table[index2, i])
                    sum++;
                else
                    for (int j = 0; j < table.GetLength(0); j++)
                        if (table[j, index1] != table[j, index2])
                            sum++;

            return sum;
        }

        public static int NumberOfTrue(bool[,] table, int index, bool row)
        {
            int sum = 0;
            if (!row) return sum;
            for (int i = 0; i < table.GetLength(1); i++)
                if (table[index, i])
                    sum++;
                else
                    for (int j = 0; j < table.GetLength(0); j++)
                        if (table[j, index])
                            sum++;

            return sum;
        }
    }
}