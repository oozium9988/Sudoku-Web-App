using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SudokuSolverWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SudokuSolverWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IndexViewModel indexViewModel = new IndexViewModel();

            return View(indexViewModel);
        }

        [HttpPost]
        public IActionResult Index([Bind("Sudoku, Solution, Steps")] IndexViewModel indexViewModel)
        {
            if (!ModelState.IsValid)
            {
                indexViewModel.ValidationMessage = "Please enter a valid sudoku to be solved.";

                return View(indexViewModel);
            }

            indexViewModel.ValidationMessage = null;

            char[,] board = new char[9, 9];
            var modelBoard = indexViewModel.Sudoku;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    string input = modelBoard[i][j];
                    board[i, j] = string.IsNullOrWhiteSpace(input) ? '.' : input[0];
                }
            }

            int steps = indexViewModel.Steps;

            indexViewModel = new IndexViewModel { Sudoku = indexViewModel.Sudoku, Solution = SudokuSolver.SudokuSolver.SolveSudoku(board, steps), Steps = indexViewModel.Steps };

            return View(indexViewModel);
        }

        public IActionResult ResetSudoku()
        {
            IndexViewModel indexViewModel = new IndexViewModel();

            return View("Index", indexViewModel);
        }

        public IActionResult GenerateRandomSudoku()
        {
            IndexViewModel indexViewModel = new IndexViewModel();

            indexViewModel.Sudoku = SudokuSolver.SudokuSolver.GenerateSudoku();

            return View("Index", indexViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
