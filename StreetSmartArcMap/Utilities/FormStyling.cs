using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreetSmartArcMap.Utilities
{
    public static class FormStyling
    {
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
    }
}
