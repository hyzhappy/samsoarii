using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{

    public static class VariableManager
    {
        public static LinkedList<IVariableValue> VariableCollection;

       // public static VariableChangedEventHandler VariableChanged = delegate { };

        public static void Initialize()
        {
            VariableCollection = new LinkedList<IVariableValue>();
        }

        public static IVariableValue GetVariableByName(string name)
        {
            return VariableCollection.First(x => x.VarName == name);
        }

        public static bool ContainVariable(string name)
        {
            return VariableCollection.Any(x => x.VarName == name);
        }

        public static void AddVariable(IVariableValue variable)
        {
            VariableCollection.AddLast(variable);
          //  VariableChanged.Invoke(new VariableChangedEventArgs(ValueChangedType.Add, variable));
        }

        public static void RemoveVariable(string name)
        {
            try
            {
                var variable = GetVariableByName(name);
                VariableCollection.Remove(variable);
               // VariableChanged.Invoke(new VariableChangedEventArgs(ValueChangedType.Remove, variable));
            }
            catch
            {

            }
        }

        public static void RemoveVariable(IVariableValue variable)
        {
            VariableCollection.Remove(variable);
        }

        public static void ReplactVarialbe(IVariableValue oldvar, IVariableValue newvar)
        {
            var node = VariableCollection.Find(oldvar);
            if(node != null)
            {
                VariableCollection.AddAfter(node, newvar);
                VariableCollection.Remove(node);
            }
        }

        public static void Clear()
        {
            VariableCollection.Clear();
           // VariableChanged.Invoke(new VariableChangedEventArgs(ValueChangedType.Clear, null));
        }
    }
}
