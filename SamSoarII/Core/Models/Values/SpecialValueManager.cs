using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class SpecialValue
    {
        public string Name { get; private set; }
        public string Base { get; private set; }
        public int Offset { get; private set; }
        public string Type { get; private set; }
        public string NickName { get; private set; }
        public string Describe { get; private set; }
        public bool CanRead { get; private set; }
        public bool CanWrite { get; private set; }

        public SpecialValue(XElement xele)
        {
            Name = xele.Attribute("Name").Value;
            Base = xele.Attribute("Base").Value;
            Offset = int.Parse(xele.Attribute("Offset").Value);
            Type = xele.Attribute("Type").Value;
            NickName = xele.Attribute("NickName").Value;
            Describe = xele.Attribute("Describe").Value;
            CanRead = bool.Parse(xele.Attribute("CanRead").Value);
            CanWrite = bool.Parse(xele.Attribute("CanWrite").Value);
        }
    }

    public class SpecialValueManager
    {
        static private List<SpecialValue> svalues;

        static public void Initialize()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("SamSoarII.Resources.SpecialRegisters.xml");
            XDocument xDoc = XDocument.Load(stream);
            XElement rootNode = xDoc.Root;
            IEnumerable<XElement> nodes = rootNode.Elements();
            svalues = nodes.Select(
                (XElement xele) => { return new SpecialValue(xele); }).ToList();
        }

        static public IEnumerable<SpecialValue> Values
        {
            get { return svalues; }
        }

        static public SpecialValue ValueOfName(string name)
        {
            IEnumerable<SpecialValue> fit = svalues.Where(
                (SpecialValue svalue) => { return svalue.Name.Equals(name); });
            return fit.FirstOrDefault();
        }

        static public IEnumerable<SpecialValue> ValueOfProfix(string profix)
        {
            IEnumerable<SpecialValue> fit = svalues.Where(
                (SpecialValue svalue) => { return svalue.NickName.StartsWith(profix); });
            return fit;
        }
    }
}