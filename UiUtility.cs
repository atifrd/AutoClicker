using Clicker.DataSection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace Clicker
{
    public static class UiUtility
    {
        //private static MSSqlDataAccess.PostDbDataContext db =
        //    new MSSqlDataAccess.PostDbDataContext(System.Configuration.ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
        private const string ActiveYearSessionName = "ActiveYear";

        private static System.Web.SessionState.HttpSessionState _session
        {
            get
            {
                return HttpContext.Current.Session;
            }
        }

        public static int ActiveYear
        {
            get
            {
                try
                {
                    return (int)_session[ActiveYearSessionName];
                }
                catch
                {
                    PersianCalendar calendar = new PersianCalendar();
                    return calendar.GetYear(DateTime.Now);
                }
            }
            set
            {
                _session[ActiveYearSessionName] = value;
            }
        }

        public static bool IsAdmin(this string user)
        {
            //var usr = db.aspnet_Users.FirstOrDefault(u => u.UserName == user);
            //if (usr != null)
            //{
            //    return (from u in usr.aspnet_UsersInRoles
            //            join r in db.aspnet_Roles on u.RoleId equals r.RoleId
            //            where r.RoleName == "مدیر ارشد سیستم"
            //            select u).Count() > 0;
            //}
            return false;
        }

        public static object Session(SessionNames session)
        {
            return HttpContext.Current.Session[session.ToString()];
        }

        public static object Session(SessionNames session, object _default)
        {
            var result = Session(session);
            if (result == null)
                return _default;
            else
                return result;
        }

        public static void SetSession(SessionNames session, object value)
        {
            HttpContext.Current.Session[session.ToString()] = value;
        }

        
        public static int retInt(this string val)
        {
            try
            {
                if (val == null || val == "")
                    val = "0";
                Int32 result;
                return (!Int32.TryParse(val, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out result)) ? 0 : int.Parse(val);
            }
            catch { return 0; }

        }

        public static double retDouble(this string val)
        {
            try
            {
                if (val == null || val == "")
                    val = "0";
                Double result;
                return (!Double.TryParse(val, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out result)) ? 0 : double.Parse(val);
            }
            catch { return 0; }

        }

        public static long retLong(this string val)
        {
            try
            {
                if (val == null || val == "")
                    val = "0";
                Int64 result;
                return (!Int64.TryParse(val, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out result)) ? 0 : long.Parse(val);
            }
            catch { return 0; }
        }

        public static byte retByte(this string val)
        {
            try
            {
                if (val == null || val == "")
                    val = "0";
                byte result;
                return (!Byte.TryParse(val, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out result)) ? (byte)0 : byte.Parse(val);
            }
            catch { return 0; }

        }
     /*   public static Clicker.Employee GetCurrentEmoloyee()
        {// new Guid(Membership.GetUser().ProviderUserKey.ToString());
            MembershipUser newUser = Membership.GetUser();
            string MyCurrentUserName = newUser.UserName;
            using (var Context = new AutoClickDBEntities())
            {
                var MyCurrentUserInfo = Context.aspnet_Users.Where(p => p.UserName == MyCurrentUserName).Select(b => new
                {
                    b.UserId,
                }).FirstOrDefault();
                return Context.Employees.Where(p => p.PersoneCode MyCurrentUserInfo.UserId)
                      .FirstOrDefault();
            }
        }
        */
        public static IQueryable<TSource> WhereIf<TSource>(
    this IQueryable<TSource> source, bool condition,
    Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

       

        public static string String_LastNchar(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }
        public static string FarsiDate_ConvertDMYToYMD(string DMYDate)
        {
            string YMDDate = DMYDate;
            if (DMYDate != string.Empty && DMYDate != null)
            {
                string[] a = DMYDate.Split('/');
                string day = a[0];
                string month = a[1];
                string Year = a[2];
                YMDDate = Year + "/" + month + "/" + day;

            }
            return YMDDate;
        }


        public static string FarsiDate_ConvertYMDToDMY(string YMDDate)
        {
            string DMYDate = YMDDate;
            if (YMDDate != string.Empty && YMDDate != null)
            {
                string[] a = YMDDate.Split('/');
                string Year = a[0];
                string month = a[1];
                string Day = a[2];
                DMYDate = Day + "/" + month + "/" + Year;

            }
            return DMYDate;
        }

        public static string mypath;
      public static string s2 = @"D:\AutoClicker_ClientDefPath.txt";
        public static void WriteInLogFile(string Path)
        {

           
                //= System.Configuration.ConfigurationSettings.AppSettings["ClientDefPath"].ToString().Trim();
            mypath = new string(s2.ToCharArray());


            try
            {
                StreamWriter srw = new StreamWriter(mypath, false);
                srw.WriteLine(Path);
                srw.Close();
            }
            catch (Exception ex)
            { }
        }
     public static   string ReadPath()
        {
            String line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(s2);
                line = sr.ReadLine();

                //close the file
                sr.Close();
                return line;
               
            }
            catch (Exception e)
            {
                return "";
            }
            
        }

    }

    public enum SessionNames { Budget_Print_BudgetId }
    public enum UserControlsName { Budget_Print }



}