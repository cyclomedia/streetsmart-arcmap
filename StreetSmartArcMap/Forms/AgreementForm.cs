using StreetSmartArcMap.Utilities;
using System;
using System.Windows.Forms;

namespace StreetSmartArcMap.Forms
{
    public partial class AgreementForm : Form
    {
        private Configuration.Configuration Config => Configuration.Configuration.Instance;

        public AgreementForm()
        {
            InitializeComponent();

            FormStyling.SetStyling(this);
        }

        private void AgreementForm_Load(object sender, EventArgs e)
        {
            txtAgreement.Text = Properties.Resources.Agreement;
            txtAgreement.Select(0, 0);

            ckAgreement.Checked = Config.Agreement;
            ckAgreement.Focus();

            FormStyling.SetFont(this);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = ckAgreement.Checked ? DialogResult.Yes : DialogResult.No;

            Close();
        }

        public static bool ShowAgreement()
        {
            using (var form = new AgreementForm())
            {
                return form.ShowDialog() == DialogResult.Yes;
            };
        }
    }
}