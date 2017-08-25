using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Windows;

namespace SamSoarII.Shell.Windows
{
    public class ErrorReportElement : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private int status;
        private string message;
        private LadderUnitModel unit;
        private LadderNetworkModel network;
        private LadderDiagramModel diagram;

        public ErrorReportElement(PLCOriginInst _inst)
        {
            this.status = _inst.Status;
            this.message = _inst.Message;
            this.unit = _inst.Inst.ProtoType;
            this.network = unit.Parent;
            this.diagram = network.Parent;
            unit.Changed += OnUnitChanged;
            network.Changed += OnNetworkChanged;
            diagram.Changed += OnDiagramChanged;
        }
        
        public void Dispose()
        {
            unit.Changed -= OnUnitChanged;
            network.Changed -= OnNetworkChanged;
            diagram.Changed -= OnDiagramChanged;
            unit = null;
            network = null;
            diagram = null;
            Changed = delegate { };
            PropertyChanged = delegate { };
        }

        public LadderUnitModel Unit
        {
            get { return this.unit; }
        }

        public int Status
        {
            get { return this.status; }
        }

        public Brush BrushFill
        {
            get
            {
                switch (status)
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
            get { return this.message; }
        }

        public string InstText
        {
            get { return unit.ToInstString(); }
        }

        public string Network
        {
            get { return String.Format("{0:d}", network.ID); }
        }

        public string Diagram
        {
            get { return diagram.Name; }
        }

        #region Event Handler

        public event RoutedEventHandler Changed = delegate { };

        private void OnUnitChanged(LadderUnitModel sender, LadderUnitChangedEventArgs e)
        {
            Changed(this, new RoutedEventArgs());
        }

        private void OnNetworkChanged(LadderNetworkModel sender, LadderNetworkChangedEventArgs e)
        {
            Changed(this, new RoutedEventArgs());
        }

        private void OnDiagramChanged(LadderDiagramModel sender, LadderDiagramChangedEventArgs e)
        {
            Changed(this, new RoutedEventArgs());
        }

        #endregion
    }
}
