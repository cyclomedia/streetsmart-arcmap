using IntegrationArcMap.Utilities;
using StreetSmartArcMap.Logic.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreetSmartArcMap.Forms
{
    public partial class StreetSmartConfigurationForm : Form
    {
        private static StreetSmartConfigurationForm _StreetSmartConfigurationForm;

        private Configuration _config;
        private bool _mssgBoxShow;

        public StreetSmartConfigurationForm()
        {
            InitializeComponent();

            _config = Configuration.Instance;

            LoadLoginData();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Save(true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Save(false);
        }

        private void LoadLoginData()
        {
            txtUsername.Text = _config.ApiUsername;
            txtPassword.Text = _config.ApiPassword;
            txtKey.Text = _config.ApiKey;
        }

        private void LoadSpatialReferenceData()
        {
            //Clear existing items
            cbCycloramaSRS.Items.Clear();
            cbRecordingSRS.Items.Clear();

            SpatialReferences spatialReferences = SpatialReferences.Instance;
        }

        public static void OpenCloseSwitch()
        {
            if (_StreetSmartConfigurationForm == null)
                OpenForm();
            else
                CloseForm();
        }

        private static void OpenForm()
        {
            if (_StreetSmartConfigurationForm == null)
            {
                _StreetSmartConfigurationForm = new StreetSmartConfigurationForm();
                var application = ArcMap.Application;
                int hWnd = application.hWnd;
                IWin32Window parent = new WindowWrapper(hWnd);
                _StreetSmartConfigurationForm.Show(parent);
            }
        }

        public static void CloseForm()
        {
            _StreetSmartConfigurationForm?.Close();

            _StreetSmartConfigurationForm = null;
        }

        private void Save(bool close)
        {
            if (close)
                Close();
        }

        private void txtUsername_KeyUp(object sender, KeyEventArgs e)
        {
            if (((e.KeyCode == Keys.Enter) && (!string.IsNullOrEmpty(txtUsername.Text))) && (!_mssgBoxShow))
            {
                txtLoginStatus.Text = string.Empty;

                txtPassword.Focus();
            }
            else
            {
                _mssgBoxShow = false;
            }

            btnApply.Enabled = true;
        }

        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (((e.KeyCode == Keys.Enter) && (!string.IsNullOrEmpty(txtPassword.Text))) && (!_mssgBoxShow))
            {
                txtLoginStatus.Text = string.Empty;

                txtKey.Focus();
            }
            else
            {
                _mssgBoxShow = false;
            }

            btnApply.Enabled = true;
        }

        private void txtKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (((e.KeyCode == Keys.Enter) && (!string.IsNullOrEmpty(txtKey.Text))) && (!_mssgBoxShow))
            {
                Login();
            }
            else
            {
                _mssgBoxShow = false;
            }

            btnApply.Enabled = true;
        }

        private void Login()
        {
            //txtLoginStatus.Text = Properties.Resources.LoginSuccessfully;
        }
    }
}
