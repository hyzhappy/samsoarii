using SamSoarII.Core.Generate;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class InstructionDiagramModel : IModel
    {
        public InstructionDiagramModel(LadderDiagramModel _parent)
        {
            Parent = _parent;
        }

        public void Dispose()
        {
            if (View != null) View.Dispose();
            foreach (InstructionNetworkModel instnet in Children)
                instnet.Dispose();
            Parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private LadderDiagramModel parent;
        public LadderDiagramModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (parent == value) return;
                LadderDiagramModel _parent = parent;
                this.parent = value;
                if (_parent != null)
                {
                    _parent.PropertyChanged -= OnParentPropertyChanged;
                    _parent.ChildrenChanged -= OnParentChildrenChanged;
                    if (_parent.Inst != null) _parent.Inst = null;
                }
                if (parent != null)
                {
                    parent.PropertyChanged += OnParentPropertyChanged;
                    parent.ChildrenChanged += OnParentChildrenChanged;
                    if (parent.Inst != this) parent.Inst = this;
                }
            }
        }
        IModel IModel.Parent { get { return Parent; } }
        public ValueManager ValueManager { get { return parent?.Parent?.Parent?.MNGValue; } }

        public IEnumerable<InstructionNetworkModel> Children { get { return parent?.Children.Select(n => n.Inst); } }
        
        #endregion

        #region Shell

        private InstructionDiagramViewModel view;
        public InstructionDiagramViewModel View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                InstructionDiagramViewModel _view = view;
                this.view = value;
                if (_view != null && _view.Core != null) _view.Core = null;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View
        {
            get { return view; }
            set { View = (InstructionDiagramViewModel)value; }
        }
        
        private ProjectTreeViewItem ptvitem;
        public ProjectTreeViewItem PTVItem
        {
            get { return this.ptvitem; }
            set { this.ptvitem = value; }
        }
        
        public event PropertyChangedEventHandler ViewPropertyChanged = delegate { };

        private LadderModes laddermode;
        public LadderModes LadderMode
        {
            get
            {
                return this.laddermode;
            }
            set
            {
                this.laddermode = value;
                foreach (InstructionNetworkModel netinst in Children)
                    netinst.LadderMode = laddermode;
                ViewPropertyChanged(this, new PropertyChangedEventArgs("LadderMode"));
            }
        }

        private bool iscommentmode;
        public bool IsCommentMode
        {
            get
            {
                return this.iscommentmode;
            }
            set
            {
                this.iscommentmode = value;
                foreach (InstructionNetworkModel netinst in Children)
                    netinst.IsCommentMode = iscommentmode;
                ViewPropertyChanged(this, new PropertyChangedEventArgs("IsCommentMode"));
            }
        }
        
        private double viewheight;
        public double ViewHeight { get { return this.viewheight; } }
        
        public void UpdateCanvasTop()
        {
            double currenttop = 0;
            foreach (InstructionNetworkModel instnet in Children)
            {
                instnet.CanvasTop = currenttop;
                currenttop += 26;
                if (instnet.IsExpand)
                    currenttop += instnet.Invalid ? 20 : 20 * instnet.Insts.Count;
                instnet.ViewHeight = currenttop - instnet.CanvasTop;
            }
            viewheight = currenttop + 4;
            ViewPropertyChanged(this, new PropertyChangedEventArgs("ViewHeight"));
        }

        #endregion

        #region Check

        public void Check()
        {
            int stkcount = 0;
            int outcount = 0;
            int forcount = 0;
            int stlcount = 0;
            Dictionary<string, PLCOriginInst> lbdict = new Dictionary<string, PLCOriginInst>();
            Match match = null;
            
            foreach (InstructionNetworkModel invmodel in Children.Where(inv => !inv.Parent.IsMasked))
            {
                if (invmodel.IsModified) invmodel.Update();
                if (invmodel.IsOpenCircuit || invmodel.IsShortCircuit || invmodel.IsFusionCircuit) continue;
                stkcount = invmodel.Insts.Where(i => i[0].StartsWith("LD")).Count();
                outcount = invmodel.Insts.Where(i => i.Inst.ProtoType != null
                    && (i.Inst.ProtoType.Shape == LadderUnitModel.Shapes.Output
                        || i.Inst.ProtoType.Shape == LadderUnitModel.Shapes.OutputRect)).Count();
                foreach (PLCOriginInst inst in invmodel.Insts)
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        if (inst[i].Equals("???"))
                        {
                            inst.Status = PLCOriginInst.STATUS_ERROR;
                            inst.Message = String.Format(Properties.Resources.Message_Missing_Parameters);
                            break;
                        }
                    }
                    if (inst.Status == PLCOriginInst.STATUS_ERROR) continue;
                    switch (inst[0])
                    {
                        case "ANDB":
                        case "ORB":
                            stkcount--;
                            break;
                        case "FOR":
                            if (outcount > 1)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_FOR);
                            }
                            forcount++;
                            break;
                        case "NEXT":
                            if (stkcount > 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_LBL);
                            }
                            if (outcount > 1)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_NEXT);
                            }
                            if (forcount == 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_FOR_Not_Found);
                            }
                            else
                            {
                                forcount--;
                            }
                            break;
                        case "LBL":
                            if (stkcount > 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_LBL);
                            }
                            if (outcount > 1)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_LBL_Error);
                            }
                            if (!lbdict.ContainsKey(inst[1]))
                            {
                                inst.Status = PLCOriginInst.STATUS_WARNING;
                                inst.Message = String.Format(Properties.Resources.Message_LBL_Warning);
                                lbdict.Add(inst[1], inst);
                            }
                            else
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("{0}{1:s}{2}", Properties.Resources.Jump_Mark, inst[1], Properties.Resources.Message_Has_Been_Used);
                            }
                            break;
                        case "STL":
                            if (stkcount > 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_STL);
                            }
                            if (stlcount > 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_STL_Over);
                            }
                            if (outcount > 1)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_STL_OnlyOne);
                            }
                            stlcount++;
                            break;
                        case "STLE":
                            if (stkcount > 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_STLE);
                            }
                            if (stlcount == 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_STLE_Over);
                            }
                            if (outcount > 1)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_STLE_OnlyOne);
                            }
                            stlcount--;
                            break;
                        case "ST":
                            if (stlcount == 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_ST);
                            }
                            break;
                        case "CTU":
                        case "CTD":
                        case "CTUD":
                            match = Regex.Match(inst[1], @"^(CV)([0-9]+)$");
                            if (!match.Success)
                            {
                                inst.Status = PLCOriginInst.STATUS_WARNING;
                                inst.Message = String.Format(Properties.Resources.Message_Intra_Error);
                                break;
                            }
                            break;
                        case "TON":
                        case "TONR":
                        case "TOF":
                            match = Regex.Match(inst[1], @"^(TV)([0-9]+)$");
                            if (!match.Success)
                            {
                                inst.Status = PLCOriginInst.STATUS_WARNING;
                                inst.Message = String.Format(Properties.Resources.Message_Intra_Error);
                                break;
                            }
                            break;
                        case "OUT":
                        case "OUTIM":
                        case "SET":
                        case "SETIM":
                        case "RST":
                        case "RSTIM":
                            match = Regex.Match(inst[1], @"^([YMSTC])([0-9]+)$");
                            if (!match.Success)
                            {
                                inst.Status = PLCOriginInst.STATUS_WARNING;
                                inst.Message = String.Format(Properties.Resources.Message_Intra_Coil_Error);
                                break;
                            }
                            break;
                        case "CALLM":
                            {
                                IEnumerable<FuncModel> fit =
                                    Parent.Parent.Funcs.Where(
                                        (FuncModel _fmodel) => { return _fmodel.Name.Equals(inst[1]); });
                                if (fit.Count() <= 0)
                                {
                                    inst.Status = PLCOriginInst.STATUS_ERROR;
                                    inst.Message = String.Format("{0}{1:s}。", Properties.Resources.Message_Func_Not_Found, inst[1]);
                                    break;
                                }
                                FuncModel fmodel = fit.First();
                                if (!fmodel.CanCALLM())
                                {
                                    inst.Status = PLCOriginInst.STATUS_ERROR;
                                    inst.Message = String.Format("{0}{1:s}{2}", Properties.Resources.User_Function, inst[1], Properties.Resources.Message_CALL);
                                    break;
                                }
                                try
                                {
                                    LadderUnitModel unit = inst.Inst.ProtoType;
                                    unit.Parse(fmodel, unit.InstArgs);
                                }
                                catch (ValueParseException e)
                                {
                                    inst.Status = PLCOriginInst.STATUS_ERROR;
                                    if (e.Format != null)
                                        inst.Message = String.Format("参数{1:s}不合法。", e.Format.Name);
                                    else
                                        inst.Message = String.Format("参数与调用的函数功能块不符。");
                                    break;
                                }
                            }
                            break;
                        case "MBUS":
                            {
                                IEnumerable<ModbusModel> fit = Parent.Parent.Modbus.Children.Where(
                                    m => m.Name.Equals(inst[2]));
                                if (fit.Count() <= 0)
                                {
                                    inst.Status = PLCOriginInst.STATUS_ERROR;
                                    inst.Message = String.Format("{0}{1:s}。", Properties.Resources.Message_Modbus_Table, inst[2]);
                                    return;
                                }
                                ModbusModel mmodel = fit.First();
                                if (!mmodel.IsValid)
                                {
                                    inst.Status = PLCOriginInst.STATUS_ERROR;
                                    inst.Message = String.Format("{0}{1:s}{2}", Properties.Resources.Modbus_Table, inst[2], Properties.Resources.is_illegal);
                                }
                            }
                            break;
                        case "CALL":
                        case "ATCH":
                            {
                                IEnumerable<LadderDiagramModel> fit = Parent.Parent.Diagrams.Where(
                                    d => d.Name.Equals(inst[1]));    
                                if (fit.Count() <= 0)
                                {
                                    inst.Status = PLCOriginInst.STATUS_ERROR;
                                    inst.Message = String.Format("{0}{1:s}", Properties.Resources.Message_SubRoutine_Not_Found, inst[1]);
                                    break;
                                }
                                if (inst[0].Equals("ATCH"))
                                    fit.First().IsInterruptLadder = true;
                            }
                            break;
                    }
                    switch (inst[0])
                    {
                        case "NEXT":
                        case "LBL":
                        case "STL":
                        case "STLE":
                            break;
                        default:
                            if (stkcount == 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_Stack);
                            }
                            break;
                    }
                }
            }
            
            for (int i = Parent.Children.Count() - 1; i >= 0; i--)
            { 
                InstructionNetworkModel inmodel = Parent.Children[i].Inst;
                if (inmodel.Parent.IsMasked) continue;
                foreach (PLCOriginInst inst in inmodel.Insts)
                {
                    if (inst.Status == PLCOriginInst.STATUS_ERROR) continue;
                    switch (inst[0])
                    {
                        case "FOR":
                            if (forcount > 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format(Properties.Resources.Message_NEXT_Not_Found);
                                forcount--;
                            }
                            break;
                        case "JMP":
                            if (!lbdict.ContainsKey(inst[1]))
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("{0}{1:s}", Properties.Resources.Message_Jump_Not_Found, inst[1]);
                            }
                            PLCOriginInst lblinst = lbdict[inst[1]];
                            lblinst.Status = PLCOriginInst.STATUS_ACCEPT;
                            lblinst.Message = String.Empty;
                            break;
                    }
                }
            }
        }


        public void CheckForInterrrupt()
        {
            if (!Parent.IsInterruptLadder) return;
            foreach (InstructionNetworkModel inmodel in Children.Where(inv => !inv.Parent.IsMasked))
            {
                foreach (PLCOriginInst inst in inmodel.Insts)
                {
                    if (inst.Status == PLCOriginInst.STATUS_ERROR) continue;
                    switch (inst[0])
                    {
                        case "OUT":
                        case "OUTIM":
                            inst.Status = PLCOriginInst.STATUS_ERROR;
                            inst.Message = Properties.Resources.Message_Interrupt_1;
                            break;
                        case "LDP":
                        case "LDF":
                        case "MEP":
                        case "MEF":
                            inst.Status = PLCOriginInst.STATUS_ERROR;
                            inst.Message = Properties.Resources.Message_Interrupt_2;
                            break;
                        case "TON":
                        case "TONR":
                        case "TOF":
                            inst.Status = PLCOriginInst.STATUS_ERROR;
                            inst.Message = Properties.Resources.Message_Interrupt_3;
                            break;
                        case "CTU":
                        case "CTD":
                        case "CTUD":
                            inst.Status = PLCOriginInst.STATUS_ERROR;
                            inst.Message = Properties.Resources.Message_Interrupt_4;
                            break;
                        case "ALTP":
                            inst.Status = PLCOriginInst.STATUS_ERROR;
                            inst.Message = Properties.Resources.Message_Interrupt_5;
                            break;
                    }
                }
            }
        }

        #endregion

        #region Save & Load

        public void Load(XElement xele)
        {
            throw new NotImplementedException();
        }

        public void Save(XElement xele)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Event Handler

        public event NotifyCollectionChangedEventHandler ChildrenChanged = delegate { };

        private void OnParentChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            /*
            LadderNetworkModel lnmodel = null;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    lnmodel = (LadderNetworkModel)(e.NewItems[0]);
                    if (lnmodel.Inst == null) lnmodel.Inst = new InstructionNetworkModel(lnmodel);
                    break;
            }
            */
            ChildrenChanged(this, e);
            UpdateCanvasTop();
        }
        
        private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged(this, e);
        }

        #endregion
    }
}
