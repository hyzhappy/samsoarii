using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Utility
{
    public interface IResource : IDisposable
    {
        int ResourceID { get; set; }
        IResource Create(params object[] args);
        void Recreate(params object[] args);
    }

    public class ResourceManager<T> where T : IResource
    {
        public ResourceManager(T _template, int _count, params object[] args)
        {
            template = _template;
            count = _count;
            usedcount = 0;
            data = new T[count*4];
            for (int i = 0; i < count; i++)
            {
                data[i] = (T)(template.Create(args));
                data[i].ResourceID = i;
            }
        }

        #region Number

        private T template;
        private T[] data;
        private int count;
        private int usedcount;
        
        #endregion
        
        public T Create(params object[] args)
        {
            if (usedcount >= count)
            {
                if (count < data.Length)
                {
                    data[count] = (T)(template.Create(args));
                    data[count].ResourceID = count;
                    count++;
                }
                else
                {
                    template = (T)(template.Create(args));
                    template.ResourceID = -1;
                    return template;
                }
            }
            data[usedcount].Recreate(args);
            return data[usedcount++];
        }

        public void Dispose(T item)
        {
            if (item.ResourceID == -1) return;
            int id1 = item.ResourceID;
            int id2 = --usedcount;
            data[id1].ResourceID = id2;
            data[id2].ResourceID = id1;
            template = data[id1];
            data[id1] = data[id2];
            data[id2] = template;
        }
    }
}
