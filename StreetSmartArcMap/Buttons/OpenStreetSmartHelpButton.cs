using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Microsoft.Win32;
using StreetSmartArcMap.AddIns;
using StreetSmartArcMap.Forms;
using StreetSmartArcMap.Logic.Utilities;

namespace StreetSmartArcMap.Buttons
{
    public class OpenStreetSmartHelpButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private Process _process;

        public OpenStreetSmartHelpButton()
        {
            _process = null;
        }

        protected override void OnClick()
        {
            try
            {
                OnUpdate();
                var adobePath = Registry.GetValue(@"HKEY_CLASSES_ROOT\Software\Adobe\Acrobat\Exe", string.Empty, string.Empty);

                if (adobePath != null)
                {
                    if (_process == null)
                    {
                        Type thisType = GetType();
                        Assembly thisAssembly = Assembly.GetAssembly(thisType);
                        const string manualName = "Street Smart for ArcMap User Manual.pdf";
                        const string manualPath = @"StreetSmartArcMap.Doc." + manualName;
                        Stream manualStream = thisAssembly.GetManifestResourceStream(manualPath);
                        string fileName = Path.Combine(FileUtils.FileDir, manualName);

                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }

                        if (manualStream != null)
                        {
                            var fileStream = new FileStream(fileName, FileMode.CreateNew);
                            const int readBuffer = 2048;
                            var buffer = new byte[readBuffer];
                            int readBytes;

                            do
                            {
                                readBytes = manualStream.Read(buffer, 0, readBuffer);
                                fileStream.Write(buffer, 0, readBytes);
                            } while (readBytes != 0);

                            fileStream.Flush();
                            fileStream.Close();

                            var processInfo = new ProcessStartInfo
                            {
                                FileName = fileName,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true,
                            };

                            _process = Process.Start(processInfo);

                            if (_process != null)
                            {
                                _process.EnableRaisingEvents = true;
                                _process.Exited += ExitProcess;
                            }
                        }
                    }
                    else
                    {
                        _process.Kill();
                    }
                }
                else
                {
                    MessageBox.Show("Adobe reader is not installed on your system");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Help error");
            }
        }

        protected override void OnUpdate()
        {
            Enabled = StreetSmartExtension.GetExtension()?.IsEnabled ?? false;

            Caption = Properties.Resources.OpenStreetSmartHelpButtonCaption;
            Tooltip = Properties.Resources.OpenStreetSmartHelpButtonTip;
        }

        private void ExitProcess(object sender, EventArgs e)
        {
            _process.Exited -= ExitProcess;
            _process = null;
            OnUpdate();
        }
    }
}
