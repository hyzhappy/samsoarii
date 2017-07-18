using SamSoarII.PLCDevice;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class ModbusItem : IModel
    {
        #region Resources

        static readonly public ValueFormat BitFormat = new ValueFormat(
            "Bit", ValueModel.Types.BOOL, true, true, 0, new Regex[] { ValueModel.VerifyBitRegex1 });
        static readonly public ValueFormat WordFormat = new ValueFormat(
            "Word", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyWordRegex1 });

        static readonly private string[] selectedhandlecodes = {
            string.Format("0x01({0})",Properties.Resources.Read_Bit), string.Format("0x02({0})",Properties.Resources.Read_Bit),
            string.Format("0x03({0})",Properties.Resources.Read_Word), string.Format("0x04({0})",Properties.Resources.Read_Word),
            string.Format("0x05({0})",Properties.Resources.Write_Bit), string.Format("0x06({0})",Properties.Resources.Write_Word),
            string.Format("0x0f({0})",Properties.Resources.Write_Bits),string.Format("0x10({0})",Properties.Resources.Write_Words) };
        public IEnumerable<string> SelectedHandleCodes()
        {
            return selectedhandlecodes;
        }

        #endregion

        public ModbusItem(ModbusModel _parent)
        {
            parent = _parent;
        }

        public void Dispose()
        {
            PropertyChanged = null;
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ModbusModel parent;
        public ModbusModel Parent { get { return this.parent; } }
        IModel IModel.Parent { get { return Parent; } }

        private string slaveid = String.Empty;
        private string handlecode = String.Empty;
        private string slaveregister = String.Empty;
        private string slavecount = String.Empty;
        private string masteregister = String.Empty;
        
        public string SlaveID
        {
            get
            {
                return this.slaveid;
            }
            set
            {
                this.slaveid = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SlaveID"));
            }
        }
        public string HandleCode
        {
            get
            {
                return this.handlecode;
            }
            set
            {
                this.handlecode = value;
                PropertyChanged(this, new PropertyChangedEventArgs("HandleCode"));
            }
        }
        public string SlaveRegister
        {
            get
            {
                return this.slaveregister;
            }
            set
            {
                this.slaveregister = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SlaveRegister"));
            }
        }
        public string SlaveCount
        {
            get
            {
                return this.slavecount;
            }
            set
            {
                this.slavecount = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SlaveCount"));
            }
        }
        public string MasteRegister
        {
            get
            {
                return this.masteregister;
            }
            set
            {
                this.masteregister = value;
                PropertyChanged(this, new PropertyChangedEventArgs("MasteRegister"));
            }
        }

        #endregion
        
        #region Shell

        public IViewModel View { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        
        private ProjectTreeViewItem ptvitem;
        public ProjectTreeViewItem PTVItem
        {
            get { return this.ptvitem; }
            set { this.ptvitem = value; }
        }

        #endregion

        #region Check

        public bool SlaveID_IsValid
        {
            get
            {
                try
                {
                    int slaveid = int.Parse(SlaveID);
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
        }

        public bool HandleCode_IsValid
        {
            get
            {
                return HandleCode.Length > 0;
            }
        }

        public bool SlaveRegister_IsValid
        {
            get
            {
                try
                {
                    int slaveregister = int.Parse(SlaveRegister);
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
        }

        public bool SlaveCount_IsValid
        {
            get
            {
                try
                {
                    int slavecount = int.Parse(SlaveCount);
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
        }

        public bool MasterRegister_IsValid
        {
            get
            {
                try
                {
                    bool check1 = WordFormat.Match(MasteRegister).Success;
                    bool check2 = BitFormat.Match(MasteRegister).Success;
                    if (HandleCode.Equals(selectedhandlecodes[0])
                     || HandleCode.Equals(selectedhandlecodes[1])
                     || HandleCode.Equals(selectedhandlecodes[4])
                     || HandleCode.Equals(selectedhandlecodes[6]))
                    {
                        if (!check2)
                        {
                            throw new ValueParseException(Properties.Resources.Message_Bit_Required, BitFormat);
                        }
                    }
                    else if (HandleCode.Equals(selectedhandlecodes[2])
                          || HandleCode.Equals(selectedhandlecodes[3])
                          || HandleCode.Equals(selectedhandlecodes[5])
                          || HandleCode.Equals(selectedhandlecodes[7]))
                    {
                        if (!check1)
                        {
                            throw new ValueParseException(Properties.Resources.Message_Word_Requried, WordFormat);
                        }
                    }
                    else
                        return false;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsValid
        {
            get
            {
                return SlaveID_IsValid
                    && HandleCode_IsValid
                    && SlaveRegister_IsValid
                    && SlaveCount_IsValid
                    && MasterRegister_IsValid;
            }
        }

        #endregion
        
        #region Save & Load

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("SlaveID", SlaveID);
            xele.SetAttributeValue("HandleCode", HandleCode);
            xele.SetAttributeValue("SlaveRegister", SlaveRegister);
            xele.SetAttributeValue("SlaveCount", SlaveCount);
            xele.SetAttributeValue("MasteRegister", MasteRegister);
        }

        public void Load(XElement xele)
        {
            SlaveID = xele.Attribute("SlaveID").Value;
            HandleCode = xele.Attribute("HandleCode").Value;
            SlaveRegister = xele.Attribute("SlaveRegister").Value;
            SlaveCount = xele.Attribute("SlaveCount").Value;
            MasteRegister = xele.Attribute("MasteRegister").Value;
        }

        #endregion
    }
}
