using SamSoarII.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace SamSoarII.Shell.Models
{
    public interface ILoadModel : IViewModel
    {
        bool IsFullLoaded { get; }
        void FullLoad();
        void UpdateFullLoadProgress();
        void Update();
        ViewThreadManager ViewThread { get; }
        IEnumerable<ILoadModel> LoadChildren { get; }
        Dispatcher Dispatcher { get; }
    }
}
