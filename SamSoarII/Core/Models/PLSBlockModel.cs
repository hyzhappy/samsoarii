using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using SamSoarII.Utility.DXF;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;

namespace SamSoarII.Core.Models
{
    public class PLSBlockModel : IModel
    {
        public PLSBlockModel(ProjectModel _parent, string _filename)
        {
            parent = _parent;
            filename = _filename;
            velocity = new ValueModel(null, new ValueFormat("V", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex }, null, "速度", "Velocity"));
            actime = new ValueModel(null, new ValueFormat("AC", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex }, null, "加速时间", "Accelerate Time"));
            dctime = new ValueModel(null, new ValueFormat("DC", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex }, null, "减速时间", "Decelerate Time"));
            velocity.Text = "K0";
            actime.Text = "K0";
            dctime.Text = "K0";
        }

        public void Dispose()
        {
            parent = null;
            velocity.Dispose();
            actime.Dispose();
            dctime.Dispose();
            velocity = null;
            actime = null;
            dctime = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ProjectModel parent;
        public ProjectModel Parent { get { return this.parent; } }
        IModel IModel.Parent { get { return Parent; } }

        #region Base

        private string filename;
        public string FileName { get { return this.filename; } }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; PropertyChanged(this, new PropertyChangedEventArgs("Name")); }
        }

        private int systemid;
        public int SystemID
        {
            get { return this.systemid; }
            set { this.systemid = value; PropertyChanged(this, new PropertyChangedEventArgs("SystemID")); }
        }

        private DXFEntity[] elements;
        public IList<DXFEntity> Elements { get { return this.elements; } }
        public int Count { get { return elements.Length; } }

        private byte[] data;
        public IList<byte> Data { get { return this.data; } }
        public int DataLength { get { return data.Length; } }

        private ValueModel velocity;
        public ValueModel Velocity { get { return this.velocity; } }

        private ValueModel actime;
        public ValueModel ACTime { get { return this.actime; } }

        private ValueModel dctime;
        public ValueModel DDTime { get { return this.dctime; } }

        #endregion

        #region DataGrid

        private string systemid_s;
        public string SystemID_S
        {
            get { return this.systemid_s; }
            set { this.systemid_s = value; PropertyChanged(this, new PropertyChangedEventArgs("SystemID_S")); }
        }

        private string velocity_s;
        public string Velocity_S
        {
            get { return this.velocity_s; }
            set { this.velocity_s = value; PropertyChanged(this, new PropertyChangedEventArgs("Velocity_S")); }
        }

        private string actime_s;
        public string ACTime_S
        {
            get { return this.actime_s; }
            set { this.actime_s = value; PropertyChanged(this, new PropertyChangedEventArgs("ACTime_S")); }
        }

        private string dctime_s;
        public string DCTime_S
        {
            get { return this.dctime_s; }
            set { this.dctime_s = value; PropertyChanged(this, new PropertyChangedEventArgs("DCTime_S")); }
        }

        #endregion

        #endregion

        #region View

        IViewModel IModel.View { get { return null; } set { } }
        
        ProjectTreeViewItem IModel.PTVItem { get { return null; } set { } }

        #endregion

        #region Save & Load

        public void Load(XElement xele)
        {
            filename = xele.Attribute("FileName").Value;
            name = xele.Attribute("Name").Value;
            systemid = int.Parse(xele.Attribute("SystemID").Value);
            velocity.Text = xele.Attribute("Velocity").Value;
            actime.Text = xele.Attribute("ACTime").Value;
            dctime.Text = xele.Attribute("DCTime").Value;
        }

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("FileName", filename);
            xele.SetAttributeValue("Name", name);
            xele.SetAttributeValue("SystemID", systemid);
            xele.SetAttributeValue("Velocity", velocity.Text);
            xele.SetAttributeValue("ACTime", actime.Text);
            xele.SetAttributeValue("DCTime", dctime.Text);
        }

        public void CreateToDataGrid()
        {
            velocity_s = velocity.Text;
            actime_s = actime.Text;
            dctime_s = dctime.Text;
        }

        public void LoadFromDataGrid()
        {
            velocity.Text = velocity_s;
            actime.Text = actime_s;
            dctime.Text = dctime_s;
        }

        #endregion
    }
}
