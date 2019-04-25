using IntegrationArcMap.Utilities;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Logic.Configuration;
using System;
using System.Drawing;
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
            txtKey.Text = _config.ApiKey;
        }

        private void LoadSpatialReferenceData()
        {
            //Clear existing items
            cbCycloramaSRS.Items.Clear();
            cbRecordingsSRS.Items.Clear();
            var selectedCMSRS = default(SpatialReference);
            var selectedRCSRS = default(SpatialReference);
            SpatialReferences spatialReferences = SpatialReferences.Instance;
            foreach (var spatialReference in spatialReferences)
            {
                if (spatialReference.KnownInArcMap)
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

        private void StreetSmartConfigurationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _StreetSmartConfigurationForm = null;
        }

        private void SetFont(Control parent)
        {
            Font font = SystemFonts.MenuFont;

            foreach(Control child in parent.Controls)
            {
                var fontProperty = child.GetType().GetProperty("Font");

                fontProperty?.SetValue(child, (Font)font.Clone());

                if (child.Controls.Count > 0)
                    SetFont(child);
            }
        } 

    }
}
