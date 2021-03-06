﻿using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.UI
{
    public static class CSVFileHelper
    {
        private static string DQuotation = string.Format("\"\"");
        private static string Quotation = string.Format("\"");
        private static string SBCDQuotation = ToSBC(DQuotation);
        private static string SBCQuotation = ToSBC(Quotation);
        public static void ImportExcute(string filename, IEnumerable<ValueCommentAlias> elementCollection, string separator)
        {
            StreamReader stream = new StreamReader(File.OpenRead(filename));
            string line;
            while (!stream.EndOfStream)
            {
                line = stream.ReadLine();
                string[] eles = line.Split(new string[] {separator },StringSplitOptions.None);
                int last, first = line.IndexOf(separator);
                string name = line.Substring(0, first);
                string alias, comment;
                if (QuotationOperation(ref line, out comment, out alias, out last, separator))
                {
                    if (elementCollection.ToList().Exists(x => { return x.Name == name; }))
                    {
                        var valueCommentAlias = elementCollection.Where(x => { return x.Name == name; }).First();
                        if (valueCommentAlias.Comment != comment)
                        {
                            if (comment == string.Empty)
                            {
                                valueCommentAlias.HasComment = false;
                            }
                            else
                            {
                                valueCommentAlias.HasComment = true;
                            }
                            ValueCommentManager.UpdateComment(name, comment);
                        }
                        if (valueCommentAlias.Alias != alias && !elementCollection.ToList().Exists(x => { return x.Name != name && x.Alias == alias && alias != string.Empty; }))
                        {
                            if (alias == string.Empty)
                            {
                                valueCommentAlias.HasAlias = false;
                            }
                            else
                            {
                                valueCommentAlias.HasAlias = true;
                            }
                            ValueAliasManager.UpdateAlias(name, alias);
                        }
                    }
                }
            }
            stream.Close();
        }
        public static void ExportExcute(string filename, IEnumerable<ValueCommentAlias> elementCollection, string separator)
        {
            StreamWriter stream = new StreamWriter(File.OpenWrite(filename));
            foreach (var item in elementCollection)
            {
                string comment = item.Comment;
                string alias = item.Alias;
                if (comment.Contains(Quotation) || comment.Contains(separator))
                {
                    comment = comment.Replace(Quotation,DQuotation).Insert(0,Quotation);
                    comment = comment.Insert(comment.Length,Quotation);
                }
                if (alias.Contains(Quotation) || alias.Contains(separator))
                {
                    alias = alias.Replace(Quotation, DQuotation).Insert(0, Quotation);
                    alias = alias.Insert(alias.Length, Quotation);
                }
                stream.WriteLine(string.Format("{0}{3}{1}{3}{2}",item.Name, comment, alias, separator));
            }
            stream.Close();
        }
        private static bool QuotationOperation(ref string line,out string comment,out string alias,out int last,string separator)
        {
            int first = line.IndexOf(separator);
            GetNextIndexSeparator(line,line.Length,out alias,out last,separator);
            int temp;
            string tempstr;
            GetNextIndexSeparator(line,last,out tempstr,out temp,separator);
            comment = line.Substring(first + 1, last - first - 1);
            if (comment.Contains(Quotation))
            {
                comment = comment.Remove(0,1);
                comment = comment.Remove(comment.Length - 1,1);
            }
            if (alias.Contains(Quotation))
            {
                alias = alias.Remove(0, 1);
                alias = alias.Remove(alias.Length - 1, 1);
            }
            comment = comment.Replace(DQuotation,Quotation);
            alias = alias.Replace(DQuotation,Quotation);
            if (temp != first)
            {
                return false;
            }
            return true;
        }
        public static string ToSBC(string input)
        {
            char[] chars = input.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == 32)
                {
                    chars[i] = (char)12288;
                    continue;
                }
                if (chars[i] < 127 && chars[i] > 32)
                {
                    chars[i] = (char)(chars[i] + 65248);
                }
            }
            return new string(chars);
        }
        private static void GetNextIndexSeparator(string line,int count,out string value,out int index,string separator)
        {
            index = line.Substring(0, count).LastIndexOf(separator);
            if (index == line.Length - 1)
            {
                value = string.Empty;
            }
            else
            {
                value = line.Substring(index + 1);
                string temp = value.Replace(Quotation, string.Empty);
                while ((value.Count() - temp.Count()) % 2 != 0)
                {
                    index = line.Substring(0, index).LastIndexOf(separator);
                    value = line.Substring(index + 1);
                    temp = value.Replace(Quotation, string.Empty);
                }
            }
        }
    }
}
