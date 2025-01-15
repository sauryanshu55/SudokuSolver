using System;

namespace Algorithm
{
    class GFG
    {

        public static bool isSafe(int[,] board,
                                  int row, int col,
                                  int num)
        {

            // Row has the unique (row-clash)
            for (int d = 0; d < board.GetLength(0); d++)
            {

                // Check if the number
                // we are trying to
                // place is already present in
                // that row, return false;
                if (board[row, d] == num)
                {
                    return false;
                }
            }

            // Column has the unique numbers (column-clash)
            for (int r = 0; r < board.GetLength(0); r++)
            {

                // Check if the number 
                // we are trying to
                // place is already present in
                // that column, return false;
                if (board[r, col] == num)
                {
                    return false;
                }
            }

            // corresponding square has
            // unique number (box-clash)
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

            // if there is no clash, it's safe
            return true;
        }

        public static (bool, int[,]) solveSudoku(int[,] board, int n)
        {
            int row = -1;
            int col = -1;
            bool isEmpty = true;

            // Find an empty cell
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

            // If no empty cell is found, Sudoku is solved
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
                    if (result.Item1) // If the recursive call succeeded
                    {
                        return result;
                    }

                    // Undo the move (backtracking)
                    board[row, col] = 0;
                }
            }

            // If no number can fit, the Sudoku is unsolvable
            return (false, board);
        }
    }
}