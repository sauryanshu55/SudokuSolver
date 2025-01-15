using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Algorithm;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateSudokuGrid();
        }

        private void CreateSudokuGrid()
        {
            // Define the grid size (9x9)
            int gridSize = 9;
            SudokuGrid.RowDefinitions.Clear();
            SudokuGrid.ColumnDefinitions.Clear();

            // Create row and column definitions (9 rows and 9 columns)
            for (int i = 0; i < gridSize; i++)
            {
                SudokuGrid.RowDefinitions.Add(new RowDefinition());
                SudokuGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Add 81 TextBox elements to the grid
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    TextBox numberBox = new TextBox
                    {
                        Width = 40,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        MaxLength = 1, // Allow only one character (digit)
                        Margin = new Thickness(5),
                        TextAlignment = TextAlignment.Center,
                        FontSize = 16,
                    };

                    // Assign the TextBox to the grid cell
                    Grid.SetRow(numberBox, row);
                    Grid.SetColumn(numberBox, col);
                    SudokuGrid.Children.Add(numberBox);

                    // Attach the PreviewTextInput event to restrict input to digits
                    numberBox.PreviewTextInput += NumberBox_PreviewTextInput;
                }
            }
        }

        private void CollectButton_Click(object sender, RoutedEventArgs e)
        {
            int[,] gridData = CollectGridNumbers();

            // Solve the Sudoku (assumes solveSudoku returns a tuple with solvability and solved grid)
            var (isSolvable, solvedGrid) = GFG.solveSudoku(gridData, 9);

            if (isSolvable)
            {
                // Update the Sudoku grid with the solved numbers
                DisplaySolvedSudoku(solvedGrid);
            }
            else
            {
                MessageBox.Show("The Sudoku puzzle cannot be solved.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DisplaySolvedSudoku(int[,] solvedGrid)
        {
            foreach (UIElement element in SudokuGrid.Children)
            {
                if (element is TextBox textBox)
                {
                    // Get the row and column of the TextBox
                    int row = Grid.GetRow(textBox);
                    int col = Grid.GetColumn(textBox);

                    // Update the TextBox with the solved value
                    textBox.Text = solvedGrid[row, col].ToString();
                }
            }
        }

        private int[,] CollectGridNumbers()
        {
            int gridSize = 9;
            int[,] numbers = new int[gridSize, gridSize];

            foreach (UIElement element in SudokuGrid.Children)
            {
                if (element is TextBox textBox)
                {
                    // Get the row and column of the TextBox
                    int row = Grid.GetRow(textBox);
                    int col = Grid.GetColumn(textBox);

                    // Parse the text into an integer, default to 0 if empty or invalid
                    if (int.TryParse(textBox.Text, out int value))
                    {
                        numbers[row, col] = value;
                    }
                    else
                    {
                        numbers[row, col] = 0; // Treat empty or invalid cells as 0
                    }
                }
            }


            (bool, int[,]) solvable = GFG.solveSudoku(numbers, 9);

            return numbers;
        }


        // Method to handle input validation (only allow numeric values)
        private void NumberBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]$");
            e.Handled = !regex.IsMatch(e.Text); // Reject non-digit input
        }
    }
}