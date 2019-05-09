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

using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Logic.Utilities;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using SystemIOFile = System.IO.File;
using StreetSmartArcMap.Client;
using System.Threading;
using System.Globalization;

namespace StreetSmartArcMap.Configuration
{
    [XmlRoot("Configuration")]
    public class Configuration : INotifyPropertyChanged, IStreetSmartOptions
    {

        public const string DefaultCulture = "en-GB";
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Members

        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(Configuration));
        private static Configuration _configuration;

        public string AddressLocale { get; set; }
        public string AddressDatabase { get; set; }
        public string AddressDefaultQuery { get; set; }

        public string ApiSRS { get; set; }
        public string ApiUsername { get; set; }
        public string ApiPassword { get; set; }

        public string BaseUrl { get; set; }
        public string RecordingsServiceUrl { get; set; }
        public string SpatialReferencesUrl { get; set; }

        public string DefaultRecordingSrs { get; set; }

        public int OverlayDrawDistanceInMeters { get; set; }

        public const string ApiKey = "O3Qd-D85a3YF6DkNmLEp-XU9OrQpGX8RG7IZi7UFKTAFO38ViDo9CD4xmbcdejcd";

        private static bool IsLoading { get; set; }
        public static EventHandler<bool> AgreementChanged;

        private bool _Agreement { get; set; }

        public bool Agreement
        {
            get
            {
                return _Agreement;
            }
            set
            {
                if (_Agreement != value)
                {
                    _Agreement = value;

                    if (!IsLoading)
                        AgreementChanged?.Invoke(this, value);
                }
            }
        }

        public string Culture { get; set; }

        #endregion

        #region Constructors

        static Configuration()
        {
            //
        }

        #endregion

        #region Properties

        public static Configuration Instance
        {
            get
            {
                if (_configuration == null)
                    Load();

                return _configuration ?? (_configuration = Create());
            }
        }

        private static string FileName => Path.Combine(FileUtils.FileDir, "Configuration.xml");

        public string LoggingLocation { get; set; }
        public bool UseLogging { get; set; }
        public bool CycloramaVectorLayerLocationDefault
        {
            get
            {
                return string.IsNullOrEmpty(CycloramaVectorLayerLocation);
            }
        }

        public string CycloramaVectorLayerLocation { get; set; }
        public SpatialReference SpatialReference { get; set; }
        #endregion

        #region Functions

        public void Save()
        {
            OnPropertyChanged();

            using (var input = SystemIOFile.Open(FileName, FileMode.Create))
            {
                Serializer.Serialize(input, this);
            }

            SetCulture(this);
        }

        private static void Load()
        {
            if (SystemIOFile.Exists(FileName))
            {
                using (var input = new FileStream(FileName, FileMode.Open))
                {
                    IsLoading = true;

                    var configuration = Serializer.Deserialize(input);

                    _configuration = (Configuration)configuration;

                    SetCulture(_configuration);

                    IsLoading = false;
                }
            }
        }

        public void SetCulture()
        {
            SetCulture(this);
        }

        private static void SetCulture(Configuration config)
        {
            if (string.IsNullOrWhiteSpace(config.Culture))
                config.Culture = DefaultCulture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(config.Culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(config.Culture);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static Configuration Create()
        {
            var result = new Configuration
            {
                AddressLocale = string.Empty,
                AddressDatabase = string.Empty,
                AddressDefaultQuery = string.Empty,

                ApiSRS = string.Empty,
                ApiUsername = string.Empty,
                ApiPassword = string.Empty,

                BaseUrl = "https://atlas.cyclomedia.com",
                RecordingsServiceUrl = "https://atlas.cyclomedia.com/recordings/wfs",
                SpatialReferencesUrl = "https://streetsmart.cyclomedia.com/api/v18.10/assets/srs/SpatialReference.xml",

                DefaultRecordingSrs = string.Empty,
                OverlayDrawDistanceInMeters = 30,

                Agreement = false,
                Culture = DefaultCulture,
            };

            result.Save();

            return result;
        }

        #endregion
    }
}
