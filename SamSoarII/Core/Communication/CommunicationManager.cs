using SamSoarII.Core.Models;
using SamSoarII.Threads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Windows;
using System.Threading;
using SamSoarII.Utility;
using System.IO;
using System.ComponentModel;

namespace SamSoarII.Core.Communication
{
    public enum PortTypes { SerialPort, USB, NULL}

    public class CommunicationManager : BaseThreadManager
    {
        public CommunicationManager(InteractionFacade _ifParent) : base(false)
        {
            ifParent = _ifParent;
            mngPort = new SerialPortManager(this);
            mngUSB = new USBManager(this);
            mngCurrent = null;
            //elements = new ObservableCollection<MonitorElement>();
            //elements.CollectionChanged += OnElementCollectionChanged;
            writecmds = new Queue<ICommunicationCommand>();
            Paused += OnThreadPaused;
            //PARACom.PropertyChanged += OnCommunicationParamsChanged;
        }
        
        #region Number

        private InteractionFacade ifParent;
        public InteractionFacade IFParent { get { return this.ifParent; } }
        public ValueManager ValueManager { get { return ifParent.MNGValue; } }
        public CommunicationParams PARACom { get { return ifParent.MDProj?.PARAProj?.PARACom; } }
        public MonitorModel MDMoni { get { return ifParent.MDProj?.Monitor; } }

        #region Port & USB

        private SerialPortManager mngPort;
        private USBManager mngUSB;
        private IPortManager mngCurrent;
        public PortTypes PortType
        {
            get
            {
                if (mngCurrent == mngPort) return PortTypes.SerialPort;
                if (mngCurrent == mngUSB) return PortTypes.USB;
                return PortTypes.NULL;
            }
            set
            {
                switch (value)
                {
                    case PortTypes.SerialPort: mngCurrent = mngPort; break;
                    case PortTypes.USB: mngCurrent = mngUSB; break;
                    default: mngCurrent = null; break;
                }
            }
        }

        #endregion

        #region Elements

        private bool isenable;
        public bool IsEnable
        {
            get
            {
                return this.isenable;
            }
            set
            {
                if (isenable == value) return;
                this.isenable = value;
                if (isenable)
                {
                    ValueManager.PostValueStoreEvent += OnReceiveValueStoreEvent;
                    PARACom.PropertyChanged += OnCommunicationParamsChanged;
                    OnCommunicationParamsChanged(this, null);
                    visualtable = new MonitorTable(MDMoni, "Visual");
                    readcmds = ValueManager.GetReadCommands().ToArray();
                }
                else
                {
                    ValueManager.PostValueStoreEvent -= OnReceiveValueStoreEvent;
                    PARACom.PropertyChanged -= OnCommunicationParamsChanged;
                    visualtable.Dispose();
                    readcmds = null;
                }
            }
        }
        
        private MonitorTable visualtable;
        /*
        private ObservableCollection<MonitorElement> elements;
        public IList<MonitorElement> Elements { get { return this.elements; } }
        public event NotifyCollectionChangedEventHandler ElementChanged = delegate { };
        private void OnElementCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (MonitorElement element in e.NewItems)
                    element.Store.Post += OnReceiveValueStoreEvent;
            if (e.OldItems != null)
                foreach (MonitorElement element in e.OldItems)
                    element.Store.Post -= OnReceiveValueStoreEvent;
        }
        */
        #endregion

        #region Commands

        private IList<GeneralReadCommand> readcmds;
        private Queue<ICommunicationCommand> writecmds;

        #endregion

        #endregion

        #region Check

        public bool CheckLink()
        {
            switch (PortType)
            {
                case PortTypes.SerialPort:
                    if (PARACom.IsAutoCheck && !mngPort.AutoCheck())
                        return false;
                    if (mngPort.Start() != 0)
                        return false;
                    return true;
                case PortTypes.USB:
                    if (mngUSB.Start() != 0)
                        return false;
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Thread

        private ICommunicationCommand current = null;
        private int readindex = 0;

        protected override void Handle()
        {
            if (current == null)
            {
                if (writecmds.Count() > 0)
                {
                    current = writecmds.Dequeue();
                }
                else
                {
                    if (readindex >= readcmds.Count())
                    {
                        ValueManager.ReadMonitorData();
                        readcmds = ValueManager.GetReadCommands().ToArray();
                        readindex = 0;   
                    }
                    current = readcmds.Count() == 0 ? null : readcmds[readindex++];
                }
            }
            bool hassend = false;
            bool hasrecv = false;
            int sendtime = 0;
            int recvtime = 0;
            if (current != null)
            {
                while (ThAlive && ThActive
                    && sendtime < 5)
                {
                    if (Send(current))
                    {
                        hassend = true;
                        break;
                    }
                    sendtime++;
                }
                int itvtime = 20;
                if (current is GeneralWriteCommand
                 || current is IntrasegmentWriteCommand)
                {
                    itvtime = 100;
                }
                //_Thread_WaitForActive(itvtime);
                Thread.Sleep(itvtime);
                while (hassend)
                {
                    if (Recv(current) || current.IsComplete)
                    {
                        hasrecv = true;
                        break;
                    }
                    recvtime++;
                }
            }
            if (ThAlive && hassend && hasrecv)
            {
                Execute(current);
                current = null;
            }
        }
        
        private bool Send(ICommunicationCommand cmd)
        {
            return (mngCurrent.Write(cmd) == 0);
        }

        private bool Recv(ICommunicationCommand cmd)
        {
            return (mngCurrent.Read(cmd) == 0);
        }

        private void Execute(ICommunicationCommand cmd)
        {
            if (cmd.IsSuccess)
            {
                //cmd.UpdataValues();
                if (cmd is GeneralReadCommand)
                    ValueManager.AnalyzeReadCommand((GeneralReadCommand)cmd);
            }
        }

        private void OnThreadPaused(object sender, RoutedEventArgs e)
        {
            Start();
        }

        public void AbortAll()
        {
            Abort();
            while (IsAlive) Thread.Sleep(10);
            mngCurrent.Abort();
        }
        
        #endregion

        #region Download

        private List<byte> execdata;
        public int ExecLen { get { return execdata.Count(); } }

        public void LoadExecute()
        {
            execdata = new List<byte>();
            string currentpath = FileHelper.AppRootPath;
            string execfile = String.Format(@"{0:s}\downc.bin", currentpath);
            BinaryReader br = new BinaryReader(
                new FileStream(execfile, FileMode.Open));
            while (br.BaseStream.CanRead)
            {
                try
                {
                    execdata.Add(br.ReadByte());
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }
            br.Close();
        }

        public bool DownloadExecute()
        {
            DownloadFBCommand dFBCmd = new DownloadFBCommand();
            DownloadFCCommand dFCCmd = new DownloadFCCommand();
            Download80Command d80Cmd = null;
            Download81Command d81Cmd = new Download81Command();
            if (!DownloadHandle(dFBCmd)) return false;
            int time = 0;
            while (time < 20 && !DownloadHandle(dFCCmd)) time++;
            if (time >= 20) return false;
            byte[] data = execdata.ToArray();
            byte[] pack = new byte[1024];
            int len = data.Length / 1024;
            int rem = data.Length % 1024;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < 1024; j++)
                    pack[j] = data[i * 1024 + j];
                d80Cmd = new Download80Command(i, pack);
                for (time = 0; time < 3 && !DownloadHandle(d80Cmd);) time++;
                if (time >= 3) return false;
            }
            if (rem > 0)
            {
                pack = new byte[rem];
                for (int j = 0; j < rem; j++)
                    pack[j] = data[len * 1024 + j];
                d80Cmd = new Download80Command(len, pack);
                for (time = 0; time < 3 && !DownloadHandle(d80Cmd);) time++;
                if (time >= 3) return false;
            }
            if (!DownloadHandle(d81Cmd)) return false;
            return true;
        }

        public bool DownloadHandle(ICommunicationCommand cmd, int waittime = 10)
        {
            bool hassend = false;
            bool hasrecv = false;
            int sendtime = 0;
            int recvtime = 0;
            while (sendtime < 5)
            {
                if (mngCurrent.Write(cmd) == 0)
                {
                    hassend = true;
                    break;
                }
                sendtime++;
            }
            if (!hassend) return false;
            Thread.Sleep(waittime);
            if (cmd.RecvDataLen == 0) return true;
            while (recvtime < 100)
            {
                if (mngCurrent.Read(cmd) == 0)
                {
                    hasrecv = true;
                    break;
                }
                recvtime++;
            }
            return hasrecv && cmd.IsComplete && cmd.IsSuccess;
        }


        #endregion

        #region Upload

        #endregion

        #region Arrange
        /*
        public void Arrange()
        {
            readcmds.Clear();
            Queue<string> tempQueue_Base = new Queue<string>();
            foreach (var ele in elements)
                if (!tempQueue_Base.Contains(ele.AddrType))
                    tempQueue_Base.Enqueue(ele.AddrType);
            string addrType;
            int gIndex = 0;
            bool gisFirst = true, iisFirst = true;
            int gstart = 0, istart = 0;
            MonitorElement gstartele = null, istartele = null;
            GeneralReadCommand gcmd = new GeneralReadCommand();
            gcmd.SegmentsGroup[gIndex] = new List<AddrSegment>();
            IntrasegmentReadCommand icmd = new IntrasegmentReadCommand();
            while (tempQueue_Base.Count > 0)
            {
                addrType = tempQueue_Base.Dequeue();
                List<MonitorElement> _elements = elements.Where(x => { return x.AddrType == addrType; }).OrderBy(x => { return x.StartAddr; }).ToList();
                if (_elements.Count > 0)
                {
                    for (int i = 0; i < _elements.Count; i++)
                    {
                        if ((iisFirst && _elements[i].IsIntrasegment) || (gisFirst && !_elements[i].IsIntrasegment))
                        {
                            if (_elements[i].IsIntrasegment)
                            {
                                if (!iisFirst)
                                {
                                    readcmds.Add(icmd);
                                    icmd = new IntrasegmentReadCommand();
                                }
                                else iisFirst = false;
                                icmd.Segments.Add(GenerateIntraSegmentByElement(_elements[i]));
                                istart = _elements[i].StartAddr;
                                istartele = _elements[i];
                            }
                            else
                            {
                                gstart = _elements[i].StartAddr;
                                gstartele = _elements[i];
                                if (gisFirst)
                                {
                                    gcmd.SegmentsGroup[gIndex].Add(GenerateAddrSegmentByElement(_elements[i]));
                                    gisFirst = false;
                                }
                                else ArrangeCmd(ref gcmd, ref gIndex, _elements[i]);
                            }
                        }
                        else if (!_elements[i].IsIntrasegment && GetAddrSpan(_elements[i], gstart) < GetMaxRange(gstartele))
                        {
                            if (_elements[i].AddrType == gstartele.AddrType)
                            {
                                gcmd.SegmentsGroup[gIndex].Add(GenerateAddrSegmentByElement(_elements[i]));
                            }
                            else
                            {
                                gstart = _elements[i].StartAddr;
                                gstartele = _elements[i];
                                ArrangeCmd(ref gcmd, ref gIndex, _elements[i]);
                            }
                        }
                        else if (_elements[i].IsIntrasegment && GetAddrSpan(_elements[i], istart) < GetMaxRange(istartele) && IsSameIntraBase(istartele, _elements[i]))
                        {
                            if (_elements[i].AddrType == istartele.AddrType)
                            {
                                icmd.Segments.Add(GenerateIntraSegmentByElement(_elements[i]));
                            }
                            else
                            {
                                readcmds.Add(icmd);
                                icmd = new IntrasegmentReadCommand();
                                icmd.Segments.Add(GenerateIntraSegmentByElement(_elements[i]));
                                istart = _elements[i].StartAddr;
                                istartele = _elements[i];
                            }
                        }
                        else
                        {
                            if (_elements[i].IsIntrasegment)
                            {
                                readcmds.Add(icmd);
                                icmd = new IntrasegmentReadCommand();
                                icmd.Segments.Add(GenerateIntraSegmentByElement(_elements[i]));
                                istart = _elements[i].StartAddr;
                                istartele = _elements[i];
                            }
                            else
                            {
                                gstart = _elements[i].StartAddr;
                                gstartele = _elements[i];
                                ArrangeCmd(ref gcmd, ref gIndex, _elements[i]);
                            }
                        }
                    }
                }
            }
            //添加剩余命令
            if (!gisFirst) readcmds.Add(gcmd);
            if (!iisFirst) readcmds.Add(icmd);
        }
        /// <summary>
        /// 对命令进行分组
        /// </summary>
        /// <param name="command">当前命令</param>
        /// <param name="index">当前命令片段的索引</param>
        /// <param name="element">需添加的元素</param>
        private void ArrangeCmd(ref GeneralReadCommand command, ref int index, MonitorElement element)
        {
            if (index < CommunicationDataDefine.MAX_ADDRESS_TYPE - 1) index++;
            else
            {
                index = 0;
                readcmds.Add(command);
                command = new GeneralReadCommand();
            }
            command.SegmentsGroup[index] = new List<AddrSegment>();
            command.SegmentsGroup[index].Add(GenerateAddrSegmentByElement(element));
        }
        private bool IsSameIntraBase(MonitorElement one, MonitorElement two)
        {
            return one.IntrasegmentAddr == two.IntrasegmentAddr && one.IntrasegmentType == two.IntrasegmentType;
        }
        public int GetMaxRange(MonitorElement ele)
        {
            if (ele == null)
            {
                return 32;
            }
            if (ele.AddrType == "CV" && ele.StartAddr >= 200)
            {
                return 16;
            }
            else
            {
                return 32;
            }
        }
        private int GetAddrSpan(MonitorElement element, int startAddr)
        {
            //不管是不是双字，最后一个寄存器都要读到它的下一位
            if (!(element.AddrType == "CV" && element.StartAddr >= 200))
            {
                return (int)(element.StartAddr - startAddr + 1);
            }
            else
            {
                return (int)(element.StartAddr - startAddr);
            }
        }
        private AddrSegment GenerateAddrSegmentByElement(MonitorElement element)
        {
            return new AddrSegment(element);
        }
        private IntraSegment GenerateIntraSegmentByElement(MonitorElement element)
        {
            AddrSegment bseg = new AddrSegment(element);
            AddrSegment iseg = new AddrSegment(element, true);
            return new IntraSegment(bseg, iseg);
        }
        */
        #endregion

        #region Event Handler
        
        private void OnCommunicationParamsChanged(object sender, PropertyChangedEventArgs e)
        {
            PortType = PARACom.IsComLinked ? PortTypes.SerialPort : PortTypes.USB;
            mngPort.InitializePort();
        }

        private void OnReceiveValueStoreEvent(object sender, ValueStoreWriteEventArgs e)
        {
            ValueStore vstore = (ValueStore)sender;
            //MonitorElement element = new MonitorElement(visualtable, vstore);
            if (e.IsWrite)
            {
                byte[] data = null;
                switch (vstore.Type)
                {
                    case ValueModel.Types.BOOL:
                        data = new byte[] { (byte)(e.ToValue.ToString().Equals("ON") ? 1 : 0) };
                        break;
                    case ValueModel.Types.WORD:
                        data = ValueConverter.GetBytes(
                            (UInt16)(Int16.Parse(e.ToValue.ToString())));
                        break;
                    case ValueModel.Types.UWORD:
                        data = ValueConverter.GetBytes(
                            UInt16.Parse(e.ToValue.ToString()));
                        break;
                    case ValueModel.Types.BCD:
                        data = ValueConverter.GetBytes(
                            ValueConverter.ToBCD(
                                UInt16.Parse(e.ToValue.ToString())));
                        break;
                    case ValueModel.Types.DWORD:
                        data = ValueConverter.GetBytes(
                            (UInt32)(Int32.Parse(e.ToValue.ToString())));
                        break;
                    case ValueModel.Types.UDWORD:
                        data = ValueConverter.GetBytes(
                            UInt32.Parse(e.ToValue.ToString()));
                        break;
                    case ValueModel.Types.FLOAT:
                        data = ValueConverter.GetBytes(
                            ValueConverter.FloatToUInt(
                                float.Parse(e.ToValue.ToString())));
                        break;
                    default:
                        data = new byte[0];
                        break;

                }
                if (vstore.Intra != ValueModel.Bases.NULL)
                {
                    IntrasegmentWriteCommand command = new IntrasegmentWriteCommand(data, vstore);
                    writecmds.Enqueue(command);
                }
                else
                {
                    GeneralWriteCommand command = new GeneralWriteCommand(data, vstore);
                    writecmds.Enqueue(command);
                }
            }
            if (e.Unlock)
            {
                ForceCancelCommand command = new ForceCancelCommand(false, vstore);
                writecmds.Enqueue(command);
            }
            if (e.UnlockAll)
            {
                ForceCancelCommand command = new ForceCancelCommand(true, vstore);
                writecmds.Enqueue(command);
            }
        }

        #endregion

        
    }
}
