/*
 * Integration in ArcMap for StreetSmart
 * Copyright (c) 2019 - 2020, CycloMedia, All rights reserved.
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

using StreetSmartArcMap.AddIns;
using StreetSmartArcMap.Forms;

namespace StreetSmartArcMap.Buttons
{
    public class OpenSearchImageIdButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private Configuration.Configuration Config => Configuration.Configuration.Instance;

        public OpenSearchImageIdButton()
        {
            Config.SetCulture();
            Caption = Properties.Resources.OpenSearchImageButtonCaption;
            Tooltip = Properties.Resources.OpenSearchImageButtonTip;
        }

        protected override void OnClick()
        {
            if (Config.Agreement)
                CycloramaSearchForm.OpenCloseSwitch();
        }

        protected override void OnUpdate()
        {
            Enabled = StreetSmartExtension.GetExtension()?.IsEnabled ?? false;
            Checked = CycloramaSearchForm.IsActive;

            Caption = Properties.Resources.OpenSearchImageButtonCaption;
            Tooltip = Properties.Resources.OpenSearchImageButtonTip;

            base.OnUpdate();
        }
    }
}