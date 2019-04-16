using Clicker.DataSection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Windows.Forms;

namespace Clicker
{
    public partial class LoginPage : Form
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void button_CheckAuth_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            try
            {
                using (var Context = new AutoClickDBEntities())
                {
                    /*  UserTracking UserTrackingOb = new UserTracking();
                      UserTrackingOb.FkUserId = new Guid(Membership.GetUser(LoginUser.UserName).ProviderUserKey.ToString());
                      UserTrackingOb.LogDateTime = System.DateTime.Now.ToShortTimeString().Substring(0, 5) + " " + UiUtility.FarsiDate_ConvertDMYToYMD(AutoClicker.Manager.BLLDate.getPersianDate(System.DateTime.Now));
                      UserTrackingOb.LogEventName = "User Login";
                      Context.UserTrackings.(UserTrackingOb);
                      Context.SaveChanges();*/

                    UTF8Encoding encoder = new UTF8Encoding();
                   // byte[]
                    string pass = encoder.GetBytes(textBox_Pass.Text).ToString();
                    //System.Web.Security.Membership.password
                    if (Context.aspnet_Users.Any(s => s.UserName == textBox_UserName.Text))
                    {
                      if(Membership.ValidateUser(textBox_UserName.Text, textBox_Pass.Text))
                        // Log the user into the site
                        // FormsAuthentication.RedirectFromLoginPage(textBox_UserName.Text,true);
                        
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                }
            }
            catch (Exception ex)
            {                
            }
        }

        private void LoginPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == System.Windows.Forms.Keys.Enter)
            {

                button_CheckAuth_Click(sender, e);
            }

            
        }
    }
}
