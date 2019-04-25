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
            this.ckAgreement = new System.Windows.Forms.CheckBox();
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
            this.plButtons.Controls.Add(this.btnOk);
            this.plButtons.Controls.Add(this.btnCancel);
            this.plButtons.Controls.Add(this.btnApply);
            this.plButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plButtons.Location = new System.Drawing.Point(0, 468);
            this.plButtons.Name = "plButtons";
            this.plButtons.Size = new System.Drawing.Size(432, 35);
            this.plButtons.TabIndex = 15;
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Location = new System.Drawing.Point(183, 5);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 25);
            this.btnOk.TabIndex = 10;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(264, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApply.Location = new System.Drawing.Point(345, 5);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 25);
            this.btnApply.TabIndex = 12;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // tcSettings
            // 
            this.tcSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcSettings.Controls.Add(this.tbSettings);
            this.tcSettings.Controls.Add(this.tbConfiguration);
            this.tcSettings.Controls.Add(this.tbAbout);
            this.tcSettings.Controls.Add(this.tbAgreement);
            this.tcSettings.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcSettings.Location = new System.Drawing.Point(12, 12);
            this.tcSettings.Multiline = true;
            this.tcSettings.Name = "tcSettings";
            this.tcSettings.SelectedIndex = 0;
            this.tcSettings.Size = new System.Drawing.Size(408, 450);
            this.tcSettings.TabIndex = 16;
            // 
            // tbSettings
            // 
            this.tbSettings.Controls.Add(this.grLogin);
            this.tbSettings.Controls.Add(this.groupBox1);
            this.tbSettings.Controls.Add(this.grCoordinateSystems);
            this.tbSettings.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSettings.Location = new System.Drawing.Point(4, 23);
            this.tbSettings.Name = "tbSettings";
            this.tbSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tbSettings.Size = new System.Drawing.Size(400, 423);
            this.tbSettings.TabIndex = 1;
            this.tbSettings.Text = "Settings";
            this.tbSettings.UseVisualStyleBackColor = true;
            // 
            // grLogin
            // 
            this.grLogin.Controls.Add(this.btnLogin);
            this.grLogin.Controls.Add(this.lblLogin);
            this.grLogin.Controls.Add(this.lblUsername);
            this.grLogin.Controls.Add(this.txtUsername);
            this.grLogin.Controls.Add(this.txtPassword);
            this.grLogin.Controls.Add(this.lblPassword);
            this.grLogin.Location = new System.Drawing.Point(6, 6);
            this.grLogin.Name = "grLogin";
            this.grLogin.Size = new System.Drawing.Size(388, 108);
            this.grLogin.TabIndex = 25;
            this.grLogin.TabStop = false;
            this.grLogin.Text = "Credentials";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(122, 73);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(100, 23);
            this.btnLogin.TabIndex = 9;
            this.btnLogin.Text = "Test connection";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(228, 77);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(0, 14);
            this.lblLogin.TabIndex = 8;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsername.Location = new System.Drawing.Point(6, 22);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(59, 14);
            this.lblUsername.TabIndex = 4;
            this.lblUsername.Text = "Username:";
            this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.Location = new System.Drawing.Point(122, 19);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(260, 20);
            this.txtUsername.TabIndex = 1;
            this.txtUsername.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtUsername_KeyUp);
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(122, 47);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(260, 20);
            this.txtPassword.TabIndex = 2;
            this.txtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyUp);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.Location = new System.Drawing.Point(6, 50);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(60, 14);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "Password:";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.nudOverlayDrawDistance);
            this.groupBox1.Location = new System.Drawing.Point(6, 208);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(388, 52);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Overlay draw distance (m):";
            // 
            // nudOverlayDrawDistance
            // 
            this.nudOverlayDrawDistance.Location = new System.Drawing.Point(162, 19);
            this.nudOverlayDrawDistance.Name = "nudOverlayDrawDistance";
            this.nudOverlayDrawDistance.Size = new System.Drawing.Size(57, 20);
            this.nudOverlayDrawDistance.TabIndex = 0;
            this.nudOverlayDrawDistance.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // grCoordinateSystems
            // 
            this.grCoordinateSystems.Controls.Add(this.label1);
            this.grCoordinateSystems.Controls.Add(this.cbRecordingsSRS);
            this.grCoordinateSystems.Controls.Add(this.lblCycloramaSRS);
            this.grCoordinateSystems.Controls.Add(this.cbCycloramaSRS);
            this.grCoordinateSystems.Location = new System.Drawing.Point(6, 120);
            this.grCoordinateSystems.Name = "grCoordinateSystems";
            this.grCoordinateSystems.Size = new System.Drawing.Size(388, 82);
            this.grCoordinateSystems.TabIndex = 21;
            this.grCoordinateSystems.TabStop = false;
            this.grCoordinateSystems.Text = "Coordinate systems";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 14);
            this.label1.TabIndex = 21;
            this.label1.Text = "Recordings layer:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbRecordingsSRS
            // 
            this.cbRecordingsSRS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbRecordingsSRS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRecordingsSRS.FormattingEnabled = true;
            this.cbRecordingsSRS.Location = new System.Drawing.Point(122, 47);
            this.cbRecordingsSRS.Name = "cbRecordingsSRS";
            this.cbRecordingsSRS.Size = new System.Drawing.Size(260, 22);
            this.cbRecordingsSRS.Sorted = true;
            this.cbRecordingsSRS.TabIndex = 22;
            // 
            // lblCycloramaSRS
            // 
            this.lblCycloramaSRS.AutoSize = true;
            this.lblCycloramaSRS.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCycloramaSRS.Location = new System.Drawing.Point(6, 22);
            this.lblCycloramaSRS.Name = "lblCycloramaSRS";
            this.lblCycloramaSRS.Size = new System.Drawing.Size(98, 14);
            this.lblCycloramaSRS.TabIndex = 19;
            this.lblCycloramaSRS.Text = "Cyclorama viewer:";
            this.lblCycloramaSRS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbCycloramaSRS
            // 
            this.cbCycloramaSRS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbCycloramaSRS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCycloramaSRS.FormattingEnabled = true;
            this.cbCycloramaSRS.Location = new System.Drawing.Point(122, 19);
            this.cbCycloramaSRS.Name = "cbCycloramaSRS";
            this.cbCycloramaSRS.Size = new System.Drawing.Size(260, 22);
            this.cbCycloramaSRS.Sorted = true;
            this.cbCycloramaSRS.TabIndex = 20;
            // 
            // tbConfiguration
            // 
            this.tbConfiguration.Controls.Add(this.grCycloramaVectorLayerLocation);
            this.tbConfiguration.Controls.Add(this.grProxyServer);
            this.tbConfiguration.Controls.Add(this.grRecordingService);
            this.tbConfiguration.Controls.Add(this.grBaseUrl);
            this.tbConfiguration.Controls.Add(this.grSwfUrl);
            this.tbConfiguration.Location = new System.Drawing.Point(4, 23);
            this.tbConfiguration.Name = "tbConfiguration";
            this.tbConfiguration.Size = new System.Drawing.Size(400, 423);
            this.tbConfiguration.TabIndex = 3;
            this.tbConfiguration.Text = "Configuration";
            this.tbConfiguration.UseVisualStyleBackColor = true;
            // 
            // grCycloramaVectorLayerLocation
            // 
            this.grCycloramaVectorLayerLocation.Controls.Add(this.lblLocationCycloramaVectorLayerLocation);
            this.grCycloramaVectorLayerLocation.Controls.Add(this.ckDefaultCycloramaVectorLayerLocation);
            this.grCycloramaVectorLayerLocation.Controls.Add(this.txtCycloramaVectorLayerLocation);
            this.grCycloramaVectorLayerLocation.Location = new System.Drawing.Point(3, 198);
            this.grCycloramaVectorLayerLocation.Name = "grCycloramaVectorLayerLocation";
            this.grCycloramaVectorLayerLocation.Size = new System.Drawing.Size(456, 65);
            this.grCycloramaVectorLayerLocation.TabIndex = 26;
            this.grCycloramaVectorLayerLocation.TabStop = false;
            this.grCycloramaVectorLayerLocation.Text = "Cyclorama vector layer location";
            // 
            // lblLocationCycloramaVectorLayerLocation
            // 
            this.lblLocationCycloramaVectorLayerLocation.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocationCycloramaVectorLayerLocation.Location = new System.Drawing.Point(3, 37);
            this.lblLocationCycloramaVectorLayerLocation.Name = "lblLocationCycloramaVectorLayerLocation";
            this.lblLocationCycloramaVectorLayerLocation.Size = new System.Drawing.Size(79, 20);
            this.lblLocationCycloramaVectorLayerLocation.TabIndex = 22;
            this.lblLocationCycloramaVectorLayerLocation.Text = "Location:";
            this.lblLocationCycloramaVectorLayerLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ckDefaultCycloramaVectorLayerLocation
            // 
            this.ckDefaultCycloramaVectorLayerLocation.AutoSize = true;
            this.ckDefaultCycloramaVectorLayerLocation.Location = new System.Drawing.Point(3, 15);
            this.ckDefaultCycloramaVectorLayerLocation.Name = "ckDefaultCycloramaVectorLayerLocation";
            this.ckDefaultCycloramaVectorLayerLocation.Size = new System.Drawing.Size(81, 18);
            this.ckDefaultCycloramaVectorLayerLocation.TabIndex = 20;
            this.ckDefaultCycloramaVectorLayerLocation.Text = "Use default";
            this.ckDefaultCycloramaVectorLayerLocation.UseVisualStyleBackColor = true;
            // 
            // txtCycloramaVectorLayerLocation
            // 
            this.txtCycloramaVectorLayerLocation.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCycloramaVectorLayerLocation.Location = new System.Drawing.Point(82, 37);
            this.txtCycloramaVectorLayerLocation.Name = "txtCycloramaVectorLayerLocation";
            this.txtCycloramaVectorLayerLocation.Size = new System.Drawing.Size(312, 20);
            this.txtCycloramaVectorLayerLocation.TabIndex = 21;
            // 
            // grProxyServer
            // 
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
            this.grProxyServer.Location = new System.Drawing.Point(3, 263);
            this.grProxyServer.Name = "grProxyServer";
            this.grProxyServer.Size = new System.Drawing.Size(456, 157);
            this.grProxyServer.TabIndex = 25;
            this.grProxyServer.TabStop = false;
            this.grProxyServer.Text = "Proxy server";
            // 
            // lblProxyDomain
            // 
            this.lblProxyDomain.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProxyDomain.Location = new System.Drawing.Point(3, 129);
            this.lblProxyDomain.Name = "lblProxyDomain";
            this.lblProxyDomain.Size = new System.Drawing.Size(79, 20);
            this.lblProxyDomain.TabIndex = 32;
            this.lblProxyDomain.Text = "Domain:";
            this.lblProxyDomain.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtProxyDomain
            // 
            this.txtProxyDomain.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProxyDomain.Location = new System.Drawing.Point(82, 129);
            this.txtProxyDomain.Name = "txtProxyDomain";
            this.txtProxyDomain.Size = new System.Drawing.Size(145, 20);
            this.txtProxyDomain.TabIndex = 31;
            // 
            // lblProxyPassword
            // 
            this.lblProxyPassword.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProxyPassword.Location = new System.Drawing.Point(228, 105);
            this.lblProxyPassword.Name = "lblProxyPassword";
            this.lblProxyPassword.Size = new System.Drawing.Size(79, 20);
            this.lblProxyPassword.TabIndex = 30;
            this.lblProxyPassword.Text = "Password:";
            this.lblProxyPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtProxyPassword
            // 
            this.txtProxyPassword.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProxyPassword.Location = new System.Drawing.Point(307, 105);
            this.txtProxyPassword.Name = "txtProxyPassword";
            this.txtProxyPassword.PasswordChar = '*';
            this.txtProxyPassword.Size = new System.Drawing.Size(87, 20);
            this.txtProxyPassword.TabIndex = 29;
            // 
            // lblProxyUserName
            // 
            this.lblProxyUserName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProxyUserName.Location = new System.Drawing.Point(3, 105);
            this.lblProxyUserName.Name = "lblProxyUserName";
            this.lblProxyUserName.Size = new System.Drawing.Size(79, 20);
            this.lblProxyUserName.TabIndex = 28;
            this.lblProxyUserName.Text = "Username:";
            this.lblProxyUserName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtProxyUsername
            // 
            this.txtProxyUsername.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProxyUsername.Location = new System.Drawing.Point(82, 105);
            this.txtProxyUsername.Name = "txtProxyUsername";
            this.txtProxyUsername.Size = new System.Drawing.Size(145, 20);
            this.txtProxyUsername.TabIndex = 27;
            // 
            // ckUseDefaultProxyCredentials
            // 
            this.ckUseDefaultProxyCredentials.AutoSize = true;
            this.ckUseDefaultProxyCredentials.Location = new System.Drawing.Point(33, 83);
            this.ckUseDefaultProxyCredentials.Name = "ckUseDefaultProxyCredentials";
            this.ckUseDefaultProxyCredentials.Size = new System.Drawing.Size(137, 18);
            this.ckUseDefaultProxyCredentials.TabIndex = 26;
            this.ckUseDefaultProxyCredentials.Text = "Use default credentials";
            this.ckUseDefaultProxyCredentials.UseVisualStyleBackColor = true;
            // 
            // ckBypassProxyOnLocal
            // 
            this.ckBypassProxyOnLocal.AutoSize = true;
            this.ckBypassProxyOnLocal.Location = new System.Drawing.Point(33, 61);
            this.ckBypassProxyOnLocal.Name = "ckBypassProxyOnLocal";
            this.ckBypassProxyOnLocal.Size = new System.Drawing.Size(226, 18);
            this.ckBypassProxyOnLocal.TabIndex = 25;
            this.ckBypassProxyOnLocal.Text = "Bypass proxy server for local addresses";
            this.ckBypassProxyOnLocal.UseVisualStyleBackColor = true;
            // 
            // lblProxyPort
            // 
            this.lblProxyPort.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProxyPort.Location = new System.Drawing.Point(228, 37);
            this.lblProxyPort.Name = "lblProxyPort";
            this.lblProxyPort.Size = new System.Drawing.Size(79, 20);
            this.lblProxyPort.TabIndex = 24;
            this.lblProxyPort.Text = "Port:";
            this.lblProxyPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtProxyPort
            // 
            this.txtProxyPort.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProxyPort.Location = new System.Drawing.Point(307, 37);
            this.txtProxyPort.Name = "txtProxyPort";
            this.txtProxyPort.Size = new System.Drawing.Size(87, 20);
            this.txtProxyPort.TabIndex = 23;
            // 
            // lblProxyAddress
            // 
            this.lblProxyAddress.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProxyAddress.Location = new System.Drawing.Point(3, 37);
            this.lblProxyAddress.Name = "lblProxyAddress";
            this.lblProxyAddress.Size = new System.Drawing.Size(79, 20);
            this.lblProxyAddress.TabIndex = 22;
            this.lblProxyAddress.Text = "Address:";
            this.lblProxyAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ckUseProxyServer
            // 
            this.ckUseProxyServer.AutoSize = true;
            this.ckUseProxyServer.Location = new System.Drawing.Point(3, 15);
            this.ckUseProxyServer.Name = "ckUseProxyServer";
            this.ckUseProxyServer.Size = new System.Drawing.Size(120, 18);
            this.ckUseProxyServer.TabIndex = 20;
            this.ckUseProxyServer.Text = "Use a proxy server";
            this.ckUseProxyServer.UseVisualStyleBackColor = true;
            // 
            // txtProxyAddress
            // 
            this.txtProxyAddress.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProxyAddress.Location = new System.Drawing.Point(82, 37);
            this.txtProxyAddress.Name = "txtProxyAddress";
            this.txtProxyAddress.Size = new System.Drawing.Size(145, 20);
            this.txtProxyAddress.TabIndex = 21;
            // 
            // grRecordingService
            // 
            this.grRecordingService.Controls.Add(this.lblLocationRecordingService);
            this.grRecordingService.Controls.Add(this.ckDefaultRecordingService);
            this.grRecordingService.Controls.Add(this.txtRecordingServiceLocation);
            this.grRecordingService.Location = new System.Drawing.Point(3, 68);
            this.grRecordingService.Name = "grRecordingService";
            this.grRecordingService.Size = new System.Drawing.Size(456, 65);
            this.grRecordingService.TabIndex = 24;
            this.grRecordingService.TabStop = false;
            this.grRecordingService.Text = "Recording Service";
            // 
            // lblLocationRecordingService
            // 
            this.lblLocationRecordingService.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocationRecordingService.Location = new System.Drawing.Point(3, 37);
            this.lblLocationRecordingService.Name = "lblLocationRecordingService";
            this.lblLocationRecordingService.Size = new System.Drawing.Size(79, 20);
            this.lblLocationRecordingService.TabIndex = 22;
            this.lblLocationRecordingService.Text = "Location:";
            this.lblLocationRecordingService.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ckDefaultRecordingService
            // 
            this.ckDefaultRecordingService.AutoSize = true;
            this.ckDefaultRecordingService.Location = new System.Drawing.Point(3, 15);
            this.ckDefaultRecordingService.Name = "ckDefaultRecordingService";
            this.ckDefaultRecordingService.Size = new System.Drawing.Size(81, 18);
            this.ckDefaultRecordingService.TabIndex = 20;
            this.ckDefaultRecordingService.Text = "Use default";
            this.ckDefaultRecordingService.UseVisualStyleBackColor = true;
            // 
            // txtRecordingServiceLocation
            // 
            this.txtRecordingServiceLocation.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRecordingServiceLocation.Location = new System.Drawing.Point(82, 37);
            this.txtRecordingServiceLocation.Name = "txtRecordingServiceLocation";
            this.txtRecordingServiceLocation.Size = new System.Drawing.Size(312, 20);
            this.txtRecordingServiceLocation.TabIndex = 21;
            // 
            // grBaseUrl
            // 
            this.grBaseUrl.Controls.Add(this.lblLocationBaseUrl);
            this.grBaseUrl.Controls.Add(this.ckDefaultBaseUrl);
            this.grBaseUrl.Controls.Add(this.txtBaseUrlLocation);
            this.grBaseUrl.Location = new System.Drawing.Point(3, 3);
            this.grBaseUrl.Name = "grBaseUrl";
            this.grBaseUrl.Size = new System.Drawing.Size(456, 65);
            this.grBaseUrl.TabIndex = 23;
            this.grBaseUrl.TabStop = false;
            this.grBaseUrl.Text = "Base url";
            // 
            // lblLocationBaseUrl
            // 
            this.lblLocationBaseUrl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocationBaseUrl.Location = new System.Drawing.Point(3, 37);
            this.lblLocationBaseUrl.Name = "lblLocationBaseUrl";
            this.lblLocationBaseUrl.Size = new System.Drawing.Size(79, 20);
            this.lblLocationBaseUrl.TabIndex = 22;
            this.lblLocationBaseUrl.Text = "Location:";
            this.lblLocationBaseUrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ckDefaultBaseUrl
            // 
            this.ckDefaultBaseUrl.AutoSize = true;
            this.ckDefaultBaseUrl.Location = new System.Drawing.Point(3, 15);
            this.ckDefaultBaseUrl.Name = "ckDefaultBaseUrl";
            this.ckDefaultBaseUrl.Size = new System.Drawing.Size(81, 18);
            this.ckDefaultBaseUrl.TabIndex = 20;
            this.ckDefaultBaseUrl.Text = "Use default";
            this.ckDefaultBaseUrl.UseVisualStyleBackColor = true;
            // 
            // txtBaseUrlLocation
            // 
            this.txtBaseUrlLocation.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBaseUrlLocation.Location = new System.Drawing.Point(82, 37);
            this.txtBaseUrlLocation.Name = "txtBaseUrlLocation";
            this.txtBaseUrlLocation.Size = new System.Drawing.Size(312, 20);
            this.txtBaseUrlLocation.TabIndex = 21;
            // 
            // grSwfUrl
            // 
            this.grSwfUrl.Controls.Add(this.lblLocationSwfUrl);
            this.grSwfUrl.Controls.Add(this.ckDefaultSwfUrl);
            this.grSwfUrl.Controls.Add(this.txtSwfUrlLocation);
            this.grSwfUrl.Location = new System.Drawing.Point(3, 133);
            this.grSwfUrl.Name = "grSwfUrl";
            this.grSwfUrl.Size = new System.Drawing.Size(456, 65);
            this.grSwfUrl.TabIndex = 22;
            this.grSwfUrl.TabStop = false;
            this.grSwfUrl.Text = "swf";
            // 
            // lblLocationSwfUrl
            // 
            this.lblLocationSwfUrl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocationSwfUrl.Location = new System.Drawing.Point(3, 37);
            this.lblLocationSwfUrl.Name = "lblLocationSwfUrl";
            this.lblLocationSwfUrl.Size = new System.Drawing.Size(79, 20);
            this.lblLocationSwfUrl.TabIndex = 22;
            this.lblLocationSwfUrl.Text = "Location:";
            this.lblLocationSwfUrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ckDefaultSwfUrl
            // 
            this.ckDefaultSwfUrl.AutoSize = true;
            this.ckDefaultSwfUrl.Location = new System.Drawing.Point(3, 15);
            this.ckDefaultSwfUrl.Name = "ckDefaultSwfUrl";
            this.ckDefaultSwfUrl.Size = new System.Drawing.Size(81, 18);
            this.ckDefaultSwfUrl.TabIndex = 20;
            this.ckDefaultSwfUrl.Text = "Use default";
            this.ckDefaultSwfUrl.UseVisualStyleBackColor = true;
            // 
            // txtSwfUrlLocation
            // 
            this.txtSwfUrlLocation.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSwfUrlLocation.Location = new System.Drawing.Point(82, 37);
            this.txtSwfUrlLocation.Name = "txtSwfUrlLocation";
            this.txtSwfUrlLocation.Size = new System.Drawing.Size(312, 20);
            this.txtSwfUrlLocation.TabIndex = 21;
            // 
            // tbAbout
            // 
            this.tbAbout.Controls.Add(this.rtbAbout);
            this.tbAbout.Location = new System.Drawing.Point(4, 23);
            this.tbAbout.Name = "tbAbout";
            this.tbAbout.Size = new System.Drawing.Size(400, 423);
            this.tbAbout.TabIndex = 4;
            this.tbAbout.Text = "About";
            this.tbAbout.UseVisualStyleBackColor = true;
            // 
            // rtbAbout
            // 
            this.rtbAbout.BackColor = System.Drawing.SystemColors.Window;
            this.rtbAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbAbout.Location = new System.Drawing.Point(3, 3);
            this.rtbAbout.Name = "rtbAbout";
            this.rtbAbout.ReadOnly = true;
            this.rtbAbout.Size = new System.Drawing.Size(394, 70);
            this.rtbAbout.TabIndex = 20;
            this.rtbAbout.Text = "";
            this.rtbAbout.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbAbout_LinkClicked);
            // 
            // tbAgreement
            // 
            this.tbAgreement.Controls.Add(this.ckAgreement);
            this.tbAgreement.Controls.Add(this.txtAgreement);
            this.tbAgreement.Location = new System.Drawing.Point(4, 23);
            this.tbAgreement.Name = "tbAgreement";
            this.tbAgreement.Size = new System.Drawing.Size(400, 423);
            this.tbAgreement.TabIndex = 5;
            this.tbAgreement.Text = "Agreement";
            this.tbAgreement.UseVisualStyleBackColor = true;
            // 
            // txtAgreement
            // 
            this.txtAgreement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAgreement.Location = new System.Drawing.Point(3, 3);
            this.txtAgreement.Multiline = true;
            this.txtAgreement.Name = "txtAgreement";
            this.txtAgreement.ReadOnly = true;
            this.txtAgreement.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAgreement.Size = new System.Drawing.Size(394, 393);
            this.txtAgreement.TabIndex = 0;
            // 
            // ckAgreement
            // 
            this.ckAgreement.AutoSize = true;
            this.ckAgreement.Location = new System.Drawing.Point(3, 402);
            this.ckAgreement.Name = "ckAgreement";
            this.ckAgreement.Size = new System.Drawing.Size(229, 18);
            this.ckAgreement.TabIndex = 1;
            this.ckAgreement.Text = "I have read and agree to the terms of use.";
            this.ckAgreement.UseVisualStyleBackColor = true;
            // 
            // StreetSmartConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 503);
            this.Controls.Add(this.tcSettings);
            this.Controls.Add(this.plButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StreetSmartConfigurationForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Street Smart for ArcMap";
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
        private System.Windows.Forms.CheckBox ckAgreement;
    }
}