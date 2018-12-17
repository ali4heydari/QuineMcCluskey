using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuineMcCluskey
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool FirstSelect = true;
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
                .ForEach((button =>
                {
                    button.Content = "D";
                    button.IsEnabled = false;
                }));
        }

        private int GetMintermCount(int variableCount)
        {
            {
                return (int) Math.Pow(2, (double) (variableCount));
            }
        }

        private void InitialazeMintermButtons()
        {
            for (int i = 0; i < this._mintermButtons.Length; i++)
            {
                this._mintermButtons[i] = (Button) this.FindName($"BtnMinterm{i}");
            }
        }

        private void BtnMinterm_Click(object sender, RoutedEventArgs e)
        {
            Button mintermButton = (Button) (sender);
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
                    if (FirstSelect)
                        FirstSelect = false;
                    else
                    {
                        DisableAllMintemButton();
                    }

                    break;
            }
        }

        private void RenderMintermButtons(int mintermCount)
        {
            if (mintermCount == 4)
            {
                int[] enableMintrms = new[] {0, 1, 4, 5};
                for (int i = 0; i < enableMintrms.Length; i++)
                {
                    this._mintermButtons[enableMintrms[i]].IsEnabled = true;
                }

                for (int i = 0; i < TOTAL_MINTERM_COUNT; i++)
                {
                    if (!enableMintrms.Contains(i))
                        _mintermButtons[i].IsEnabled = false;
                }
            }
            else
            {
                for (int i = 0; i < TOTAL_MINTERM_COUNT; i++)
                {
                    if (i < mintermCount)
                        this._mintermButtons[i].IsEnabled = true;
                    else
                    {
                        this._mintermButtons[i].IsEnabled = false;
                    }
                }
            }
        }

        private void BtnSimplify_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}