using SamSoarII.PLCDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.ComponentModel;
using SamSoarII.Shell.Managers;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using SamSoarII.Global;
using SamSoarII.Core.Generate;

namespace SamSoarII.Core.Models
{
    /*
    public class ValueInfoChangedEventArgs : PropertyChangedEventArgs
    {
        public ValueInfo Info { get; private set; }
        public ValueInfoChangedEventArgs(ValueInfo _info, string _name) : base(_name) { Info = _info; }
    }

    public delegate void ValueInfoChangedEventHandler(ValueManager sender, ValueInfoChangedEventArgs e);
    */
    public class ValueManager : IDisposable, IList<ValueInfo>, IModel
    {
        #region Resources

        public readonly static MaxRangeDevice MaxRange;
        private readonly static int XOffset;
        private readonly static int YOffset;
        private readonly static int MOffset;
        private readonly static int SOffset;
        private readonly static int COffset;
        private readonly static int TOffset;
        private readonly static int DOffset;
        private readonly static int CVOffset;
        private readonly static int TVOffset;
        private readonly static int AIOffset;
        private readonly static int AOOffset;
        private readonly static int VOffset;
        private readonly static int ZOffset;
        private readonly static int InfoCount;

        static ValueManager()
        {
            MaxRange = new MaxRangeDevice();
            XOffset = 0;
            YOffset = XOffset + MaxRange.XRange.Count;
            MOffset = YOffset + MaxRange.YRange.Count;
            SOffset = MOffset + MaxRange.MRange.Count;
            COffset = SOffset + MaxRange.SRange.Count;
            TOffset = COffset + MaxRange.CRange.Count;
            DOffset = TOffset + MaxRange.TRange.Count;
            CVOffset = DOffset + MaxRange.DRange.Count;
            TVOffset = CVOffset + MaxRange.CVRange.Count;
            AIOffset = TVOffset + MaxRange.TVRange.Count;
            AOOffset = AIOffset + MaxRange.AIRange.Count;
            VOffset = AOOffset + MaxRange.AORange.Count;
            ZOffset = VOffset + MaxRange.VRange.Count;
            InfoCount = ZOffset + MaxRange.ZRange.Count;
        }
        
        #endregion

        public ValueManager(InteractionFacade _parent)
        {
            parent = _parent;
            infos = new ValueInfo[InfoCount];
            emptyinfo = new ValueInfo(new ValuePrototype(ValueModel.Bases.NULL, 0));
            tempmodel = new ValueModel(null, new ValueFormat("TEMP", ValueModel.Types.NULL, false, false, 0, new Regex[] { ValueModel.VarRegex }));
            for (int i = 0; i < MaxRange.XRange.Count; i++)
                infos[i + XOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.X, i));
            for (int i = 0; i < MaxRange.YRange.Count; i++)
                infos[i + YOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.Y, i));
            for (int i = 0; i < MaxRange.MRange.Count; i++)
                infos[i + MOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.M, i));
            for (int i = 0; i < MaxRange.SRange.Count; i++)
                infos[i + SOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.S, i));
            for (int i = 0; i < MaxRange.CRange.Count; i++)
                infos[i + COffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.C, i));
            for (int i = 0; i < MaxRange.TRange.Count; i++)
                infos[i + TOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.T, i));
            for (int i = 0; i < MaxRange.DRange.Count; i++)
                infos[i + DOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.D, i));
            for (int i = 0; i < MaxRange.CVRange.Count; i++)
                infos[i + CVOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.CV, i));
            for (int i = 0; i < MaxRange.TVRange.Count; i++)
                infos[i + TVOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.TV, i));
            for (int i = 0; i < MaxRange.AIRange.Count; i++)
                infos[i + AIOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.AI, i));
            for (int i = 0; i < MaxRange.AORange.Count; i++)
                infos[i + AOOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.AO, i));
            for (int i = 0; i < MaxRange.VRange.Count; i++)
                infos[i + VOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.V, i));
            for (int i = 0; i < MaxRange.ZRange.Count; i++)
                infos[i + ZOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.Z, i));
            for (int i = 0; i < infos.Length; i++)
                infos[i].PropertyChanged += OnInfoPropertyChanged;
        }
        
        public void Dispose()
        {
            for (int i = 0; i < InfoCount; i++)
            {
                infos[i].PropertyChanged -= OnInfoPropertyChanged;
                infos[i].Dispose();
            }
            emptyinfo.Dispose();
            infos = null;
        }
        
        public void Initialize()
        {
            foreach (ValueInfo info in infos)
                info.Initialize();
            emptyinfo.Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        #region Number

        private InteractionFacade parent;
        public InteractionFacade Parent { get { return this.parent; } }
        IModel IModel.Parent { get { return null; } }
        public Device Device { get { return parent?.MDProj != null ? parent.MDProj.Device : MaxRange; } }
        private int XDelta { get { return MaxRange.XRange.Count - Device.XRange.Count; } }
        private int YDelta { get { return MaxRange.YRange.Count - Device.YRange.Count; } }

        private ValueInfo[] infos;
        private ValueInfo emptyinfo;
        public ValueInfo EmptyInfo { get { return this.emptyinfo; } }
        private ValueModel tempmodel;

        #endregion

        #region Shell

        private ProjectTreeViewItem ptvitem;
        public ProjectTreeViewItem PTVItem
        {
            get { return this.ptvitem; }
            set { this.ptvitem = value; }
        }

        #endregion

        #region View Manager

        private ValueViewManager view;
        public ValueViewManager View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view != null) view.Core = null;
                this.view = value;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View
        {
            get { return View; }
            set { View = (ValueViewManager)View; }
        }

        #endregion

        #region IList

        public int GetOffset(ValueModel.Bases bas)
        {
            switch (bas)
            {
                case ValueModel.Bases.X:
                    return XOffset;
                case ValueModel.Bases.Y:
                    return YOffset - XDelta;
                case ValueModel.Bases.M:
                    return MOffset - XDelta - YDelta;
                case ValueModel.Bases.S:
                    return SOffset - XDelta - YDelta;
                case ValueModel.Bases.C:
                    return COffset - XDelta - YDelta;
                case ValueModel.Bases.T:
                    return YOffset - XDelta - YDelta;
                case ValueModel.Bases.D:
                    return DOffset - XDelta - YDelta;
                case ValueModel.Bases.CV:
                    return CVOffset - XDelta - YDelta;
                case ValueModel.Bases.TV:
                    return TVOffset - XDelta - YDelta;
                case ValueModel.Bases.AI:
                    return AIOffset - XDelta - YDelta;
                case ValueModel.Bases.AO:
                    return AOOffset - XDelta - YDelta;
                case ValueModel.Bases.V:
                    return VOffset - XDelta - YDelta;
                case ValueModel.Bases.Z:
                    return ZOffset - XDelta - YDelta;
                default:
                    return -1;
            }
        }
        
        public int IndexOf(ValueModel value)
        {
            return value.Offset < Device.GetRange(value.Base).Count ? GetOffset(value.Base) + value.Offset : -1;
        }

        public int IndexOf(ValueInfo item)
        {
            return IndexOf(item.Prototype);
        }

        public void Insert(int index, ValueInfo item)
        {
        }

        public void RemoveAt(int index)
        {
        }

        public void Add(ValueInfo item)
        {
        }

        public void Clear()
        {
        }

        public bool Contains(ValueInfo item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ValueInfo[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < Count; i++)
            {
                array[i - arrayIndex] = this[i];
            }
        }

        public bool Remove(ValueInfo item)
        {
            return false;
        }

        public class ValueManagerEnumerator : IEnumerator<ValueInfo>
        {
            public ValueManagerEnumerator(ValueManager _parent)
            {
                parent = _parent;
                iter = -1;
            }

            private ValueManager parent;
            private int iter;

            public ValueInfo Current { get { return iter < parent.Count ? parent[iter] : null; } }

            object IEnumerator.Current { get { return this.Current; } }

            public void Dispose()
            {
                parent = null;
            }

            public bool MoveNext()
            {
                return ++iter < parent.Count;
            }

            public void Reset()
            {
                iter = -1;
            }
        }

        public IEnumerator<ValueInfo> GetEnumerator()
        {
            return new ValueManagerEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        
        public int Count { get { return InfoCount - XDelta - YDelta; } }

        public bool IsReadOnly { get { return true; } }

        public ValueInfo this[int index]
        {
            get
            {
                if (index < GetOffset(ValueModel.Bases.Y))
                    return infos[index];
                if (index < GetOffset(ValueModel.Bases.M))
                    return infos[index + XDelta];
                return infos[index + XDelta + YDelta];
            }
            set { }
        }

        public ValueInfo this[ValueModel value]
        {
            get
            {
                int id = IndexOf(value);
                return id >= 0 ? this[id] : emptyinfo;
            }
        }
        public ValueInfo this[string text]
        {
            get
            {
                tempmodel.Text = text;
                return this[tempmodel];
            }
        }

        #endregion

        #region Manipulation

        public void Add(ValueModel value)
        {
            ValueInfo vinfo = this[value];
            if (vinfo != emptyinfo)
            {
                vinfo.Add(value);
                vinfo.Add(value.Parent);   
            }
        }
        public void Remove(ValueModel value)
        {
            ValueInfo vinfo = this[value];
            if (vinfo != emptyinfo)
            {
                vinfo.Remove(value);
                vinfo.Remove(value.Parent);

            }
        }
        public void Add(LadderUnitModel unit)
        {
            foreach (ValueModel value in unit.Children) Add(value);
        }
        public void Remove(LadderUnitModel unit)
        {
            foreach (ValueModel value in unit.Children) Remove(value);
        }
        public void Add(LadderNetworkModel net)
        {
            foreach (LadderUnitModel unit in net.Children) Add(unit);
        }
        public void Remove(LadderNetworkModel net)
        {
            foreach (LadderUnitModel unit in net.Children) Remove(unit);
        }
        public void Add(LadderDiagramModel dia)
        {
            foreach (LadderNetworkModel net in dia.Children) Add(net);
        }
        public void Remove(LadderDiagramModel dia)
        {
            foreach (LadderNetworkModel net in dia.Children) Remove(net);
        }

        public void RemoveInvalidValues()
        {
            int bas = 0;
            int len = 0;
            int maxlen = 0;
            bas = XOffset;
            len = Device.XRange.Count;
            maxlen = MaxRange.XRange.Count;
            for (int i = bas + len; i < bas + maxlen; i++)
            {
                ValueInfo vinfo = infos[i];
                foreach (ValueModel value in vinfo.Values)
                    value.Text = "???";
                foreach (LadderUnitModel unit in vinfo.Units)
                    unit.Invoke(LadderUnitAction.UPDATE);
                vinfo.Initialize();
            }
            bas = YOffset;
            len = Device.YRange.Count;
            maxlen = MaxRange.YRange.Count;
            for (int i = bas + len; i < bas + maxlen; i++)
            {
                ValueInfo vinfo = infos[i];
                foreach (ValueModel value in vinfo.Values)
                    value.Text = "???";
                foreach (LadderUnitModel unit in vinfo.Units)
                    unit.Invoke(LadderUnitAction.UPDATE);
                vinfo.Initialize();
            }
        }

        #endregion

        #region Check

        public void Check()
        {
            if (GlobalSetting.IsCheckCoil)
            {
                CheckCoil(GetOffset(ValueModel.Bases.Y), Device.YRange.Count);
                CheckCoil(GetOffset(ValueModel.Bases.M), Device.MRange.Count);
                CheckCoil(GetOffset(ValueModel.Bases.S), Device.SRange.Count);
            }
            int bas = 0;
            int len = 0;
            //int maxlen = 0;
            bas = GetOffset(ValueModel.Bases.TV);
            len = Device.TVRange.Count;
            for (int i = bas; i < bas + len; i++)
            {
                ValueInfo vinfo = this[i];
                IEnumerable<LadderUnitModel> fits = vinfo.Units.Where(
                    u => u.Type == LadderUnitModel.Types.TON
                      || u.Type == LadderUnitModel.Types.TONR
                      || u.Type == LadderUnitModel.Types.TOF);
                if (fits.Count() > 1)
                {
                    foreach (PLCOriginInst inst in fits.Select(u => u.Inst.Origin))
                    {
                        inst.Status = PLCOriginInst.STATUS_ERROR;
                        inst.Message = String.Format("{0}{1:s}{2}", Properties.Resources.Counter, inst[1], Properties.Resources.Message_Has_Been_Used);
                    }
                }
            }
            bas = GetOffset(ValueModel.Bases.CV);
            len = Device.CVRange.Count;
            for (int i = bas; i < bas + len; i++)
            {
                ValueInfo vinfo = this[i];
                IEnumerable<LadderUnitModel> fits = vinfo.Units.Where(
                    u => u.Type == LadderUnitModel.Types.CTD
                      || u.Type == LadderUnitModel.Types.CTD
                      || u.Type == LadderUnitModel.Types.CTUD);
                if (fits.Count() > 1)
                {
                    foreach (PLCOriginInst inst in fits.Select(u => u.Inst.Origin))
                    {
                        inst.Status = PLCOriginInst.STATUS_ERROR;
                        inst.Message = String.Format("{0}{1:s}{2}", Properties.Resources.Counter, inst[1], Properties.Resources.Message_Has_Been_Used);
                    }
                }
            }
            /*
            bas = XOffset;
            len = Device.XRange.Count;
            maxlen = MaxRange.XRange.Count;
            for (int i = bas + len; i < bas + maxlen; i++)
            {
                ValueInfo vinfo = infos[i];
                if (vinfo.Units.Count() > 0)
                {
                    foreach (PLCOriginInst inst in vinfo.Units.Select(u => u.Inst.Origin))
                    {
                        inst.Status = PLCOriginInst.STATUS_ERROR;
                        inst.Message = Properties.Resources.Message_OutOfXRange;
                    }
                }
            }
            bas = YOffset;
            len = Device.YRange.Count;
            maxlen = MaxRange.YRange.Count;
            for (int i = bas + len; i < bas + maxlen; i++)
            {
                ValueInfo vinfo = infos[i];
                if (vinfo.Units.Count() > 0)
                {
                    foreach (PLCOriginInst inst in vinfo.Units.Select(u => u.Inst.Origin))
                    {
                        inst.Status = PLCOriginInst.STATUS_ERROR;
                        inst.Message = Properties.Resources.Message_OutOfYRange;
                    }
                }
            }
            */
        }

        private void CheckCoil(int bas, int len)
        {
            for (int i = bas; i < bas + len; i++)
            {
                ValueInfo vinfo = this[i];
                IEnumerable<LadderUnitModel> fits = vinfo.Units.Where(
                    u => u.Type == LadderUnitModel.Types.OUT 
                      || u.Type == LadderUnitModel.Types.OUTIM);
                if (fits.Count() > 1)
                {
                    foreach (PLCOriginInst inst in fits.Select(u => u.Inst.Origin))
                    {
                        inst.Status = PLCOriginInst.STATUS_ERROR;
                        inst.Message = String.Format("{0:s}{1}", inst[1], Properties.Resources.Message_Multi_Coil);
                    }
                }
            }
        }

        #endregion
        
        #region Save & Load

        public void Save(XElement xele)
        {
            foreach (ValueInfo info in infos)
            {
                if (info.IsUsed)
                {
                    XElement xele_i = new XElement("ValueInfo");
                    xele_i.SetAttributeValue("Name", info.Name);
                    xele_i.SetAttributeValue("Comment", info.Comment);
                    xele_i.SetAttributeValue("Alias", info.Alias);
                    if (info.InitModel != null)
                        xele_i.Add(info.InitModel.CreateXElementByModel());
                    xele.Add(xele_i);
                }
            }
        }

        public void Load(XElement xele)
        {
            foreach (XElement xele_i in xele.Elements("ValueInfo"))
            {
                string name = xele_i.Attribute("Name").Value;
                ValueInfo vinfo = this[name];
                vinfo.Comment = xele_i.Attribute("Comment").Value;
                vinfo.Alias = xele_i.Attribute("Alias").Value;
                XElement xele_im = xele_i.Element("InitModel");
                if (xele_im != null)
                {
                    string type = xele_im.Attribute("Type").Value;
                    IElementInitializeModel model = type.Equals("Bit") 
                        ? (IElementInitializeModel)(new BitElementModel()) 
                        : (IElementInitializeModel)(new WordElementModel());
                    model.LoadByXElement(xele_im);
                    vinfo.InitModel = model;
                }
            }
        }

        #endregion

        #region Event Handler

        private void OnInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (parent.MDProj != null)
                parent.MDProj.InvokeModify(this);
        }

        #endregion
    }

}
