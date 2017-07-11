using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;
using SamSoarII.LadderInstViewModel.Monitor;
using SamSoarII.AppMain.Project;

namespace SamSoarII.AppMain.UI
{
    public class MonitorContextMenu : ContextMenu
    {
        public MenuItem[] MI_Values { get; private set; }
            = { new MenuItem(), new MenuItem(), new MenuItem(), new MenuItem(), new MenuItem(), };
        public MenuItem MI_Breakpoint = new MenuItem();
        public IValueModel[] values = new IValueModel[5];
        public IMoniValueModel[] mvalues = new IMoniValueModel[5];
        
        public int Count { get; private set; }

        private bool IsVariable(IValueModel model)
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

        private LadderNetworkViewModel lnvmodel;
        private BaseViewModel bvmodel;
        public BaseViewModel BVModel
        {
            get
            {
                return bvmodel;
            }
            set
            {
                foreach (MenuItem MI_Value in MI_Values)
                {
                    MI_Value.Visibility = System.Windows.Visibility.Collapsed;
                }
                Visibility = System.Windows.Visibility.Hidden;
                bvmodel = value;
                if (bvmodel == null) return;
                BaseModel bmodel = bvmodel.Model;
                if (bmodel == null || bmodel.ParaCount == 0) return;
                Count = 0;
                for (int i = 0; i < bmodel.ParaCount; i++)
                {
                    values[Count] = bmodel.GetPara(i);
                    mvalues[Count] = bvmodel.GetValueModel(i);
                    if (!IsVariable(values[Count])
                     || values.Where(
                        (_value) => { return _value != null && _value.ValueString.Equals(values[Count]); }
                    ).Count() > 0)
                    {
                        continue;
                    }
                    MI_Values[Count].Visibility = System.Windows.Visibility.Visible;
                    MI_Values[Count++].Header = String.Format("修改{0:s}", bmodel.GetPara(i).ValueString);
                }
                if (Count == 0) return;
                if (lnvmodel.LadderMode == LadderMode.Simulate)
                {
                    MI_Breakpoint.Header = bvmodel.BPRect == null
                        ? "添加断点" : "删除断点";
                    MI_Breakpoint.Visibility = Visibility.Visible;
                }
                else
                {
                    MI_Breakpoint.Visibility = Visibility.Collapsed;
                }
                Visibility = System.Windows.Visibility.Visible;
            }
        }
        
        public MonitorContextMenu(LadderNetworkViewModel _lnvmodel)
        {
            lnvmodel = _lnvmodel;
            foreach (MenuItem MI_Value in MI_Values)
            {
                Items.Add(MI_Value);
                MI_Value.Click += OnMenuItemClick;
            }
            Items.Add(new Separator());
            Items.Add(MI_Breakpoint);
            MI_Breakpoint.Click += OnMenuItemClick;
        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            LadderDiagramViewModel ldvmodel = lnvmodel.LDVModel;
            for (int i = 0; i < Count; i++)
            {
                if (sender == MI_Values[i])
                {
                    ShowModifyDialog(i);
                }
            }
            if (sender == MI_Breakpoint)
            {
                switch (MI_Breakpoint.Header.ToString())
                {
                    case "添加断点":
                        BreakpointRect brect = new BreakpointRect(String.Empty);
                        brect.BVModel = bvmodel;
                        ldvmodel.ReplaceBreakpoint(lnvmodel, brect);
                        break;
                    case "删除断点":
                        ldvmodel.RemoveBreakpoint(lnvmodel, bvmodel.BPRect);
                        break;
                }
            }
        }

        public void ShowModifyDialog(int select = 0)
        {
            if (Count == 0) return;
            string[] varnames = new string[Count];
            string[] vartypes = new string[Count];
            string[] varvalues = new string[Count];
            for (int i = 0; i < Count; i++)
            {
                varnames[i] = values[i].ValueString;
                varvalues[i] = mvalues[i].Value;
                if (values[i] is BitValue)
                {
                    vartypes[i] = "BIT";
                    switch (mvalues[i].Value)
                    {
                        case "0": varvalues[i] = "OFF"; break;
                        case "1": varvalues[i] = "ON"; break;
                    }
                }
                else if (values[i] is WordValue)
                    vartypes[i] = "WORD";
                else if (values[i] is DoubleWordValue)
                    vartypes[i] = "DWORD";
                else if (values[i] is FloatValue)
                    vartypes[i] = "FLOAT";
                
            }
            using (ElementValueMultiplyModifyDialog dialog = new ElementValueMultiplyModifyDialog(varnames, vartypes, varvalues))
            {
                dialog.SelectedIndex = select;
                dialog.ValueModify += OnValueModify;
                dialog.ShowDialog();
            }
        }

        public event ElementValueModifyEventHandler ValueModify = delegate { };
        private void OnValueModify(object sender, ElementValueModifyEventArgs e)
        {
            ValueModify(this, e);
        }
    }
}
