using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
namespace SamSoarII.AppMain
{
    public static class GlobalVariableList
    {

        public static ObservableCollection<LadderVariable> VariableCollection = new ObservableCollection<LadderVariable>();

        public static LadderVariable GetVariableByName(string name)
        {
            return VariableCollection.First(x => x.Name == name);
        }

        public static bool ContainVariable(string name)
        {
            return VariableCollection.Any(x => x.Name == name);
        }

        public static void AddVariable(LadderVariable variable)
        {
            VariableCollection.Add(variable);
        }

        public static void AddVariable(string name, IValueModel value)
        {
            VariableCollection.Add(new LadderVariable(name, value));
        }

        public static void AddVariable(string name, IValueModel value, string comment)
        {
            VariableCollection.Add(new LadderVariable(name, value, comment));
        }

        public static void RemoveVariable(LadderVariable variable)
        {
            VariableCollection.Remove(variable);
        }

        public static void ModifyVariable(string name, string newname, IValueModel value)
        {
            try
            {
                var variable = GetVariableByName(name);
                variable.Name = newname;
                variable.ValueModel = value;
            }
            catch
            {

            }
        }

        public static void ModifyVariable(LadderVariable variable, string name, IValueModel value)
        {
            variable.Name = name;
            variable.ValueModel = value;
        }

        public static void ModifyVariable(string name, IValueModel value)
        {
            try
            {
                var variable = GetVariableByName(name);
                variable.ValueModel = value;
            }
            catch
            {

            }
        }
        public static void ModifyVariable(string name, IValueModel value, string comment)
        {
            try
            {
                var variable = GetVariableByName(name);
                variable.ValueModel = value;
                variable.Comment = comment;
            }
            catch
            {

            }
        }
        public static void ModifyVariable(string name, string newname, IValueModel value, string comment)
        {
            try
            {
                var variable = GetVariableByName(name);
                variable.Name = newname;
                variable.ValueModel = value;
                variable.Comment = comment;
            }
            catch
            {

            }
        }

        public static void Clear()
        {
            VariableCollection.Clear();
        }

        static GlobalVariableList()
        {

        }      
    }
}
