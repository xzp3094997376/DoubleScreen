

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.UI;

using UnityEngine.EventSystems;




[ExecuteInEditMode]
public class Utils 
{
    public static bool IsDebug = true;
    public static int DedebugFontSize = 30;
    #region  
    /// <summary>
    /// 返回0-1的随机数
    /// </summary>
    /// <param name="times">随机次数</param>
    /// <returns></returns>
    public static float GetRandom(float times = 1)
    {
        if (times == 0) times = 1;
        double temp = 0;
        double tempI = 0;
        for (int i = 0; i < times; i++)
        {
            tempI = UnityEngine.Random.value;
            //   LogManager.Instance.Log("tempI------------>" + tempI);
            temp += tempI;
        }
        return (float)(temp / times);
    }

    /// <summary>
    /// 通过16进制颜色值 获取颜色
    /// </summary>
    /// <param name="colorStr"></param>
    /// <returns></returns>
    public static Color StringToColor(string colorStr)
    {
        if (string.IsNullOrEmpty(colorStr))
        {
            return new Color();
        }
        int colorInt = int.Parse(colorStr, System.Globalization.NumberStyles.AllowHexSpecifier);
        return IntToColor(colorInt);
    }
    public static Color IntToColor(int colorInt)
    {
        float basenum = 255;

        int b = 0xFF & colorInt;
        int g = 0xFF00 & colorInt;
        g >>= 8;
        int r = 0xFF0000 & colorInt;
        r >>= 16;
        return new Color((float)r / basenum, (float)g / basenum, (float)b / basenum, 1);
    }
    /// <summary>
    /// 获取枚举属性描述----前提是枚举添加自定义属性描述
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetDescription(Enum value)
    {
        if (value == null)
        {
            throw new ArgumentException("value");
        }
        string description = value.ToString();
        var fieldInfo = value.GetType().GetField(description);
        var attributes = (EnumDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
        if (attributes != null && attributes.Length > 0)
        {
            description = attributes[0].Description;
        }
        return description;
    }

    #endregion

   

    public static bool SetAlpha(Transform node, float alpha,Transform beExcluded)
    {
        if (node != null)
        {
            if(node== beExcluded|| node.parent == beExcluded)
            {
                return false;
            }           
            Color c;
            bool hasGraphic = false;

            Graphic graphic = node.GetComponent<Graphic>();
            if (graphic != null)
            {
                c = graphic.color;
                c.a = alpha;
                graphic.color = c;
                hasGraphic = true;
            }
            Graphic[] graphics = node.GetComponentsInChildren<Graphic>();
            if (graphics != null)
            {
                int length = graphics.Length;

                for (int i = 0; i < length; i++)
                {
                    if (graphics[i] != null)
                    {
                        if (graphics[i].transform.name != beExcluded.name&& !graphics[i].transform.IsChildOf(beExcluded))
                        {
                            c = graphics[i].color;
                            c.a = alpha;
                            graphics[i].color = c;
                        }
                    }
                }
                hasGraphic = true;
            }
            return hasGraphic;
        }
        return false;
    }


    /// <summary>
    /// 递归遍历
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindChild(Transform node, string name)
    {
        if (node.name == name)
        {
            return node;
        }
        foreach (Transform child in node)
        {
            Transform tNode = FindChild(child, name);
            if (tNode != null && tNode.name == name)
            {
                return tNode;
            }
        }
        return null;
    }

    /// <summary>
    /// 递归遍历
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindChild<T>(Transform node, string name)
    {
        T t = default(T);
        if (node.name == name)
        {
            t = node.GetComponent<T>();
            if (t != null)
                return t;
        }
        foreach (Transform child in node)
        {
            Transform tNode = FindChild(child, name);
            if (tNode != null && tNode.name == name)
            {
                t = tNode.GetComponent<T>();
                if (t != null)
                    return t;
            }
        }
        return t;
    }



    #region other

    /// <summary>
    /// 唯一ID生成器，使用从2000-01-01开始到目前时间的秒数单位TICK数量作为ID，
    /// 32位整形，够用136年。足够了确保唯一性。但是使用时必须注意，不同制作机器的时间
    /// 同步
    /// </summary>
    /// <returns></returns>    
    static public int GenID()
    {
        DateTime centuryBegin = new DateTime(2000, 1, 1);
        DateTime currentDate = DateTime.Now;
        long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
        TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
        return (int)elapsedSpan.TotalSeconds;
    }
    /// <summary>
    /// get the object hierarchy path of the Transform me
    /// </summary>
    /// <param name="me"></param>
    /// <returns></returns>
    public static string GetPath<T>(Transform me) where T : Component
    {
        string ret = me.name;
        T par = me.GetComponent<T>();
        //search all of the parents until the parent has a T component
        while (me)
        {
            me = me.parent;
            par = me.GetComponent<T>();
            if (!par)
                ret = string.Format("{0}/{1}", me.name, ret);
            else
                break;
        }
        return ret;
    }

    //mesh
    public static void SetVertexColor(Mesh mesh, Color color)
    {
        if (!mesh) return;
        //change vertex color
        Vector3[] vertices = mesh.vertices;//顶点
        Color[] colors = new Color[vertices.Length];
        int i = 0;
        while (i < vertices.Length)
        {
            colors[i] = color;
            i++;
        }
        mesh.colors = colors;
    }

    #endregion


    #region format time



    //解析并格式化时间 时:分:秒  这个时间不能超出24小时
    public static string GetFormatTime(int seconds)
    {
        TimeSpan span = new TimeSpan(0, 0, 0, seconds);
        if (span.Days < 1)
            return string.Format("{0:D2}:{1:D2}:{2:D2}", span.Hours + span.Days * 24, span.Minutes, span.Seconds);
        else
            return string.Format("{0:D}D {1:D2}:{2:D2}:{3:D2}", span.Days, span.Hours, span.Minutes, span.Seconds);
    }

    //用于主页 解析并格式化时间 时:分:秒
    public static string GetFormatTimeForHome(int seconds)
    {
        TimeSpan span = new TimeSpan(0, 0, 0, seconds);
        if (span.Days < 1)
            return string.Format("{0:D2}:{1:D2}:{2:D2}", span.Hours + span.Days * 24, span.Minutes, span.Seconds);
        else
            return string.Format("{0:D}D {1:D2}:{2:D2}:{3:D2}", span.Days, span.Hours, span.Minutes, span.Seconds);

    }

    public static string GetFormatTimeForHome(long seconds)
    {
        TimeSpan span = new TimeSpan(seconds * 10000000);
        if (span.Days < 1)
            return string.Format("{0:D2}:{1:D2}:{2:D2}", span.Hours + span.Days * 24, span.Minutes, span.Seconds);
        else
            return string.Format("{0:D}D {1:D2}:{2:D2}:{3:D2}", span.Days, span.Hours, span.Minutes, span.Seconds);
    }

    public static string GetFormatTimeForTwoCon(int seconds)
    {
        TimeSpan span = new TimeSpan(0, 0, 0, seconds);
        if (span.Days < 1)
        {
            if (span.Hours < 1)
            {
                return string.Format("{0:D}m : {1:D2}s", span.Minutes, span.Seconds);
            }
            else
            {
                return string.Format("{0:D}h : {1:D2}m", span.Hours, span.Minutes);
            }
        }
        else
        {
            return string.Format("{0:D}d : {1:D2}h", span.Days, span.Hours);
        }
    }

    public static List<DateTime> Days(DateTime month)
    {
        List<DateTime> days = new List<DateTime>();
        DateTime firstDay = new DateTime(month.Year, month.Month, 1);
        DayOfWeek week = firstDay.DayOfWeek;
        int lastMonthDays = (int)week;
        if (lastMonthDays.Equals(0))
            lastMonthDays = 7;
        for (int i = lastMonthDays; i > 0; i--)
            days.Add(firstDay.AddDays(-i));
        for (int i = 0; i < 42 - lastMonthDays; i++)
            days.Add(firstDay.AddDays(i));
        return days;
    }
    public static List<DateTime> Months(DateTime year)
    {
        List<DateTime> months = new List<DateTime>();
        DateTime firstMonth = new DateTime(year.Year, 1, 1);
        months.Add(firstMonth);
        for (int i = 1; i < 12; i++)
            months.Add(firstMonth.AddMonths(i));
        return months;
    }
    public static List<DateTime> Years(DateTime year)
    {
        List<DateTime> years = new List<DateTime>();
        for (int i = 5; i > 0; i--)
            years.Add(year.AddYears(-i));
        for (int i = 0; i < 7; i++)
            years.Add(year.AddYears(i));
        return years;
    }

    //解析并格式化时间 分:秒  这个时间不能超出24小时
    public static string GetFormatTimeInMinites(int seconds)
    {
        TimeSpan span = new TimeSpan(0, 0, 0, seconds);
        return string.Format("{0:D2}:{1:D2}", span.Minutes + span.Hours * 60, span.Seconds);
    }

    //解析并格式化时间 分钟为结束点
    public static string GetFormatTimeEndMinutes(int seconds)
    {
        TimeSpan span = new TimeSpan(0, 0, 0, seconds);
        if (span.Days < 1)
            return string.Format("{0:D2}:{1:D2}", span.Hours + span.Days * 24, span.Minutes);
        else
            return string.Format("{0:D}D {1:D2}:{2:D2}", span.Days, span.Hours, span.Minutes);
    }



    /// <summary>
    /// 将剩余秒数转化为时间
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static string FormatTime(int seconds)
    {
        TimeSpan timeSpan = new TimeSpan(0, 0, 0, seconds);
        int d = timeSpan.Days;
        int h = timeSpan.Hours;
        int m = timeSpan.Minutes;
        int s = timeSpan.Seconds;
        if (seconds > 0)
            return (d > 0 ? (d + "D ") : "") + (h > 0 ? (h + "H ") : "") + (m > 0 ? (m + "M ") : "") + (s > 0 ? (s + "S") : "");
        else
            return "0S";
    }


    public static string FormatTime(long seconds)
    {
        TimeSpan timeSpan = new TimeSpan(seconds * 10000000);
        int d = timeSpan.Days;
        int h = timeSpan.Hours;
        int m = timeSpan.Minutes;
        int s = timeSpan.Seconds;
        return (d > 0 ? (d + "D ") : "") + (h > 0 ? (h + "H ") : "") + (m > 0 ? (m + "M ") : "") + (s > 0 ? (s + "S") : "");
    }

    public static string FormatShortTime(int seconds)
    {
        TimeSpan timeSpan = new TimeSpan(0, 0, 0, seconds);
        int d = timeSpan.Days;
        int h = timeSpan.Hours;
        int m = timeSpan.Minutes;
        int s = timeSpan.Seconds;
        if (d > 0)
            return d + "D " + (h > 0 ? (h + "H ") : "");

        if (h > 0)
            return h + "H " + (m > 0 ? (m + "M ") : "");

        if (m > 0)
            return m + "M " + (s > 0 ? (s + "S") : "");

        return s + "S";
    }

    public static string FormatTimeSpan(long timeData)
    {
        DateTime time = new System.DateTime(1970, 1, 1, 8, 0, 0);
        time.AddMilliseconds(timeData);
        TimeSpan timeSpan = DateTime.Now.Subtract(time);
        return FormatTimeSpan(timeSpan);
    }

    public static string FormatTimeSpan(TimeSpan timeSpan)
    {
        double totalSeconds = timeSpan.TotalSeconds;
        if (totalSeconds <= 60)
        {
            return "Just Now";
        }

        if (totalSeconds <= 3600)
        {
            return string.Format("{0}M ago", timeSpan.Minutes);
        }

        if (totalSeconds <= (3600 * 24))
        {
            return string.Format("{0}H ago", timeSpan.Hours);
        }

        else
        {
            return string.Format("{0}Days ago", timeSpan.Days);
        }
    }

    public static string Format3DShortTime(int seconds)
    {
        TimeSpan timeSpan = new TimeSpan(0, 0, 0, seconds);
        int d = timeSpan.Days;
        int h = timeSpan.Hours;
        int m = timeSpan.Minutes;
        int s = timeSpan.Seconds;

        if (d > 0)
        {
            if (d == 1)
                return d + " Day";
            else
                return d + " Days";
        }

        if (h > 0)
            return h + "H" + m + "M" + s + "S";

        if (m > 0)
            return m + "M" + s + "S";

        return s + "S";
    }

    public static string Format2DShortTime(int seconds)
    {
        TimeSpan timeSpan = new TimeSpan(0, 0, 0, seconds);
        int d = timeSpan.Days;
        int h = timeSpan.Hours;
        int m = timeSpan.Minutes;
        int s = timeSpan.Seconds;
        if (d > 0)
        {
            if (h > 0)
                return d + "D" + h + "H";
            else
                return d + "D";
        }

        if (h > 0)
        {
            if (m > 0)
                return h + "H" + m + "M";
            else
                return h + "H";
        }

        if (m > 0)
        {
            if (s > 0)
                return m + "M" + s + "S";
            else
                return m + "M";
        }
        return s + "S";
    }

    public static string FormatShortTimeEndMinutes(int seconds)
    {
        string timestr = string.Empty;
        TimeSpan timeSpan = new TimeSpan(0, 0, 0, seconds);
        int d = timeSpan.Days;
        int h = timeSpan.Hours;
        int m = timeSpan.Minutes;
        int s = timeSpan.Seconds;
        //
        if (d > 0)
        {
            if (d > 1)
                timestr += d + " Days  ";
            else
                timestr += d + " Day  ";
        }
        //
        if (h > 0)
        {
            if (h > 1) timestr += h + " Hours  ";
            else timestr += h + " Hour  ";

        }
        //
        if (m > 0)
        {
            if (m > 1) timestr += m + " Minutes  ";
            else timestr += m + " Minute  ";
        }
        return timestr;
    }

    public static DateTime GetDateTime(long timeStamp)
    {
        DateTime dtStart = SendDateTime(timeStamp);
        TimeSpan toNow = new TimeSpan(timeStamp);
        DateTime targetDt = dtStart.Add(toNow);
        return dtStart.Add(toNow);
    }

    public static DateTime SendDateTime(long sendTime)
    {
        DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return time.AddMilliseconds(sendTime);
    }

    public static string FormatNum(long num)
    {
        if (num >= 1000000000)
        {
            float tenNum = (float)num / 1000000000f;
            if (tenNum / 100 >= 1)
                return string.Format("{0:###.##}B", Mathf.FloorToInt(tenNum));
            if (tenNum / 10 >= 1)
                return string.Format("{0:###.#}B", tenNum);

            return string.Format("{0:###.##}B", tenNum - 0.005f);
        }
        if (num >= 1000000)
        {
            float tenNum = (float)num / 1000000f;
            if (tenNum / 100 >= 1)
                return string.Format("{0:###.##}M", Mathf.FloorToInt(tenNum));
            if (tenNum / 10 >= 1)
                return string.Format("{0:###.#}M", tenNum);

            return string.Format("{0:###.##}M", tenNum - 0.005f);
        }
        if (num >= 1000)
        {
            float tenNum = (float)num / 1000f;
            if (tenNum / 100 >= 1)
                return string.Format("{0:###.##}K", Mathf.FloorToInt(tenNum));
            if (tenNum / 10 >= 1)
                return string.Format("{0:###.#}K", tenNum);

            return string.Format("{0:###.##}K", tenNum - 0.005f);
        }

        return (num > 0) ? string.Format("{0:###.##}", num) : "0";
    }

    /// <summary>
    /// 千位分割
    ///</summary>
    public static string FormatNumDoubler(int num)
    {
        if (num < 1000) return num.ToString();
        return string.Format("{0:0,0}", num);
    }
    #endregion
    /// <summary>
    /// 判定字符串里面是否含有中文
    /// </summary>
    /// <returns><c>true</c> if has chines the specified str; otherwise, <c>false</c>.</returns>
    /// <param name="str">String.</param>
    public static bool HasChines(string str)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
    }
    /// <summary>
    /// 去除字符串中的所有中括号以及中括号中的内容
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static string GetFrormatValRemoveBracket(string val)
    {
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\[.*?\]");
        System.Text.RegularExpressions.Match m = regex.Match(val, 0);
        string temp = val;
        while (m.Success)
        {
            string t = m.ToString();
            if (t.Length != 5)//把标签去掉
            {
                temp = temp.Replace(t, "");
            }
            m = m.NextMatch();
        }
        return temp;
    }
    /// <summary>
    /// 将json字符串中的空数组改成null
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static string GetJsonFromatRemoveBrackets(string val)
    {
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\[\]");
        System.Text.RegularExpressions.Match m = regex.Match(val, 0);
        string temp = val;
        while (m.Success)
        {
            string t = m.ToString();
            temp = temp.Replace(t, "null");
            m = m.NextMatch();
        }
        return temp;
    }

    public static string GetJsonFromatRemoveBracketsObject(string val)
    {
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\{\}");
        System.Text.RegularExpressions.Match m = regex.Match(val, 0);
        string temp = val;
        while (m.Success)
        {
            string t = m.ToString();
            temp = temp.Replace(t, "null");
            m = m.NextMatch();
        }
        return temp;
    }

    /// <summary>
    /// 判定字符串里面只能有英文和数字
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsDigitOrNumber(string str)
    {
        if (System.Text.RegularExpressions.Regex.IsMatch(str, @"(?i)^[0-9a-z]+$"))
            return true;
        else return false;
    }
    /// <summary>
    /// 判断字符串是正整数
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsNumber(string str)
    {
        if (System.Text.RegularExpressions.Regex.IsMatch(str, @"^([1-9][0-9]*)$"))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 邮件地址验证
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsEmailAdress(string str)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(str, @"[0-9a-zA-Z_]+@([-\dA-Za-z]+\.)+[a-zA-Z]{2,}");
    }

    /// <summary>
    /// 通过字符串获取图片
    /// </summary>
    /// <returns>The to texter2d.</returns>
    /// <param name="Base64STR">Base64 ST.</param>
    public static Texture2D Base64ToTexter2d(string Base64STR, int width = 60, int height = 60, TextureFormat format = TextureFormat.ARGB32)
    {
        Texture2D pic = new Texture2D(width, height, format, false);

        byte[] data = System.Convert.FromBase64String(Base64STR);

        pic.LoadImage(data);

        return pic;
    }

    public static int GetTextPixelWidth(Text target,int fontSize)
    {
        int allTextWidth = 0;
        foreach (char item in target.text)
        {
            CharacterInfo info = new CharacterInfo();
            target.font.GetCharacterInfo(item, out info, fontSize);
            allTextWidth += info.advance;
        }
        return allTextWidth;
    }
   
    public static GameObject Canvas()
    {
        GameObject canvas;
        var cc = UnityEngine.Object.FindObjectOfType<Canvas>();
        if (!cc)
        {
            canvas = new GameObject("Canvas", typeof(Canvas));
            canvas.layer = LayerMask.NameToLayer("UI");
        }
        else
        {
            canvas = cc.gameObject;
        }
        if (!UnityEngine.Object.FindObjectOfType<EventSystem>())
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem));
        }

        return canvas;
    }

}



/// <summary>
/// 对类进行扩展
/// </summary>
public static class NativeExtend
{
    /// <summary>
    /// 设置同屏下主canvas 激活
    /// </summary>
    /// <param name="canvas"></param>
    static public void SameScreenDisplayUISort(this MultPlateformCanvas canvas)
    {
        MultiDisplayInit.Instance.SameScreenDisplayUISort(canvas);
    }
    /// <summary>
    /// 添加canvas 到管理类中
    /// </summary>
    /// <param name="canvas"></param>
    static public void RegisterCanvas(this MultPlateformCanvas canvas)
    {
        MultiDisplayInit.Instance.AddCanvas(canvas);
    }
    /// <summary>
    /// 设为主屏canvas
    /// </summary>
    /// <param name="canvas"></param>
    static public void SetMainScreenCanvas(this MultPlateformCanvas canvas)
    {
        MultiDisplayInit.Instance.SetMainScreenCanvas(canvas);
    }
    /// <summary>
    /// 设为副屏canvas
    /// </summary>
    /// <param name="canvas"></param>
    static public void SetSubScreenCanvas(this MultPlateformCanvas canvas)
    {
        MultiDisplayInit.Instance.SetSubScreenCanvas(canvas);
    }
    static public T GetMissingComponent<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }
        return comp;
    }

    static public T GetMissingComponent<T>(this Transform tf) where T : Component
    {
        T comp = tf.GetComponent<T>();
        if (comp == null)
        {
            comp = tf.gameObject.AddComponent<T>();
        }
        return comp;
    }

  
    static public void SetParent(this Transform tran, Transform p, Vector3 pos, Vector3 roa, Vector3 zoo)
    {
        tran.SetParent(p);
        tran.localEulerAngles = roa;
        tran.localPosition = pos;
        tran.localScale = zoo;
    }
    static public void SetParentNormal(this Transform tran, Transform p)
    {
        tran.SetParent(p);
        tran.localEulerAngles = Vector3.zero;
        tran.localPosition = Vector3.zero;
        tran.localScale = Vector3.one;
    }

    static public T AddMissingComponent<T>(this GameObject go) where T : Component
    {
#if UNITY_FLASH
		object comp = go.GetComponent<T>();
#else
        T comp = go.GetComponent<T>();
#endif
        if (comp == null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                RegisterUndo(go, "Add " + typeof(T));
#endif
            comp = go.AddComponent<T>();
        }
#if UNITY_FLASH
		return (T)comp;
#else
        return comp;
#endif
    }

    static public void RegisterUndo(UnityEngine.Object obj, string name)
    {
#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(obj, name);
        SetDirty(obj);
#endif
    }

    /// <summary>
    /// Convenience function that marks the specified object as dirty in the Unity Editor.
    /// </summary>

    static public void SetDirty(UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        if (obj)
        {
            UnityEditor.EditorUtility.SetDirty(obj);
        }
#endif
    }
    /// <summary>
    /// Linq 字典 去重复 扩展
    /// 针对ID，和Name进行Distinct
    /// people.DistinctBy(p => new { p.Id, p.Name });
    /// 仅仅针对ID进行distinct：
    /// people.DistinctBy(p => p.Id);
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> seenKeys = new HashSet<TKey>();
        foreach (TSource element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }
   
}

/// <summary>
/// 自定义枚举属性
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class EnumDescriptionAttribute : Attribute
{
    private string description;
    public string Description { get { return description; } }

    public EnumDescriptionAttribute(string description) : base()
    {
        this.description = description;
    }
    
}

