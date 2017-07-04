using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Utility
{

    public class CanvasDic<T> : IEnumerable<T>, IEnumerator<T>
    {
        #region Range
        private int sx1;
        private int sx2;
        private int sy1;
        private int sy2;
        #endregion

        private int _cursorX;
        private int _cursorY;

        private bool _isSelectMode = false;
        private T[][] models;
        private int XCapacity;
        private int _YCapacity = 8;
        public int YCapacity
        {
            get
            {
                return _YCapacity;
            }
            set
            {
                _YCapacity = value;
                ArrangeDic();
            }
        }
        
        private bool _Assert(int x,int y)
        {
            return (x >= sx1 && x <= sx2 && y >= sy1 && y <= sy2 && models[y] != null);
        }

        public CanvasDic(int XCapacity)
        {
            models = new T[YCapacity][];
            this.XCapacity = XCapacity;
            SetRange(0,XCapacity - 1,0,YCapacity - 1);
        }
        private void Insert(int x, int y, T value)
        {
            if (!Assert(x)) return;
            while (y > YCapacity - 1) { YCapacity *= 2; }
            if (models[y] == null) models[y] = new T[XCapacity];
            models[y][x] = value;
        }
        public void Remove(int x,int y)
        {
            if (!Assert(x, y)) return;
            models[y][x] = default(T);
        }
        public CanvasDic<T> SelectRange(int _x1, int _x2, int _y1, int _y2)
        {
            SetRange(_x1,_x2,_y1,_y2);
            _isSelectMode = true;
            return this;
        }
        public void Clear(int _x1,int _x2,int _y1,int _y2)
        {
            for (int y = _y1; y <= _y2; y++)
            {
                if (models[y] == null) continue;
                for (int x = _x1; x <= _x2; x++)
                    models[y][x] = default(T);
            }
        }
        public void Clear()
        {
            Clear(0,XCapacity - 1,0,YCapacity - 1);
        }
        public T this[int x,int y]
        {
            get
            {
                if (Assert(x, y))
                    return models[y][x];
                else
                    return default(T);
            }
            set { Insert(x, y, value); }
        }

        public T[] this[int index,bool isRow]
        {
            get
            {
                if (isRow && index < YCapacity) return models[index];
                else if (!isRow && index < XCapacity)
                {
                    T[] temp = new T[YCapacity];
                    for (int i = 0; i < YCapacity; i++)
                    {
                        temp[i] = models[i][index];
                    }
                    return temp;
                }
                else return null;
            }
        }

        private bool Assert(int x, int y)
        {
            return (x >= 0 && x < XCapacity && y >= 0 && y < YCapacity && models[y] != null);
        }
        private bool Assert(int x)
        {
            return (x >= 0 && x < XCapacity);
        }
        private void ArrangeDic()
        {
            var temp = new T[_YCapacity][];
            for (int y = 0; y < Math.Min(models.Length,_YCapacity); y++)
                temp[y] = models[y];
            models = temp;
        }
        private void SetRange(int _x1, int _x2, int _y1, int _y2)
        {
            sx1 = Math.Min(_x1, _x2);
            sx2 = Math.Max(_x1, _x2);
            sy1 = Math.Min(_y1, _y2);
            sy2 = Math.Max(_y1, _y2);
            Reset();
        }
        public IEnumerator<T> GetEnumerator()
        {
            if (!_isSelectMode)
                SetRange(0, XCapacity - 1, 0, YCapacity - 1);
            else
                _isSelectMode = false;
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T Current
        {
            get
            {
                if (!_Assert(_cursorX, _cursorY)) return default(T);
                else return models[_cursorY][_cursorX];
            }
        }
        object IEnumerator.Current => Current;

        public void Dispose()
        {
            //models = null;
        }

        public bool MoveNext()
        {
            while(_cursorY <= sy2)
            {
                if (_cursorX > sx2)
                {
                    _cursorX = sx1;
                    _cursorY++;
                }
                else
                    _cursorX++;
                if (Current != null)
                    return true;
            }
            return false;
        }

        public void Reset()
        {
            _cursorX = sx1 - 1;
            _cursorY = sy1;
        }
    }
}
