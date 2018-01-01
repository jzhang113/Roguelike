using System;
using Roguelike.Interfaces;

namespace Roguelike.Systems
{
    class MinHeap
    {
        private IAction[] _heap;
        private int _heapSize;

        public MinHeap() : this(10)
        {
        }

        public MinHeap(int size)
        {
            _heap = new IAction[size];
            _heapSize = 0;
        }

        public void Add(IAction item)
        {
            if (_heapSize >= _heap.Length)
                Resize();

            _heap[_heapSize++] = item;
            ReheapUp();
        }

        public void Cancel(IActor actor)
        {
            for (int i = 0; i < _heapSize; i++)
            {
                if (_heap[i].Source == actor)
                {
                    _heap[i] = _heap[--_heapSize];
                    ReheapDown(i);
                }
            }
        }

        public IAction GetMin()
        {
            IAction item = _heap[0];
            _heap[0] = _heap[--_heapSize];
            ReheapDown(0);

            return item;
        }

        public void UpdateAllActions(int dt, IActor actor)
        {
            for (int i = 0; i < _heapSize; i++)
            {
                _heap[i].Time -= dt;
            }
        }

        public bool IsEmpty() => _heapSize == 0;

        private void ReheapUp()
        {
            int pos = _heapSize - 1;
            IAction item = _heap[pos];

            while (pos > 0 && item.Time < _heap[(pos-1)/2].Time)
            {
                _heap[pos] = _heap[(pos-1)/2];
                pos = (pos-1)/2;
            }

            _heap[pos] = item;
        }

        private void ReheapDown(int initial)
        {
            int pos = initial;
            IAction item = _heap[pos];

            while (pos < _heapSize)
            {
                int left = 2 * pos + 1;
                int right = 2 * pos + 2;
                int swap = pos;
                IAction tempItem = item;

                if (left < _heapSize && tempItem.Time > _heap[left].Time)
                {
                    swap = left;
                    tempItem = _heap[swap];
                }

                if (right < _heapSize && tempItem.Time > _heap[right].Time)
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
            IAction[] newHeap = new IAction[_heap.Length * 2];

            for (int i = 0; i < _heapSize; i++)
            {
                newHeap[i] = _heap[i];
            }

            _heap = newHeap;
        }
    }
}
