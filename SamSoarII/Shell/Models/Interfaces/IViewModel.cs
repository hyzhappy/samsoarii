using SamSoarII.Core.Models;
using System;

namespace SamSoarII.Shell.Models
{
    public interface IViewModel : IDisposable
    {
        IModel Core { get; set; }
        IViewModel ViewParent { get; }
    }
}
