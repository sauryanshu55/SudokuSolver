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

            var preFilledCells = new Dictionary<(int row, int col), string>
                    {
                        {(0, 0), "3"}, {(0, 2), "6"}, {(0, 3), "5"}, {(0, 5), "8"}, {(0, 6), "4"},
                        {(1, 0), "5"}, {(1, 1), "2"},
                        {(2, 1), "8"}, {(2, 2), "7"}, {(2, 7), "3"}, {(2, 8), "1"},
                        {(3, 2), "3"}, {(3, 4), "1"}, {(3, 7), "8"},
                        {(4, 0), "9"}, {(4, 3), "8"}, {(4, 4), "6"}, {(4, 5), "3"}, {(4, 8), "5"},
                        {(5, 1), "5"}, {(5, 4), "9"}, {(5, 6), "6"},
                        {(6, 0), "1"}, {(6, 1), "3"}, {(6, 6), "2"}, {(6, 7), "5"},
                        {(7, 7), "7"}, {(7, 8), "4"},
                        {(8, 2), "5"}, {(8, 3), "2"}, {(8, 5), "6"}, {(8, 6), "3"}
                    };


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
                        Margin = new Thickness(2),
                        TextAlignment = TextAlignment.Center,
                        FontSize = 16,
                        BorderThickness = new Thickness(
                    col % 3 == 0 ? 5 : 0.5,        // Left
                    row % 3 == 0 ? 5 : 0.5,        // Top
                    (col + 1) % 3 == 0 ? 5 : 0.5,  // Right
                    (row + 1) % 3 == 0 ? 5 : 0.5   // Bottom
                ),
                        BorderBrush=Brushes.Black,
                    };



                    // Check if this cell is pre-filled
                    if (preFilledCells.TryGetValue((row, col), out string? value))
                    {
                        numberBox.Text = value;
                        numberBox.Foreground = Brushes.Red; // Optional: Different color for pre-filled numbers
                        numberBox.Tag = "UserInput";
                    }

                    // Assign the TextBox to the grid cell;
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
            var isSolved = GFG.IsValid(solvedGrid);

            if (isSolvable && isSolved)
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

                    // Check if the TextBox was user-inputted (i.e., it has the "UserInput" tag)
                    if (textBox.Tag?.ToString() == "UserInput")
                    {
                        // User input: Set text and leave it red
                        textBox.Text = solvedGrid[row, col].ToString();
                        textBox.Foreground = Brushes.Red;
                    }
                    else
                    {
                        // Automatically filled: Set text without red color
                        textBox.Text = solvedGrid[row, col].ToString();
                        textBox.Foreground = Brushes.Black;  // Reset to default color
                    }
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

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement element in SudokuGrid.Children)
            {
                if (element is TextBox numberBox)
                {

                    numberBox.Text = string.Empty; // Clear user-entered text
                    numberBox.Tag = null;

                }
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            CreateSudokuGrid();
        }

        // Method to handle input validation (only allow numeric values)
        private void NumberBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]$");
            e.Handled = !regex.IsMatch(e.Text); // Reject non-digit input

            if (sender is TextBox textBox)
            {
                textBox.Foreground = Brushes.Red;
                textBox.Tag = "UserInput";
            }
        }
    }
}