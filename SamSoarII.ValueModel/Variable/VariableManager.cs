using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{

    public static class VariableManager
    {
        public static LinkedList<IVariableValue> VariableCollection;

        public static event VariableChangedEventHandler VariableChanged = delegate { };

        public static void Initialize()
        {
            VariableCollection = new LinkedList<IVariableValue>();
        }

        public static IVariableValue GetVariableByName(string name)
        {
            return VariableCollection.First(x => x.VarName == name);
        }
        public static IVariableValue GetVariableByValueString(string valueString)
        {
            return VariableCollection.First(x => { return x.MappedValue.ValueString == valueString; });
        }
        public static bool ContainVariable(string name)
        {
            return VariableCollection.Any(x => x.VarName == name);
        }
        public static bool ContainVariableValueString(string valueString)
        {
            return VariableCollection.Any(x => { return x.MappedValue.ValueString == valueString; });
        }
        public static void AddVariable(IVariableValue variable)
        {
            VariableCollection.AddLast(variable);
            VariableChanged.Invoke(new VariableChangedEventArgs(ValueChangedType.Add, variable));
        }
        public static void RemoveVariable(string name)
        {
            try
            {
                var variable = GetVariableByName(name);
                VariableCollection.Remove(variable);
                VariableChanged.Invoke(new VariableChangedEventArgs(ValueChangedType.Remove, variable));
            }
            catch
            {

            }
        }
        public static void RemoveVariableByValueString(string valueString)
        {
            var variable = GetVariableByValueString(valueString);
            VariableCollection.Remove(variable);
            VariableChanged.Invoke(new VariableChangedEventArgs(ValueChangedType.Remove, variable));
        }
        public static void RemoveVariable(IVariableValue variable)
        {
            VariableCollection.Remove(variable);
            VariableChanged.Invoke(new VariableChangedEventArgs(ValueChangedType.Remove, variable));
        }

        public static void ReplactVarialbe(IVariableValue oldvar, IVariableValue newvar)
        {
            var node = VariableCollection.Find(oldvar);
            if(node != null)
            {
                VariableCollection.AddAfter(node, newvar);
                VariableChanged.Invoke(new VariableChangedEventArgs(ValueChangedType.Add, newvar));
                VariableCollection.Remove(node);
                VariableChanged.Invoke(new VariableChangedEventArgs(ValueChangedType.Remove, node.Value));
            }
        }

        public static void Clear()
        {
            VariableCollection.Clear();
            VariableChanged.Invoke(new VariableChangedEventArgs(ValueChangedType.Clear, null));
        }
    }
}
