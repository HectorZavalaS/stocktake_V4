using stocktake_V4.Class;
using stocktake_V4.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stocktake_V4
{
    public partial class Login : Form
    {
        Thread splashthread;
        int m_type_scan;
        CAlert m_alerts;
        public Login()
        {
            InitializeComponent();
            m_type_scan = 2;
            m_alerts = new CAlert();
        }
        public static string GetSHA1(String texto)
        {
            SHA1 sha1 = SHA1CryptoServiceProvider.Create();
            Byte[] textOriginal = ASCIIEncoding.Default.GetBytes(texto);
            Byte[] hash = sha1.ComputeHash(textOriginal);
            StringBuilder cadena = new StringBuilder();

            foreach (byte i in hash)
            {
                cadena.AppendFormat("{0:x2}", i);
            }
            return cadena.ToString();
        }
        private void Login_Load(object sender, EventArgs e)
        {
            txtUser.Focus();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            login();
        }

        private void login()
        {
            int idUser = 0;
            int level = 0;
            String userName = "";
            if (String.IsNullOrEmpty(txtUser.Text))
            {
                m_alerts.playWarning();
                MessageBox.Show("Debes ingresar un usuario.", "Login...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUser.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtPass.Text))
            {
                m_alerts.playWarning();
                MessageBox.Show("Debes ingresar tu password.", "Login...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPass.Focus();
                return;
            }
            progressBar1.Visible = true;
            //splashthread = new Thread(new ThreadStart(SplashScreen.ShowSplashScreen));
            //splashthread.IsBackground = true;
            //splashthread.Start();
            if (SignInDB(txtUser.Text.ToUpper(), txtPass.Text.ToUpper(), ref idUser, ref userName, ref level))
            {

                Form1 Main = new Form1(idUser, userName, level,m_type_scan);
                this.Hide();
                try
                {
                    //splashthread.Abort();
                    //SplashScreen.CloseSplashScreen();
                    m_alerts.playSuccess();
                    Main.ShowDialog();
                    Cursor = Cursors.Arrow;
                }
                catch(Exception ex)
                {
                    m_alerts.playWarning();
                    MessageBox.Show(ex.Message);
                }
                this.Show();
                txtUser.Text = "";
                txtPass.Text = "";
                txtUser.Focus();
            }
            else
            {
                m_alerts.playWarning();
                MessageBox.Show("Usuario o password incorrectos.", "Login...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUser.Text = "";
                txtPass.Text = "";
                txtUser.Focus();
            }
            //splashthread.Abort();
            //SplashScreen.CloseSplashScreen();
            progressBar1.Visible = false;
        }

        private bool SignInDB(String user, string pass, ref int idUser, ref String userName, ref int level)
        {
            siixsem_main_dbEntities db = new siixsem_main_dbEntities();
            string strPass = GetSHA1(pass);
            Cursor = Cursors.WaitCursor;
            validate_user_Result res = db.validate_user(user, strPass).First();
            //Cursor = Cursors.Arrow;

            idUser = res.se_id_user;
            userName = res.se_name;
            level = res.se_level;
            

            return res.RESULT == 1 ? true : false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                login();
            }
        }

        private void btnAccept_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                login();
            }
        }

        private void Login_Shown(object sender, EventArgs e)
        {
            txtUser.Focus();
        }

        private void selBox_CheckedChanged(object sender, EventArgs e)
        {
            m_type_scan = 1;
        }

        private void selMag_CheckedChanged(object sender, EventArgs e)
        {
            m_type_scan = 2;
        }
    }
}
