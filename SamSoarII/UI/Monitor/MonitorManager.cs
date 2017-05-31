using SamSoarII.AppMain.Project;
using SamSoarII.Communication;
using SamSoarII.Communication.Command;
using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.LadderInstViewModel.Monitor;
using SamSoarII.UserInterface;
using SamSoarII.Utility;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.AppMain.UI.Monitor
{
    public interface IMonitorManager : IMoniViewCtrl
    {
        void Start();
        void Abort();
        void Initialize();
        void Initialize(ProjectModel pmodel);
        ElementModel Get(ElementModel emodel);
        void Add(ElementModel emodel);
        void Remove(ElementModel emodel);
        void Write(ElementModel emodel);
        bool CanLock { get; }
        void Lock(ElementModel emodel);
        void Unlock(ElementModel emodel);
        void Add(IEnumerable<ElementModel> emodel);
        void Remove(IEnumerable<ElementModel> emodel);
        void Add(ICommunicationCommand cmd);
        void Remove(ICommunicationCommand cmd);
        void Handle(IMoniValueModel mvmodel, ElementValueModifyEventArgs e);
    }

    public enum MonitorManager_ElementModelHandle
    {
        NULL = 0x00, ADD, REMOVE, WRITE, MULTIADD, MULTIREMOVE, MULTIWRITE
    }
    
    public enum MonitorManager_CommunicationCommandHandle
    {
        NULL = 0x00, ADD, REMOVE
    }

    public class MonitorManager : IMonitorManager, IDisposable
    {
        public ICommunicationManager CManager { get; set; }

        public MainMonitor MMWindow { get; set; }

        #region View Control

        public bool IsRunning { get; private set; }

        public event RoutedEventHandler Started = delegate { };

        public event RoutedEventHandler Aborted = delegate { };

        #endregion

        #region Thread

        private Thread ComThread { get; set; }
        private bool _Thread_Alive = false;
        private bool _Thread_IsAlive = false;
        private bool _Thread_Active = false;
        private bool _Thread_IsActive = false;
        public event RoutedEventHandler ThreadResume = delegate { };
        public event RoutedEventHandler ThreadPause = delegate { };
        private void _Thread_WaitForActive()
        {
            _Thread_IsActive = false;
            ThreadPause(this, new RoutedEventArgs());
            while (_Thread_Alive && !_Thread_Active)
            {
                Thread.Sleep(10);
            }
            _Thread_IsActive = true;
            ThreadResume(this, new RoutedEventArgs());
        }
        private void _Thread_Run()
        {
            _Thread_IsAlive = true;
            while (_Thread_Alive)
            {
                while (_Thread_Alive && _Thread_Active)
                {
                    if (CurrentCommand == null)
                    {
                        if (WriteCommands.Count() > 0)
                        {
                            CurrentCommand = WriteCommands.Dequeue();
                        }
                        else
                        {
                            if (ReadCommands.Count() == 0)
                            {
                                CurrentCommand = null;
                            }
                            else
                            {
                                if (ReadCommandIndex >= ReadCommands.Count())
                                {
                                    ReadCommandIndex = 0;
                                }
                                CurrentCommand = ReadCommands[ReadCommandIndex++];
                            }
                        }
                    }
                    bool hassend = false;
                    bool hasrecv = false;
                    int sendtime = 0;
                    int recvtime = 0;
                    if (CurrentCommand != null)
                    {
                        while (_Thread_Alive && _Thread_Active && sendtime < 10)
                        {
                            if (Send(CurrentCommand))
                            {
                                hassend = true;
                                break;
                            }
                            sendtime++;
                        }
                        _Thread_WaitForActive();
                        while (_Thread_Alive && _Thread_Active && recvtime < 10)
                        {
                            if (Recv(CurrentCommand))
                            {
                                hasrecv = true;
                                break;
                            }
                            recvtime++;
                        }
                    }
                    if (hassend && hasrecv)
                    {
                        Execute(CurrentCommand);
                        CurrentCommand = null;
                    }
                    Thread.Sleep(10);
                }
                _Thread_WaitForActive();
            }
            _Thread_IsAlive = false;
        }
        
        public void Start()
        {
            if (!_Thread_IsAlive
             && (ComThread == null || !_Thread_IsAlive))
            {
                if (ComThread != null && ComThread.IsAlive)
                {
                    ComThread.Abort();
                }
                ComThread = new Thread(_Thread_Run);
                _Thread_Alive = true;
                _Thread_Active = true;
                ComThread.Start();
                IsRunning = true;
                Started(this, new RoutedEventArgs());
            }
        }

        public void Abort()
        {
            _Thread_Alive = false;
            IsRunning = false;
            Aborted(this, new RoutedEventArgs());
        }

        #endregion
        
        public MonitorManager(ProjectModel projectModel)
        {
            MMWindow = new MainMonitor(projectModel);
            ComThread = new Thread(_Thread_Run);
            ThreadPause += OnThreadPause;
        }

        public void Dispose()
        {
            ThreadPause -= OnThreadPause;
            Abort();
        }
        
        #region Initialize
        
        public void Initialize()
        {
            MMWindow.Manager = this;
            ReadModels.Clear();
            WriteModels.Clear();
            ReadCommands.Clear();
            WriteCommands.Clear();
            _Add(LadderModels);
            if (MMWindow != null)
            {
                foreach (MonitorVariableTable mvtable in MMWindow.tables)
                {
                    Initialize(mvtable);
                }
            }
            Arrange();
        }

        public void Initialize(ProjectModel pmodel)
        {
            Initialize(pmodel.MainRoutine);
            foreach (LadderDiagramViewModel ldvmodel in pmodel.SubRoutines)
            {
                Initialize(ldvmodel);
            }
            LadderModels = ReadModels.Values.ToArray();
            Initialize();
        }

        private void Initialize(LadderDiagramViewModel ldvmodel)
        {
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                Initialize(lnvmodel);
            }
        }
        
        private void Initialize(LadderNetworkViewModel lnvmodel)
        {
            foreach (BaseViewModel bvmodel in lnvmodel.GetElements())
            {
                bvmodel.ViewCtrl = this;
                BaseModel bmodel = bvmodel.Model;
                if (bmodel == null) continue;
                for (int i = 0; i < bmodel.ParaCount; i++)
                {
                    IValueModel ivmodel = bmodel.GetPara(i);
                    if (AssertValueModel(ivmodel))
                    {
                        ElementModel elementmodel = new ElementModel();
                        elementmodel.AddrType = ivmodel.Base;
                        elementmodel.StartAddr = ivmodel.Index;
                        elementmodel.DataType = GetDataType(ivmodel.Type);
                        if (ivmodel.Offset != WordValue.Null)
                        {
                            elementmodel.IsIntrasegment = true;
                            elementmodel.IntrasegmentType = ivmodel.Offset.Base;
                            elementmodel.IntrasegmentAddr = ivmodel.Offset.Index;
                        }
                        else
                        {
                            elementmodel.IsIntrasegment = false;
                        }
                        elementmodel.SetShowTypes();
                        _Add(elementmodel, false);
                        elementmodel = Get(elementmodel);
                        bvmodel.SetValueModel(i, elementmodel);
                    }
                }
            }
        }

        private void Initialize(MonitorVariableTable mvtable)
        {
            ObservableCollection<ElementModel> _elements =
                new ObservableCollection<ElementModel>();
            foreach (ElementModel emodel in mvtable.Elements)
            {
                _Add(emodel, false);
                _elements.Add(Get(emodel));
            }
            mvtable.Elements = _elements;
        }

        private bool AssertValueModel(IValueModel model)
        {
            if (model is NullBitValue || model is NullWordValue || model is NullFloatValue
             || model is NullDoubleWordValue || model is HDoubleWordValue || model is KDoubleWordValue
             || model is KFloatValue || model is HWordValue || model is KWordValue
             || model is StringValue || model is ArgumentValue)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private int GetDataType(LadderValueType type)
        {
            switch (type)
            {
                case LadderValueType.Bool:
                    return 0;
                case LadderValueType.DoubleWord:
                    return 3;
                case LadderValueType.Word:
                    return 1;
                case LadderValueType.Float:
                    return 6;
                default:
                    return -1;
            }
        }

        #endregion
        
        #region Value Modify Handle

        public void Handle(IMoniValueModel mvmodel, ElementValueModifyEventArgs e)
        {
            ElementModel element = (ElementModel)mvmodel;
            element.ShowType = e.VarType;
            byte bvalue = 0;
            ICommunicationCommand command = null;
            switch (e.Type)
            {
                case ElementValueModifyEventType.ForceON:
                    bvalue = 0x01;
                    element.SetValue = "ON";
                    Force(element, bvalue);
                    break;
                case ElementValueModifyEventType.ForceOFF:
                    bvalue = 0x01;
                    element.SetValue = "OFF";
                    Force(element, bvalue);
                    break;
                case ElementValueModifyEventType.ForceCancel:
                    element.SetValue = String.Empty;
                    command = new ForceCancelCommand(false, element);
                    Add(command);
                    break;
                case ElementValueModifyEventType.AllCancel:
                    element.SetValue = String.Empty;
                    command = new ForceCancelCommand(true, element);
                    Add(command);
                    break;
                case ElementValueModifyEventType.WriteOFF:
                    bvalue = 0x00;
                    element.SetValue = "OFF";
                    Write(element, bvalue);
                    break;
                case ElementValueModifyEventType.WriteON:
                    bvalue = 0x00;
                    element.SetValue = "ON";
                    Write(element, bvalue);
                    break;
                case ElementValueModifyEventType.Write:
                    element.SetValue = e.Value;
                    Write(element);
                    break;
            }
        }
        private void Force(ElementModel element, byte value)
        {
            GeneralWriteCommand command = new GeneralWriteCommand(new byte[] { value }, element);
            command.RefElements_A.Add(element);
            Add(command);
        }
        private void Write(ElementModel element, byte value)
        {
            if (element.IsIntrasegment)
            {
                IntrasegmentWriteCommand command = new IntrasegmentWriteCommand(new byte[] { value }, element);
                command.RefElement = element;
                Add(command);
            }
            else
            {
                GeneralWriteCommand command = new GeneralWriteCommand(new byte[] { value }, element);
                command.RefElements_A.Add(element);
                Add(command);
            }
        }

        private void Write(ElementModel element, string value)
        {
            byte[] data;
            element.SetValue = value;
            switch (element.ShowType)
            {
                case "WORD":
                    data = ValueConverter.GetBytes(
                        (UInt16)(Int16.Parse(value)));
                    break;
                case "UWORD":
                    data = ValueConverter.GetBytes(
                        UInt16.Parse(value));
                    break;
                case "BCD":
                    data = ValueConverter.GetBytes(
                        ValueConverter.ToUINT16(
                            UInt16.Parse(value)));
                    break;
                case "DWORD":
                    data = ValueConverter.GetBytes(
                        (UInt32)(Int32.Parse(value)));
                    break;
                case "UDWORD":
                    data = ValueConverter.GetBytes(
                        UInt32.Parse(value));
                    break;
                case "FLOAT":
                    data = ValueConverter.GetBytes(
                        ValueConverter.FloatToUInt(
                            float.Parse(value)));
                    break;
                default:
                    data = new byte[0];
                    break;
            }
            if (element.IsIntrasegment)
            {
                IntrasegmentWriteCommand command = new IntrasegmentWriteCommand(data, element);
                command.RefElement = element;
                Add(command);
            }
            else
            {
                GeneralWriteCommand command = new GeneralWriteCommand(data, element);
                command.RefElements_A.Add(element);
                Add(command);
            }
        }

        #endregion

        #region Lock

        public bool CanLock { get { return false; } }

        public void Lock(ElementModel emodel)
        {

        }

        public void Unlock(ElementModel emodel)
        {

        }

        #endregion

        #region Monitor Element

        private SortedList<string, ElementModel> ReadModels
            = new SortedList<string, ElementModel>();
        private SortedList<string, ElementModel> WriteModels
            = new SortedList<string, ElementModel>();
        private ElementModel[] LadderModels
            = new ElementModel[0];
        
        public MonitorManager_ElementModelHandle H_Model { get; private set; }

        public ElementModel O_Model { get; private set; }

        public ElementModel[] A_Models { get; private set; }

        public ElementModel Get(ElementModel emodel)
        {
            string flagname = emodel.FlagName;
            if (ReadModels.ContainsKey(flagname))
            {
                return ReadModels[flagname];
            }
            else
            {
                return null;
            }
        }

        public void Add(ElementModel emodel)
        {
            if (_Thread_IsActive)
            {
                if (H_Model == MonitorManager_ElementModelHandle.NULL)
                {
                    O_Model = emodel;
                    H_Model = MonitorManager_ElementModelHandle.ADD;
                    _Thread_Active = false;
                }
            }
            else
            {
                _Add(emodel);
            }
        }

        private void _Add(ElementModel emodel, bool arrange = true)
        {
            string flagname = emodel.FlagName;
            if (!ReadModels.ContainsKey(flagname))
            {
                ReadModels.Add(flagname, emodel);
            }
            else
            {
                ReadModels[flagname].RefCount++;
            }
            if (arrange) Arrange();
        }

        public void Remove(ElementModel emodel)
        {
            if (_Thread_IsActive)
            {
                if (H_Model == MonitorManager_ElementModelHandle.NULL)
                {
                    O_Model = emodel;
                    H_Model = MonitorManager_ElementModelHandle.REMOVE;
                    _Thread_Active = false;
                }
            }
            else
            {
                _Remove(emodel);
            }
        }

        private void _Remove(ElementModel emodel, bool arrange = true)
        {
            string flagname = emodel.FlagName;
            ElementModel _emodel = null;
            if (ReadModels.ContainsKey(flagname))
            {
                _emodel = ReadModels[flagname];
                if (--_emodel.RefCount == 0)
                {
                    ReadModels.Remove(flagname);
                }
            }
            if (arrange) Arrange();
        }

        public void Write(ElementModel emodel)
        {
            if (_Thread_IsActive)
            {
                if (H_Model == MonitorManager_ElementModelHandle.NULL)
                {
                    O_Model = emodel;
                    H_Model = MonitorManager_ElementModelHandle.WRITE;
                    _Thread_Active = false;
                }
            }
            else
            {
                _Write(emodel);
            }
        }

        private void _Write(ElementModel emodel)
        {
            string flagname = emodel.FlagName;
            if (!WriteModels.ContainsKey(flagname))
            {
                WriteModels.Add(flagname, emodel);
            }
            else
            {
                throw new ArgumentException(
                    String.Format("It already had a same ElementModel named as {0}", flagname));
            }
        }

        public void Add(IEnumerable<ElementModel> emodels)
        {
            if (_Thread_IsActive)
            {
                if (H_Model == MonitorManager_ElementModelHandle.NULL)
                {
                    A_Models = emodels.ToArray();
                    H_Model = MonitorManager_ElementModelHandle.MULTIADD;
                    _Thread_Active = false;
                }
            }
            else
            {
                _Add(emodels.ToArray());
            }
        }

        private void _Add(ElementModel[] emodels, bool arrange = true)
        {
            foreach (ElementModel emodel in emodels)
            {
                string flagname = emodel.FlagName;
                if (!ReadModels.ContainsKey(flagname))
                {
                    ReadModels.Add(flagname, emodel);
                }
                else
                {
                    ReadModels[flagname].RefCount++;
                }
            }
            if (arrange) Arrange();
        }

        public void Remove(IEnumerable<ElementModel> emodels)
        {
            if (_Thread_IsActive)
            {
                if (H_Model == MonitorManager_ElementModelHandle.NULL)
                {
                    A_Models = emodels.ToArray();
                    H_Model = MonitorManager_ElementModelHandle.MULTIREMOVE;
                    _Thread_Active = false;
                }
            }
            else
            {
                _Remove(emodels.ToArray());
            }
        }

        private void _Remove(ElementModel[] emodels, bool arrange = true)
        {
            foreach (ElementModel emodel in emodels)
            {
                string flagname = emodel.FlagName;
                ElementModel _emodel = null;
                if (ReadModels.ContainsKey(flagname))
                {
                    _emodel = ReadModels[flagname];
                    if (--_emodel.RefCount == 0)
                    {
                        ReadModels.Remove(flagname);
                    }
                }
            }
            if (arrange) Arrange();
        }

        public void Write(IEnumerable<ElementModel> emodels)
        {
            if (_Thread_IsActive)
            {
                if (H_Model == MonitorManager_ElementModelHandle.NULL)
                {
                    A_Models = emodels.ToArray();
                    H_Model = MonitorManager_ElementModelHandle.MULTIWRITE;
                    _Thread_Active = false;
                }
            }
            else
            {
                _Write(emodels.ToArray());
            }
        }

        private void _Write(ElementModel[] emodels)
        {
            foreach (ElementModel emodel in emodels)
            {
                _Write(emodel);
            }
        }
        
        #endregion

        #region Communication Commands
        
        private List<ICommunicationCommand> ReadCommands
            = new List<ICommunicationCommand>();
        private int ReadCommandIndex;
        private Queue<ICommunicationCommand> WriteCommands
            = new Queue<ICommunicationCommand>();
        private ICommunicationCommand CurrentCommand;
        
        private MonitorManager_CommunicationCommandHandle H_Command
            = MonitorManager_CommunicationCommandHandle.NULL;
        private ICommunicationCommand O_Command = null;
        
        public void Add(ICommunicationCommand cmd)
        {
            if (cmd is GeneralReadCommand
             || cmd is IntrasegmentReadCommand)
            {
                throw new NotSupportedException("Unsupported operation : directly add a read command.");
            }
            if (cmd is GeneralWriteCommand
             || cmd is IntrasegmentWriteCommand)
            {
                if (_Thread_IsActive)
                {
                    if (H_Command == MonitorManager_CommunicationCommandHandle.NULL)
                    {
                        H_Command = MonitorManager_CommunicationCommandHandle.ADD;
                        O_Command = cmd;
                        _Thread_Active = false;
                    }
                }
                else
                {
                    _Add(cmd);
                }
            }
        }

        private void _Add(ICommunicationCommand cmd)
        {
            WriteCommands.Enqueue(cmd);
        }

        public void Remove(ICommunicationCommand cmd)
        {
            throw new NotSupportedException("Unsupported operation : directly remove a command.");
        }

        private void _Remove(ICommunicationCommand cmd)
        {
            throw new NotSupportedException("Unsupported operation : directly remove a command.");
        }

        private bool Send(ICommunicationCommand cmd)
        {
            return (CManager.Write(cmd) == 0);
        }

        private bool Recv(ICommunicationCommand cmd)
        {
            return (CManager.Read(cmd) == 0);
        }

        private void Execute(ICommunicationCommand cmd)
        {

        }
        public void ArrangeOld()
        {
            //ElementModel prev = null;
            //ICommunicationCommand prevcmd = null;
            //foreach (ElementModel next in ReadModels.Values)
            //{
            //    bool canmerge = (prev != null);
            //    if (canmerge)
            //    {
            //        canmerge &= (prev.AddrType == next.AddrType);
            //        //canmerge &= !(prev.IsIntrasegment ^ next.IsIntrasegment);
            //        canmerge &= (prev.IntrasegmentType == next.IntrasegmentType);
            //        canmerge &= (prev.IntrasegmentAddr == next.IntrasegmentAddr);
            //    }
            //    if (canmerge)
            //    {
            //        if (prevcmd is GeneralReadCommand)
            //        {
            //            GeneralReadCommand grcmd = (GeneralReadCommand)prevcmd;
            //            AddrSegment seg1 = grcmd.AddrSeg1;
            //            AddrSegment seg2 = grcmd.AddrSeg2;
            //            if (!seg1.Merge(next) && !seg2.Merge(next))
            //            {
            //                canmerge = false;
            //            }
            //            grcmd.AddrSeg1 = seg1;
            //            grcmd.AddrSeg2 = seg2;
            //        }
            //        else if (prevcmd is IntrasegmentReadCommand)
            //        {
            //            IntrasegmentReadCommand ircmd = (IntrasegmentReadCommand)prevcmd;
            //            IntraSegment iseg = ircmd.IntraSeg;
            //            if (!iseg.Base.Merge(next))
            //            {
            //                canmerge = false;
            //            }
            //            ircmd.IntraSeg = iseg;
            //        }
            //    }
            //    if (!canmerge)
            //    {
            //        if (next.IsIntrasegment)
            //        {
            //            IntrasegmentReadCommand ircmd = new IntrasegmentReadCommand();
            //            byte addrtype1 = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), next.AddrType), next.StartAddr);
            //            byte[] startaddr = ValueConverter.GetBytes((ushort)next.StartAddr);
            //            byte startLowAddr1 = startaddr[0];
            //            byte startHighAddr = startaddr[1];
            //            byte addrtype2 = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), next.IntrasegmentType), next.IntrasegmentAddr);
            //            byte startLowAddr2 = (byte)next.IntrasegmentAddr;
            //            AddrSegment bseg = new AddrSegment(
            //                addrtype1, (byte)(next.ByteCount), startLowAddr1, startHighAddr);
            //            AddrSegment iseg = new AddrSegment(
            //                addrtype2, 1, startLowAddr2, 0);
            //            ircmd.IntraSeg = new IntraSegment(
            //                bseg, iseg);
            //            ReadCommands.Add(ircmd);
            //        }
            //        else
            //        {
            //            GeneralReadCommand grcmd = new GeneralReadCommand();
            //            byte addrtype = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), next.AddrType), next.StartAddr);
            //            byte[] startaddr = ValueConverter.GetBytes((ushort)next.StartAddr);
            //            byte startLowAddr = startaddr[0];
            //            byte startHighAddr = startaddr[1];
            //            grcmd.AddrSeg1 = new AddrSegment(
            //                addrtype, (byte)(next.ByteCount), startLowAddr, startHighAddr);
            //            ReadCommands.Add(grcmd);
            //        }
            //    }
            //    prev = next;
            //}
        }
        public void Arrange()
        {
            ReadCommands.Clear();
            Queue<string> tempQueue_Base = new Queue<string>();
            foreach (var ele in ReadModels.Values)
            {
                if (!tempQueue_Base.Contains(ele.AddrType))
                {
                    tempQueue_Base.Enqueue(ele.AddrType);
                }
            }
            string addrType;
            int gIndex = 0;
            bool gisFirst = true,iisFirst = true;
            GeneralReadCommand gcmd = new GeneralReadCommand();
            gcmd.SegmentsGroup[gIndex] = new List<AddrSegment>();
            IntrasegmentReadCommand icmd = new IntrasegmentReadCommand();
            while (tempQueue_Base.Count > 0)
            {
                uint gstart = 0,istart = 0;
                ElementModel gstartele = null,istartele = null;
                addrType = tempQueue_Base.Dequeue();
                List<ElementModel> elements = ReadModels.Values.Where(x => { return x.AddrType == addrType; }).OrderBy(x => { return x.StartAddr; }).ToList();
                if (elements.Count > 0)
                {
                    for (int i = 0; i < elements.Count; i++)
                    {
                        if (i == 0)
                        {
                            if (elements[i].IsIntrasegment)
                            {
                                if (!iisFirst)
                                {
                                    ReadCommands.Add(icmd);
                                    icmd = new IntrasegmentReadCommand();
                                    iisFirst = false;
                                }
                                icmd.Segments.Add(GenerateIntraSegmentByElement(elements[i]));
                                istart = elements[i].StartAddr;
                                istartele = elements[i];
                            }
                            else
                            {
                                gstart = elements[i].StartAddr;
                                gstartele = elements[i];
                                if (gisFirst)
                                {
                                    gcmd.SegmentsGroup[gIndex].Add(GenerateAddrSegmentByElement(elements[i]));
                                    gisFirst = false;
                                }
                                else ArrangeCmd(gcmd, ref gIndex, elements[i]);
                            }
                        }
                        else if (!elements[i].IsIntrasegment && elements[i].StartAddr - gstart < GetMaxRange(gstartele))
                        {
                            gcmd.SegmentsGroup[gIndex].Add(GenerateAddrSegmentByElement(elements[i]));
                        }
                        else if (elements[i].IsIntrasegment && elements[i].StartAddr - istart < GetMaxRange(istartele) && IsSameIntraBase(istartele, elements[i]))
                        {
                            icmd.Segments.Add(GenerateIntraSegmentByElement(elements[i]));
                        }
                        else
                        {
                            if (elements[i].IsIntrasegment)
                            {
                                ReadCommands.Add(icmd);
                                icmd = new IntrasegmentReadCommand();
                                icmd.Segments.Add(GenerateIntraSegmentByElement(elements[i]));
                                istart = elements[i].StartAddr;
                                istartele = elements[i];
                            }
                            else
                            {
                                gstart = elements[i].StartAddr;
                                gstartele = elements[i];
                                ArrangeCmd(gcmd,ref gIndex,elements[i]);
                            }
                        }
                    }
                }
            }
            //添加剩余命令
            if (!gisFirst) ReadCommands.Add(gcmd);
            if (!iisFirst) ReadCommands.Add(icmd);
        }
        private void ArrangeCmd(GeneralReadCommand command,ref int index,ElementModel element)
        {
            if (index < CommunicationDataDefine.MAX_ADDRESS_TYPE - 1) index++;
            else
            {
                index = 0;
                ReadCommands.Add(command);
                command = new GeneralReadCommand();
            }
            command.SegmentsGroup[index] = new List<AddrSegment>();
            command.SegmentsGroup[index].Add(GenerateAddrSegmentByElement(element));
        }
        private bool IsSameIntraBase(ElementModel one,ElementModel two)
        {
            return one.IntrasegmentAddr == two.IntrasegmentAddr && one.IntrasegmentType == two.IntrasegmentType;
        }
        public int GetMaxRange(ElementModel ele)
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
        private AddrSegment GenerateAddrSegmentByElement(ElementModel element)
        {
            return new AddrSegment(element);
        }
        private IntraSegment GenerateIntraSegmentByElement(ElementModel element)
        {
            AddrSegment bseg = new AddrSegment(element);
            AddrSegment iseg = new AddrSegment(element,true);
            return new IntraSegment(bseg, iseg);
        }
        #endregion

        #region Event Handler

        private void OnThreadPause(object sender, RoutedEventArgs e)
        {
            switch (H_Model)
            {
                case MonitorManager_ElementModelHandle.ADD:
                    _Add(O_Model);
                    break;
                case MonitorManager_ElementModelHandle.REMOVE:
                    _Remove(O_Model);
                    break;
                case MonitorManager_ElementModelHandle.WRITE:
                    _Write(O_Model);
                    break;
                case MonitorManager_ElementModelHandle.MULTIADD:
                    _Add(A_Models);
                    break;
                case MonitorManager_ElementModelHandle.MULTIREMOVE:
                    _Remove(A_Models);
                    break;
                case MonitorManager_ElementModelHandle.MULTIWRITE:
                    _Write(A_Models);
                    break;
            }
            H_Model = MonitorManager_ElementModelHandle.NULL;
            switch (H_Command)
            {
                case MonitorManager_CommunicationCommandHandle.ADD:
                    _Add(O_Command);
                    break;
                case MonitorManager_CommunicationCommandHandle.REMOVE:
                    _Remove(O_Command);
                    break;
            }
            H_Command = MonitorManager_CommunicationCommandHandle.NULL;
            _Thread_Active = true;
        }

        #endregion
    }
}
