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

using StreetSmartArcMap.AddIns;
using StreetSmartArcMap.Forms;

namespace StreetSmartArcMap.Buttons
{
    public class OpenStreetSmartOptionsButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private Configuration.Configuration Config => Configuration.Configuration.Instance;

        public OpenStreetSmartOptionsButton()
        {
            Config.SetCulture();

            Caption = Properties.Resources.OpenStreetSmartOptionsButtonCaption;
            Tooltip = Properties.Resources.OpenStreetSmartOptionsButtonTip;
        }

        protected override void OnClick()
        {
            if (Config.Agreement)
                StreetSmartConfigurationForm.OpenCloseSwitch();
        }

        protected override void OnUpdate()
        {


            Enabled = StreetSmartExtension.GetExtension().IsEnabled;
            Checked = StreetSmartConfigurationForm.IsActive();

            Caption = Properties.Resources.OpenStreetSmartOptionsButtonCaption;
            Tooltip = Properties.Resources.OpenStreetSmartOptionsButtonTip;

            base.OnUpdate();
        }
    }
}