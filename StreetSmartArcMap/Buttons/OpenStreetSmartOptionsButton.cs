using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Forms;

namespace StreetSmartArcMap.Buttons
{
    public class OpenStreetSmartOptionsButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            StreetSmartConfigurationForm.OpenCloseSwitch();
        }
    }
}
