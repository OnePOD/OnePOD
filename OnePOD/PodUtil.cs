using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OnePOD
{
    class PodUtil
    {
        public static string POD_TITLE = "OnePOD";
        public static string POD_VERSION = "beta150811";
        public static string POD_CONTACT = "click OnePod.me, or email onepod@onepod.me";

        public static void Error(string errMsg)
        {
            MessageBox.Show(errMsg, POD_TITLE);
        }

        internal static void CreateDirIfNotAvailable(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        internal static void CreateEmptyFileIfNotAvailable(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
        }

        internal static bool IsAllWhiteSpace(string s)
        {
            char[] ca = s.ToCharArray();
            foreach (char c in ca)
            {
                if (char.IsLetterOrDigit(c))
                    return false;
            }
            return true;
        }

        internal static string RemoveConsecutiveWhiteSpaces(string s)
        {
            char[] ca = s.ToCharArray();
            StringBuilder sb = new StringBuilder();
            char c;
            int i = 0;
            while(i < ca.Length)
            {
                c = ca[i];
                if (char.IsWhiteSpace(c) && i+1 < ca.Length && char.IsWhiteSpace(ca[i+1]))
                {   // find the next char that is NOT a white 
                    bool nextLineFound = c == '\n';
                    i++;
                    while (i < ca.Length && char.IsWhiteSpace(ca[i]))
                    {
                        if (!nextLineFound && ca[i] == '\n')
                            nextLineFound = true;
                        i++; 
                    }
                    if (nextLineFound)
                        sb.Append("\n\n");
                    else
                        sb.Append(" ");
                }
                if(i<ca.Length)
                    sb.Append(ca[i]);
                i++;
            }
            return sb.ToString();
        }

        internal static string RemoveSubstring(string s, string sub)
        {
            int start = s.IndexOf(sub);
            string result = s.Remove(start, sub.Length);
            return result;
        }

        internal static string ProcessGarbledChar(string s)
        {
            char[] ca = s.ToCharArray();
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            int i = 0;
            while(i < ca.Length)
            {
                char c = ca[i];
                if (c > 127)
                {
                    if (c == 37413) // combination
                    {
                        char nextc = ca[i + 1];
                        if      (nextc == 28141) { sb.Append("\"T"); i = i + 2; }
                        else if (nextc == 28200) { sb.Append("\"w"); i = i + 2; }
                        else if (nextc == 27290) { sb.Append("'s"); i = i + 2; }
                        else if (nextc == 25878) { sb.Append(" - t"); i = i + 2; }
                        else if (nextc == 63)
                        {
                            char next2c = ca[i + 2]; char next3c = ca[i + 3]; char next4c = ca[i + 4];
                            if (next2c == '/' && next3c == 'p' && next4c == '>') // ending tag </p> is ruied
                            {
                                sb.Append("\"");
                                i = i + 5;
                            }
                            else
                            {
                                sb.Append("\" ");
                                i = i + 2;
                            }
                        }
                        else if (garbledCharMap.ContainsKey(nextc))
                        {
                            string gs = garbledCharMap[nextc];
                            //if (gc == '-')
                            //{ // break here for debug
                            //}
                            sb.Append("\"" + gs);
                            i = i + 2;
                        }
                        else // a new char not in the map
                        {
                            sb.Append("\"");
                            i++;
                        }
                    }
                    else // single
                    {
                        if (garbledCharMap.ContainsKey(c))
                        {
                            sb.Append(garbledCharMap[c]);
                        }
                        else // a new char not in the map
                        {
                            sb.Append(c);
                        }
                        i++;
                    }
                }
                else // normal ascii
                {
                    sb.Append(c);
                    i++;
                }
            }
            //MessageBox.Show(sb2.ToString());
            return sb.ToString();
        }
        //private static Dictionary<int, List<int>> garbledCharCombinationMap = new Dictionary<int,List<int>>();
        private static Dictionary<int, string> garbledCharMap = new Dictionary<int, string> { 
            //{63, " "},
            //{25878, '-'},
            //{27290, "s"},
            {28122, "I"},
            {35881, "a"},
            {31108, ">>"}
        };

    }
}
