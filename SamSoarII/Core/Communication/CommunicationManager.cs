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
using SamSoarII.Core.Helpers;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Shell.Models;

namespace SamSoarII.Core.Communication
{
    public enum PortTypes { SerialPort, USB, NULL}
    public enum CommunicationType {Download,Upload,Monitor }
    public class CommunicationManager : BaseThreadManager
    {
        public CommunicationManager(InteractionFacade _ifParent) : base(false)
        {
            ifParent = _ifParent;
            mngPort = new SerialPortManager(this);
            mngUSB = new USBManager(this);
            mngCurrent = mngUSB;
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
        private CommunicationParams _PARACom;
        public CommunicationParams PARACom
        {
            get
            {
                if (ifParent.MDProj != null)
                {
                    return ifParent.MDProj?.PARAProj?.PARACom;
                }
                if(_PARACom == null)
                {
                    _PARACom = new CommunicationParams(null);
                }
                return _PARACom;
            }
        }
        public MonitorModel MDMoni { get { return ifParent.MDProj?.Monitor; } }

        #region Port & USB
        //下载（或上载，监视）前用于获取当前PLC信息(PLC型号，PLC运行状态，PLC当前程序，是否需要下载密码等)
        public PLCMessage PLCMessage;

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
                    //visualtable = new MonitorTable(MDMoni, "Visual");
                    readcmds = ValueManager.GetReadCommands().ToArray();
                }
                else
                {
                    ValueManager.PostValueStoreEvent -= OnReceiveValueStoreEvent;
                    PARACom.PropertyChanged -= OnCommunicationParamsChanged;
                    //visualtable.Dispose();
                    writecmds.Clear();
                    readcmds = null;
                }
            }
        }
        
        //private MonitorTable visualtable;
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
        private int time = 0;
        private int readindex = 0;

        protected override void Handle()
        {
            if (current == null)
            {
                if (writecmds.Count() > 0)
                    current = writecmds.Dequeue();
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
            if (current != null)
            {
                if(time > 5)
                {
                    current = null;
                    AbortAll();
                    IsEnable = false;
                    time = 0;
                    if (!ThAlive)
                    {
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                        {
                            ifParent.VMDProj.LadderMode = LadderModes.Edit;
                            LocalizedMessageBox.Show(Properties.Resources.Communication_Error, LocalizedMessageIcon.Error);
                        });
                        return;
                    }
                }
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
                Thread.Sleep(itvtime);
                while (hassend)
                {
                    if (Recv(current))
                    {
                        hasrecv = true;
                        Thread.Sleep(2);
                        break;
                    }
                }
            }
            if (ThAlive && hassend && hasrecv)
            {
                if (!current.IsSuccess)
                {
                    time++;
                    if (current is GeneralWriteCommand || current is IntrasegmentWriteCommand || current is ForceCancelCommand)
                        writecmds.Enqueue(current);
                    else
                    {
                        if (readindex == 0)
                            readindex = Math.Max(0, readcmds.Count - 1);
                        else readindex--;
                    }
                    Thread.Sleep(200);
                }else
                {
                    time = 0;
                    Execute(current);
                }
                current = null;
            }else time++;
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
            if (IsAlive)
                Aborted += OnAbortedToAbortAll;
            else
                mngCurrent.Abort();
        }

        private void OnAbortedToAbortAll(object sender, RoutedEventArgs e)
        {
            Aborted -= OnAbortedToAbortAll;
            Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                mngCurrent.Abort();
            });
        }

        #endregion

        #region Download & Upload
        //通信时，传送一包数据的最大长度
        public int DOWN_MAX_DATALEN
        {
            get
            {
                switch (PortType)
                {
                    case PortTypes.SerialPort:
                        return CommunicationDataDefine.SERIAL_COMMU_LEN;
                    case PortTypes.USB:
                        return CommunicationDataDefine.USB_DOWN_LEN;
                    default:
                        return CommunicationDataDefine.USB_DOWN_LEN;
                }
            }
        }

        public int UP_MAX_DATALEN
        {
            get
            {
                switch (PortType)
                {
                    case PortTypes.SerialPort:
                        return CommunicationDataDefine.SERIAL_COMMU_LEN;
                    case PortTypes.USB:
                        return CommunicationDataDefine.USB_UP_LEN;
                    default:
                        return CommunicationDataDefine.USB_UP_LEN;
                }
            }
        }

        private List<byte> execdata;

        public List<byte> ExecData { get { return execdata; } }

        public int ExecLen { get { return execdata.Count(); } }

        public void LoadExecute()
        {
            string currentpath = FileHelper.AppRootPath;
            string execfile = string.Format(@"{0:s}\downc.bin", currentpath);
            execdata = FileHelper.GetBytesByBinaryFile(execfile).ToList();
        }

        public DownloadError DownloadExecute(LoadingWindowHandle handle)
        {
            return DownloadHelper.DownloadExecute(this, handle);
        }

        public UploadError UploadExecute(LoadingWindowHandle handle)
        {
            return UploadHelper.UploadExecute(this, handle);
        }

        public bool PasswordHandle(CommunicationType commuType)
        {
            //首先通信测试获取底层PLC的状态
            CommunicationTestCommand CTCommand = new CommunicationTestCommand();
            if (!CommunicationHandle(CTCommand))
                return false;
            else PLCMessage = new PLCMessage(CTCommand);



            if(commuType == CommunicationType.Monitor)
            {
                //设置从站站号
                CommunicationDataDefine.SLAVE_ADDRESS = PLCMessage.StationNumber;
            }

            bool passwordNeed = false;
            byte passwordType = 0;
            string message = string.Empty;
            switch (commuType)
            {
                case CommunicationType.Download:
                    passwordNeed = PLCMessage.IsDPNeed;
                    break;
                case CommunicationType.Upload:
                    passwordNeed = PLCMessage.IsUPNeed;
                    break;
                case CommunicationType.Monitor:
                    passwordNeed = PLCMessage.IsMPNeed;
                    break;
            }

            //验证是否需要上载(或下载，监视)密码
            if (passwordNeed)
            {
                bool retp = false;
                LocalizedMessageResult retcl = LocalizedMessageResult.None;
                PasswordDialog pwdialog = new PasswordDialog();

                pwdialog.EnsureButtonClick += (sender, e) =>
                {
                    if (pwdialog.Password.Length > 12 || pwdialog.Password.Length < 5)
                        LocalizedMessageBox.Show(Properties.Resources.Password_Length_Error);
                    else
                    {
                        switch (commuType)
                        {
                            case CommunicationType.Download:
                                passwordType = CommunicationDataDefine.CMD_PASSWD_DOWNLOAD;
                                break;
                            case CommunicationType.Upload:
                                passwordType = CommunicationDataDefine.CMD_PASSWD_UPLOAD;
                                break;
                            case CommunicationType.Monitor:
                                passwordType = CommunicationDataDefine.CMD_PASSWD_MONITOR;
                                break;
                        }
                        int time = 0;
                        ICommunicationCommand command = new PasswordCheckCommand(passwordType, pwdialog.Password);
                        for (time = 0; time < 3 && !CommunicationHandle(command);)
                        {
                            Thread.Sleep(200);
                            time++;
                        }
                        if (time >= 3)
                        {
                            retp = false;
                            LocalizedMessageBox.Show(Properties.Resources.Password_Error, LocalizedMessageIcon.Error);
                        }
                        else
                        {
                            retp = true;
                            pwdialog.Close();
                        }
                    }
                };

                pwdialog.Closing += (sender, e) =>
                {
                    if (!retp)
                    {
                        switch (commuType)
                        {
                            case CommunicationType.Download:
                                message = Properties.Resources.MainWindow_Download;
                                break;
                            case CommunicationType.Upload:
                                message = Properties.Resources.MainWindow_Upload;
                                break;
                            case CommunicationType.Monitor:
                                message = Properties.Resources.MainWindow_Monitor;
                                break;
                        }
                        retcl = LocalizedMessageBox.Show(string.Format("{0}{1}", Properties.Resources.Dialog_Closing, message), LocalizedMessageButton.OKCancel,LocalizedMessageIcon.Question);
                        if (retcl == LocalizedMessageResult.No) e.Cancel = true;
                    }
                    else retcl = LocalizedMessageResult.None;
                };

                pwdialog.ShowDialog();
                if (retcl == LocalizedMessageResult.Yes) return false;
            }
            return true;
        }

        public bool CommunicationHandle(ICommunicationCommand cmd, bool hasRecvData = true, int waittime = 10)
        {
            bool hassend = false;
            bool hasrecv = false;
            int sendtime = 0;
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
            if (!hasRecvData && cmd.RecvDataLen == 0) return true;
            while (hassend)
            {
                if (mngCurrent.Read(cmd) == 0)
                {
                    hasrecv = true;
                    break;
                }
            }
            return hasrecv && cmd.IsComplete && cmd.IsSuccess;
        }


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
            //mngPort.InitializePort();
        }

        private void OnReceiveValueStoreEvent(object sender, ValueStoreWriteEventArgs e)
        {
            ValueStore vstore = (ValueStore)sender;
            
            if (e.IsWrite)
            {
                byte[] data = null;
                int value = 0;
                ValueModel.Types type = e.Type != ValueModel.Types.NULL ? e.Type : vstore.Type;
                string vstr = e.ToValue.ToString();
                if (vstore.IsWordBit)
                {
                    value = (int)(ValueManager.ReadMonitorData(vstore));
                    if (vstr.Equals("ON"))
                        value |=  (1 << vstore.Flag);
                    else
                        value &= ~(1 << vstore.Flag);
                    data = ValueConverter.GetBytes((UInt16)value);
                }
                else if (vstore.IsBitWord || vstore.IsBitDoubleWord)
                {
                    data = new byte[vstore.Flag];
                    switch (vstore.Type)
                    {
                        case ValueModel.Types.WORD:
                            value = (int)(Int16.Parse(vstr));
                            break;
                        case ValueModel.Types.UWORD:
                            value = (int)(UInt16.Parse(vstr));
                            break;
                        case ValueModel.Types.BCD:
                            value = (int)ValueConverter.ToBCD(
                                UInt16.Parse(vstr));
                            break;
                        case ValueModel.Types.DWORD:
                            value = Int32.Parse(vstr);
                            break;
                        case ValueModel.Types.UDWORD:
                            value = (int)(UInt32.Parse(vstr));
                            break;
                        case ValueModel.Types.HEX:
                            if (vstr.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
                                value = (int)(UInt16.Parse(vstr.Substring(2), System.Globalization.NumberStyles.HexNumber));
                            else
                                value = (int)(UInt16.Parse(vstr, System.Globalization.NumberStyles.HexNumber));
                            break;
                        case ValueModel.Types.DHEX:
                            if (vstr.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
                                value = (int)(UInt32.Parse(vstr.Substring(2), System.Globalization.NumberStyles.HexNumber));
                            else
                                value = (int)(UInt32.Parse(vstr, System.Globalization.NumberStyles.HexNumber));
                            break;
                    }
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = (byte)(value & 1);
                        value >>= 1;
                    }
                }
                else
                {
                    switch (vstore.Type)
                    {
                        case ValueModel.Types.BOOL:
                            data = new byte[] { (byte)(vstr.Equals("ON") ? 1 : 0) };
                            break;
                        case ValueModel.Types.WORD:
                            data = ValueConverter.GetBytes(
                                (UInt16)(Int16.Parse(vstr)));
                            break;
                        case ValueModel.Types.UWORD:
                            data = ValueConverter.GetBytes(
                                UInt16.Parse(vstr));
                            break;
                        case ValueModel.Types.BCD:
                            data = ValueConverter.GetBytes(
                                ValueConverter.ToBCD(
                                    UInt16.Parse(vstr)));
                            break;
                        case ValueModel.Types.DWORD:
                            data = ValueConverter.GetBytes(
                                (UInt32)(Int32.Parse(vstr)));
                            break;
                        case ValueModel.Types.UDWORD:
                            data = ValueConverter.GetBytes(
                                UInt32.Parse(vstr));
                            break;
                        case ValueModel.Types.FLOAT:
                            data = ValueConverter.GetBytes(
                                ValueConverter.FloatToUInt(
                                    float.Parse(vstr)));
                            break;
                        case ValueModel.Types.HEX:
                            if (vstr.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
                                data = ValueConverter.GetBytes(UInt16.Parse(vstr.Substring(2), System.Globalization.NumberStyles.HexNumber));
                            else
                                data = ValueConverter.GetBytes(UInt16.Parse(vstr, System.Globalization.NumberStyles.HexNumber));
                            break;
                        case ValueModel.Types.DHEX:
                            if (vstr.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
                                data = ValueConverter.GetBytes(UInt32.Parse(vstr.Substring(2), System.Globalization.NumberStyles.HexNumber));
                            else
                                data = ValueConverter.GetBytes(UInt32.Parse(vstr, System.Globalization.NumberStyles.HexNumber));
                            break;
                        default:
                            data = new byte[0];
                            break;
                    }
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
