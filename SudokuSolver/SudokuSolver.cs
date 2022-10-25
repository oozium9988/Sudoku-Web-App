using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolverWebApp.SudokuSolver
{
    public class SudokuSolver
    {
        private static int solvedCount;
        private static int solutionsLeft;
        private static List<List<List<HashSet<int>>>> sudokus;

        public static List<List<List<HashSet<int>>>> SolveSudoku(char[,] board, int stepsToShow)
        {
            sudokus = new List<List<List<HashSet<int>>>>();

            solvedCount = 0;
            solutionsLeft = 729;

            List<List<HashSet<int>>> sudoku = new List<List<HashSet<int>>>();

            for (int i = 0; i < 9; i++)
            {
                sudoku.Add(new List<HashSet<int>>());
                List<HashSet<int>> list = sudoku[i];
                for (int j = 0; j < 9; j++)
                {
                    sudoku[i].Add(new HashSet<int>());
                    HashSet<int> set = sudoku[i][j];
                    char c = board[i, j];

                    if (c != '.')
                    {
                        set.Add((int)(c - '0'));
                        solvedCount++;
                        solutionsLeft -= 8;
                    }
                    else
                    {
                        for (int k = 1; k <= 9; k++)
                        {
                            set.Add(k);
                        }
                    }
                }
            }

            int prevSolutionsLeft = 0;
            List<List<List<HashSet<int>>>> prevSudokus = new List<List<List<HashSet<int>>>>();
            List<List<int>> prevSudokuNums = new List<List<int>>();
            char[,,] prevBoards = new char[81, 9, 9];
            int prevBoardsCount = 0;
            int[] initialCoords = new int[2];
            List<List<int>> initCoords = new List<List<int>>();
            List<int> initSolvedCounts = new List<int>();
            List<int> initSolutionsLeft = new List<int>();
            initialCoords[0] = -1;
            initialCoords[1] = -1;
            int contradictionCount = 0;
            int noFurtherSolutionsCount = 0;

            while (solvedCount < 81)
            {
                for (int i = 0; i < 9; i += 3)
                {
                    for (int j = 0; j < 9; j += 3)
                    {
                        CheckBox(i, j, sudoku, board);
                    }
                }

                if (solvedCount < 81)
                {
                    sudokus.Add(CreateSudokuCopy(sudoku));
                }

                for (int i = 0; i < 9; i++)
                {
                    CheckRow(i, sudoku, board);
                }

                if (solvedCount < 81)
                {
                    sudokus.Add(CreateSudokuCopy(sudoku));
                }

                for (int i = 0; i < 9; i++)
                {
                    CheckColumn(i, sudoku, board);
                }

                if (solvedCount < 81)
                {
                    sudokus.Add(CreateSudokuCopy(sudoku));
                }

                for (int i = 0; i < 9; i += 3)
                {
                    for (int j = 0; j < 9; j += 3)
                    {
                        CheckBox1(i, j, sudoku, board);
                    }
                }

                if (solvedCount < 81)
                {
                    sudokus.Add(CreateSudokuCopy(sudoku));
                }

                for (int i = 0; i < 9; i++)
                {
                    CheckRow1(i, sudoku, board);
                }

                if (solvedCount < 81)
                {
                    sudokus.Add(CreateSudokuCopy(sudoku));
                }

                for (int i = 0; i < 9; i++)
                {
                    CheckColumn1(i, sudoku, board);
                }

                if (solvedCount < 81)
                {
                    sudokus.Add(CreateSudokuCopy(sudoku));
                }

                if (solutionsLeft == prevSolutionsLeft)
                {
                    noFurtherSolutionsCount++;

                    //if (noFurtherSolutionsCount == 2)
                    //    return sudoku;

                    int minCount = int.MaxValue;
                    int[] coords = new int[2] { -1, -1 };

                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            int sudokuCount = sudoku[i][j].Count;

                            if (sudokuCount > 1 && sudokuCount < minCount)
                            {
                                minCount = sudokuCount;
                                coords[0] = i;
                                coords[1] = j;
                            }
                        }
                    }

                    if (coords[0] != -1 && coords[1] != -1)
                    {
                        initCoords.Add(new List<int> { coords[0], coords[1] });

                        var prevSudoku = CreateSudokuCopy(sudoku);

                        prevSudokus.Add(prevSudoku);

                        List<int> sudokuCellNums = sudoku[coords[0]][coords[1]].ToList();
                        prevSudokuNums.Add(sudokuCellNums);

                        int valueToSet = sudokuCellNums[sudokuCellNums.Count - 1];

                        prevSudokuNums[prevSudokuNums.Count - 1].RemoveAt(sudokuCellNums.Count - 1);

                        sudoku[coords[0]][coords[1]] = new HashSet<int>() { valueToSet };                        

                        for (int i = 0; i < 9; i++)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                prevBoards[prevBoardsCount, i, j] = board[i, j];                              
                            }
                        }

                        prevBoardsCount++;

                        board[coords[0], coords[1]] = (char)(valueToSet + '0');
                        initSolvedCounts.Add(solvedCount);
                        initSolutionsLeft.Add(solutionsLeft);
                        solvedCount++;
                        solutionsLeft--;
                    }

                    if (solvedCount < 81)
                    {
                        sudokus.Add(CreateSudokuCopy(sudoku));
                    }
                }

                prevSolutionsLeft = solutionsLeft;

                if (Contradiction(board))
                {
                    contradictionCount++;

                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            board[i, j] = prevBoards[prevBoardsCount - 1, i, j];                                                                     
                        }
                    }

                    sudoku.Clear();

                    int prevSudokusCount = prevSudokus.Count;

                    sudoku = CreateSudokuCopy(prevSudokus[prevSudokusCount - 1]);

                    solvedCount = initSolvedCounts[initSolvedCounts.Count - 1];

                    solutionsLeft = initSolutionsLeft[initSolvedCounts.Count - 1];
                    prevSolutionsLeft = solutionsLeft;

                    int prevSudokuNumsCount = prevSudokuNums.Count;
                    int prevListCount = prevSudokuNums[prevSudokuNumsCount - 1].Count;                   
            
                    int ICC = initCoords.Count;

                    sudoku[initCoords[ICC - 1][0]][initCoords[ICC - 1][1]] = new HashSet<int>() { prevSudokuNums[prevSudokuNumsCount - 1][prevListCount - 1] };

                    int[] coords = new int[] { initCoords[ICC - 1][0], initCoords[ICC - 1][1] };
                    int valueToSet = prevSudokuNums[prevSudokuNumsCount - 1][prevListCount - 1];

                    Console.WriteLine("Now try setting " + "[" + (coords[1] + 1) + ", " + (coords[0] + 1) + "]" + "to" + valueToSet);
                    board[coords[0], coords[1]] = (char)(valueToSet + '0');
                    solutionsLeft--;
                    solvedCount++;

                    if (prevListCount == 1)
                    {                      
                        initSolvedCounts.RemoveAt(initSolvedCounts.Count - 1);
                        initSolutionsLeft.RemoveAt(initSolutionsLeft.Count - 1);
                        prevSudokuNums.RemoveAt(prevSudokuNumsCount - 1);
                        initCoords.RemoveAt(initCoords.Count - 1);
                        prevSudokus.RemoveAt(prevSudokus.Count - 1);
                        prevBoardsCount--;
                    } else
                    {
                        prevSudokuNums[prevSudokuNumsCount - 1].RemoveAt(prevListCount - 1);
                    }

                    if (solvedCount < 81)
                    {
                        sudokus.Add(CreateSudokuCopy(sudoku));
                    }
                }
            }

            
            
            if (sudokus.Count != 0 && stepsToShow != 0 && sudokus.Count > stepsToShow)
            {
                int stepInterval = sudokus.Count / stepsToShow;

                for (int i = sudokus.Count - 1; i >= 0; i--)
                {
                    if (i % stepInterval != 0)
                    {
                        sudokus.RemoveAt(i);
                    }
                }
            }
            
            if (sudokus.Count > stepsToShow)
            {
                sudokus.RemoveRange(stepsToShow, sudokus.Count - stepsToShow);
            }

            sudokus.Add(CreateSudokuCopy(sudoku));

            return sudokus;
        }

        public static char[,] SolveSudoku(char[,] board)
        {
            solvedCount = 0;
            solutionsLeft = 729;

            List<List<HashSet<int>>> sudoku = new List<List<HashSet<int>>>();

            for (int i = 0; i < 9; i++)
            {
                sudoku.Add(new List<HashSet<int>>());
                List<HashSet<int>> list = sudoku[i];
                for (int j = 0; j < 9; j++)
                {
                    sudoku[i].Add(new HashSet<int>());
                    HashSet<int> set = sudoku[i][j];
                    char c = board[i, j];

                    if (c != '.')
                    {
                        set.Add((int)(c - '0'));
                        solvedCount++;
                        solutionsLeft -= 8;
                    }
                    else
                    {
                        for (int k = 1; k <= 9; k++)
                        {
                            set.Add(k);
                        }
                    }
                }
            }

            int prevSolutionsLeft = 0;
            List<List<List<HashSet<int>>>> prevSudokus = new List<List<List<HashSet<int>>>>();
            List<List<int>> prevSudokuNums = new List<List<int>>();
            char[,,] prevBoards = new char[81, 9, 9];
            int prevBoardsCount = 0;
            int[] initialCoords = new int[2];
            List<List<int>> initCoords = new List<List<int>>();
            List<int> initSolvedCounts = new List<int>();
            List<int> initSolutionsLeft = new List<int>();
            initialCoords[0] = -1;
            initialCoords[1] = -1;
            int contradictionCount = 0;
            int noFurtherSolutionsCount = 0;

            while (solvedCount < 81)
            {
                for (int i = 0; i < 9; i += 3)
                {
                    for (int j = 0; j < 9; j += 3)
                    {
                        CheckBox(i, j, sudoku, board);
                    }
                }

                for (int i = 0; i < 9; i++)
                {
                    CheckRow(i, sudoku, board);
                }

                for (int i = 0; i < 9; i++)
                {
                    CheckColumn(i, sudoku, board);
                }

                for (int i = 0; i < 9; i += 3)
                {
                    for (int j = 0; j < 9; j += 3)
                    {
                        CheckBox1(i, j, sudoku, board);
                    }
                }

                for (int i = 0; i < 9; i++)
                {
                    CheckRow1(i, sudoku, board);
                }

                for (int i = 0; i < 9; i++)
                {
                    CheckColumn1(i, sudoku, board);
                }

                if (solutionsLeft == prevSolutionsLeft)
                {
                    noFurtherSolutionsCount++;

                    //if (noFurtherSolutionsCount == 2)
                    //    return sudoku;

                    int minCount = int.MaxValue;
                    int[] coords = new int[2] { -1, -1 };

                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            int sudokuCount = sudoku[i][j].Count;

                            if (sudokuCount > 1 && sudokuCount < minCount)
                            {
                                minCount = sudokuCount;
                                coords[0] = i;
                                coords[1] = j;
                            }
                        }
                    }

                    if (coords[0] != -1 && coords[1] != -1)
                    {
                        initCoords.Add(new List<int> { coords[0], coords[1] });

                        var prevSudoku = CreateSudokuCopy(sudoku);

                        prevSudokus.Add(prevSudoku);

                        List<int> sudokuCellNums = sudoku[coords[0]][coords[1]].ToList();
                        prevSudokuNums.Add(sudokuCellNums);

                        int valueToSet = sudokuCellNums[sudokuCellNums.Count - 1];

                        prevSudokuNums[prevSudokuNums.Count - 1].RemoveAt(sudokuCellNums.Count - 1);

                        sudoku[coords[0]][coords[1]] = new HashSet<int>() { valueToSet };

                        for (int i = 0; i < 9; i++)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                prevBoards[prevBoardsCount, i, j] = board[i, j];
                            }
                        }

                        prevBoardsCount++;

                        board[coords[0], coords[1]] = (char)(valueToSet + '0');
                        initSolvedCounts.Add(solvedCount);
                        initSolutionsLeft.Add(solutionsLeft);
                        solvedCount++;
                        solutionsLeft--;
                    }
                }

                prevSolutionsLeft = solutionsLeft;

                if (Contradiction(board))
                {
                    contradictionCount++;

                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            board[i, j] = prevBoards[prevBoardsCount - 1, i, j];
                        }
                    }

                    sudoku.Clear();

                    int prevSudokusCount = prevSudokus.Count;

                    sudoku = CreateSudokuCopy(prevSudokus[prevSudokusCount - 1]);

                    solvedCount = initSolvedCounts[initSolvedCounts.Count - 1];

                    solutionsLeft = initSolutionsLeft[initSolvedCounts.Count - 1];
                    prevSolutionsLeft = solutionsLeft;

                    int prevSudokuNumsCount = prevSudokuNums.Count;
                    int prevListCount = prevSudokuNums[prevSudokuNumsCount - 1].Count;

                    int ICC = initCoords.Count;

                    sudoku[initCoords[ICC - 1][0]][initCoords[ICC - 1][1]] = new HashSet<int>() { prevSudokuNums[prevSudokuNumsCount - 1][prevListCount - 1] };

                    int[] coords = new int[] { initCoords[ICC - 1][0], initCoords[ICC - 1][1] };
                    int valueToSet = prevSudokuNums[prevSudokuNumsCount - 1][prevListCount - 1];

                    Console.WriteLine("Now try setting " + "[" + (coords[1] + 1) + ", " + (coords[0] + 1) + "]" + "to" + valueToSet);
                    board[coords[0], coords[1]] = (char)(valueToSet + '0');
                    solutionsLeft--;
                    solvedCount++;

                    if (prevListCount == 1)
                    {
                        initSolvedCounts.RemoveAt(initSolvedCounts.Count - 1);
                        initSolutionsLeft.RemoveAt(initSolutionsLeft.Count - 1);
                        prevSudokuNums.RemoveAt(prevSudokuNumsCount - 1);
                        initCoords.RemoveAt(initCoords.Count - 1);
                        prevSudokus.RemoveAt(prevSudokus.Count - 1);
                        prevBoardsCount--;
                    }
                    else
                    {
                        prevSudokuNums[prevSudokuNumsCount - 1].RemoveAt(prevListCount - 1);
                    }
                }
            }

            var solvedSudoku = new char[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    HashSet<int> cell = sudoku[i][j];
                    int n = sudoku[i][j].ToList()[0];
                    solvedSudoku[i, j] = (char)(n + '0');
                }
            }

            return solvedSudoku;
        }

        public static string[][] GenerateSudoku()
        {
            char[,] initialBoard = new char[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    initialBoard[i, j] = '.';
                }
            }

            Random rnd = new Random();

            for (int k = 1; k <= 9; k++)
            {
                int n = rnd.Next(81);

                int i = n / 9;
                int j = n % 9;

                initialBoard[i, j] = (char)(k + '0');
            }

            var solvedSudoku = SolveSudoku(initialBoard);

            for (int k = 0; k < 70; k++)
            {
                int n = rnd.Next(81);

                int i = n / 9;
                int j = n % 9;

                solvedSudoku[i, j] = '.';
            }

            string[][] sudokuToReturn = new string[9][];

            for (int i = 0; i < 9; i++)
            {
                sudokuToReturn[i] = new string[9];

                for (int j = 0; j < 9; j++)
                {
                    sudokuToReturn[i][j] = solvedSudoku[i, j] == '.' ? null : solvedSudoku[i, j].ToString();
                }
            }

            return sudokuToReturn;
        }

        private static void CheckBox(int x, int y, List<List<HashSet<int>>> sudoku, char[,] board)
        {
            Dictionary<ImprovedSet, int> map = new Dictionary<ImprovedSet, int>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    HashSet<int> initialset = sudoku[x + i][y + j];
                    var set = new ImprovedSet(initialset);
                    if (map.ContainsKey(set))
                    {
                        map[set]++;
                    }
                    else
                    {
                        map.Add(set, 1);
                    }
                }
            }

            List<HashSet<int>> list = new List<HashSet<int>>();

            foreach (KeyValuePair<ImprovedSet, int> entry in map)
            {
                if (entry.Key.Hashset.Count == entry.Value)
                {
                    list.Add(entry.Key.Hashset);
                }
            }

            var setOfSets = new HashSet<HashSet<int>>();

            foreach (HashSet<int> set in list)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        HashSet<int> st = sudoku[x + i][y + j];
                        if (st.Count > 1 && !st.SetEquals(set))
                        {
                            foreach (int num in set)
                            {
                                if (st.Remove(num))
                                {
                                    solutionsLeft--;
                                    if (set.Count > 1)
                                    {
                                        setOfSets.Add(set);
                                    }
                                }
                            }

                            if (st.Count == 1)
                            {
                                foreach (int num in st)
                                {
                                    board[x + i, y + j] = (char)(num + '0');
                                }
                                solvedCount++;
                            }
                        }
                    }
                }
            }
        }

        private static void CheckColumn(int y, List<List<HashSet<int>>> sudoku, char[,] board)
        {
            var map = new Dictionary<ImprovedSet, int>();

            for (int i = 0; i < 9; i++)
            {
                HashSet<int> initialset = sudoku[i][y];
                ImprovedSet set = new ImprovedSet(initialset);
                if (map.ContainsKey(set))
                {
                    map[set]++;
                }
                else
                {
                    map.Add(set, 1);
                }
            }

            List<HashSet<int>> list = new List<HashSet<int>>();

            foreach (KeyValuePair<ImprovedSet, int> entry in map)
            {
                if (entry.Key.Hashset.Count == entry.Value)
                {
                    list.Add(entry.Key.Hashset);
                }
            }

            HashSet<HashSet<int>> setOfSets = new HashSet<HashSet<int>>();

            foreach (HashSet<int> set in list)
            {
                for (int i = 0; i < 9; i++)
                {
                    HashSet<int> st = sudoku[i][y];
                    if (st.Count > 1 && !st.SetEquals(set))
                    {
                        foreach (int num in set)
                        {
                            if (st.Remove(num))
                            {
                                solutionsLeft--;
                                if (set.Count > 1)
                                {
                                    setOfSets.Add(set);
                                }
                            }
                        }

                        if (st.Count == 1)
                        {
                            foreach (int num in st)
                            {
                                board[i, y] = (char)(num + '0');
                            }
                            solvedCount++;
                        }
                    }
                }
            }
        }

        private static void CheckRow(int x, List<List<HashSet<int>>> sudoku, char[,] board)
        {
            Dictionary<ImprovedSet, int> map = new Dictionary<ImprovedSet, int>();

            for (int i = 0; i < 9; i++)
            {
                HashSet<int> initialset = sudoku[x][i];
                ImprovedSet set = new ImprovedSet(initialset);
                if (map.ContainsKey(set))
                {
                    map[set]++;
                }
                else
                {
                    map.Add(set, 1);
                }
            }

            List<HashSet<int>> list = new List<HashSet<int>>();

            foreach (KeyValuePair<ImprovedSet, int> entry in map)
            {
                if (entry.Key.Hashset.Count == entry.Value)
                {
                    list.Add(entry.Key.Hashset);
                }
            }

            HashSet<HashSet<int>> setOfSets = new HashSet<HashSet<int>>();

            foreach (HashSet<int> set in list)
            {
                for (int i = 0; i < 9; i++)
                {
                    HashSet<int> st = sudoku[x][i];
                    if (st.Count > 1 && !st.SetEquals(set))
                    {
                        foreach (int num in set)
                        {
                            if (st.Remove(num))
                            {
                                solutionsLeft--;
                                if (set.Count > 1)
                                {
                                    setOfSets.Add(set);
                                }
                            }
                        }

                        if (st.Count == 1)
                        {
                            foreach (int num in st)
                            {
                                board[x, i] = (char)(num + '0');
                            }
                            solvedCount++;
                        }
                    }
                }
            }
        }

        private static void CheckBox1(int x, int y, List<List<HashSet<int>>> sudoku, char[,] board)
        {
            List<HashSet<int>> indices = new List<HashSet<int>>();

            for (int i = 0; i < 10; i++)
            {
                indices.Add(new HashSet<int>());
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    HashSet<int> set = sudoku[x + i][y + j];
                    foreach (int num in set)
                    {
                        indices[num].Add(i * 3 + j);
                    }
                }
            }

            Dictionary<HashSet<int>, HashSet<int>> map = new Dictionary<HashSet<int>, HashSet<int>>();

            for (int i = 1; i < 10; i++)
            {
                HashSet<int> st = indices[i];
                if (map.ContainsKey(st))
                {
                    map[st].Add(i);
                }
                else
                {
                    HashSet<int> newSet = new HashSet<int>();
                    newSet.Add(i);
                    map[st] = newSet;
                }
            }

            HashSet<HashSet<int>> setOfSets = new HashSet<HashSet<int>>();

            foreach (KeyValuePair<HashSet<int>, HashSet<int>> entry in map)
            {
                if (entry.Key.Count == entry.Value.Count)
                {

                    foreach (int num in entry.Key)
                    {
                        int i = num / 3;
                        int j = num % 3;
                        HashSet<int> st = sudoku[x + i][y + j];

                        if (st.Count == 1)
                        {
                            continue;
                        }

                        HashSet<int> removeSet = new HashSet<int>();

                        foreach (int number in st)
                        {
                            if (!entry.Value.Contains(number))
                            {
                                removeSet.Add(number);
                                solutionsLeft--;
                                if (entry.Value.Count > 1)
                                {
                                    setOfSets.Add(entry.Value);
                                }
                            }
                        }

                        st.RemoveWhere(i => removeSet.Contains(i));

                        if (st.Count == 1)
                        {
                            foreach (int n in st)
                            {
                                board[x + i, y + j] = (char)(n + '0');
                            }
                            solvedCount++;
                        }
                    }
                }
            }
        }

        private static void CheckColumn1(int y, List<List<HashSet<int>>> sudoku, char[,] board)
        {
            List<HashSet<int>> indices = new List<HashSet<int>>();

            for (int i = 0; i < 10; i++)
            {
                indices.Add(new HashSet<int>());
            }

            for (int i = 0; i < 9; i++)
            {
                HashSet<int> set = sudoku[i][y];
                foreach (int num in set)
                {
                    indices[num].Add(i);
                }
            }

            Dictionary<HashSet<int>, HashSet<int>> map = new Dictionary<HashSet<int>, HashSet<int>>();

            for (int i = 1; i < 10; i++)
            {
                HashSet<int> st = indices[i];
                if (map.ContainsKey(st))
                {
                    map[st].Add(i);
                    //map.put(st, map.get(st).add(i));
                }
                else
                {
                    HashSet<int> newSet = new HashSet<int>();
                    newSet.Add(i);
                    map[st] = newSet;
                }
            }

            HashSet<HashSet<int>> setOfSets = new HashSet<HashSet<int>>();

            foreach (KeyValuePair<HashSet<int>, HashSet<int>> entry in map)
            {
                if (entry.Key.Count == entry.Value.Count)
                {

                    foreach (int num in entry.Key)
                    {
                        HashSet<int> st = sudoku[num][y];

                        if (st.Count == 1)
                        {
                            continue;
                        }

                        HashSet<int> removeSet = new HashSet<int>();

                        foreach (int number in st)
                        {
                            if (!entry.Value.Contains(number))
                            {
                                removeSet.Add(number);
                                solutionsLeft--;
                                if (entry.Value.Count > 1)
                                {
                                    setOfSets.Add(entry.Value);
                                }
                            }
                        }

                        st.RemoveWhere(i => removeSet.Contains(i));

                        if (st.Count == 1)
                        {
                            foreach (int n in st)
                            {
                                board[num, y] = (char)(n + '0');
                            }
                            solvedCount++;
                        }
                    }
                }
            }
        }

        private static void CheckRow1(int x, List<List<HashSet<int>>> sudoku, char[,] board)
        {
            List<HashSet<int>> indices = new List<HashSet<int>>();

            for (int i = 0; i < 10; i++)
            {
                indices.Add(new HashSet<int>());
            }

            for (int i = 0; i < 9; i++)
            {
                HashSet<int> set = sudoku[x][i];
                foreach (int num in set)
                {
                    indices[num].Add(i);
                }
            }

            Dictionary<HashSet<int>, HashSet<int>> map = new Dictionary<HashSet<int>, HashSet<int>>();

            for (int i = 1; i < 10; i++)
            {
                HashSet<int> st = indices[i];
                if (map.ContainsKey(st))
                {
                    map[st].Add(i);
                }
                else
                {
                    HashSet<int> newSet = new HashSet<int>();
                    newSet.Add(i);
                    map[st] = newSet;
                }
            }

            HashSet<HashSet<int>> setOfSets = new HashSet<HashSet<int>>();

            foreach (KeyValuePair<HashSet<int>, HashSet<int>> entry in map)
            {
                if (entry.Key.Count == entry.Value.Count)
                {

                    foreach (int num in entry.Key)
                    {
                        HashSet<int> st = sudoku[x][num];

                        if (st.Count == 1)
                        {
                            continue;
                        }

                        HashSet<int> removeSet = new HashSet<int>();

                        foreach (int number in st)
                        {
                            if (!entry.Value.Contains(number))
                            {
                                removeSet.Add(number);
                                solutionsLeft--;
                                if (entry.Value.Count > 1)
                                {
                                    setOfSets.Add(entry.Value);
                                }
                            }
                        }

                        st.RemoveWhere(i => removeSet.Contains(i));

                        if (st.Count == 1)
                        {
                            foreach (int n in st)
                            {
                                board[x, num] = (char)(n + '0');
                            }
                            solvedCount++;
                        }
                    }
                }
            }
        }

        public static bool Contradiction(char[,] board)
        {
            for (int i = 0; i < 9; i++)
            {
                int[] numCounts = new int[10];
                for (int j = 0; j < 9; j++)
                {
                    int num = (int)(board[i, j] - '0');

                    if (num < 1)
                    {
                        continue;
                    }

                    numCounts[num]++;

                    if (numCounts[num] > 1)
                    {
                        return true;
                    }
                }
            }

            for (int i = 0; i < 9; i++)
            {
                int[] numCounts = new int[10];
                for (int j = 0; j < 9; j++)
                {
                    int num = (int)(board[j, i] - '0');

                    if (num < 1)
                    {
                        continue;
                    }

                    numCounts[num]++;
                    if (numCounts[num] > 1)
                    {
                        return true;
                    }

                }
            }

            int x = 0;
            int y = 0;

            while (x < 9)
            {
                while (y < 9)
                {
                    int[] numCounts = new int[10];
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            int num = (int)(board[x + i, y + j] - '0');

                            if (num < 1)
                            {
                                continue;
                            }

                            numCounts[num]++;
                            if (numCounts[num] > 1)
                            {
                                return true;
                            }
                        }
                    }
                    y += 3;
                }
                x += 3;
            }

            return false;
        }

        private static void XWingRows(List<List<HashSet<int>>> sudoku, char[,] board)
        {
            List<List<HashSet<int>>> indices = new List<List<HashSet<int>>>();

            for (int i = 0; i < 9; i++)
            {
                indices.Add(new List<HashSet<int>>());
                for (int j = 0; j < 10; j++)
                {
                    indices[i].Add(new HashSet<int>());
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        if (sudoku[k][i].Contains(j))
                        {
                            indices[i][j].Add(k);
                        }
                    }
                }
            }

            for (int i = 1; i <= 9; i++)
            {
                Dictionary<HashSet<int>, List<int>> map = new Dictionary<HashSet<int>, List<int>>();
                for (int j = 0; j < 9; j++)
                {
                    HashSet<int> set = indices[j][i];
                    if (set.Count == 2)
                    {
                        if (!map.ContainsKey(set))
                        {
                            List<int> list = new List<int>();
                            list.Add(j);
                            map[set] = list;
                        }
                        else
                        {
                            map[set].Add(j);
                        }
                    }
                }

                foreach (KeyValuePair<HashSet<int>, List<int>> entry in map)
                {
                    List<int> value = entry.Value;
                    HashSet<int> key = entry.Key;
                    for (int k = 0; k < value.Count - 1; k++)
                    {
                        for (int j = value[k] + 1; j < value[k + 1]; j++)
                        {
                            foreach (int num in key)
                            {
                                HashSet<int> st = sudoku[num][j];

                                if (st.Count == 1)
                                {
                                    continue;
                                }

                                st.Remove(i);

                                if (st.Count == 1)
                                {
                                    foreach (int n in st)
                                    {
                                        board[num, j] = (char)(n + '0');
                                        solvedCount++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void XWingColumns(List<List<HashSet<int>>> sudoku, char[,] board)
        {
            List<List<HashSet<int>>> indices = new List<List<HashSet<int>>>();

            for (int i = 0; i < 9; i++)
            {
                indices.Add(new List<HashSet<int>>());
                for (int j = 0; j < 10; j++)
                {
                    indices[i].Add(new HashSet<int>());
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        if (sudoku[i][k].Contains(j))
                        {
                            indices[i][j].Add(k);
                        }
                    }
                }
            }

            for (int i = 1; i <= 9; i++)
            {
                Dictionary<HashSet<int>, List<int>> map = new Dictionary<HashSet<int>, List<int>>();
                for (int j = 0; j < 9; j++)
                {
                    HashSet<int> set = indices[j][i];
                    if (set.Count == 2)
                    {
                        if (!map.ContainsKey(set))
                        {
                            List<int> list = new List<int>();
                            list.Add(j);
                            map[set] = list;
                        }
                        else
                        {
                            map[set].Add(j);
                        }
                    }
                }


                foreach (KeyValuePair<HashSet<int>, List<int>> entry in map)
                {
                    List<int> value = entry.Value;
                    HashSet<int> key = entry.Key;
                    for (int k = 0; k < value.Count - 1; k++)
                    {
                        for (int j = value[k] + 1; j < value[k + 1]; j++)
                        {
                            foreach (int num in key)
                            {
                                HashSet<int> st = sudoku[j][num];

                                if (st.Count == 1)
                                {
                                    continue;
                                }

                                st.Remove(i);

                                if (st.Count == 1)
                                {
                                    foreach (int n in st)
                                    {
                                        board[j, num] = (char)(n + '0');
                                        solvedCount++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static List<List<HashSet<int>>> CreateSudokuCopy(List<List<HashSet<int>>> sudokuToCopy)
        {
            var sudoku = new List<List<HashSet<int>>>();

            int count = 0;

            foreach (List<HashSet<int>> list in sudokuToCopy)
            {
                sudoku.Add(new List<HashSet<int>>());
                int setCount = 0;

                foreach (HashSet<int> set in list)
                {
                    sudoku[count].Add(new HashSet<int>());
                    foreach (int num in set)
                    {
                        sudoku[count][setCount].Add(num);
                    }
                    setCount++;
                }

                count++;
            }

            return sudoku;
        }
    }
}
