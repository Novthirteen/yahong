using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace com.Sconit.Utility
{
    public static class StringHelper
    {
        /// <summary>
        /// "Code [Description]"
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static string GetCodeDescriptionString(string code, string description)
        {
            if (code == null || code.Trim() == string.Empty)
                code = string.Empty;
            if (description == null || description.Trim() == string.Empty)
                description = string.Empty;

            if (description == string.Empty)
                return code;
            else
                return code + " [" + description + "]";
        }

        public static string SubStr(string sString, int nLeng)
        {
            int totalLength = 0;
            int currentIndex = 0;
            while (totalLength < nLeng && currentIndex < sString.Length)
            {
                if (sString[currentIndex] < 0 || sString[currentIndex] > 255)
                    totalLength += 2;
                else
                    totalLength++;

                currentIndex++;
            }

            if (currentIndex < sString.Length)
                return sString.Substring(0, currentIndex) + "...";
            else
                return sString.ToString();
        }

        /// <summary>
        /// Sconit Common String Comparer, ignore case, support Null
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Eq(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

    
     public static string NumberCn(decimal ANumber)
        {

            const string cPointCn = "点十百千万十百千亿十百千";

            const string cNumberCn = "零一二三四五六七八九";

            string S = ANumber.ToString();

            if (S == "0") return "" + cPointCn[0];

            if (!S.Contains(".")) S += ".";

            int P = S.IndexOf(".");

            string Result = "";

            for (int i = 0; i < S.Length; i++)
            {

                if (P == i)
                {

                    Result = Result.Replace("零十零", "零");

                    Result = Result.Replace("零百零", "零");

                    Result = Result.Replace("零千零", "零");

                    Result = Result.Replace("零十", "零");

                    Result = Result.Replace("零百", "零");

                    Result = Result.Replace("零千", "零");

                    Result = Result.Replace("零万", "万");

                    Result = Result.Replace("零亿", "亿");

                    Result = Result.Replace("亿万", "亿");

                    Result = Result.Replace("零点", "点");

                }

                else
                {

                    if (P > i)

                        Result += "" + cNumberCn[S[i] - '0'] + cPointCn[P - i - 1];

                    else Result += "" + cNumberCn[S[i] - '0'];

                }

            }

            if (Result.Substring(Result.Length - 1, 1) == "" + cPointCn[0])

                Result = Result.Remove(Result.Length - 1); // 一点-> 一

            if (Result[0] == cPointCn[0])

                Result = cNumberCn[0] + Result; // 点三-> 零点三

            if ((Result.Length > 1) && (Result[1] == cPointCn[1]) &&

                   (Result[0] == cNumberCn[1]))

                Result = Result.Remove(0, 1); // 一十三-> 十三

            return Result;

        }

        public static string MoneyCn(decimal? ANumber)
        {

            if (!ANumber.HasValue || ANumber.Value == 0) return "零";

            string Result = NumberCn(Math.Truncate(ANumber.Value * 100) / 100);

            Result = Result.Replace("一", "壹");

            Result = Result.Replace("二", "贰");

            Result = Result.Replace("三", "叁");

            Result = Result.Replace("四", "肆");

            Result = Result.Replace("五", "伍");

            Result = Result.Replace("六", "陆");

            Result = Result.Replace("七", "柒");

            Result = Result.Replace("八", "捌");

            Result = Result.Replace("九", "玖");

            Result = Result.Replace("九", "玖");

            Result = Result.Replace("十", "拾");

            Result = Result.Replace("百", "佰");

            Result = Result.Replace("千", "仟");

            if (Result.Contains("点"))
            {

                int P = Result.IndexOf("点");

                if (Result.Length >= P + 3)
                {
                    try
                    {
                        Result = Result.Insert(P + 3, "分");
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    }
                }

                Result = Result.Insert(P + 2, "角");

                Result = Result.Replace("点", "元");

                Result = Result.Replace("角分", "角");

                Result = Result.Replace("零分", "");

                Result = Result.Replace("零角", "");

                Result = Result.Replace("分角", "");

                if (Result.Substring(0, 2) == "零元")

                    Result = Result.Replace("零元", "");

            }
            else Result += "元整";

            //Result = "人民币" + Result;

            return Result;
        }


        public static string HtmlToTxt(string strHtml)
        {
            string[] aryReg ={
            @"<script[^>]*?>.*?</script>",
            @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
            @"([\r\n])[\s]+",
            @"&(quot|#34);",
            @"&(amp|#38);",
            @"&(lt|#60);",
            @"&(gt|#62);",
            @"&(nbsp|#160);",
            @"&(iexcl|#161);",
            @"&(cent|#162);",
            @"&(pound|#163);",
            @"&(copy|#169);",
            @"&#(\d+);",
            @"-->",
            @"<!--.*\n"
            };
            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, string.Empty);
            }
            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");
            if (strOutput == string.Empty)
            {
                if (strHtml.Contains("type=\"checkbox\""))
                {
                    if (strHtml.Contains("checked=\"checked\""))
                    {
                        strOutput = "是";
                    }
                    else
                    {
                        strOutput = "否";
                    }
                }
                else
                {
                    MatchCollection mc = Regex.Matches(strHtml, @"(?is)<input.*?(?:(?<! \w+=""[^""]*? *)name[^=]*=.*?(['""]?)(?<name>[^'"" ]*)\1|(?<! \w+=""[^""]*? *)value[^=]*=.*?(['""]?)(?<value>[^'""]*?)\2|.)+?>");
                    foreach (Match m in mc)
                    {
                        strOutput = m.Groups["value"].Value;
                        break;
                    }
                }
            }
            return strOutput;
        }

    }



}
