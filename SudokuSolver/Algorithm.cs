using System;

/*
 * This namespace contains the Sudoku Algorithm.
 * I converted a class assignment from C Cuda to C# for this project.
 */
namespace Algorithm
{
    /*
     * A Helper function that the algo uses to check if placing a number on  a cell is safe (by the Sudoku rules) or not
     */
    class SudokuAlgorithm
    {

        public static bool isSafe(int[,] board,
                                  int row, int col,
                                  int num)
        {

            // Checking if row is unique (cannot have the same number in the row)
            for (int d = 0; d < board.GetLength(0); d++)
            {
                if (board[row, d] == num)
                {
                    return false;
                }
            }

            // Checking the smae for the column
            for (int r = 0; r < board.GetLength(0); r++)
            {
                if (board[r, col] == num)
                {
                    return false;
                }
            }

            // Checking if the mini 3x3 one have the same numbers
            int sqrt = (int)Math.Sqrt(board.GetLength(0));
            int boxRowStart = row - row % sqrt;
            int boxColStart = col - col % sqrt;

            for (int r = boxRowStart;
                 r < boxRowStart + sqrt; r++)
            {
                for (int d = boxColStart;
                     d < boxColStart + sqrt; d++)
                {
                    if (board[r, d] == num)
                    {
                        return false;
                    }
                }
            }

            // if there is no clash, it's safe. We put it there
            return true;
        }

        /*
         * Driver code for the algorithm
         */
        public static (bool, int[,]) solveSudoku(int[,] board, int n)
        {
            int row = -1;
            int col = -1;
            bool isEmpty = true;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (board[i, j] == 0)
                    {
                        row = i;
                        col = j;
                        isEmpty = false;
                        break;
                    }
                }
                if (!isEmpty)
                {
                    break;
                }
            }

            // Solved Sudoku. Retun a tuple
            if (isEmpty)
            {
                return (true, board);
            }

            // Try filling numbers 1 to n
            for (int num = 1; num <= n; num++)
            {
                if (isSafe(board, row, col, num))
                {
                    board[row, col] = num;

                    var result = solveSudoku(board, n);
                    if (result.Item1) 
                    {
                        return result;
                    }

                    // Backtracking
                    board[row, col] = 0;
                }
            }

            // Usolvable sudoku
            return (false, board);
        }

        /*
         * A helper function to check if the algorithm's solution is correct
         * */
        public static bool IsValid(int[,] mat)
        {
            int[,] rows = new int[9, 10];
            int[,] cols = new int[9, 10];
            int[,] subMat = new int[9, 10];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    // Skiping empty cells
                    if (mat[i, j] == 0)
                        continue;

                    // Current value
                    int val = mat[i, j];

                    // Checking for row-clash
                    if (rows[i, val] == 1)
                        return false;

                    // Mark as seen
                    rows[i, val] = 1;

                    // Checking column clash
                    if (cols[j, val] == 1)
                        return false;

                    // Mark as seen
                    cols[j, val] = 1;

                    // Checking the mini 3x3 box clash
                    int idx = (i / 3) * 3 + j / 3;
                    if (subMat[idx, val] == 1)
                        return false;

                    // Mark as seen
                    subMat[idx, val] = 1;
                }
            }

            return true;
        }
    }
}