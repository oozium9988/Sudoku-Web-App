using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolverWebApp.SudokuSolver
{
    class ImprovedSet
    {
        public HashSet<int> Hashset { get; set; }

        public ImprovedSet(HashSet<int> st)
        {
            this.Hashset = st;
        }

        public override bool Equals(object obj)
        {
            ImprovedSet other = obj as ImprovedSet;
            return this.Hashset.SetEquals(other.Hashset);
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}
