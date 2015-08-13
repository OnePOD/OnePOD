using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows;

namespace OnePOD
{
    class PodInterop
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);

        private static UInt32 SPI_SETDESKWALLPAPER = 20;
        private static UInt32 SPIF_UPDATEINIFILE = 0x1;

        private static string WP_S = @"WallpaperStyle";
        private static string WP_T = @"TileWallpaper";

        public static bool SetImage(string filePath)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                string wps = key.GetValue(WP_S).ToString();
                if (wps != "0")
                    key.SetValue(WP_S, 0.ToString()); // 0 = original size
                if (key.GetValue(WP_T).ToString() != 0.ToString())
                    key.SetValue(WP_T, 0.ToString()); // 0 = no tile
            }
            catch (Exception e)
            {
                PodUtil.Error(e.Message);
                return false;
            }
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath, SPIF_UPDATEINIFILE);
            return true;
        }
    }
}
