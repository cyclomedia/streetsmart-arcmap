using System;
using System.IO;
using System.Reflection;

namespace StreetSmartArcMap.Logic.Utilities
{
    public class FileUtils
    {
        #region Properties

        public static string FileDir
        {
            get
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string result = Path.Combine(folder, "StreetSmartArcMap");

                if (!Directory.Exists(result))
                {
                    Directory.CreateDirectory(result);
                }

                return result;
            }
        }

        #endregion
    }
}
