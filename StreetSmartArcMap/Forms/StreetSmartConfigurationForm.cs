using IntegrationArcMap.Utilities;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Logic.Configuration;
using StreetSmartArcMap.Utilities;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace StreetSmartArcMap.Forms
{
    public partial class StreetSmartConfigurationForm : Form
    {
        private static StreetSmartConfigurationForm _StreetSmartConfigurationForm;

        private static Login _login;

        private Configuration _config;
        private bool _mssgBoxShow;

        public StreetSmartConfigurationForm()
        {
            InitializeComponent();

            _config = Configuration.Instance;
            _login = Client.Login.Instance;

            LoadLoginData();
            LoadSpatialReferenceData();
            LoadGeneralSettings();

            SetFont(this);
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

        private void LoadGeneralSettings()
        {
            nudOverlayDrawDistance.Value = _config.OverlayDrawDistanceInMeters;
        }

        private void LoadLoginData()
        {
            txtUsername.Text = _config.ApiUsername;
            txtPassword.Text = _config.ApiPassword;
        }

        private void LoadSpatialReferenceData()
        {
            //Clear existing items
            cbCycloramaSRS.Items.Clear();
            cbRecordingsSRS.Items.Clear();

            var selectedCMSRS = default(SpatialReference);
            var selectedRCSRS = default(SpatialReference);

            var boundary = ArcUtils.ActiveView?.Extent;

            SpatialReferences spatialReferences = SpatialReferences.Instance;

            foreach (var spatialReference in spatialReferences)
            {
                if (spatialReference.KnownInArcMap && spatialReference.WithinBoundary(boundary))
                {
                    cbCycloramaSRS.Items.Add(spatialReference);
                    cbRecordingsSRS.Items.Add(spatialReference);

                    if (spatialReference.SRSName == _config.ApiSRS)
                    {
                        selectedCMSRS = spatialReference;
                    }
                    if (spatialReference.SRSName == _config.DefaultRecordingSrs)
                    {
                        selectedRCSRS = spatialReference;
                    }

                }
            }
            if (selectedCMSRS != null)
            {
                cbCycloramaSRS.SelectedItem = selectedCMSRS;
            }
            if (selectedRCSRS != null)
            {
                cbRecordingsSRS.SelectedItem = selectedRCSRS;
            }
        }

        public static bool IsActive()
        {
            return (_StreetSmartConfigurationForm != null);
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
            var selectedSRS = (SpatialReference)cbCycloramaSRS.SelectedItem;
            _config.ApiSRS = selectedSRS?.SRSName ?? _config.ApiSRS;

            var selectedRecordingSRS = (SpatialReference)cbRecordingsSRS.SelectedItem;
            _config.DefaultRecordingSrs = selectedRecordingSRS?.SRSName ?? _config.DefaultRecordingSrs;

            var overlayDrawDistance = (int)nudOverlayDrawDistance.Value;
            if (overlayDrawDistance > -1 && overlayDrawDistance < 101)
            {
                _config.OverlayDrawDistanceInMeters = overlayDrawDistance;
                StreetSmartApiWrapper.Instance.SetOverlayDrawDistance(overlayDrawDistance, ArcMap.Document.FocusMap.MapUnits);
            }



            _config.Save();

            // TODO: do we need to restart the API here?

            if (close)
                Close();
        }

        private void txtUsername_KeyUp(object sender, KeyEventArgs e)
        {
            lblLogin.Text = string.Empty;

            if (((e.KeyCode == Keys.Enter) && (!string.IsNullOrEmpty(txtUsername.Text))) && (!_mssgBoxShow))
            {
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
            lblLogin.Text = string.Empty;

            if (((e.KeyCode == Keys.Enter) && (!string.IsNullOrEmpty(txtPassword.Text))) && (!_mssgBoxShow))
            {
                Login();
            }
            else
            {
                _mssgBoxShow = false;
            }

            btnApply.Enabled = true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblLogin.Text = string.Empty;

            Login();
        }

        private void Login()
        {
            _login.SetLoginCredentials(txtUsername.Text, txtPassword.Text);

            if (_login.Check())
                lblLogin.Text = Properties.Resources.LoginSuccessfully;
            else
                lblLogin.Text = Properties.Resources.LoginFailed;
        }

        private void StreetSmartConfigurationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _StreetSmartConfigurationForm = null;
        }

        private void SetFont(Control parent)
        {
            Font font = SystemFonts.MenuFont;

            foreach (Control child in parent.Controls)
            {
                var fontProperty = child.GetType().GetProperty("Font");

                fontProperty?.SetValue(child, (Font)font.Clone());

                if (child.Controls.Count > 0)
                    SetFont(child);
            }
        }
    }
}
