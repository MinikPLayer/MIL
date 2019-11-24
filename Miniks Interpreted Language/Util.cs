using System;
using System.Collections.Generic;
using System.Text;

namespace MIL
{
    public static class MUtil
    {
        public static string GetStringToSpecialChar(string str, char specialChar)
        {
            string value = "";

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == specialChar) return value;
                value += str[i];
            }

            return value;
        }

        /// <summary>
        /// Returns string from special character ( without it )
        /// </summary>
        /// <param name="str"></param>
        /// <param name="specialChar"></param>
        /// <param name="strToSpecialChar"></param>
        /// <returns></returns>
        public static string GetStringToSpecialCharAndDelete(string str, char specialChar, out string strToSpecialChar)
        {
            strToSpecialChar = GetStringToSpecialChar(str, specialChar);

            if(strToSpecialChar.Length >= str.Length)
            {
                return "";
            }
            return str.Remove(0, strToSpecialChar.Length + 1);
        }

        public static bool AskUserYesNo(string action = "do this")
        {
            ConsoleColor orColor = Console.ForegroundColor;

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Are you sure you want to " + action + "? Y - yes, N - no");
                Console.ForegroundColor = orColor;

                ConsoleKeyInfo info = Console.ReadKey();
                if(info.Key == ConsoleKey.Y)
                {
                    return true;
                }
                if(info.Key == ConsoleKey.N)
                {
                    return false;
                }
            }


            return false;

        }

        public static List<string> RemoveEmptyLines(List<string> lines, bool removeAlsoNLandCR = true)
        {
            

            //List<string> linesList = new List<string>(lines);
            for(int i = 0;i< lines.Count;i++)
            {
                if(lines[i].Length == 0)
                {
                    lines.RemoveAt(i);
                    i--;
                    continue;
                }
                if(removeAlsoNLandCR)
                {
                    if(lines[i] == "\n" || lines[i] == "\r")
                    {
                        lines.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }

            

            return lines;
        }

        public static string[] RemoveEmptyLines(string[] lines)
        {
            string[] lns = RemoveEmptyLines(new List<string>(lines)).ToArray();
            return lns;
        }

        public static string[] StringToStringArray(string input)
        {
            List<string> array = new List<string>();
            string actualLine = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\n' || input[i] == '\r')
                {
                    array.Add(actualLine);


                    actualLine = "";
                    continue;
                }

                actualLine += input[i];
            }

            if (actualLine.Length != 0)
            {
                array.Add(actualLine);
            }

            return array.ToArray();
        }

        public static string ByteArrayToString(byte[] array)
        {
            return Convert.ToBase64String(array);
        }

        public static byte[] StringToByteArray(string input)
        {
            return Convert.FromBase64String(input);
        }


        public static bool lastParseError = false;
        public static T Parse<T>(string str, bool exception = false, bool fatalError = false)
        {
            lastParseError = false;

            Type tp = typeof(T);


            string sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            str = str.Replace(",", sep).Replace(".", sep);

            try
            {
                switch (tp.Name)
                {
                    // Int
                    case "Int32":
                        return (T)Convert.ChangeType(Int32.Parse(str), typeof(T));

                    // Unsigned int
                    case "UInt32":
                        return (T)Convert.ChangeType(UInt32.Parse(str), typeof(T));

                    // Float
                    case "Single":
                        return (T)Convert.ChangeType(Single.Parse(str), typeof(T));

                    // Double
                    case "Double":
                        return (T)Convert.ChangeType(Double.Parse(str), typeof(T));

                    // Long
                    case "Int64":
                        return (T)Convert.ChangeType(Int64.Parse(str), typeof(T));

                    // Unsigned Long
                    case "UInt64":
                        return (T)Convert.ChangeType(UInt64.Parse(str), typeof(T));

                    // Decimal
                    case "Decimal":
                        return (T)Convert.ChangeType(Decimal.Parse(str), typeof(T));

                    // Bool
                    case "Boolean":
                        return (T)Convert.ChangeType(Boolean.Parse(str), typeof(T));

                    // Byte
                    case "Byte":
                        return (T)Convert.ChangeType(Byte.Parse(str), typeof(T));

                    // Signed Byte
                    case "SByte":
                        return (T)Convert.ChangeType(SByte.Parse(str), typeof(T));

                    // Short
                    case "Int16":
                        return (T)Convert.ChangeType(Int16.Parse(str), typeof(T));

                    // Unsigned Short
                    case "UInt16":
                        return (T)Convert.ChangeType(UInt16.Parse(str), typeof(T));

                    // Char
                    case "Char":
                        return (T)Convert.ChangeType(Char.Parse(str), typeof(T));

                    default:
                        Debug.LogError("Type " + tp.Name + " not supported");
                        break;
                }
                //return int.Parse(str);
            }
            catch(Exception e)
            {
                lastParseError = true;

                if(exception)
                {
                    throw e;
                }

                Debug.Exception(e);
                if(fatalError)
                {
                    Debug.FatalError("Fatal exception occured during conversion, exiting", -1999, 5000);
                }
            }

            return default(T);
        }
    }

    public class ParsingExcetpion : Exception
    {

        public string src = "";
        public object dst;

        public ParsingExcetpion(string _src, object _dst)
            : base("Cannot convert \"" + _src + "\" to " + _dst.GetType().Name)
        {
            src = _src;
            dst = _dst;
        }
    }
}
