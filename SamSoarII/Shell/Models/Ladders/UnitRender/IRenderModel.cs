using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace SamSoarII.Shell.Models
{
    public interface IRenderModel : IDisposable
    {
        void Render(int flag);
    }
}
