using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using IntegrationArcMap.Utilities;
using StreetSmartArcMap.AddIns;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.DockableWindows;
using StreetSmartArcMap.Layers;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Logic.Model;
using StreetSmartArcMap.Logic.Model.Atlas;
using StreetSmartArcMap.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace StreetSmartArcMap.Forms
{
    public partial class CycloramaSearchForm : Form
    {
        #region members

        // =========================================================================
        // Members
        // =========================================================================
        private static CycloramaSearchForm _CycloramaSearchForm;

        public static bool IsActive => _CycloramaSearchForm != null;

        #endregion

        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        public static bool IsVisible
        {
            get { return _CycloramaSearchForm != null; }
        }

        #endregion

        #region constructor

        public CycloramaSearchForm()
        {
            InitializeComponent();

            FormStyling.SetStyling(this);
        }

        #endregion

        #region functions

        // =========================================================================
        // Functions
        // =========================================================================
        public static void OpenCloseSwitch()
        {
            if (_CycloramaSearchForm == null)
                OpenForm();
            else
                CloseForm();
        }

        public static void CloseForm()
        {
            _CycloramaSearchForm?.Close();
        }

        private static void OpenForm()
        {
            if (_CycloramaSearchForm == null)
            {
                _CycloramaSearchForm = new CycloramaSearchForm();
                var application = ArcMap.Application;
                int hWnd = application.hWnd;
                IWin32Window parent = new WindowWrapper(hWnd);
                _CycloramaSearchForm.Show(parent);
            }
        }

        #endregion

        #region event handlers

        // =========================================================================
        // Eventhandlers
        // =========================================================================

        private void btnFind_Click(object sender, EventArgs e)
        {
            backgroundImageSearch.RunWorkerAsync();
            lvResults.Items.Clear();
            btnFind.Enabled = false;
            btnClose.Enabled = false;
            txtImageId.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CycloramaSearchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _CycloramaSearchForm = null;
        }

        private void CycloramaSearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundImageSearch.IsBusy)
                e.Cancel = true;
        }

        #endregion

        private async void lvResults_DoubleClick(object sender, EventArgs e)
        {
            var extension = StreetSmartExtension.GetExtension();

            if (extension != null)
            {
                extension.ShowStreetSmart();

                foreach (ListViewItem selectedItem in lvResults.SelectedItems)
                {
                    var tag = selectedItem.Tag as object[];

                    if ((tag != null) && (tag.Length >= 2) && (selectedItem.SubItems.Count >= 1))
                    {
                        var feature = tag[0] as IFeature;
                        var cycloMediaLayer = tag[1] as CycloMediaLayer;
                        IActiveView activeView = ArcUtils.ActiveView;
                        ListViewItem.ListViewSubItem item = selectedItem.SubItems[0];

                        if ((feature != null) && (cycloMediaLayer != null) && (activeView != null) && (item != null))
                        {
                            var point = feature.Shape as IPoint;

                            if (point != null)
                            {
                                string imageId = item.Text;
                                await StreetSmartApiWrapper.Instance.Open(Configuration.Configuration.Instance.ApiSRS,
                                    imageId, ModifierKeys == Keys.Shift);
                            }

                            //Zoom to location on map
                            IEnvelope envelope = activeView.Extent;
                            envelope.CenterAt(point);
                            activeView.Extent = envelope;
                            activeView.Refresh();
                        }
                    }
                }
            }
        }

        private void backgroundImageSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            Web web = Web.Instance;
            string imageId = txtImageId.Text.Trim();
            StreetSmartExtension extension = StreetSmartExtension.GetExtension();
            CycloMediaGroupLayer groupLayer = extension?.CycloMediaGroupLayer;
            IList<CycloMediaLayer> layers = groupLayer?.Layers;

            if (layers != null)
            {
                foreach (var layer in layers)
                {
                    try
                    {
                        List<XElement> featureMemberElements = web.GetByImageId(imageId, layer);
                        e.Result = featureMemberElements;
                    }
                    catch (Exception)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void backgroundImageSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                var featureMemberElements = e.Result as List<XElement>;

                if (featureMemberElements != null)
                {
                    string imageId = txtImageId.Text.Trim();
                    StreetSmartExtension extension = StreetSmartExtension.GetExtension();
                    CycloMediaGroupLayer groupLayer = extension?.CycloMediaGroupLayer;
                    IList<CycloMediaLayer> layers = groupLayer?.Layers;

                    if (layers != null)
                    {
                        foreach (var layer in layers)
                        {
                            if (layer.IsVisible)
                            {
                                layer.SaveFeatureMembers(featureMemberElements, null);
                                IMappedFeature mappedFeature = layer.GetLocationInfo(imageId);
                                var recording = mappedFeature as Recording;

                                if (recording != null)
                                {
                                    DateTime? recordedAt = recording.RecordedAt;
                                    string recordedAtString = (recordedAt == null)
                                        ? string.Empty
                                        : ((DateTime) recordedAt).ToString(CultureInfo.InvariantCulture);

                                    string imageIdString = recording.ImageId;
                                    IFeature feature = layer.GetFeature(imageId);
                                    var items = new[] {imageIdString, recordedAtString};
                                    var listViewItem = new ListViewItem(items) {Tag = new object[] {feature, layer}};
                                    lvResults.Items.Add(listViewItem);
                                }
                            }
                        }
                    }
                }
            }

            btnFind.Enabled = true;
            btnClose.Enabled = true;
            txtImageId.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtImageId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(sender, e);
            }
        }
    }
}
