using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreetSmartArcMap.Utilities
{
    public static class FormStyling
    {
        public static void SetStyling(Control parent)
        {
            SetFont(parent);
            SetResources(parent, new CultureInfo(Configuration.Configuration.Instance.Culture));
        }

        public static void SetFont(Control parent)
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

        public static void SetResources(Control parent, CultureInfo culture, ComponentResourceManager resources = null)
        {
            if (resources == null)
            {
                Configuration.Configuration.Instance.SetCulture();

                resources = new ComponentResourceManager(parent.GetType());
            }

            resources.ApplyResources(parent, parent.Name, culture);

            foreach (Control child in parent.Controls)
            {
                SetResources(child, culture, resources);
            }
        }
    }
}
