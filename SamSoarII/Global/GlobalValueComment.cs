using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;

namespace SamSoarII.AppMain
{
    public static class GlobalValueComment
    {
        public static Dictionary<string, string> ValueCommentDict = new Dictionary<string, string>();

        static GlobalValueComment()
        {

        }

        public static bool ContainValue(IValueModel valueModel)
        {
            return ValueCommentDict.ContainsKey(valueModel.ToString());
        } 

        public static void ModifyComment(IValueModel valueModel, string comment)
        {
            if(ContainValue(valueModel))
            {
                ValueCommentDict[valueModel.ToString()] = comment;
            }
            else
            {
                ValueCommentDict.Add(valueModel.ToString(), comment);
            }
        }

        public static string GetComment(IValueModel valueModel)
        {
            if(valueModel != null)
            {
                if (ContainValue(valueModel))
                {
                    return ValueCommentDict[valueModel.ToString()];
                }
            }
            return string.Empty;
        }

    }
}
