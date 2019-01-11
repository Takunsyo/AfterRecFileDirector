using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace RVMCore
{
    public static class MasterHelper
    {
         /// <summary>
        ///  A TV program witch runs on seasons will contain a start time and a end time.
        ///  </summary>
        public struct ProgramTimeSpan
        {
            public DateTime StartQuarterly;
            public DateTime EndQuarterly;
        }

        /// <summary>
        /// Get time span from a <see cref="String"/>
        /// </summary>
        /// <param name="span">a string looks like "[Q1'18,Q1'18]"</param>
        /// <returns>Returns a <see cref="ProgramTimeSpan"/> structure.</returns>
        public static ProgramTimeSpan GetTimeSpan(string span)
        {
            ProgramTimeSpan mResult = new ProgramTimeSpan();
            if (!span.Contains(","))
                return mResult;
            var mStr = span.Replace("[", "").Replace("]", "").ToUpper();
            var mSpan = Strings.Split(mStr, ",");
            if (mSpan.Count() != 2)
                return mResult;
            if (mSpan[0].Contains("'"))
            {
                var sQ = Strings.Split(mSpan[0], "'");
                if (sQ[0].Contains("Q"))
                {
                    if (!int.TryParse(sQ[0].Substring(1, 1), out var Quarterly))
                        return mResult;
                    if (!int.TryParse(sQ[1], out var Years))
                        return mResult;
                    Quarterly = Quarterly * 3 - 2;
                    Years += 2000;
                    var a = string.Format("{0}/01/{1}", Quarterly, Years);
                    mResult.StartQuarterly = DateTime.ParseExact(string.Format("{0}/01/{1}", Quarterly, Years), "d", new CultureInfo("en-US"));
                }
                else
                    return mResult;
            }
            if (mSpan[1].Contains("'"))
            {
                var sQ = Strings.Split(mSpan[1], "'");
                if (sQ[0].Contains("Q"))
                {
                    
                    if (!int.TryParse(sQ[0].Substring(1, 1), out var Quarterly))
                        return mResult;
                    if (!int.TryParse(sQ[1], out var Years))
                        return mResult;
                    Quarterly = Quarterly * 3;
                    Years += 2000;
                    mResult.EndQuarterly = DateTime.ParseExact(string.Format("{0}/30/{1}", Quarterly, Years), "d", new CultureInfo("en-US"));
                }
                else
                    return mResult;
            }
            return mResult;
        }

        /// <summary>
        /// Get time span from <see cref="Date"/>s to <see cref="String"/>
        /// </summary>
        /// <returns>a string looks like "[Q1'18,Q1'18]"</returns>
        public static string GetTimeSpan(DateTime start, DateTime over)
        {
            // StartDate
            var startQ = Conversion.Fix(start.Month / (double)3) + ((start.Month % 3) > 0 ? 1 : 0);
            var startY = start.Year - 2000;
            // EndDate
            var endQ = Conversion.Fix(over.Month / (double)3) + ((over.Month % 3) > 0 ? 1 : 0);
            var endY = start.Year - 2000;
            // result
            return string.Format("[Q{0}'{1},Q{2}'{3}]", startQ, startY, endQ, endY);
        }

        /// <summary>
        /// Get time span from <see cref="ProgramTimeSpan"/> to <see cref="String"/>
        /// </summary>
        /// <returns>a string looks like "[Q1'18,Q1'18]"</returns>
        public static string GetTimeSpan(this ProgramTimeSpan span)=>
            GetTimeSpan(span.StartQuarterly, span.EndQuarterly);

        /// <summary>
        /// Find the general title of the TV program.
        /// </summary>
        public static string FindTitle(string str)
        {
            str = str.CheckString(); // Make it more eazy to process.
            var SharpIndex = str.IndexOf("#");
            if (SharpIndex > 0)
                return str.Substring(0, SharpIndex).Trim().CheckAgain();// Convert to full-width char to prevent IO errors.
                                                                      // Find pattern'<第>[Number]<话，回>
            string pet = @"[\\p{N}第\(（][\\p{N}零一二三四五六七八九十百千萬億万亿壹貳叄肆伍陸柒捌玖拾佰仟贰叁陆\d]+[\\p{N}\)）話回].*";
            if (Regex.IsMatch(str, pet))
                return Regex.Replace(str, pet, "").Trim().CheckAgain();

                // Find pattern '<第>[Number]<All double byte cherator>
            pet = @"[第][\\p{N}零一二三四五六七八九十百千萬億万亿壹貳叄肆伍陸柒捌玖拾佰仟贰叁陆\d]+[\u4e00-\u9fa5].*";
            if (Regex.IsMatch(str, pet))
                return Regex.Replace(str, pet, "").Trim().CheckAgain();

            // if has no match then find something in a batch with a number.
            pet = @"[「][\\p{N}\s\S]+[\\p{N}零一二三四五六七八九十百千萬億万亿壹貳叄肆伍陸柒捌玖拾佰仟贰叁陆\d]+[」].*";
            if (Regex.IsMatch(str, pet))
                return Regex.Replace(str, pet, "").Trim().CheckAgain();

            // find a number follors with a word witch follows with a whilte space and ends with a dot.
            pet = @"[\\p{N}a-zA-Z]+[.][\\p{N}\uFF10-\uFF190-9]+.*";
            if (Regex.IsMatch(str, pet))
                return Regex.Replace(str, pet, "").Trim().CheckAgain();

            // find a number follows with a dot.
            pet = @"[\\p{N}\uFF10-\uFF190-9]+[.].*";
            if (Regex.IsMatch(str, pet))
                return Regex.Replace(str, pet, "").Trim().CheckAgain();

            // if still has no match then just find somthing in a batch.
            SharpIndex = str.LastIndexOf("「");
            SharpIndex = SharpIndex > 0 ? SharpIndex : str.LastIndexOf("｢");
            if (SharpIndex > 0)
                return str.Substring(0, SharpIndex).Trim().CheckAgain();

            // if has no match then just find a number in single byte.
            pet = @"[\uFF10-\uFF190-9]+.*";
            if (Regex.IsMatch(str, pet))
                return Regex.Replace(str, pet, "").Trim().CheckAgain();

            // if even a fuckin batch can't been found. just find a Number.
            // Ok, so find numbers in double bytes cherators. This may cause some problem.
            // pet = "[零一二三四五六七八九十百千萬億万亿壹貳叄肆伍陸柒捌玖拾佰仟贰叁陆]"
            return str.Trim().CheckAgain();
        }

        private static string CheckString(this string input)
        {
            string str = input.Replace("＃", "#");
            str = Regex.Replace(str, @"[\<|\>|\|\\\/:]", x => { return Strings.StrConv(x.Value, VbStrConv.Wide); });
            return Regex.Replace(str, @"(?<=[aA-zZ0-9])\s(?=[aA-zZ0-9])", " ");
        }

        private static string CheckAgain(this string input)
        {
            string str = input;
            str = Regex.Replace(str, @"(^(\[[^\x00-\xff]\])+)|((\[[^\x00-\xff]\])+$)", "");
            str = Regex.Replace(str, @"(^(★|☆|▲|▼|▽|△|●|〇|◎)+)|((★|☆|▲|▼|▽|△|●|〇|◎)+$)", "");
            return str.Trim();
        }

        /// <summary>
        /// Rename folder, change it's time stamp up to date.
        /// </summary>
        public static bool RenameDirUpToDate(ref string dirPath, DateTime newDate)
        {
            if (!System.IO.Directory.Exists(dirPath))
                return false;
            int i = dirPath.LastIndexOf(@"\") + 1;
            string dirName = dirPath.Substring(i);
            i = dirName.IndexOf("[") + 1;
            int i2 = dirName.IndexOf("]");
            if (i2 <= 0)
                return false;
            var OldDate = GetTimeSpan(dirName.Substring(i, i2));
            if (DateTime.Compare(OldDate.EndQuarterly, newDate) < 0)
            {
                OldDate.EndQuarterly = newDate;
                dirName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(dirPath), OldDate.GetTimeSpan() + dirName.Substring(i2 + 1));
                try
                {
                    System.IO.Directory.Move(dirPath, dirName);
                    dirPath = dirName;
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }

        /// <summary>
        /// A just for fun method get fibonacci sequence.
        /// </summary>
        public static IEnumerable<ulong> GetFibonacciSequence()
        {
            ulong x = 0 , y =1;
            while (true)
            {                
                yield return y;
                var tmp = y;
                try{
                    y += x;
                }catch{//Overflow.
                    break;}
                x = tmp;
            }
        }

        /// <summary>
        /// Foreach extension method for <see cref="IEnumerable{T}"/> object using iterator.
        /// <para>*Faster then foreach statement in certen instance.</para>
        /// </summary>
        /// <typeparam name="T">Input collection <see cref="Type>"/></typeparam>
        /// <typeparam name="TResult">Return collection <see cref="Type"/></typeparam>
        /// <param name="input">Input collection.</param>
        /// <param name="func">Delegation for the foreach statement.</param>
        /// <returns>A collection processed by <see cref="Func{T, TResult}"/> paramater in a foreach style.</returns>
        public static IEnumerable<TResult> ForEach<T, TResult>(this IEnumerable<T> input, Func<T, TResult> func)
        {
            if (input is null || func is null) yield break;
            foreach (T item in input) yield return func(item);
        }

        /// <summary>
        /// Foreach extension method for <see cref="IEnumerable{T}"/> object using iterator.
        /// <para>*Faster then foreach statement in certen instance.</para>
        /// </summary>
        /// <typeparam name="T">Collection <see cref="Type"/>.</typeparam>
        /// <param name="input">Input collection.</param>
        /// <param name="action">Delegation for the foreach statement.</param>
        /// <returns>A collection process by <see cref="Action{T}"/> paramater in a foreach style.</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> input, Action<T> action)
        {
            if (input is null || action is null) yield break;
            foreach (T item in input)
            {
                action(item);
                yield return item;
            }
        }

        /// <summary>
        /// Append an array after another array.
        /// </summary>
        /// <typeparam name="T">Array collection <see cref="Type"/>.</typeparam>
        /// <param name="source">Base array, index from 0.</param>
        /// <param name="array">Second array, index after base array's last item.</param>
        /// <returns>A full array.</returns>
        public static T[] AppendArray<T>(this T[] source, T[] array)
        {
            if (source == null & array == null) return null;
            if (source == null) return array;
            if (array == null) return source;
            int ltmp = source.Length;
            var tmp = source;
            Array.Resize(ref tmp, ltmp + array.Length);
            Array.Copy(array, 0, tmp, ltmp, array.Length);
            return tmp;
        }
        /// <summary>
        /// Append an array after another array.
        /// </summary>
        /// <typeparam name="T">Array collection <see cref="Type"/>.</typeparam>
        /// <param name="source">Base array, index from 0.</param>
        /// <param name="array">Second array, index after base array's last item.</param>
        /// <returns>A full array.</returns>
        //public static void AppendArray2<T>(this ref T[] source, T[] array)
        //{
        //    if (source == null & array == null) return;
        //    if (source == null)
        //    {
        //        source = array;
        //        return;
        //    }

        //    if (array == null) return;
        //    int ltmp = source.Length;
        //    var tmp = source;
        //    Array.Resize(ref tmp, ltmp + array.Length);
        //    Array.Copy(array, 0, tmp, ltmp, array.Length);
        //    source = tmp;
        //}
    }
}
