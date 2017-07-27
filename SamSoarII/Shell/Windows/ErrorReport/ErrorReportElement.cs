using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;
using System.Text;

namespace SamSoarII.Shell.Windows
{
    public class ErrorReportElement : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private PLCOriginInst inst;
        private LadderNetworkModel network;

        public ErrorReportElement
        (
            PLCOriginInst _inst,
            LadderNetworkModel _network
        )
        {
            this.inst = _inst;
            this.network = _network;
        }

        public void Dispose()
        {
            inst = null;
            network = null;
            PropertyChanged = null;
        }

        public LadderUnitModel Prototype
        {
            get { return inst.Inst.ProtoType; }
        }

        public int Status
        {
            get { return inst.Status; }
        }

        public Brush BrushFill
        {
            get
            {
                switch (inst.Status)
                {
                    case PLCOriginInst.STATUS_WARNING:
                        return Brushes.Yellow;
                    case PLCOriginInst.STATUS_ERROR:
                        return Brushes.Red;
                    default:
                        return Brushes.Transparent;
                }
            }
        }


        public string Detail
        {
            get { return inst.Message; }
        }

        public string InstText
        {
            get { return inst.Inst.Text; }
        }

        public string Network
        {
            get { return String.Format("{0:d}", network.ID); }
        }

        public string Diagram
        {
            get { return network.Parent.Name; }
        }
    }
}
