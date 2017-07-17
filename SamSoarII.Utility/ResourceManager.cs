using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Utility
{
    public interface IResource : IDisposable
    {
        IResource Create(params object[] args);
        void Recreate(params object[] args);
        bool IsDisposed { get; set; }
    }

    public class ResourceManager<T> where T : IResource
    {
        public ResourceManager(T _template)
        {
            template = _template;
            data = new T[1024];
            usedcount = 0;
            freecount = 0;
        }

        #region Number

        private T template;
        private T[] data;
        private int usedcount;
        private int freecount;
        
        public int Count
        {
            get
            {
                return data.Length;
            }
            private set
            {
                T[] _data = new T[value];
                int _count = Math.Min(Count, value);
                Array.Copy(data, _data, _count);
                data = _data;
            }
        }
        
        #endregion

        public T Create(params object[] args)
        {
            if (usedcount >= Count) Count *= 2;
            if (data[usedcount] == null)
                data[usedcount] = (T)(template.Create(args));
            else
                data[usedcount].Recreate(args);
            data[usedcount].IsDisposed = false;
            return data[usedcount++];
        }

        public void Dispose(T item)
        {
            item.IsDisposed = true;
            if (++freecount >= 256) Collect();
        }
        
        public void Collect()
        {
            usedcount--;
            while (usedcount >= 0 && data[usedcount].IsDisposed) usedcount--;
            for (int i = 0; i < usedcount; i++)
            {
                if (data[i].IsDisposed)
                {
                    template = data[i];
                    data[i] = data[usedcount];
                    data[usedcount--] = template;
                    while (usedcount >= 0 && data[usedcount].IsDisposed) usedcount--;
                }
            }
            usedcount++;
            freecount = 0;
        }
    }
}
