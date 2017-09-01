using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SamSoarII.Utility.DXF
{
    public class DXFReader
    {
        public int CurrentCode;
        public string CurrentValue;
        public StreamReader Reader;
        public DXFReader(string filename)
        {
            try
            {
                Reader = new StreamReader(filename);
            }
            catch (Exception)
            {
            }
        }
        public void MoveNext()
        {
            CurrentCode = Convert.ToInt32(Reader.ReadLine());
            CurrentValue = Reader.ReadLine();
        }

        public void Close()
        {
            Reader.Close();
        }
    }
}
