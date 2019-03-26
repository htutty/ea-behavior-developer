using System;
using System.Diagnostics;

namespace VoidNish.Diff
{
    public class SimpleDiff<T> where T : IComparable<T>
    {
        private T[] _left;
        private T[] _right;
        private int[,] _matrix;
        private bool _matrixCreated;
        private int _preSkip;
        private int _postSkip;

        private Func<T, T, bool> _compareFunc;

        public SimpleDiff(T[] left, T[] right)
        {
            _left = left;
            _right = right;

            InitializeCompareFunc();
        }

        public event EventHandler<DiffEventArgs<T>> LineUpdate;

        public TimeSpan ElapsedTime { get; private set; }

        /// <summary>
        /// This is the sole public method and it initializes
        /// the LCS matrix the first time it's called, and 
        /// proceeds to fire a series of LineUpdate events
        /// </summary>
        public void RunDiff()
        {
            if (!_matrixCreated)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                CalculatePreSkip();
                CalculatePostSkip();
                CreateLCSMatrix();
                sw.Stop();
                this.ElapsedTime = sw.Elapsed;
            }

            for (int i = 0; i < _preSkip; i++)
            {
                FireLineUpdate(DiffType.None, _left[i]);
            }

            //int totalSkip = _preSkip + _postSkip;
            int totalSkip = _preSkip;
            ShowDiff(_left.Length - totalSkip, _right.Length - totalSkip);

            int leftLen = _left.Length;
            for (int i = _postSkip; i > 0; i--)
            {
                FireLineUpdate(DiffType.None, _left[leftLen - i]);
            }
        }

        /// <summary>
        /// This method is an optimization that
        /// skips matching elements at the end of the 
        /// two arrays being diff'ed.
        /// Care's taken so that this will never
        /// overlap with the pre-skip.
        /// </summary>
        private void CalculatePostSkip()
        {
            int leftLen = _left.Length;
            int rightLen = _right.Length;
            while (_postSkip < leftLen && _postSkip < rightLen &&
                _postSkip < (leftLen - _preSkip) &&
                _compareFunc(_left[leftLen - _postSkip - 1], _right[rightLen - _postSkip - 1]))
            {
                _postSkip++;
            }
        }

        /// <summary>
        /// This method is an optimization that
        /// skips matching elements at the start of
        /// the arrays being diff'ed
        /// </summary>
        private void CalculatePreSkip()
        {
            int leftLen = _left.Length;
            int rightLen = _right.Length;
            while (_preSkip < leftLen && _preSkip < rightLen &&
                _compareFunc(_left[_preSkip], _right[_preSkip]))
            {
                _preSkip++;
            }
        }

        /// <summary>
        /// This traverses the elements using the LCS matrix
        /// and fires appropriate events for added, subtracted, 
        /// and unchanged lines.
        /// It's recursively called till we run out of items.
        /// </summary>
        /// <param name="leftIndex"></param>
        /// <param name="rightIndex"></param>
        private void ShowDiff(int leftIndex, int rightIndex)
        {
            if (leftIndex > 0 && rightIndex > 0 &&
                _compareFunc(_left[_preSkip + leftIndex - 1], _right[_preSkip + rightIndex - 1]))
            {
                ShowDiff(leftIndex - 1, rightIndex - 1);
                FireLineUpdate(DiffType.None, _left[_preSkip + leftIndex - 1]);
            }
            else
            {
                if (rightIndex > 0 &&
                    (leftIndex == 0 ||
                    _matrix[leftIndex, rightIndex - 1] >= _matrix[leftIndex - 1, rightIndex]))
                {
                    ShowDiff(leftIndex, rightIndex - 1);
                    FireLineUpdate(DiffType.Add, _right[_preSkip + rightIndex - 1]);
                }
                else if (leftIndex > 0 &&
                    (rightIndex == 0 ||
                    _matrix[leftIndex, rightIndex - 1] < _matrix[leftIndex - 1, rightIndex]))
                {
                    ShowDiff(leftIndex - 1, rightIndex);
                    FireLineUpdate(DiffType.Subtract, _left[_preSkip + leftIndex - 1]);
                }
            }

        }

        /// <summary>
        /// This is the core method in the entire class,
        /// and uses the standard LCS calculation algorithm.
        /// </summary>
        private void CreateLCSMatrix()
        {
            int totalSkip = _preSkip + _postSkip;
            if (totalSkip >= _left.Length || totalSkip >= _right.Length)
                return;

            // We only create a matrix large enough for the
            // unskipped contents of the diff'ed arrays
            _matrix = new int[_left.Length - totalSkip + 1, _right.Length - totalSkip + 1];

            for (int i = 1; i <= _left.Length - totalSkip; i++)
            {
                // Simple optimization to avoid this calculation
                // inside the outer loop (may have got JIT optimized 
                // but my tests showed a minor improvement in speed)
                int leftIndex = _preSkip + i - 1;

                // Again, instead of calculating the adjusted index inside
                // the loop, I initialize it under the assumption that
                // incrementing will be a faster operation on most CPUs
                // compared to addition. Again, this may have got JIT
                // optimized but my tests showed a minor speed difference.
                for (int j = 1, rightIndex = _preSkip + 1; j <= _right.Length - totalSkip; j++, rightIndex++)
                {
                    _matrix[i, j] = _compareFunc(_left[leftIndex], _right[rightIndex - 1]) ?
                        _matrix[i - 1, j - 1] + 1 :
                        Math.Max(_matrix[i, j - 1], _matrix[i - 1, j]);
                }
            }

            _matrixCreated = true;
        }

        private void FireLineUpdate(DiffType diffType, T lineValue)
        {
            var local = this.LineUpdate;

            if (local == null)
                return;

            local(this, new DiffEventArgs<T>(diffType, lineValue));
        }

        private void InitializeCompareFunc()
        {
            // Special case for String types
            if (typeof(T) == typeof(String))
            {
                _compareFunc = StringCompare;
            }
            else
            {
                _compareFunc = DefaultCompare;
            }
        }

        /// <summary>
        /// This comparison is specifically
        /// for strings, and was nearly thrice as 
        /// fast as the default comparison operation.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private bool StringCompare(T left, T right)
        {
            return Object.Equals(left, right);
        }

        private bool DefaultCompare(T left, T right)
        {
            return left.CompareTo(right) == 0;
        }
    }

}
