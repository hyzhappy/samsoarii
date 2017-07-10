using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SamSoarII.AppMain.Global
{
    public class GlobalThreeHotKey
    {
        private UIElement _source;
        private RoutedUICommand _command;
        private KeyPartTwo _keyPart;
        public GlobalThreeHotKey(UIElement source, RoutedUICommand command, KeyPartTwo keyPart)
        {
            _source = source;
            _command = command;
            _keyPart = keyPart;
        }
        public bool CanExecute
        {
            get
            {
                return _command.CanExecute(null,_source);
            }
        }
        public void Execute()
        {
            _command.Execute(null, _source);
        }
        public bool Assert(Key key)
        {
            return _keyPart.AssertPartTwo(key);
        }
        public override string ToString()
        {
            return _keyPart.ToString();
        }
    }
    public class KeyPartTwo:IComparable<KeyPartTwo>
    {
        private ModifierKeys _modifier;
        private Key _keyMain;
        private Key _keySub;
        public KeyPartTwo(ModifierKeys modifier,Key key1,Key key2)
        {
            _modifier = modifier;
            _keyMain = key1;
            _keySub = key2;
        }

        public int CompareTo(KeyPartTwo other)
        {
            if (_modifier != other._modifier) return _modifier - other._modifier;
            if (_keyMain != other._keyMain) return _keyMain - other._keyMain;
            if (_keySub != other._keySub) return _keySub - other._keySub;
            return 0;
        }

        public bool AssertPartOne(ModifierKeys modifier, Key key)
        {
            return _modifier == modifier && _keyMain == key;
        }
        public bool AssertPartTwo(Key key)
        {
            return _keySub == key;
        }
        public override string ToString()
        {
            return string.Format("(Ctrl+{0},",_keyMain);
        }
    }
    public class ThreeHotKeyManager
    {
        public static bool IsWaitForSecondModifier = false;
        public static bool IsWaitForSecondKey = false;
        private static Dictionary<KeyPartTwo, GlobalThreeHotKey> ThreeHotKeys;
        static ThreeHotKeyManager()
        {
            ThreeHotKeys = new Dictionary<KeyPartTwo, GlobalThreeHotKey>();
        }
        public static List<GlobalThreeHotKey> GetHotKeys(ModifierKeys modifier,Key key)
        {
            List<GlobalThreeHotKey> keys = new List<GlobalThreeHotKey>();
            foreach (var KVPair in ThreeHotKeys)
            {
                if (KVPair.Key.AssertPartOne(modifier, key)) keys.Add(KVPair.Value);
            }
            return keys;
        }
        public static void AddHotKey(KeyPartTwo key, GlobalThreeHotKey value)
        {
            ThreeHotKeys.Add(key, value);
        }
    }
}
