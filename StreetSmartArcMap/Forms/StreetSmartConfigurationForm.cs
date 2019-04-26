/*
 * Integration in ArcMap for StreetSmart
 * Copyright (c) 2019, CycloMedia, All rights reserved.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */

using IntegrationArcMap.Utilities;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Logic.Configuration;
using StreetSmartArcMap.Utilities;
using System;
using System.Diagnostics;
using System.Reflection;
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

            FormStyling.SetFont(this);

            SetAbout();
            SetAgreement();
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

        public static void OpenForm()
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

        private void ShowAgreement()
        {
            tcSettings.SelectTab(tbAgreement);
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

        private void SetAbout()
        {
            // Assembly info
            var assembly = Assembly.GetExecutingAssembly();
            var info = FileVersionInfo.GetVersionInfo(assembly.Location);

            // Street Smart API info
            var type = typeof(StreetSmart.WinForms.StreetSmartGUI);
            var apiAssembly = type.Assembly;
            var apiInfo = FileVersionInfo.GetVersionInfo(apiAssembly.Location);

            string[] text =
            {
                $"{info.ProductName} {assembly.GetName().Version.ToString(3)}",
                $"{apiInfo.ProductName} {apiAssembly.GetName().Version.ToString(3)}",
                info.LegalCopyright,
                "https://www.cyclomedia.com"
            };

            rtbAbout.Text = string.Join(Environment.NewLine, text);
        }

        private void rtbAbout_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void SetAgreement()
        {
            txtAgreement.Text = Properties.Resources.Agreement;
        }
    }
}
