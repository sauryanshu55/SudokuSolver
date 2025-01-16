using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Algorithm;

namespace SudokuSolver
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateSudokuGrid();
        }

        /*
         * Function that creates and populates the Sudoku Grid. Using best programming principles, I used a dymanic grid
         * in the xaml file, rather than code all 81 lines of the 9x9 grid. 
         * */
        private void CreateSudokuGrid()
        {
            int gridSize = 9;
            SudokuGrid.RowDefinitions.Clear();
            SudokuGrid.ColumnDefinitions.Clear();

            // The tuples in this dictionary define what cells are pre-filled, since the user is provided with a sample Sudoku board
            // on startup
            var preFilledCells = new Dictionary<(int row, int col), string>
                    {
                        {(0, 0), "3"}, {(0, 2), "6"}, {(0, 3), "5"}, {(0, 5), "8"}, {(0, 6), "4"},
                        {(1, 0), "5"}, {(1, 1), "2"}, {(2, 1), "8"}, {(2, 2), "7"}, {(2, 7), "3"}, 
                        {(2, 8), "1"}, {(3, 2), "3"}, {(3, 4), "1"}, {(3, 7), "8"}, {(4, 0), "9"}, 
                        {(4, 3), "8"}, {(4, 4), "6"}, {(4, 5), "3"}, {(4, 8), "5"}, {(5, 1), "5"}, 
                        {(5, 4), "9"}, {(5, 6), "6"}, {(6, 0), "1"}, {(6, 1), "3"}, {(6, 6), "2"}, 
                        {(6, 7), "5"}, {(7, 7), "7"}, {(7, 8), "4"}, {(8, 2), "5"}, {(8, 3), "2"}, 
                        {(8, 5), "6"}, {(8, 6), "3"}
                    };

            for (int i = 0; i < gridSize; i++)
            {
                SudokuGrid.RowDefinitions.Add(new RowDefinition());
                SudokuGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Adding the 81 TextBoxes to the grid
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
                        MaxLength = 1,
                        Margin = new Thickness(2),
                        TextAlignment = TextAlignment.Center,
                        FontSize = 16,
                        BorderThickness = new Thickness( // I am using selective thickness for boxes to recreate the 3x3 boxes in Sudoku
                                            col % 3 == 0 ? 5 : 0.5,        // Left
                                            row % 3 == 0 ? 5 : 0.5,        // Top
                                            (col + 1) % 3 == 0 ? 5 : 0.5,  // Right
                                            (row + 1) % 3 == 0 ? 5 : 0.5   // Bottom
                        ),
                        BorderBrush=Brushes.Black,
                    };



                    // Checking if this cell is pre-filled. The numbers given to the progam by the user is gonna be red, the ones the computer fills up 
                    // is gonna be black.
                    if (preFilledCells.TryGetValue((row, col), out string? value))
                    {
                        numberBox.Text = value;
                        numberBox.Foreground = Brushes.Red;
                        numberBox.Tag = "UserInput";
                    }


                    Grid.SetRow(numberBox, row);
                    Grid.SetColumn(numberBox, col);
                    SudokuGrid.Children.Add(numberBox);

                    // Attaching the PreviewTextInput event to restrict input to 1 digits
                    numberBox.PreviewTextInput += NumberBox_PreviewTextInput;
                }
            }
        }

        /*
         * This method ensures that the input to the Grid is always a non-negative integer
         * There is also an added functionality, that the userinput is red in color
         */
        private void NumberBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]$");
            e.Handled = !regex.IsMatch(e.Text); 

            if (sender is TextBox textBox)
            {
                textBox.Foreground = Brushes.Red;
                textBox.Tag = "UserInput";
            }
        }

        /*
         * Event handler for the Solve Sudoku Button
         */
        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            int[,] gridData = CollectGridNumbers();

            // Solving the Sudoku (the algorithm is in algorithm.cs file)
            var (isSolvable, solvedGrid) = SudokuAlgorithm.solveSudoku(gridData, 9);
            var isSolved = SudokuAlgorithm.IsValid(solvedGrid);

            // If the board is solvable and the sudoku is solved, then displaying it
            if (isSolvable && isSolved)
            {
                DisplaySolvedSudoku(solvedGrid);
            }
            else // Else, showing in message dialog that it can't be solved.
            {
                MessageBox.Show("This Sudoku puzzle cannot be solved.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /*
         * This is a helper function that collects all the numbers from the Grid in a 2x2 array.
         * */
        private int[,] CollectGridNumbers()
        {
            int gridSize = 9;
            int[,] numbers = new int[gridSize, gridSize];

            foreach (UIElement element in SudokuGrid.Children)
            {
                if (element is TextBox textBox)
                {
                    int row = Grid.GetRow(textBox);
                    int col = Grid.GetColumn(textBox);

                    // Parsing the text into an integer. i'm seeting the default to 0 if empty 
                    if (int.TryParse(textBox.Text, out int value))
                    {
                        numbers[row, col] = value;
                    }
                    else
                    {
                        numbers[row, col] = 0;
                    }
                }
            }

            return numbers;
        }

        /*
         * This function displays the Solved Sudoku
         */
        private void DisplaySolvedSudoku(int[,] solvedGrid)
        {
            foreach (UIElement element in SudokuGrid.Children)
            {
                if (element is TextBox textBox)
                {
                    int row = Grid.GetRow(textBox);
                    int col = Grid.GetColumn(textBox);

                    // Checking if the TextBox was user-inputted. In that case, I leave it as Red colored. Else I set it to black.
                    if (textBox.Tag?.ToString() == "UserInput")
                    {
                        textBox.Text = solvedGrid[row, col].ToString();
                        textBox.Foreground = Brushes.Red;
                    }
                    else
                    {
                        textBox.Text = solvedGrid[row, col].ToString();
                        textBox.Foreground = Brushes.Black;  
                    }
                }
            }
        }

        /*
         * Event Handler for the Clear/"Try your own Sudoku" Button
         * */
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement element in SudokuGrid.Children)
            {
                if (element is TextBox numberBox)
                {

                    numberBox.Text = string.Empty; 
                    numberBox.Tag = null; // Changing the tag to null, since the cells that are input by thge user is now reset.

                }
            }
        }

        /*
         * Event handler for the Reload/ "Reload the sample sudoku" button. All it does is essentially reset the grid.
         * */
        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            CreateSudokuGrid();
        }


    }
}