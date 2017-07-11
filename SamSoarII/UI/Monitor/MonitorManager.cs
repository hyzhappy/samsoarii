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
        void Replace(ElementModel emodel_old, ElementModel emodel_new);
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
        NULL = 0x00, ADD, REMOVE, REPLACE, WRITE, MULTIADD, MULTIREMOVE, MULTIWRITE
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
        private void _Thread_WaitForActive(int itvtime = 10)
        {
            _Thread_IsActive = false;
            ThreadPause(this, new RoutedEventArgs());
            do
            {
                Thread.Sleep(itvtime);
            } while (_Thread_Alive && !_Thread_Active);
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
                        while (_Thread_Alive && _Thread_Active 
                            && sendtime < 5)
                        {
                            if (Send(CurrentCommand))
                            {
                                hassend = true;
                                break;
                            }
                            sendtime++;
                        }
                        int itvtime = 20;
                        if (CurrentCommand is GeneralWriteCommand
                         || CurrentCommand is IntrasegmentWriteCommand)
                        {
                            itvtime = 100;
                        }
                        //_Thread_WaitForActive(itvtime);
                        Thread.Sleep(itvtime);
                        while (hassend)
                        {
                            try
                            {
                                if (Recv(CurrentCommand))
                                {
                                    hasrecv = true;
                                    break;
                                }
                            }
                            catch (Exception e)
                            {

                            }
                            recvtime++;
                        }
                    }
                    if (_Thread_Alive && hassend && hasrecv)
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
                Arrange();
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
            //ComThread = new Thread(_Thread_Run);
            ThreadPause += OnThreadPause;
        }

        public void Dispose()
        {
            ThreadPause -= OnThreadPause;
            Abort();
            MMWindow.Manager = null;
            MMWindow.Dispose();
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
            ElementModel[] _emodels = mvtable.Elements.ToArray();
            mvtable.Elements.Clear();
            foreach (ElementModel emodel in _emodels)
            {
                _Add(emodel, false);
                mvtable.Elements.Add(Get(emodel));
            }
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
                    bvalue = 0x00;
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
                    bvalue = 0x01;
                    element.SetValue = "ON";
                    Write(element, bvalue);
                    break;
                case ElementValueModifyEventType.Write:
                    element.SetValue = e.Value;
                    Write(element,element.SetValue);
                    break;
            }
        }
        private void Force(ElementModel element, byte value)
        {
            if (element.IsIntrasegment)
                Add(new IntrasegmentWriteCommand(new byte[] { value }, element));
            else
                Add(new GeneralWriteCommand(new byte[] { value }, element));
        }
        private void Write(ElementModel element, byte value)
        {
            if (element.IsIntrasegment)
                Add(new IntrasegmentWriteCommand(new byte[] { value }, element));
            else
                Add(new GeneralWriteCommand(new byte[] { value }, element));
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
                Add(command);
            }
            else
            {
                GeneralWriteCommand command = new GeneralWriteCommand(data, element);
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
                switch (H_Model)
                {
                    case MonitorManager_ElementModelHandle.ADD:
                        if (flagname.Equals(O_Model.FlagName))
                            return O_Model;
                        break;
                    case MonitorManager_ElementModelHandle.REPLACE:
                        if (flagname.Equals(A_Models[1].FlagName))
                            return A_Models[1];
                        break;
                    case MonitorManager_ElementModelHandle.MULTIADD:
                        IEnumerable<ElementModel> fit = A_Models.Where
                        (
                            (model) => { return flagname.Equals(model.FlagName); }
                        );
                        if (fit.Count() > 0)
                            return fit.First();
                        break;
                }
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

        public void Replace(ElementModel emodel_old, ElementModel emodel_new)
        {
            if (_Thread_IsActive)
            {
                if (H_Model == MonitorManager_ElementModelHandle.NULL)
                {
                    A_Models = new ElementModel[2];
                    A_Models[0] = emodel_old;
                    A_Models[1] = emodel_new;
                    H_Model = MonitorManager_ElementModelHandle.REPLACE;
                    _Thread_Active = false;
                }
            }
            else
            {
                _Replace(emodel_old, emodel_new);
            }
        }

        private void _Replace(ElementModel emodel_old, ElementModel emodel_new, bool arrange = true)
        {
            _Remove(emodel_old, false);
            _Add(emodel_new, false);
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
             || cmd is IntrasegmentWriteCommand
             || cmd is ForceCancelCommand)
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
            if (cmd.IsSuccess)
            {
                cmd.UpdataValues();
            }
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
            uint gstart = 0, istart = 0;
            ElementModel gstartele = null, istartele = null;
            GeneralReadCommand gcmd = new GeneralReadCommand();
            gcmd.SegmentsGroup[gIndex] = new List<AddrSegment>();
            IntrasegmentReadCommand icmd = new IntrasegmentReadCommand();
            while (tempQueue_Base.Count > 0)
            {
                addrType = tempQueue_Base.Dequeue();
                List<ElementModel> elements = ReadModels.Values.Where(x => { return x.AddrType == addrType; }).OrderBy(x => { return x.StartAddr; }).ToList();
                if (elements.Count > 0)
                {
                    for (int i = 0; i < elements.Count; i++)
                    {
                        if ((iisFirst && elements[i].IsIntrasegment) || (gisFirst && !elements[i].IsIntrasegment))
                        {
                            if (elements[i].IsIntrasegment)
                            {
                                if (!iisFirst)
                                {
                                    ReadCommands.Add(icmd);
                                    icmd = new IntrasegmentReadCommand();
                                }else iisFirst = false;
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
                                else ArrangeCmd(ref gcmd, ref gIndex, elements[i]);
                            }
                        }
                        else if (!elements[i].IsIntrasegment && GetAddrSpan(elements[i],gstart) < GetMaxRange(gstartele))
                        {
                            if (elements[i].AddrType == gstartele.AddrType)
                            {
                                gcmd.SegmentsGroup[gIndex].Add(GenerateAddrSegmentByElement(elements[i]));
                            }
                            else
                            {
                                gstart = elements[i].StartAddr;
                                gstartele = elements[i];
                                ArrangeCmd(ref gcmd, ref gIndex, elements[i]);
                            }
                        }
                        else if (elements[i].IsIntrasegment && GetAddrSpan(elements[i], istart) < GetMaxRange(istartele) && IsSameIntraBase(istartele, elements[i]))
                        {
                            if (elements[i].AddrType == istartele.AddrType)
                            {
                                icmd.Segments.Add(GenerateIntraSegmentByElement(elements[i]));
                            }
                            else
                            {
                                ReadCommands.Add(icmd);
                                icmd = new IntrasegmentReadCommand();
                                icmd.Segments.Add(GenerateIntraSegmentByElement(elements[i]));
                                istart = elements[i].StartAddr;
                                istartele = elements[i];
                            }
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
                                ArrangeCmd(ref gcmd,ref gIndex,elements[i]);
                            }
                        }
                    }
                }
            }
            //添加剩余命令
            if (!gisFirst) ReadCommands.Add(gcmd);
            if (!iisFirst) ReadCommands.Add(icmd);
        }
        /// <summary>
        /// 对命令进行分组
        /// </summary>
        /// <param name="command">当前命令</param>
        /// <param name="index">当前命令片段的索引</param>
        /// <param name="element">需添加的元素</param>
        private void ArrangeCmd(ref GeneralReadCommand command,ref int index,ElementModel element)
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
        private uint GetAddrSpan(ElementModel element,uint startAddr)
        {
            if (element.ByteCount == 4 && !(element.AddrType == "CV" && element.StartAddr >= 200))
            {
                return element.StartAddr - startAddr + 1;
            }
            else
            {
                return element.StartAddr - startAddr;
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
                case MonitorManager_ElementModelHandle.REPLACE:
                    _Replace(A_Models[0], A_Models[1]);
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
