using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Linq;
using System.Text.RegularExpressions;

namespace Languages
{
    public static class Libs
    {
        private static ResourceManager _myResourceManager = null;
        private static ResourceManager myResourceManager
        {
            get
            {
                if (_myResourceManager == null)
                {
                    _myResourceManager = new ResourceManager("Languages.myResources", System.Reflection.Assembly.GetExecutingAssembly());
                    _myResourceManager.IgnoreCase = true;
                }
                return _myResourceManager;
            }
        }

        /// <summary>
        /// To retrieve text of the code [code] in language [lang] 
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetString(CommonTypes.SysLanguage lang, string code)
        {
            lock(myResourceManager)
            {
                if (String.IsNullOrEmpty(code) == false)
                {
                    string tmp = myResourceManager.GetString(code, CommonTypes.SysLibs.GetCulture(lang));
                    return (tmp == null ? "NA" + code : tmp);
                }
                return "";
            }
        }

        const short constChunkSz = 100;
        /// <summary>
        /// To retrieve chunk of text from langage resource
        /// </summary>
        /// <param name="list">List storing output data</param>
        /// <param name="lang">language for text to retrieve</param>
        /// <param name="format">format to retrieve</param>
        /// <param name="chunkNo">chunk to load (0-base index), null means load all.</param>
        /// <returns>number of items loaded. NULL if error</returns>`
        public static int? LoadText(SortedList<string, string> list, CommonTypes.SysLanguage lang, CommonTypes.TextResourceFormat format, int? chunkNo = null)
        {
            try
            {
                int? FromNo = null, ToNo = null;
                if (chunkNo != null)
                {
                    FromNo = chunkNo.Value * constChunkSz;
                    ToNo = FromNo + constChunkSz;
                }
                lock(myResourceManager)
                {
                    ResourceSet resourceSet = myResourceManager.GetResourceSet(CommonTypes.SysLibs.GetCulture(lang), true, true);
                    int count = 0, addedCount = 0;
                    foreach (DictionaryEntry entry in resourceSet)
                    {
                        count++;
                        if (FromNo != null && count < FromNo) continue;
                        if (ToNo != null && count > ToNo) break;

                        switch (format)
                        {
                            case CommonTypes.TextResourceFormat.Angular:
                                list.Add(entry.Key.ToString(), ConvertFormat((string)entry.Value, "{{n", "}}"));
                                break;
                            case CommonTypes.TextResourceFormat.ObjectC:
                                list.Add(entry.Key.ToString(), ConvertFormat((string)entry.Value, "%@", ""));
                                break;
                            default:
                                list.Add(entry.Key.ToString(), (string)entry.Value);
                                break;
                        }
                        addedCount++;
                    }
                    return addedCount;
                }
            }
            catch (Exception er)
            {
                CommonTypes.SysLibs.WriteErrorLog(er);
                return null;
            }
        }

        /// <summary>
        /// Convert a C-format string to new format that each {value} is converted to (prefix + value + postfix)
        /// </summary>
        /// <param name="format"></param>
        /// <param name="prefix"></param>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public static string ConvertFormat(string format, string prefix, string postfix)
        {
            var pattern = @"{(.*?)}";
            var matches = Regex.Matches(format, pattern);
            int count = matches.OfType<Match>().Select(m => m.Value).Distinct().Count();
            string[] values = new string[count];
            for (int idx = 0; idx < count; idx++) values[idx] = prefix + idx.ToString() + postfix;
            return String.Format(format, values);
        }

        public static string Type2Text(CommonTypes.ApiStatusCode key, CommonTypes.SysLanguage lang)
        {
            return GetString(lang, key.ToString());
        }

        internal static string Type2Text(CommonAPI.Types.Gender value, CommonTypes.SysLanguage lang)
        {
            return GetString(lang, "gender"+value.ToString());
        }

        internal static string Type2Text(CommonTypes.OrgType value, CommonTypes.SysLanguage lang)
        {
            return GetString(lang, value.ToString());
        }
        internal static string Type2Text(CommonTypes.UserType value, CommonTypes.SysLanguage lang)
        {
            return GetString(lang, value.ToString());
        }
        internal static string Type2Text(CommonTypes.UserStatus value, CommonTypes.SysLanguage lang)
        {
            return GetString(lang, value.ToString());
        }
        internal static string Type2Text(CommonTypes.UserRoles value, CommonTypes.SysLanguage lang)
        {
            return GetString(lang, value.ToString());
        }

        internal static string Type2Text(CommonTypes.TicketStatus value, CommonTypes.SysLanguage lang)
        {
            return GetString(lang, value.ToString());
        }
        internal static string Type2Text(CommonTypes.TicketNoRestartType value, CommonTypes.SysLanguage lang)
        {
            return GetString(lang, value.ToString());
        }

        internal static string Type2Text(CommonTypes.SysLanguage value, CommonTypes.SysLanguage lang)
        {
            return GetString(lang, "lang." + value.ToString());
        }
        internal static string Type2Text(CommonTypes.DayOfWeek value, CommonTypes.SysLanguage lang)
        {
            return GetString(lang, value.ToString());
        }
        internal static string Type2TextShort(CommonTypes.DayOfWeek value, CommonTypes.SysLanguage lang)
        {
            return GetString(lang, "short"+value.ToString());
        }

        public static List<CommonLibs.TextValue> GetListLanguage(CommonTypes.SysLanguage lang)
        {
            List<CommonLibs.TextValue> list = new List<CommonLibs.TextValue>();
            foreach (CommonTypes.SysLanguage type in Enum.GetValues(typeof(CommonTypes.SysLanguage)))
            {
                CommonLibs.TextValue item = new CommonLibs.TextValue("", "");
                item.Value = ((short)type).ToString();
                item.Text = Type2Text(type, lang);
                item.Tag = type;
                list.Add(item);
            }
            return list;
        }

        public static List<CommonLibs.TextValue> GetListApiProviderId()
        {
            List<CommonLibs.TextValue> list = new List<CommonLibs.TextValue>();
            foreach (CommonAPI.Types.ApiProviderId type in Enum.GetValues(typeof(CommonAPI.Types.ApiProviderId)))
            {
                CommonLibs.TextValue item = new CommonLibs.TextValue("", "");
                item.Value = ((short)type).ToString();
                item.Text = type.ToString();
                item.Tag = type;
                list.Add(item);
            }
            return list;
        }

        public static List<CommonLibs.TextValue> GetListUserType(CommonTypes.SysLanguage lang, CommonTypes.UserType? userType = null, CommonTypes.UserType? exUserType = null)
        {
            if (userType == null) userType = CommonTypes.UserType.All;
            List<CommonLibs.TextValue> list = new List<CommonLibs.TextValue>();
            foreach (CommonTypes.UserType type in Enum.GetValues(typeof(CommonTypes.UserType)))
            {
                if (type == CommonTypes.UserType.All || (userType & type) == 0) continue;
                if (exUserType != null && (exUserType & type) > 0) continue;

                CommonLibs.TextValue item = new CommonLibs.TextValue("", "");
                item.Value = ((short)type).ToString();
                item.Text = Type2Text(type, lang);
                item.Tag = type;
                list.Add(item);
            }
            return list;
        }

        public static List<CommonLibs.TextValue> GetListUserRoles(CommonTypes.SysLanguage lang)
        {
            return GetListUserRoles(lang, CommonTypes.UserRoles.OrgAdmin| CommonTypes.UserRoles.OrgOperator);
        }
        internal static List<CommonLibs.TextValue> GetListUserRoles(CommonTypes.SysLanguage lang, CommonTypes.UserRoles? roles = null, CommonTypes.UserRoles? exRoles = null)
        {
            if (roles == null) roles = CommonTypes.UserRoles.All;
            List<CommonLibs.TextValue> list = new List<CommonLibs.TextValue>();
            foreach (CommonTypes.UserRoles roleItem in Enum.GetValues(typeof(CommonTypes.UserRoles)))
            {
                if (roleItem == CommonTypes.UserRoles.All || (roles & roleItem) == 0) continue;
                if (exRoles != null && (exRoles & roleItem) > 0) continue;

                CommonLibs.TextValue item = new CommonLibs.TextValue("", "");
                item.Value = ((short)roleItem).ToString();
                item.Text = Type2Text(roleItem, lang);
                item.Tag = roleItem;
                list.Add(item);
            }
            return list;
        }

        public static List<CommonLibs.TextValue> GetListOrgType(CommonTypes.SysLanguage lang, CommonTypes.OrgType? excludeType = null)
        {
            List<CommonLibs.TextValue> list = new List<CommonLibs.TextValue>();
            foreach (CommonTypes.OrgType type in Enum.GetValues(typeof(CommonTypes.OrgType)))
            {
                if (excludeType != null && (excludeType & type) > 0) continue;
                CommonLibs.TextValue item = new CommonLibs.TextValue("", "");
                item.Value = ((short)type).ToString();
                item.Text = Type2Text(type, lang);
                item.Tag = type;
                list.Add(item);
            }
            return list;
        }

        public static List<CommonLibs.TextValue> GetListUserStatus(CommonTypes.SysLanguage lang, bool fullList = false)
        {
            List<CommonLibs.TextValue> list = new List<CommonLibs.TextValue>();
            foreach (CommonTypes.UserStatus type in Enum.GetValues(typeof(CommonTypes.UserStatus)))
            {
                CommonLibs.TextValue item = new CommonLibs.TextValue("", "");
                item.Value = ((short)type).ToString();
                item.Text = Type2Text(type, lang);
                item.Tag = type;
                list.Add(item);
            }
            return list;
        }

        public static List<CommonLibs.TextValue> GetListDayOfWeek(CommonTypes.SysLanguage lang)
        {
            List<CommonLibs.TextValue> list = new List<CommonLibs.TextValue>();
            foreach (CommonTypes.DayOfWeek type in Enum.GetValues(typeof(CommonTypes.DayOfWeek)))
            {
                CommonLibs.TextValue item = new CommonLibs.TextValue("", "");
                item.Value = ((short)type).ToString();
                item.Text = Type2Text(type, lang);
                item.Tag = Type2TextShort(type, lang);
                list.Add(item);
            }
            return list;
        }

        public static List<CommonLibs.TextValue> GetLisTicketNoRestart(CommonTypes.SysLanguage lang, CommonTypes.TicketNoRestartType? excludeType = null)
        {
            List<CommonLibs.TextValue> list = new List<CommonLibs.TextValue>();
            foreach (CommonTypes.TicketNoRestartType type in Enum.GetValues(typeof(CommonTypes.TicketNoRestartType)))
            {
                if (excludeType != null && (excludeType & type) > 0) continue;
                CommonLibs.TextValue item = new CommonLibs.TextValue("", "");
                item.Value = ((short)type).ToString();
                item.Text = Type2Text(type, lang);
                item.Tag = type;
                list.Add(item);
            }
            return list;
        }
        public static List<CommonLibs.TextValue> GetLisTicketStatus(CommonTypes.SysLanguage lang)
        {
            List<CommonTypes.TicketStatus> excludeStatus = new List<CommonTypes.TicketStatus>();
            excludeStatus.Add(CommonTypes.TicketStatus.New);
            return GetLisTicketStatus(lang, excludeStatus);
        }

        public static List<CommonLibs.TextValue> GetLisTicketStatus(CommonTypes.SysLanguage lang,
                                                                    List<CommonTypes.TicketStatus> excludeStatus)
        {
            List<CommonLibs.TextValue> list = new List<CommonLibs.TextValue>();
            foreach (CommonTypes.TicketStatus stat in Enum.GetValues(typeof(CommonTypes.TicketStatus)))
            {
                if (excludeStatus!= null && excludeStatus.Contains(stat)) continue;
                CommonLibs.TextValue item = new CommonLibs.TextValue("", "");
                item.Value = ((short)stat).ToString();
                item.Text = Type2Text(stat, lang);
                item.Tag = stat;
                list.Add(item);
            }
            return list;
        }

        public static string GetText(CommonTypes.DayOfWeek? value, CommonTypes.SysLanguage lang)
        {
            if (value == null) return "";
            string retStr = "";
            foreach (CommonTypes.DayOfWeek type in Enum.GetValues(typeof(CommonTypes.DayOfWeek)))
            {
                if ((value & type) <= 0) continue;
                if (string.IsNullOrWhiteSpace(retStr) == false) retStr += "-";
                retStr += Type2Text(type, lang);
            }
            return retStr;
        }
        public static string GetTextShort(CommonTypes.DayOfWeek? value, CommonTypes.SysLanguage lang)
        {
            if (value == null) return "";
            string retStr = "";
            foreach (CommonTypes.DayOfWeek type in Enum.GetValues(typeof(CommonTypes.DayOfWeek)))
            {
                if ((value & type) <= 0) continue;
                if (string.IsNullOrWhiteSpace(retStr) == false) retStr += "-";
                retStr += Type2TextShort(type, lang);
            }
            return retStr;
        }
    }
}