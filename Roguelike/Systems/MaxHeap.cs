using System;
using Roguelike.Interfaces;

namespace Roguelike.Systems
{
    class MaxHeap<T> where T : ISchedulable
    {
        private T[] _heap;
        private int _heapSize;

        public MaxHeap() : this(10)
        {
        }

        public MaxHeap(int size)
        {
            _heap = new T[size];
            _heapSize = 0;
        }

        public void Add(T item)
        {
            if (_heapSize >= _heap.Length)
                Resize();

            _heap[_heapSize++] = item;
            ReheapUp();
        }

        public void Remove(T item)
        {
            for (int i = 0; i < _heapSize; i++)
            {
                if (_heap[i].Equals(item))
                {
                    _heap[i] = _heap[--_heapSize];
                    ReheapDown(i);
                }
            }
        }

        public void UpdateAll()
        {
            for (int i = 0; i < _heapSize; i++)
            {
                _heap[i].Energy += _heap[i].RefreshRate;
            }
        }

        public T GetMax()
        {
            T item = _heap[0];
            _heap[0] = _heap[--_heapSize];
            ReheapDown(0);

            return item;
        }

        public ISchedulable Peek() => _heap[0];
        public bool IsEmpty() => _heapSize == 0;
        public int Size() => _heapSize;

        private void ReheapUp()
        {
            int pos = _heapSize - 1;
            T item = _heap[pos];

            while (pos > 0 && item.CompareTo(_heap[(pos - 1) / 2]) > 0)
            {
                _heap[pos] = _heap[(pos-1)/2];
                pos = (pos-1)/2;
            }

            _heap[pos] = item;
        }

        private void ReheapDown(int initial)
        {
            int pos = initial;
            T item = _heap[pos];

            while (pos < _heapSize)
            {
                int left = 2 * pos + 1;
                int right = 2 * pos + 2;
                int swap = pos;
                T tempItem = item;

                if (left < _heapSize && tempItem.CompareTo(_heap[left]) < 0)
                {
                    swap = left;
                    tempItem = _heap[swap];
                }

                if (right < _heapSize && tempItem.CompareTo(_heap[right]) < 0)
                    swap = right;

                if (swap == pos)
                {
                    break;
                }
                else
                {
                    _heap[pos] = _heap[swap];
                    pos = swap;
                }
            }

            _heap[pos] = item;
        }

        private void Resize()
        {
            T[] newHeap = new T[_heap.Length * 2];

            for (int i = 0; i < _heapSize; i++)
            {
                newHeap[i] = _heap[i];
            }

            _heap = newHeap;
        }
    }
}
