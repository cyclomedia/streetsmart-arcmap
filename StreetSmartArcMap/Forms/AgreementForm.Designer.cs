namespace StreetSmartArcMap.Forms
{
    partial class AgreementForm
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
            this.txtAgreement = new System.Windows.Forms.TextBox();
            this.ckAgreement = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtAgreement
            // 
            this.txtAgreement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAgreement.Location = new System.Drawing.Point(12, 12);
            this.txtAgreement.Multiline = true;
            this.txtAgreement.Name = "txtAgreement";
            this.txtAgreement.ReadOnly = true;
            this.txtAgreement.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAgreement.Size = new System.Drawing.Size(560, 506);
            this.txtAgreement.TabIndex = 1;
            // 
            // ckAgreement
            // 
            this.ckAgreement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ckAgreement.AutoSize = true;
            this.ckAgreement.Location = new System.Drawing.Point(12, 532);
            this.ckAgreement.Name = "ckAgreement";
            this.ckAgreement.Size = new System.Drawing.Size(224, 17);
            this.ckAgreement.TabIndex = 2;
            this.ckAgreement.Text = "I have read and agree to the terms of use.";
            this.ckAgreement.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(497, 524);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 25);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // AgreementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.ControlBox = false;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.ckAgreement);
            this.Controls.Add(this.txtAgreement);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AgreementForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Street Smart for ArcMap";
            this.Load += new System.EventHandler(this.AgreementForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAgreement;
        private System.Windows.Forms.CheckBox ckAgreement;
        private System.Windows.Forms.Button btnClose;
    }
}