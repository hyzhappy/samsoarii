using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Utility
{
    public class GridDictionary<T> : IEnumerable<T>
    {
        private T[][] data;

        private int width;
        private int height;
        public int Width
        {
            get
            {
                return this.width;
            }
        }
        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                int _height = height;
                T[][] _data = data;
                this.height = value;
                data = new T[height][];
                for (int i = 0; i < height; i++)
                {
                    data[i] = (_data != null && i < _height)
                        ? _data[i] : new T[width];
                }
            }
        }

        public GridDictionary(int _width, int _height = 8)
        {
            width = _width;
            Height = _height;
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
            Clear(0, Width - 1, 0, Height - 1);
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
        public T this[int x, int y]
        {
            get
            {
                if (!_Assert(x, y)) return default(T);
                return data[y][x];
            }
            set
            {
                if (x < 0 || x >= Width || y < 0) return;
                while (Height <= y) Height *= 2;
                data[y][x] = value;
            }
        }
        public GridDictionarySelector<T> SelectRange(int x1, int x2, int y1, int y2)
        {
            x1 = Math.Max(x1, 0);
            y1 = Math.Max(y1, 0);
            x2 = Math.Min(x2, Width - 1);
            y2 = Math.Min(y2, Height - 1);
            return _Assert(x1, x2, y1, y2)
                ? new GridDictionarySelector<T>(this, x1, x2, y1, y2)
                : GridDictionarySelector<T>.Empty;
        }
        public void Set(int x, int y, IGridDictionarySelector<T> selector)
        {
            if (selector.Width <= 0 || selector.Height <= 0
             || x < 0 || x + selector.Width >= Width
             || y < 0 || y + selector.Height >= Height)
            {
                return;
            }
            if (Height < y + selector.Height)
            {
                Height = (int)((y + selector.Height) * 1.5);
            }
            for (int _x = x; _x < x + selector.Width; _x++)
                for (int _y = y; _y < y + selector.Height; _y++)
                {
                    data[_y][_x] = selector[_x - x + selector.X1, _y - y + selector.Y1];
                }
        }
    }

    public interface IGridDictionarySelector<T> : IEnumerable<T>, IEnumerator<T> 
    {
        int X1 { get; }
        int X2 { get; }
        int Y1 { get; }
        int Y2 { get; }
        int Width { get; }
        int Height { get; }
        void Clear();
        T this[int x, int y] { get; set; }
        IGridDictionarySelector<T> Clone();
    }

    public class GridDictionarySelector<T> : IGridDictionarySelector<T> 
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
        
        public IEnumerator<T> GetEnumerator()
        {
            Reset();
            return this;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            Reset();
            return this;
        }

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
        
        public T Current { get { return cx >= x1 && cx <= x2 && cy >= y1 && cy <= y2 
                    && dict != null ? dict[cx, cy] : default(T); } }
        
        object IEnumerator.Current { get { return Current; } }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (dict == null)
            {
                return false;
            }
            while (cy <= y2)
            {
                if (++cx > x2)
                {
                    cx = x1;
                    cy++;
                }
                if (Current != null)
                    return true;
            }
            return false;
        }

        public void Reset()
        {
            cx = x1 - 1;
            cy = y1;
        }

        public void Clear()
        {
            if (dict != null) dict.Clear(x1, x2, y1, y2);
        }

        public T this[int x, int y]
        {
            get
            {
                return dict != null ? dict[x, y] : default(T);
            }
            set
            {
                if (dict != null) dict[x, y] = value;
            }
        }
        
        public IGridDictionarySelector<T> Clone()
        {
            return new GridDictionarySelectorClone<T>(this);
        }
    }

    public class GridDictionarySelectorClone<T> : IEnumerable<T>, IGridDictionarySelector<T> 
    {
        private T[,] data;
        private int top;
        private int left;
        private int width;
        private int height;
        private int x;
        private int y;

        public int X1 { get { return top; } }
        public int X2 { get { return top + width - 1; } }
        public int Y1 { get { return left; } }
        public int Y2 { get { return left + height - 1; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public GridDictionarySelectorClone(IGridDictionarySelector<T> origin)
        {
            top = origin.Y1;
            left = origin.X1;
            width = origin.Width;
            height = origin.Height;
            data = new T[width, height];

            origin.Reset();
            for (x = 0; x < width; x++)
                for (y = 0; y < height; y++)
                {
                    origin.MoveNext();
                    data[x, y] = origin.Current;
                }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            Reset();
            return this;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            Reset();
            return this;
        }

        public T Current { get { return x >= 0 && x < width && y >= 0 && y < height ? data[x, y] : default(T); } }

        object IEnumerator.Current { get { return Current; } }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            while (y < height)
            {
                if (++x >= width)
                {
                    x = 0;
                    y++;
                }
                if (Current != null)
                    return true;
            }
            return false;
        }

        public void Reset()
        {
            x = -1;
            y = 0;
        }

        public void Clear()
        {
            for (int _x = 0; _x < width; _x++)
                for (int _y = 0; _y < height; _y++)
                {
                    data[_x, _y] = default(T);
                }
        }

        public T this[int x, int y]
        {
            get
            {
                return (x >= top && x < top + width && y >= left && y < left + height)
                    ? data[x - top, y - left] : default(T);
            }
            set
            {
                if (x >= top && x < top + width && y >= left && y < left + height)
                {
                    data[x - top, y - left] = value;
                }
            }
        }
        
        public IGridDictionarySelector<T> Clone()
        {
            return new GridDictionarySelectorClone<T>(this);
        }
    }
}
