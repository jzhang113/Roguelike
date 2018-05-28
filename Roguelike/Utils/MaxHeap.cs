using System;
using System.Collections;
using System.Collections.Generic;

namespace Roguelike.Utils
{
    class MaxHeap<T> : ICollection<T> where T : IComparable<T>
    {
        private T[] _heap;
        private int[] _orderArray;
        private int _count;

        public int Count { get; private set; }
        public bool IsReadOnly => false;
        public bool IsEmpty() => Count == 0;
        internal T[] GetHeap() => _heap;

        public MaxHeap() : this(16)
        {
        }

        public MaxHeap(int size)
        {
            _heap = new T[size];
            _orderArray = new int[size];
            Count = 0;
            _count = 0;
        }
        
        public T Peek()
        {
            if (Count > 0)
                return _heap[0];

            throw new InvalidOperationException("The heap is empty");
        }

        public T PopMax()
        {
            if (Count > 0)
            {
                T item = _heap[0];
                --Count;
                _heap[0] = _heap[Count];
                _orderArray[0] = _orderArray[Count];
                ReheapDown(0);
                return item;
            }
            throw new InvalidOperationException("The heap is empty");
        }

        public void Clear()
        {
            Count = 0;
        }

        public void Add(T item)
        {
            if (Count >= _heap.Length)
                Resize();

            _orderArray[Count] = _count++;
            _heap[Count] = item;
            Count++;
            ReheapUp(Count - 1);
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (_heap[i].Equals(item))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
            if (array.Rank > 1)
                throw new ArgumentException("array is multidimensional.");
            if (Count == 0)
                return;
            if (arrayIndex >= array.Length)
                throw new ArgumentException("arrayIndex is equal to or greater than the length of the array.");
            if (Count > (array.Length - arrayIndex))
                throw new ArgumentException("The number of elements in the source ICollection is greater than the available space from arrayIndex to the end of the destination array.");

            for (int i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = _heap[i];
            }
        }

        public bool Remove(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (_heap[i].Equals(item))
                {
                    T oldItem = _heap[i];
                    int oldOrder = _orderArray[i];

                    --Count;
                    T newItem = _heap[Count];
                    int newOrder = _orderArray[Count];
                    _heap[i] = _heap[Count];
                    _orderArray[i] = _orderArray[Count];

                    if (CompareItem(oldItem, newItem, oldOrder, newOrder) > 0)
                        ReheapUp(i);
                    else
                        ReheapDown(i);
                    return true;
                }
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return _heap[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void Apply(Action<T> func)
        {
            for (int i = 0; i < Count; i++)
            {
                func(_heap[i]);
            }
        }

        internal void ReheapUp(int initial)
        {
            int pos = initial;
            T oldItem = _heap[pos];
            int oldOrder = _orderArray[pos];
            T newItem = _heap[(pos - 1) / 2];
            int newOrder = _orderArray[(pos - 1) / 2];

            while (pos > 0 && CompareItem(oldItem, newItem, oldOrder, newOrder) > 0)
            {
                _orderArray[pos] = newOrder;
                _heap[pos] = newItem;

                pos = (pos - 1) / 2;
                newItem = _heap[(pos - 1) / 2];
                newOrder = _orderArray[(pos - 1) / 2];
            }

            _orderArray[pos] = oldOrder;
            _heap[pos] = oldItem;
        }

        internal void ReheapDown(int initial)
        {
            int pos = initial;
            T oldItem = _heap[pos];
            int oldOrder = _orderArray[pos];

            while (pos < Count)
            {
                int left = 2 * pos + 1;
                int right = 2 * pos + 2;
                int swap = pos;

                if (left < Count && CompareItem(_heap[swap], _heap[left], _orderArray[swap], _orderArray[left]) < 0)
                    swap = left;

                if (right < Count && CompareItem(_heap[swap], _heap[right], _orderArray[swap], _orderArray[right]) < 0)
                    swap = right;

                if (swap == pos)
                {
                    break;
                }
                else
                {
                    _orderArray[pos] = _orderArray[swap];
                    _heap[pos] = _heap[swap];
                    pos = swap;
                }
            }

            _orderArray[pos] = oldOrder;
            _heap[pos] = oldItem;
        }

        private void Resize()
        {
            T[] newHeap = new T[_heap.Length * 3/2];
            int[] newOrder = new int[_heap.Length * 3/2];

            for (int i = 0; i < Count; i++)
            {
                newOrder[i] = _orderArray[i];
                newHeap[i] = _heap[i];
            }

            _orderArray = newOrder;
            _heap = newHeap;
        }

        private int CompareItem(T a, T b, int orderA, int orderB)
        {
            int result = a.CompareTo(b);
            if (result != 0)
                return result;
            else
                return orderA - orderB;
        }
    }
}
