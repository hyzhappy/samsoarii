using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamSoarII.AppMain.UI
{
    public class KeyInputHelper
    {
        public static bool CanInputAssert(Key input)
        {
            return (input >= Key.D0 && input <= Key.D9) || (input >= Key.NumPad0 && input <= Key.NumPad9) || input == Key.Delete || input == Key.Back || input == Key.Enter || input == Key.Left || input == Key.Right;
        }
        public static bool NumAssert(Key input)
        {
            return (input >= Key.D0 && input <= Key.D9) || (input >= Key.NumPad0 && input <= Key.NumPad9);
        }
        public static int GetKeyValue(Key input)
        {
            switch (input)
            {
                case Key.D0:
                case Key.NumPad0:
                    return 0;
                case Key.D1:
                case Key.NumPad1:
                    return 1;
                case Key.D2:
                case Key.NumPad2:
                    return 2;
                case Key.D3:
                case Key.NumPad3:
                    return 3;
                case Key.D4:
                case Key.NumPad4:
                    return 4;
                case Key.D5:
                case Key.NumPad5:
                    return 5;
                case Key.D6:
                case Key.NumPad6:
                    return 6;
                case Key.D7:
                case Key.NumPad7:
                    return 7;
                case Key.D8:
                case Key.NumPad8:
                    return 8;
                case Key.D9:
                case Key.NumPad9:
                    return 9;
                default:
                    return -1;
            }
        }
    }
}
