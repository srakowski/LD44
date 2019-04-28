using Microsoft.Xna.Framework;
using System;

namespace CryptoReaper
{
    static class Functions
    {
        public static void ForAllRowsAndCols(int rowsAndCols, Action<int, int> action) => ForAllRowsAndCols(rowsAndCols, rowsAndCols, action);

        public static void ForAllRowsAndCols(int rows, int cols, Action<int, int> action)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    action(row, col);
                }
            }
        }

        public static string GetKey(this Point point) => $"{point.Y},{point.X}";
    }
}
