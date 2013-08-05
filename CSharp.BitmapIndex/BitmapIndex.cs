using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ewah;

namespace BitmapIndex
{
    public class BitmapIndex
    {
        private Dictionary<BIKey, EwahCompressedBitArray> _bitmaps;
        private Dictionary<BIKey.BIGroup, EwahCompressedBitArray> _emptyBitmaps;
        private int _maxBitSize = 0;

        public BitmapIndex()
        {
            _bitmaps = new Dictionary<BIKey, EwahCompressedBitArray>();
            _emptyBitmaps = new Dictionary<BIKey.BIGroup, EwahCompressedBitArray>();
        }

        public void Set(BIKey key, int bit)
        {
            EwahCompressedBitArray bitmap;
            EwahCompressedBitArray emptyBitmap;

            if (_bitmaps.ContainsKey(key))
                bitmap = _bitmaps[key];
            else
            {
                bitmap = new EwahCompressedBitArray();
                _bitmaps.Add(key, bitmap);
            }

            if (_emptyBitmaps.ContainsKey(key.Group))
                emptyBitmap = _emptyBitmaps[key.Group];
            else
            {
                emptyBitmap = new EwahCompressedBitArray();
                _emptyBitmaps.Add(key.Group, emptyBitmap);
            }

            bitmap.Set(bit);

            emptyBitmap.Not();
            emptyBitmap.Set(bit);
            emptyBitmap.Not();

            _maxBitSize = (_maxBitSize < (bit + 1) ? (bit + 1) : _maxBitSize);
        }

        private EwahCompressedBitArray getFilledBitmap(bool fill)
        {
            EwahCompressedBitArray bitmap = new EwahCompressedBitArray();
            bitmap.SetSizeInBits(_maxBitSize, fill);

            return bitmap;
        }

        private EwahCompressedBitArray getCopyBitmap(EwahCompressedBitArray bitmap)
        {
            var bit = (EwahCompressedBitArray)bitmap.Clone();
            bit.SetSizeInBits(_maxBitSize, false);

            return bit;
        }

        private EwahCompressedBitArray getEmptyBitmap(BIKey key)
        {
            EwahCompressedBitArray bitmap;

            if (_emptyBitmaps.ContainsKey(key.Group))
            {
                bitmap = (EwahCompressedBitArray)_emptyBitmaps[key.Group].Clone();
                bitmap.SetSizeInBits(_maxBitSize, true);
            }
            else
                bitmap = getFilledBitmap(true);

            return bitmap;
        }


        private class Snapshot
        {
            public BICriteria criteria;
            public EwahCompressedBitArray left;
            public EwahCompressedBitArray right;
            public int state;
        }

        public EwahCompressedBitArray query(BICriteria criteria)
        {
            // This method could use recursion which would make it more readable
            // For performance reasons, and to avoid StackOverflowException, it
            // uses the Snapshot class to avoid recursion

            if (criteria == null)
                throw new ArgumentNullException("criteria");

            EwahCompressedBitArray temp;
            Snapshot previous;
            Snapshot next;

            Snapshot current = new Snapshot();
            current.criteria = criteria;
            current.state = 0;

            Stack<Snapshot> stack = new Stack<Snapshot>();
            stack.Push(current);

            while (stack.Count > 0)
            {
                current = stack.Pop();

                if (stack.Count > 0)
                    previous = stack.Peek();
                else
                    previous = null;

                if (current.criteria.CriteriaOperator == BusinessLayer.Uteis.BICriteria.Operator.OR ||
                    current.criteria.CriteriaOperator == BusinessLayer.Uteis.BICriteria.Operator.AND)
                {
                    if (current.state == 0)
                    {
                        current.state = 1;
                        stack.Push(current);

                        next = new Snapshot();
                        next.criteria = current.criteria.LeftCriteria;
                        next.state = 0;
                        stack.Push(next);

                        continue;
                    }
                    else if (current.state == 1)
                    {
                        current.state = 2;
                        stack.Push(current);

                        next = new Snapshot();
                        next.criteria = current.criteria.RightCriteria;
                        next.state = 0;
                        stack.Push(next);

                        continue;
                    }
                    else
                    {
                        if (current.criteria.CriteriaOperator == BusinessLayer.Uteis.BICriteria.Operator.AND)
                            temp = (current.left.And(current.right));
                        else
                            temp = (current.left.Or(current.right));

                        if (previous == null)
                        {
                            return temp;
                        }
                        else if (previous.state == 1)
                        {
                            previous.left = temp;
                        }
                        else if (previous.state == 2)
                        {
                            previous.right = temp;
                        }

                        continue;
                    }
                }
                else
                {
                    if (current.criteria.CriteriaOperator == BusinessLayer.Uteis.BICriteria.Operator.EMPTY_ONLY)
                    {
                        if (previous == null)
                            return getEmptyBitmap(current.criteria.Key);
                        else if (previous.state == 1)
                            previous.left = getEmptyBitmap(current.criteria.Key);
                        else if (previous.state == 2)
                            previous.right = getEmptyBitmap(current.criteria.Key);

                        continue;
                    }
                    else
                    {
                        EwahCompressedBitArray bitmap;

                        if (!_bitmaps.ContainsKey(current.criteria.Key))
                            bitmap = getFilledBitmap(false);
                        else
                            bitmap = getCopyBitmap(_bitmaps[current.criteria.Key]);

                        if (current.criteria.CriteriaOperator == BusinessLayer.Uteis.BICriteria.Operator.NOT_EQUALS ||
                            current.criteria.CriteriaOperator == BusinessLayer.Uteis.BICriteria.Operator.NOT_EQUALS_OR_EMPTY)
                            bitmap.Not();

                        if (current.criteria.CriteriaOperator == BusinessLayer.Uteis.BICriteria.Operator.NOT_EQUALS_OR_EMPTY ||
                            current.criteria.CriteriaOperator == BusinessLayer.Uteis.BICriteria.Operator.EQUALS_OR_EMPTY)
                            bitmap = bitmap.Or(getEmptyBitmap(current.criteria.Key));

                        if (previous == null)
                            return bitmap;
                        else if (previous.state == 1)
                            previous.left = bitmap;
                        else if (previous.state == 2)
                            previous.right = bitmap;

                        continue;
                    }
                }
            }

            temp = new EwahCompressedBitArray();
            temp.SetSizeInBits(_maxBitSize, false);
            return temp;
        }
    }
}
