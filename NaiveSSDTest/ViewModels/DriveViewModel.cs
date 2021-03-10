using JToolbox.Core.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NaiveSSDTest.ViewModels
{
    public class DriveViewModel
    {

        public DriveViewModel(DriveInfo driveInfo)
        {
            DriveInfo = driveInfo;
        }

        public DriveInfo DriveInfo { get; }

        public string Display
        {
            get
            {
                var infos = new List<string>
                {
                    DriveInfo.VolumeLabel,
                    Misc.BytesToString(DriveInfo.AvailableFreeSpace) + " free"
                };
                var infosDisp = string.Join(", ", infos.Where(i => !string.IsNullOrEmpty(i)));
                return $"{DriveInfo.RootDirectory.FullName} ({infosDisp})";
            }
        }
    }
}