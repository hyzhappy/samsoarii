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
using SamSoarII.Core.Communication;
using SamSoarII.Utility;

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
            emptyinfo = new ValueInfo(new ValuePrototype(ValueModel.Bases.NULL, 0), -1);
            tempmodel = new ValueModel(null, new ValueFormat("TEMP", ValueModel.Types.NULL, false, false, 0, new Regex[] { ValueModel.VarRegex, ValueModel.IntKValueRegex, ValueModel.IntHValueRegex}));
            wbitmodel = new ValueModel(null, new ValueFormat("WBIT", ValueModel.Types.BOOL, false, false, 0, new Regex[] { ValueModel.WordBitRegex }));
            bdwordmodel = new ValueModel(null, new ValueFormat("BDW", ValueModel.Types.DWORD, false, false, 0, new Regex[] { ValueModel.BitDoubleWordRegex }));
            for (int i = 0; i < MaxRange.XRange.Count; i++)
                infos[i + XOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.X, i), i + XOffset);
            for (int i = 0; i < MaxRange.YRange.Count; i++)
                infos[i + YOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.Y, i), i + YOffset);
            for (int i = 0; i < MaxRange.MRange.Count; i++)
                infos[i + MOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.M, i), i + MOffset);
            for (int i = 0; i < MaxRange.SRange.Count; i++)
                infos[i + SOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.S, i), i + SOffset);
            for (int i = 0; i < MaxRange.CRange.Count; i++)
                infos[i + COffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.C, i), i + COffset);
            for (int i = 0; i < MaxRange.TRange.Count; i++)
                infos[i + TOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.T, i), i + TOffset);
            int dataaddr = DOffset;
            for (int i = 0; i < MaxRange.DRange.Count; i++)
            {
                infos[i + DOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.D, i), dataaddr);
                dataaddr += 2;
            }
            for (int i = 0; i < MaxRange.CVRange.Count; i++)
            {
                infos[i + CVOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.CV, i), dataaddr);
                dataaddr += (i <= MaxRange.CV16Range.End ? 2 : 4);
            }
            for (int i = 0; i < MaxRange.TVRange.Count; i++)
            {
                infos[i + TVOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.TV, i), dataaddr);
                dataaddr += 2;
            }
            for (int i = 0; i < MaxRange.AIRange.Count; i++)
            {
                infos[i + AIOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.AI, i), dataaddr);
                dataaddr += 2;
            }
            for (int i = 0; i < MaxRange.AORange.Count; i++)
            {
                infos[i + AOOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.AO, i), dataaddr);
                dataaddr += 2;
            }
            storev = new ValueStore[MaxRange.VRange.Count];
            for (int i = 0; i < MaxRange.VRange.Count; i++)
            {
                infos[i + VOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.V, i), dataaddr);
                storev[i] = new ValueStore(infos[i + VOffset], ValueModel.Types.WORD);
                dataaddr += 2;
            }
            storez = new ValueStore[MaxRange.ZRange.Count];
            for (int i = 0; i < MaxRange.ZRange.Count; i++)
            {
                infos[i + ZOffset] = new ValueInfo(new ValuePrototype(ValueModel.Bases.Z, i), dataaddr);
                storez[i] = new ValueStore(infos[i + ZOffset], ValueModel.Types.WORD);
                dataaddr += 2;
            }
            datas = new byte[dataaddr];
            dataused = new bool[dataaddr];
            for (int i = 0; i < infos.Length; i++)
            {
                infos[i].PropertyChanged += OnInfoPropertyChanged;
                infos[i].PostValueStoreEvent += OnReceiveValueStoreEvent;
            }
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
        private ValueModel wbitmodel;
        private ValueModel bdwordmodel;
       
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
            if (value.IsWordBit)
                return value.Offset < Device.GetRange(value.Base).Count ? GetOffset(value.Base) + (value.Offset>>4) : -1;
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
                try
                {
                    tempmodel.Text = text;
                    if (tempmodel.Base != ValueModel.Bases.NULL)
                        return this[tempmodel];
                }
                catch (ValueParseException)
                {
                }
                try
                {
                    wbitmodel.Text = text;
                    if (wbitmodel.Base != ValueModel.Bases.NULL)
                        return this[wbitmodel];
                }
                catch (ValueParseException)
                {
                }
                try
                {
                    bdwordmodel.Text = text;
                    if (bdwordmodel.Base != ValueModel.Bases.NULL)
                        return this[bdwordmodel];
                }
                catch (ValueParseException)
                {
                }
                return emptyinfo;
            }
        }

        #endregion

        #region Manipulation

        public void Add(ValueModel value)
        {
            ValueInfo vinfo = this[value];
            //if (vinfo != emptyinfo)
            //{
                vinfo.Add(value);
                vinfo.Add(value.Parent);   
            //}
        }
        public void Remove(ValueModel value)
        {
            ValueInfo vinfo = this[value];
            //if (vinfo != emptyinfo)
            //{
                vinfo.Remove(value);
                vinfo.Remove(value.Parent);
            //}
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
            if (GlobalSetting.IsCheckTimer)
            {
                bas = GetOffset(ValueModel.Bases.TV);
                len = Device.TVRange.Count;
                for (int i = bas; i < bas + len; i++)
                {
                    ValueInfo vinfo = this[i];
                    IEnumerable<LadderUnitModel> fits = vinfo.UsedUnits.Where(
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
            }
            if (GlobalSetting.IsCheckCounter)
            {
                bas = GetOffset(ValueModel.Bases.CV);
                len = Device.CVRange.Count;
                for (int i = bas; i < bas + len; i++)
                {
                    ValueInfo vinfo = this[i];
                    IEnumerable<LadderUnitModel> fits = vinfo.UsedUnits.Where(
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
            }
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

        #region Monitor Data

        private byte[] datas;
        private bool[] dataused;

        private ValueStore[] storev;
        private ValueStore[] storez;
        
        public IList<GeneralReadCommand> GetReadCommands()
        {
            for (int i = 0; i < infos.Count(); i++)
                dataused[i] = false;
            for (int i = 0; i < infos.Count(); i++)
            {
                IEnumerable<ValueStore> visibles = infos[i].Stores.Where(vs => vs.VisualRefNum > 0);
                switch (infos[i].Prototype.Base)
                {
                    case ValueModel.Bases.V:
                    case ValueModel.Bases.Z:
                        dataused[infos[i].DataAddr] = true;
                        dataused[infos[i].DataAddr + 1] = true;
                        break;
                    default:
                        if (visibles.Count() == 0) continue;
                        int maxbyte = visibles.Max(vs => vs.ByteCount);
                        for (int j = infos[i].DataAddr; j < infos[i].DataAddr + maxbyte; j++)
                            dataused[j] = true;
                        break;
                }
                IEnumerable<ValueStore> intras = visibles.Where(vs => vs.Intra != ValueModel.Bases.NULL);
                foreach (ValueStore vstore in intras)
                {
                    int dataaddr = infos[i].DataAddr;
                    switch (vstore.Intra)
                    {
                        case ValueModel.Bases.V: dataaddr += (int)(storev[vstore.IntraOffset].Value) * vstore.ByteCount; break;
                        case ValueModel.Bases.Z: dataaddr += (int)(storez[vstore.IntraOffset].Value) * vstore.ByteCount; break;
                    }
                    for (int j = Math.Max(0, dataaddr); j < Math.Min(dataused.Length, dataaddr + vstore.ByteCount); j++)
                        dataused[j] = true;
                }
            }
            List<AddrSegment> segs = new List<AddrSegment>();
            AddrSegment seg = null;
            int start = -1;
            for (int i = 0; i < infos.Count(); i++)
            {
                bool issplit = false;
                if (dataused[infos[i].DataAddr] && start == -1) start = i;
                if (start != -1)
                {
                    issplit |= infos[i].Prototype.Base != infos[start].Prototype.Base;
                    issplit |= infos[i].Prototype.Base == ValueModel.Bases.CV && infos[start].Prototype.Base == ValueModel.Bases.CV
                        && infos[i].Prototype.Offset >= 200 && infos[start].Prototype.Offset < 200;
                    int maxrange = 32;
                    if (infos[i].Prototype.Base == ValueModel.Bases.CV && infos[i].Prototype.Offset >= 200)
                        maxrange = 16;
                    issplit |= i - start + 1 > maxrange;
                }
                if (issplit)
                {
                    seg = CommandHelper.GetAddrSegment(
                        infos[start].Prototype.Base,
                        infos[start].Prototype.Offset,
                        i - start);
                    seg.DataAddr = infos[start].DataAddr;
                    segs.Add(seg);
                    start = dataused[infos[i].DataAddr] ? i : -1;
                }
            }
            if (start != -1)
            {
                seg = CommandHelper.GetAddrSegment(
                    infos[start].Prototype.Base,
                    infos[start].Prototype.Offset,
                    infos.Count() - start);
                seg.DataAddr = infos[start].DataAddr;
                segs.Add(seg);
            }
            List<GeneralReadCommand> result = new List<GeneralReadCommand>();
            GeneralReadCommand cmd = null;
            int maxseg = CommunicationDataDefine.MAX_ADDRESS_TYPE;
            int seglen = segs.Count() / maxseg;
            int segrem = segs.Count() % maxseg;
            for (int i = 0; i < seglen; i++)
            {
                cmd = new GeneralReadCommand();
                for (int j = 0; j < maxseg; j++)
                    cmd.Segments.Add(segs[i * maxseg + j]);
                cmd.InitializeCommandByElement();
                result.Add(cmd);
            }
            if (segrem > 0)
            {
                cmd = new GeneralReadCommand();
                for (int j = 0; j < segrem; j++)
                    cmd.Segments.Add(segs[seglen * maxseg + j]);
                cmd.InitializeCommandByElement();
                result.Add(cmd);
            }
            return result;
        }

        public void AnalyzeReadCommand(GeneralReadCommand cmd)
        {
            List<byte[]> retdatas = cmd.GetRetData();
            for (int i = 0; i < cmd.Segments.Count(); i++)
            {
                AddrSegment seg = cmd.Segments[i];
                byte[] retdata = retdatas[i];
                int typeLen = CommandHelper.GetLengthByAddrType(seg.Type);
                if (typeLen == 1)
                {
                    for (int j = 0; j < seg.Length; j++)
                    {
                        int div = j / 8;
                        int mod = j % 8;
                        datas[seg.DataAddr + j] = (byte)((retdata[div] >> mod) & 1);
                    }
                }
                else
                {
                    for (int j = 0; j < Math.Min(datas.Length - seg.DataAddr, retdata.Length); j++)
                        datas[seg.DataAddr + j] = retdata[j];
                }
            }
        }

        public void ReadMonitorData()
        {
            byte[] udata = new byte[4];
            int dataaddr;
            uint value;
            for (int i = 0; i < infos.Count(); i++)
            {
                switch (infos[i].Prototype.Base)
                {
                    case ValueModel.Bases.V:
                    case ValueModel.Bases.Z:
                        dataaddr = infos[i].DataAddr;
                        for (int j = dataaddr; j < Math.Min(datas.Length, dataaddr + 4); j++)
                            udata[j - dataaddr] = datas[j];
                        value = ValueConverter.GetValue(udata);
                        if (infos[i].Prototype.Base == ValueModel.Bases.V)
                            storev[infos[i].Prototype.Offset].Value = (int)(short)value;
                        if (infos[i].Prototype.Base == ValueModel.Bases.Z)
                            storez[infos[i].Prototype.Offset].Value = (int)(short)value;
                        break;
                }
                if (infos[i].Stores.Count() == 0) continue;
                foreach (ValueStore vstore in infos[i].Stores)
                {
                    if (vstore.VisualRefNum == 0) continue;
                    dataaddr = infos[i].DataAddr;
                    switch (vstore.Intra)
                    {
                        case ValueModel.Bases.V: dataaddr += (int)(storev[vstore.IntraOffset].Value) * vstore.ByteCount; break;
                        case ValueModel.Bases.Z: dataaddr += (int)(storez[vstore.IntraOffset].Value) * vstore.ByteCount; break;
                    }
                    for (int j = dataaddr; j < Math.Min(datas.Length, dataaddr + 4); j++)
                        udata[j - dataaddr] = datas[j];
                    value = ValueConverter.GetValue(udata);
                    if (vstore.IsWordBit)
                    {
                        vstore.Value = (int)((value >> vstore.Flag) & 1);
                    }
                    else if (vstore.IsBitWord || vstore.IsBitDoubleWord)
                    {
                        value = 0;
                        for (int _addr = dataaddr + vstore.Flag - 1; _addr >= dataaddr; _addr--)
                            value = (value << 1) + (uint)(datas[_addr]);
                        if (vstore.IsBitWord)
                            vstore.Value = (short)value;
                        else
                            vstore.Value = (int)value;
                    }
                    else
                    {
                        switch (vstore.Type)
                        {
                            case ValueModel.Types.BOOL: vstore.Value = (int)(udata[0] & 1); break;
                            case ValueModel.Types.WORD: vstore.Value = (short)value; break;
                            case ValueModel.Types.HEX: 
                            case ValueModel.Types.UWORD: 
                            case ValueModel.Types.BCD: vstore.Value = (ushort)value; break;
                            case ValueModel.Types.DWORD: vstore.Value = (int)value; break;
                            case ValueModel.Types.DHEX:
                            case ValueModel.Types.UDWORD: vstore.Value = (uint)value; break;
                            case ValueModel.Types.FLOAT: vstore.Value = ValueConverter.UIntToFloat(value); break;
                        }
                    }
                }
            }
        }
        
        public uint ReadMonitorData(ValueStore vstore)
        {
            int dataaddr = vstore.Parent.DataAddr;
            switch (vstore.Intra)
            {
                case ValueModel.Bases.V: dataaddr += (int)(storev[vstore.IntraOffset].Value) * vstore.ByteCount; break;
                case ValueModel.Bases.Z: dataaddr += (int)(storez[vstore.IntraOffset].Value) * vstore.ByteCount; break;
            }
            byte[] udata = new byte[4];
            for (int i = dataaddr; i < Math.Min(datas.Length, dataaddr + 4); i++)
                udata[i - dataaddr] = datas[i];
            return ValueConverter.GetValue(udata);
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

        public event ValueStoreWriteEventHandler PostValueStoreEvent = delegate { };

        private void OnReceiveValueStoreEvent(object sender, ValueStoreWriteEventArgs e)
        {
            PostValueStoreEvent(sender, e);
        }
        
        #endregion
    }

}
