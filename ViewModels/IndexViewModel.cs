using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SudokuSolverWebApp.ViewModels
{
    public class IndexViewModel : IValidatableObject
    {
        public string[][] Sudoku { get; set; }
        public List<List<List<HashSet<int>>>> Solution { get; set; }
        public string ValidationMessage { get; set; }
        [DisplayName("Steps to show in solution:")]
        public int Steps { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            char[,] board = new char[9, 9];
            var modelBoard = Sudoku;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    string input = modelBoard[i][j];
                    board[i, j] = string.IsNullOrWhiteSpace(input) ? '.' : input[0];
                }
            }

            if (SudokuSolver.SudokuSolver.Contradiction(board))
                yield return new ValidationResult("Please enter a valid sudoku to be solved.");
        }
    }          
}
