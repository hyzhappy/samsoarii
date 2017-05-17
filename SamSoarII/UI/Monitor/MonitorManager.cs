using SamSoarII.AppMain.Project;
using SamSoarII.Communication;
using SamSoarII.Communication.Command;
using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.LadderInstViewModel.Monitor;
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
    public enum MonitorManager_ElementModelHandle
    {
        NULL = 0x00, ADD, REMOVE, WRITE, MULTIADD, MULTIREMOVE, MULTIWRITE
    }
    
    public enum MonitorManager_CommunicationCommandHandle
    {
        NULL = 0x00, ADD, REMOVE
    }

    public class MonitorManager : IDisposable
    {
        public MainMonitor MMWindow { get; set; }

        #region Thread

        private Thread ComThread { get; set; }
        private bool _Thread_Alive = false;
        private bool _Thread_IsAlive = false;
        private bool _Thread_Active = false;
        private bool _Thread_IsActive = false;
        public event RoutedEventHandler ThreadResume = delegate { };
        public event RoutedEventHandler ThreadPause = delegate { };
        private void _Thread_Run()
        {
            _Thread_IsAlive = true;
            while (_Thread_Alive)
            {
                _Thread_IsActive = true;
                ThreadResume(this, new RoutedEventArgs());
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
                            if (ReadCommandIndex >= ReadCommands.Count())
                            {
                                ReadCommandIndex = 0;
                            }
                            CurrentCommand = ReadCommands[ReadCommandIndex++];
                        }
                    }
                    bool hassend = false;
                    bool hasrecv = false;
                    if (CurrentCommand != null)
                    {
                        while (_Thread_Alive && _Thread_Active)
                        {
                            if (Send(CurrentCommand))
                            {
                                hassend = true;
                                break;
                            }
                        }
                        while (_Thread_Alive && _Thread_Active)
                        {
                            if (Recv(CurrentCommand))
                            {
                                hasrecv = true;
                                break;
                            }
                        }
                    }
                    if (hassend && hasrecv)
                    {
                        Execute(CurrentCommand);
                        CurrentCommand = null;
                    }
                    Thread.Sleep(10);
                }
                _Thread_IsActive = false;
                ThreadPause(this, new RoutedEventArgs());
                while (_Thread_Alive && !_Thread_Active)
                {
                    Thread.Sleep(10);
                }
            }
            _Thread_IsAlive = false;
        }
        
        public void Start()
        {
            if (!_Thread_IsAlive)
            {
                ComThread = new Thread(_Thread_Run);
                _Thread_Alive = true;
                ComThread.Start();
            }
        }

        public void Abort()
        {
            _Thread_Alive = false;
        }

        #endregion

        public MonitorManager(ProjectModel projectModel)
        {
            MMWindow = new MainMonitor(projectModel);
            //DataHandle = new MonitorDataHandle(MMWindow);
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
            ReadModels.Clear();
            WriteModels.Clear();
            ReadCommands.Clear();
            WriteCommands.Clear();
            if (MMWindow != null)
            {
                foreach (MonitorVariableTable mvtable in MMWindow.tables)
                {
                    Initialize(mvtable);
                }
            }
        }

        public void Initialize(ProjectModel pmodel)
        {
            Initialize();
            Initialize(pmodel.MainRoutine);
            foreach (LadderDiagramViewModel ldvmodel in pmodel.SubRoutines)
            {
                Initialize(ldvmodel);
            }
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
            foreach (MoniBaseViewModel bvmodel in lnvmodel.GetMonitors())
            {
                BaseModel bmodel = bvmodel.Model;
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
                        _Add(elementmodel);
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
                _Add(emodel);
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

        #region Monitor Element

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

        private void _Add(ElementModel emodel)
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
            Arrange();
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

        private void _Remove(ElementModel emodel)
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
            Arrange();
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

        private void _Add(ElementModel[] emodels)
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
            Arrange();
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

        private void _Remove(ElementModel[] emodels)
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
            Arrange();
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
        
        private SortedList<string, ElementModel> ReadModels
            = new SortedList<string, ElementModel>();
        private List<ICommunicationCommand> ReadCommands
            = new List<ICommunicationCommand>();
        private int ReadCommandIndex;
        private SortedList<string, ElementModel> WriteModels
            = new SortedList<string, ElementModel>();
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
            return true;
        }

        private bool Recv(ICommunicationCommand cmd)
        {
            return true;
        }

        private void Execute(ICommunicationCommand cmd)
        {
        }

        public void Arrange()
        {
            ElementModel prev = null;
            ICommunicationCommand prevcmd = null;
            foreach (ElementModel next in ReadModels.Values)
            {
                bool canmerge = (prev != null);
                if (canmerge)
                {
                    canmerge &= (prev.AddrType == next.AddrType);
                    canmerge &= !(prev.IsIntrasegment ^ next.IsIntrasegment);
                    canmerge &= (prev.IntrasegmentType == next.IntrasegmentType);
                    canmerge &= (prev.IntrasegmentAddr == next.IntrasegmentAddr);
                }
                if (canmerge)
                {
                    if (prevcmd is GeneralReadCommand)
                    {
                        GeneralReadCommand grcmd = (GeneralReadCommand)prevcmd;
                        AddrSegment seg1 = grcmd.AddrSeg1;
                        AddrSegment seg2 = grcmd.AddrSeg2;
                        if (!seg1.Merge(next) && !seg2.Merge(next))
                        {
                            canmerge = false;
                        }
                        grcmd.AddrSeg1 = seg1;
                        grcmd.AddrSeg2 = seg2;
                    }
                    else if (prevcmd is IntrasegmentReadCommand)
                    {
                        IntrasegmentReadCommand ircmd = (IntrasegmentReadCommand)prevcmd;
                        IntraSegment iseg = ircmd.IntraSeg;
                        if (!iseg.Base.Merge(next))
                        {
                            canmerge = false;
                        }
                        ircmd.IntraSeg = iseg;
                    }
                }
                if (!canmerge)
                {
                    if (next.IsIntrasegment)
                    {
                        IntrasegmentReadCommand ircmd = new IntrasegmentReadCommand();
                        byte addrtype1 = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), next.AddrType), next.StartAddr);
                        byte[] startaddr = ValueConverter.GetBytes((ushort)next.StartAddr);
                        byte startLowAddr1 = startaddr[0];
                        byte startHighAddr = startaddr[1];
                        byte addrtype2 = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), next.IntrasegmentType), next.IntrasegmentAddr);
                        byte startLowAddr2 = (byte)next.IntrasegmentAddr;
                        AddrSegment bseg = new AddrSegment(
                            addrtype1, 1, startLowAddr1, startHighAddr);
                        AddrSegment iseg = new AddrSegment(
                            addrtype2, 1, startLowAddr2, 0);
                        ircmd.IntraSeg = new IntraSegment(
                            bseg, iseg);
                    }
                    else
                    {
                        GeneralReadCommand grcmd = new GeneralReadCommand();
                        byte addrtype = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), next.AddrType), next.StartAddr);
                        byte[] startaddr = ValueConverter.GetBytes((ushort)next.StartAddr);
                        byte startLowAddr = startaddr[0];
                        byte startHighAddr = startaddr[1];
                        grcmd.AddrSeg1 = new AddrSegment(
                            addrtype, 1, startLowAddr, startHighAddr);
                    }
                }
                prev = next;
            }
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
