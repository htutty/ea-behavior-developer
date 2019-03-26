using System;

namespace VoidNish.Diff
{
    public class DiffEventArgs<T> : EventArgs
    {
        public DiffType DiffType { get; set; }

        public T LineValue { get; set; }

        public DiffEventArgs(DiffType diffType, T lineValue)
        {
            this.DiffType = diffType;
            this.LineValue = lineValue;
        }
    }
}
