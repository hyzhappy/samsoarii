using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
using System.ComponentModel;

namespace SamSoarII.AppMain
{
    public class LadderVariable : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private IValueModel _valueModel;
        public IValueModel ValueModel
        {
            get
            {
                return _valueModel;
            }
            set
            {
                _valueModel = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ValueModel"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ValueType"));
            }
        }

        public LadderValueType ValueType { get { return ValueModel.Type; } }

        private string _comment;
        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Comment"));
            }
        }

        public LadderVariable(string name, IValueModel value)
        {
            Name = name;
            ValueModel = value;
        }
        public LadderVariable(string name, IValueModel value, string comment)
        {
            Name = name;
            ValueModel = value;
            Comment = comment;
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { }; 
    }
}
