using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace Clicker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool login = false;
            if (new LoginPage().ShowDialog() != DialogResult.OK)
                MessageBox.Show("نام کاربری و کلمه عبور را اشتباه وارد کرده اید");
            else
            {
                login = true;

            }
            if (login)
            {

                Application.Run(new MainForm());
            }
        }
    }
}
