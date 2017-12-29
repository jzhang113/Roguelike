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
            ReheapUp(item);
        }

        public IAction GetMin()
        {
            IAction item = _heap[0];
            _heap[0] = _heap[--_heapSize];
            ReheapDown(item);

            return item;
        }

        public void UpdateAllActions(int dt)
        {
            for (int i = 0; i < _heapSize; i++)
            {
                _heap[i].Time -= dt;
            }
        }

        public bool IsEmpty()
        {
            return _heapSize == 0;
        }

        private void ReheapUp(IAction item)
        {
            int pos = _heapSize - 1;

            while (pos > 1 && _heap[pos].Time > _heap[pos/2].Time)
            {
                _heap[pos] = _heap[pos / 2];
                pos /= 2;
            }

            _heap[pos] = item;
        }

        private void ReheapDown(IAction item)
        {
            int pos = 0;

            while (pos < _heapSize)
            {
                int left = 2 * pos + 1;
                int right = 2 * pos + 2;

                if (left < _heapSize && _heap[left].Time > _heap[pos].Time)
                {
                    _heap[pos] = _heap[left];
                    pos = left;
                }
                else if (right < _heapSize && _heap[right].Time > _heap[pos].Time)
                {
                    _heap[pos] = _heap[right];
                    pos = right;
                }
                else
                {
                    break;
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
