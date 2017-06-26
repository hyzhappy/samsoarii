using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Extend.Utility
{
    public class GridDictionary<T> : IEnumerable<T>
    {
        private List<T[]> data;

        private int width;
        private int maxheight;
        public int Width { get { return width; } }
        public int Height { get { return data.Count(); } }

        public GridDictionary(int _width, int _maxheight = 64)
        {
            data = new List<T[]>();
            width = _width;
            maxheight = _maxheight;
        }
        
        private bool _Assert(int x, int y)
        {
            return (x >= 0 && x < Width && y >= 0 && y < Height);
        }
        private bool _Assert(int x1, int x2, int y1, int y2)
        {
            return (_Assert(x1, y1) && _Assert(x2, y2) && x1 <= x2 && y1 <= y2);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new GridDictionarySelector<T>(this, 0, Width - 1, 0, Height - 1);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            data.Clear();
        }
        public void Clear(int x1, int x2, int y1, int y2)
        {
            if (!_Assert(x1, x2, y1, y2)) return;
            for (int y = y1; y <= y2; y++)
                for (int x = x1; x <= x2; x++)
                {
                    data[y][x] = default(T);
                }
        }
        public T Get(int x, int y)
        {
            if (!_Assert(x, y)) return default(T);
            return data[y][x];
        }
        public GridDictionarySelector<T> Get(int x1, int x2, int y1, int y2)
        {
            return _Assert(x1, x2, y1, y2)
                ? new GridDictionarySelector<T>(this, x1, x2, y1, y2)
                : GridDictionarySelector<T>.Empty;
        }
        public void Set(int x, int y, T value)
        {
            if (x < 0 || x >= Width || y < 0 || y >= maxheight) return;
            while (Height <= y) data.Add(new T[Width]);
            data[y][x] = value;
        }
        public void Set(int x, int y, GridDictionarySelector<T> selector)
        {
            selector.Reset();
            if (!selector.MoveNext()
             || x < 0 || x + selector.Width >= Width
             || y < 0 || y + selector.Height >= Height)
            {
                return;
            }
            while (Height < y + selector.Height)
            {
                data.Add(new T[Width]);
            }
            for (int _x = x; _x < x + selector.Width; _x++)
                for (int _y = y; _y < y + selector.Height; _y++)
                {
                    data[_y][_x] = selector.Current;
                    selector.MoveNext();
                }
        }
    }

    public class GridDictionarySelector<T> : IEnumerator<T>
    {
        public static GridDictionarySelector<T> Empty { get; private set; }
            = new GridDictionarySelector<T> (null, 0, 0, 0, 0);

        private GridDictionary<T> dict;
        private int x1;
        private int x2;
        private int y1;
        private int y2;
        private int cx;
        private int cy;

        public int X1 { get { return x1; } }
        public int X2 { get { return x2; } }
        public int Y1 { get { return y1; } }
        public int Y2 { get { return y2; } }
        public int Width { get { return x2 - x1 + 1; } }
        public int Height { get { return y2 - y1 + 1; } }

        public GridDictionarySelector (
           GridDictionary<T> _dict, int _x1, int _x2, int _y1, int _y2)
        {
            dict = _dict;
            x1 = _x1;
            x2 = _x2;
            y1 = _y1;
            y2 = _y2;
            Reset();
        }
        
        public T Current { get { return dict != null ? dict.Get(cx, cy) : default(T); } }

        object IEnumerator.Current { get { return Current; } }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (dict == null || cy > y2)
            {
                return false;
            }
            if (++cx > x2)
            {
                cx = x1;
                return ++cy <= y2;
            }
            return true;
        }

        public void Reset()
        {
            cx = x1 - 1;
            cy = y1;
        }
    }
}
