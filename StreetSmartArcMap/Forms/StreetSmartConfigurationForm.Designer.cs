namespace StreetSmartArcMap.Forms
{
    partial class StreetSmartConfigurationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StreetSmartConfigurationForm));
            this.plButtons = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.tcSettings = new System.Windows.Forms.TabControl();
            this.tbSettings = new System.Windows.Forms.TabPage();
            this.grLogin = new System.Windows.Forms.GroupBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.lblLogin = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudOverlayDrawDistance = new System.Windows.Forms.NumericUpDown();
            this.grCoordinateSystems = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbRecordingsSRS = new System.Windows.Forms.ComboBox();
            this.lblCycloramaSRS = new System.Windows.Forms.Label();
            this.cbCycloramaSRS = new System.Windows.Forms.ComboBox();
            this.tbConfiguration = new System.Windows.Forms.TabPage();
            this.grCycloramaVectorLayerLocation = new System.Windows.Forms.GroupBox();
            this.lblLocationCycloramaVectorLayerLocation = new System.Windows.Forms.Label();
            this.ckDefaultCycloramaVectorLayerLocation = new System.Windows.Forms.CheckBox();
            this.txtCycloramaVectorLayerLocation = new System.Windows.Forms.TextBox();
            this.grProxyServer = new System.Windows.Forms.GroupBox();
            this.lblProxyDomain = new System.Windows.Forms.Label();
            this.txtProxyDomain = new System.Windows.Forms.TextBox();
            this.lblProxyPassword = new System.Windows.Forms.Label();
            this.txtProxyPassword = new System.Windows.Forms.TextBox();
            this.lblProxyUserName = new System.Windows.Forms.Label();
            this.txtProxyUsername = new System.Windows.Forms.TextBox();
            this.ckUseDefaultProxyCredentials = new System.Windows.Forms.CheckBox();
            this.ckBypassProxyOnLocal = new System.Windows.Forms.CheckBox();
            this.lblProxyPort = new System.Windows.Forms.Label();
            this.txtProxyPort = new System.Windows.Forms.TextBox();
            this.lblProxyAddress = new System.Windows.Forms.Label();
            this.ckUseProxyServer = new System.Windows.Forms.CheckBox();
            this.txtProxyAddress = new System.Windows.Forms.TextBox();
            this.grRecordingService = new System.Windows.Forms.GroupBox();
            this.lblLocationRecordingService = new System.Windows.Forms.Label();
            this.ckDefaultRecordingService = new System.Windows.Forms.CheckBox();
            this.txtRecordingServiceLocation = new System.Windows.Forms.TextBox();
            this.grBaseUrl = new System.Windows.Forms.GroupBox();
            this.lblLocationBaseUrl = new System.Windows.Forms.Label();
            this.ckDefaultBaseUrl = new System.Windows.Forms.CheckBox();
            this.txtBaseUrlLocation = new System.Windows.Forms.TextBox();
            this.grSwfUrl = new System.Windows.Forms.GroupBox();
            this.lblLocationSwfUrl = new System.Windows.Forms.Label();
            this.ckDefaultSwfUrl = new System.Windows.Forms.CheckBox();
            this.txtSwfUrlLocation = new System.Windows.Forms.TextBox();
            this.tbAbout = new System.Windows.Forms.TabPage();
            this.rtbAbout = new System.Windows.Forms.RichTextBox();
            this.tbAgreement = new System.Windows.Forms.TabPage();
            this.txtAgreement = new System.Windows.Forms.TextBox();
            this.plButtons.SuspendLayout();
            this.tcSettings.SuspendLayout();
            this.tbSettings.SuspendLayout();
            this.grLogin.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOverlayDrawDistance)).BeginInit();
            this.grCoordinateSystems.SuspendLayout();
            this.tbConfiguration.SuspendLayout();
            this.grCycloramaVectorLayerLocation.SuspendLayout();
            this.grProxyServer.SuspendLayout();
            this.grRecordingService.SuspendLayout();
            this.grBaseUrl.SuspendLayout();
            this.grSwfUrl.SuspendLayout();
            this.tbAbout.SuspendLayout();
            this.tbAgreement.SuspendLayout();
            this.SuspendLayout();
            // 
            // plButtons
            // 
            resources.ApplyResources(this.plButtons, "plButtons");
            this.plButtons.Controls.Add(this.btnOk);
            this.plButtons.Controls.Add(this.btnCancel);
            this.plButtons.Controls.Add(this.btnApply);
            this.plButtons.Name = "plButtons";
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.Name = "btnApply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // tcSettings
            // 
            resources.ApplyResources(this.tcSettings, "tcSettings");
            this.tcSettings.Controls.Add(this.tbSettings);
            this.tcSettings.Controls.Add(this.tbConfiguration);
            this.tcSettings.Controls.Add(this.tbAbout);
            this.tcSettings.Controls.Add(this.tbAgreement);
            this.tcSettings.Multiline = true;
            this.tcSettings.Name = "tcSettings";
            this.tcSettings.SelectedIndex = 0;
            // 
            // tbSettings
            // 
            resources.ApplyResources(this.tbSettings, "tbSettings");
            this.tbSettings.Controls.Add(this.grLogin);
            this.tbSettings.Controls.Add(this.groupBox1);
            this.tbSettings.Controls.Add(this.grCoordinateSystems);
            this.tbSettings.Name = "tbSettings";
            this.tbSettings.UseVisualStyleBackColor = true;
            // 
            // grLogin
            // 
            resources.ApplyResources(this.grLogin, "grLogin");
            this.grLogin.Controls.Add(this.btnLogin);
            this.grLogin.Controls.Add(this.lblLogin);
            this.grLogin.Controls.Add(this.lblUsername);
            this.grLogin.Controls.Add(this.txtUsername);
            this.grLogin.Controls.Add(this.txtPassword);
            this.grLogin.Controls.Add(this.lblPassword);
            this.grLogin.Name = "grLogin";
            this.grLogin.TabStop = false;
            // 
            // btnLogin
            // 
            resources.ApplyResources(this.btnLogin, "btnLogin");
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lblLogin
            // 
            resources.ApplyResources(this.lblLogin, "lblLogin");
            this.lblLogin.Name = "lblLogin";
            // 
            // lblUsername
            // 
            resources.ApplyResources(this.lblUsername, "lblUsername");
            this.lblUsername.Name = "lblUsername";
            // 
            // txtUsername
            // 
            resources.ApplyResources(this.txtUsername, "txtUsername");
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtUsername_KeyUp);
            // 
            // txtPassword
            // 
            resources.ApplyResources(this.txtPassword, "txtPassword");
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyUp);
            // 
            // lblPassword
            // 
            resources.ApplyResources(this.lblPassword, "lblPassword");
            this.lblPassword.Name = "lblPassword";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.nudOverlayDrawDistance);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // nudOverlayDrawDistance
            // 
            resources.ApplyResources(this.nudOverlayDrawDistance, "nudOverlayDrawDistance");
            this.nudOverlayDrawDistance.Name = "nudOverlayDrawDistance";
            this.nudOverlayDrawDistance.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // grCoordinateSystems
            // 
            resources.ApplyResources(this.grCoordinateSystems, "grCoordinateSystems");
            this.grCoordinateSystems.Controls.Add(this.label1);
            this.grCoordinateSystems.Controls.Add(this.cbRecordingsSRS);
            this.grCoordinateSystems.Controls.Add(this.lblCycloramaSRS);
            this.grCoordinateSystems.Controls.Add(this.cbCycloramaSRS);
            this.grCoordinateSystems.Name = "grCoordinateSystems";
            this.grCoordinateSystems.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cbRecordingsSRS
            // 
            resources.ApplyResources(this.cbRecordingsSRS, "cbRecordingsSRS");
            this.cbRecordingsSRS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRecordingsSRS.FormattingEnabled = true;
            this.cbRecordingsSRS.Name = "cbRecordingsSRS";
            this.cbRecordingsSRS.Sorted = true;
            // 
            // lblCycloramaSRS
            // 
            resources.ApplyResources(this.lblCycloramaSRS, "lblCycloramaSRS");
            this.lblCycloramaSRS.Name = "lblCycloramaSRS";
            // 
            // cbCycloramaSRS
            // 
            resources.ApplyResources(this.cbCycloramaSRS, "cbCycloramaSRS");
            this.cbCycloramaSRS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCycloramaSRS.FormattingEnabled = true;
            this.cbCycloramaSRS.Name = "cbCycloramaSRS";
            this.cbCycloramaSRS.Sorted = true;
            // 
            // tbConfiguration
            // 
            resources.ApplyResources(this.tbConfiguration, "tbConfiguration");
            this.tbConfiguration.Controls.Add(this.grCycloramaVectorLayerLocation);
            this.tbConfiguration.Controls.Add(this.grProxyServer);
            this.tbConfiguration.Controls.Add(this.grRecordingService);
            this.tbConfiguration.Controls.Add(this.grBaseUrl);
            this.tbConfiguration.Controls.Add(this.grSwfUrl);
            this.tbConfiguration.Name = "tbConfiguration";
            this.tbConfiguration.UseVisualStyleBackColor = true;
            // 
            // grCycloramaVectorLayerLocation
            // 
            resources.ApplyResources(this.grCycloramaVectorLayerLocation, "grCycloramaVectorLayerLocation");
            this.grCycloramaVectorLayerLocation.Controls.Add(this.lblLocationCycloramaVectorLayerLocation);
            this.grCycloramaVectorLayerLocation.Controls.Add(this.ckDefaultCycloramaVectorLayerLocation);
            this.grCycloramaVectorLayerLocation.Controls.Add(this.txtCycloramaVectorLayerLocation);
            this.grCycloramaVectorLayerLocation.Name = "grCycloramaVectorLayerLocation";
            this.grCycloramaVectorLayerLocation.TabStop = false;
            // 
            // lblLocationCycloramaVectorLayerLocation
            // 
            resources.ApplyResources(this.lblLocationCycloramaVectorLayerLocation, "lblLocationCycloramaVectorLayerLocation");
            this.lblLocationCycloramaVectorLayerLocation.Name = "lblLocationCycloramaVectorLayerLocation";
            // 
            // ckDefaultCycloramaVectorLayerLocation
            // 
            resources.ApplyResources(this.ckDefaultCycloramaVectorLayerLocation, "ckDefaultCycloramaVectorLayerLocation");
            this.ckDefaultCycloramaVectorLayerLocation.Name = "ckDefaultCycloramaVectorLayerLocation";
            this.ckDefaultCycloramaVectorLayerLocation.UseVisualStyleBackColor = true;
            // 
            // txtCycloramaVectorLayerLocation
            // 
            resources.ApplyResources(this.txtCycloramaVectorLayerLocation, "txtCycloramaVectorLayerLocation");
            this.txtCycloramaVectorLayerLocation.Name = "txtCycloramaVectorLayerLocation";
            // 
            // grProxyServer
            // 
            resources.ApplyResources(this.grProxyServer, "grProxyServer");
            this.grProxyServer.Controls.Add(this.lblProxyDomain);
            this.grProxyServer.Controls.Add(this.txtProxyDomain);
            this.grProxyServer.Controls.Add(this.lblProxyPassword);
            this.grProxyServer.Controls.Add(this.txtProxyPassword);
            this.grProxyServer.Controls.Add(this.lblProxyUserName);
            this.grProxyServer.Controls.Add(this.txtProxyUsername);
            this.grProxyServer.Controls.Add(this.ckUseDefaultProxyCredentials);
            this.grProxyServer.Controls.Add(this.ckBypassProxyOnLocal);
            this.grProxyServer.Controls.Add(this.lblProxyPort);
            this.grProxyServer.Controls.Add(this.txtProxyPort);
            this.grProxyServer.Controls.Add(this.lblProxyAddress);
            this.grProxyServer.Controls.Add(this.ckUseProxyServer);
            this.grProxyServer.Controls.Add(this.txtProxyAddress);
            this.grProxyServer.Name = "grProxyServer";
            this.grProxyServer.TabStop = false;
            // 
            // lblProxyDomain
            // 
            resources.ApplyResources(this.lblProxyDomain, "lblProxyDomain");
            this.lblProxyDomain.Name = "lblProxyDomain";
            // 
            // txtProxyDomain
            // 
            resources.ApplyResources(this.txtProxyDomain, "txtProxyDomain");
            this.txtProxyDomain.Name = "txtProxyDomain";
            // 
            // lblProxyPassword
            // 
            resources.ApplyResources(this.lblProxyPassword, "lblProxyPassword");
            this.lblProxyPassword.Name = "lblProxyPassword";
            // 
            // txtProxyPassword
            // 
            resources.ApplyResources(this.txtProxyPassword, "txtProxyPassword");
            this.txtProxyPassword.Name = "txtProxyPassword";
            // 
            // lblProxyUserName
            // 
            resources.ApplyResources(this.lblProxyUserName, "lblProxyUserName");
            this.lblProxyUserName.Name = "lblProxyUserName";
            // 
            // txtProxyUsername
            // 
            resources.ApplyResources(this.txtProxyUsername, "txtProxyUsername");
            this.txtProxyUsername.Name = "txtProxyUsername";
            // 
            // ckUseDefaultProxyCredentials
            // 
            resources.ApplyResources(this.ckUseDefaultProxyCredentials, "ckUseDefaultProxyCredentials");
            this.ckUseDefaultProxyCredentials.Name = "ckUseDefaultProxyCredentials";
            this.ckUseDefaultProxyCredentials.UseVisualStyleBackColor = true;
            // 
            // ckBypassProxyOnLocal
            // 
            resources.ApplyResources(this.ckBypassProxyOnLocal, "ckBypassProxyOnLocal");
            this.ckBypassProxyOnLocal.Name = "ckBypassProxyOnLocal";
            this.ckBypassProxyOnLocal.UseVisualStyleBackColor = true;
            // 
            // lblProxyPort
            // 
            resources.ApplyResources(this.lblProxyPort, "lblProxyPort");
            this.lblProxyPort.Name = "lblProxyPort";
            // 
            // txtProxyPort
            // 
            resources.ApplyResources(this.txtProxyPort, "txtProxyPort");
            this.txtProxyPort.Name = "txtProxyPort";
            // 
            // lblProxyAddress
            // 
            resources.ApplyResources(this.lblProxyAddress, "lblProxyAddress");
            this.lblProxyAddress.Name = "lblProxyAddress";
            // 
            // ckUseProxyServer
            // 
            resources.ApplyResources(this.ckUseProxyServer, "ckUseProxyServer");
            this.ckUseProxyServer.Name = "ckUseProxyServer";
            this.ckUseProxyServer.UseVisualStyleBackColor = true;
            // 
            // txtProxyAddress
            // 
            resources.ApplyResources(this.txtProxyAddress, "txtProxyAddress");
            this.txtProxyAddress.Name = "txtProxyAddress";
            // 
            // grRecordingService
            // 
            resources.ApplyResources(this.grRecordingService, "grRecordingService");
            this.grRecordingService.Controls.Add(this.lblLocationRecordingService);
            this.grRecordingService.Controls.Add(this.ckDefaultRecordingService);
            this.grRecordingService.Controls.Add(this.txtRecordingServiceLocation);
            this.grRecordingService.Name = "grRecordingService";
            this.grRecordingService.TabStop = false;
            // 
            // lblLocationRecordingService
            // 
            resources.ApplyResources(this.lblLocationRecordingService, "lblLocationRecordingService");
            this.lblLocationRecordingService.Name = "lblLocationRecordingService";
            // 
            // ckDefaultRecordingService
            // 
            resources.ApplyResources(this.ckDefaultRecordingService, "ckDefaultRecordingService");
            this.ckDefaultRecordingService.Name = "ckDefaultRecordingService";
            this.ckDefaultRecordingService.UseVisualStyleBackColor = true;
            // 
            // txtRecordingServiceLocation
            // 
            resources.ApplyResources(this.txtRecordingServiceLocation, "txtRecordingServiceLocation");
            this.txtRecordingServiceLocation.Name = "txtRecordingServiceLocation";
            // 
            // grBaseUrl
            // 
            resources.ApplyResources(this.grBaseUrl, "grBaseUrl");
            this.grBaseUrl.Controls.Add(this.lblLocationBaseUrl);
            this.grBaseUrl.Controls.Add(this.ckDefaultBaseUrl);
            this.grBaseUrl.Controls.Add(this.txtBaseUrlLocation);
            this.grBaseUrl.Name = "grBaseUrl";
            this.grBaseUrl.TabStop = false;
            // 
            // lblLocationBaseUrl
            // 
            resources.ApplyResources(this.lblLocationBaseUrl, "lblLocationBaseUrl");
            this.lblLocationBaseUrl.Name = "lblLocationBaseUrl";
            // 
            // ckDefaultBaseUrl
            // 
            resources.ApplyResources(this.ckDefaultBaseUrl, "ckDefaultBaseUrl");
            this.ckDefaultBaseUrl.Name = "ckDefaultBaseUrl";
            this.ckDefaultBaseUrl.UseVisualStyleBackColor = true;
            // 
            // txtBaseUrlLocation
            // 
            resources.ApplyResources(this.txtBaseUrlLocation, "txtBaseUrlLocation");
            this.txtBaseUrlLocation.Name = "txtBaseUrlLocation";
            // 
            // grSwfUrl
            // 
            resources.ApplyResources(this.grSwfUrl, "grSwfUrl");
            this.grSwfUrl.Controls.Add(this.lblLocationSwfUrl);
            this.grSwfUrl.Controls.Add(this.ckDefaultSwfUrl);
            this.grSwfUrl.Controls.Add(this.txtSwfUrlLocation);
            this.grSwfUrl.Name = "grSwfUrl";
            this.grSwfUrl.TabStop = false;
            // 
            // lblLocationSwfUrl
            // 
            resources.ApplyResources(this.lblLocationSwfUrl, "lblLocationSwfUrl");
            this.lblLocationSwfUrl.Name = "lblLocationSwfUrl";
            // 
            // ckDefaultSwfUrl
            // 
            resources.ApplyResources(this.ckDefaultSwfUrl, "ckDefaultSwfUrl");
            this.ckDefaultSwfUrl.Name = "ckDefaultSwfUrl";
            this.ckDefaultSwfUrl.UseVisualStyleBackColor = true;
            // 
            // txtSwfUrlLocation
            // 
            resources.ApplyResources(this.txtSwfUrlLocation, "txtSwfUrlLocation");
            this.txtSwfUrlLocation.Name = "txtSwfUrlLocation";
            // 
            // tbAbout
            // 
            resources.ApplyResources(this.tbAbout, "tbAbout");
            this.tbAbout.Controls.Add(this.rtbAbout);
            this.tbAbout.Name = "tbAbout";
            this.tbAbout.UseVisualStyleBackColor = true;
            // 
            // rtbAbout
            // 
            resources.ApplyResources(this.rtbAbout, "rtbAbout");
            this.rtbAbout.BackColor = System.Drawing.SystemColors.Window;
            this.rtbAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbAbout.Name = "rtbAbout";
            this.rtbAbout.ReadOnly = true;
            this.rtbAbout.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbAbout_LinkClicked);
            // 
            // tbAgreement
            // 
            resources.ApplyResources(this.tbAgreement, "tbAgreement");
            this.tbAgreement.Controls.Add(this.txtAgreement);
            this.tbAgreement.Name = "tbAgreement";
            this.tbAgreement.UseVisualStyleBackColor = true;
            // 
            // txtAgreement
            // 
            resources.ApplyResources(this.txtAgreement, "txtAgreement");
            this.txtAgreement.Name = "txtAgreement";
            this.txtAgreement.ReadOnly = true;
            // 
            // StreetSmartConfigurationForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcSettings);
            this.Controls.Add(this.plButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StreetSmartConfigurationForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.StreetSmartConfigurationForm_FormClosed);
            this.plButtons.ResumeLayout(false);
            this.tcSettings.ResumeLayout(false);
            this.tbSettings.ResumeLayout(false);
            this.grLogin.ResumeLayout(false);
            this.grLogin.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOverlayDrawDistance)).EndInit();
            this.grCoordinateSystems.ResumeLayout(false);
            this.grCoordinateSystems.PerformLayout();
            this.tbConfiguration.ResumeLayout(false);
            this.grCycloramaVectorLayerLocation.ResumeLayout(false);
            this.grCycloramaVectorLayerLocation.PerformLayout();
            this.grProxyServer.ResumeLayout(false);
            this.grProxyServer.PerformLayout();
            this.grRecordingService.ResumeLayout(false);
            this.grRecordingService.PerformLayout();
            this.grBaseUrl.ResumeLayout(false);
            this.grBaseUrl.PerformLayout();
            this.grSwfUrl.ResumeLayout(false);
            this.grSwfUrl.PerformLayout();
            this.tbAbout.ResumeLayout(false);
            this.tbAgreement.ResumeLayout(false);
            this.tbAgreement.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plButtons;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TabControl tcSettings;
        private System.Windows.Forms.TabPage tbSettings;
        private System.Windows.Forms.GroupBox grCoordinateSystems;
        private System.Windows.Forms.Label lblCycloramaSRS;
        private System.Windows.Forms.ComboBox cbCycloramaSRS;
        private System.Windows.Forms.TabPage tbConfiguration;
        private System.Windows.Forms.GroupBox grCycloramaVectorLayerLocation;
        private System.Windows.Forms.Label lblLocationCycloramaVectorLayerLocation;
        private System.Windows.Forms.CheckBox ckDefaultCycloramaVectorLayerLocation;
        private System.Windows.Forms.TextBox txtCycloramaVectorLayerLocation;
        private System.Windows.Forms.GroupBox grProxyServer;
        private System.Windows.Forms.Label lblProxyDomain;
        private System.Windows.Forms.TextBox txtProxyDomain;
        private System.Windows.Forms.Label lblProxyPassword;
        private System.Windows.Forms.TextBox txtProxyPassword;
        private System.Windows.Forms.Label lblProxyUserName;
        private System.Windows.Forms.TextBox txtProxyUsername;
        private System.Windows.Forms.CheckBox ckUseDefaultProxyCredentials;
        private System.Windows.Forms.CheckBox ckBypassProxyOnLocal;
        private System.Windows.Forms.Label lblProxyPort;
        private System.Windows.Forms.TextBox txtProxyPort;
        private System.Windows.Forms.Label lblProxyAddress;
        private System.Windows.Forms.CheckBox ckUseProxyServer;
        private System.Windows.Forms.TextBox txtProxyAddress;
        private System.Windows.Forms.GroupBox grRecordingService;
        private System.Windows.Forms.Label lblLocationRecordingService;
        private System.Windows.Forms.CheckBox ckDefaultRecordingService;
        private System.Windows.Forms.TextBox txtRecordingServiceLocation;
        private System.Windows.Forms.GroupBox grBaseUrl;
        private System.Windows.Forms.Label lblLocationBaseUrl;
        private System.Windows.Forms.CheckBox ckDefaultBaseUrl;
        private System.Windows.Forms.TextBox txtBaseUrlLocation;
        private System.Windows.Forms.GroupBox grSwfUrl;
        private System.Windows.Forms.Label lblLocationSwfUrl;
        private System.Windows.Forms.CheckBox ckDefaultSwfUrl;
        private System.Windows.Forms.TextBox txtSwfUrlLocation;
        private System.Windows.Forms.TabPage tbAbout;
        private System.Windows.Forms.RichTextBox rtbAbout;
        private System.Windows.Forms.TabPage tbAgreement;
        private System.Windows.Forms.TextBox txtAgreement;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbRecordingsSRS;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudOverlayDrawDistance;
        private System.Windows.Forms.GroupBox grLogin;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
    }
}