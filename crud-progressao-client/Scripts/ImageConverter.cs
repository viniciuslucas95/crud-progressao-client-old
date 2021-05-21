using crud_progressao_library.Scripts;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace crud_progressao.Scripts {
    internal static class ImageConverter {
        internal static BitmapImage StringToBitmapImage(string value) {
            if (string.IsNullOrEmpty(value)) return null;

            LogWritter.WriteLog("Trying to convert a string into a bitmap image");

            try {
                BitmapImage img = new();
                byte[] data = Convert.FromBase64String(value);
                using MemoryStream ms = new(data);
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = ms;
                img.EndInit();
                ms.Dispose();
                LogWritter.WriteLog("String converted");
                return img;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return null;
            }
        }

        internal static string BitmapImageToString(BitmapImage img) {
            if (img == null) return "";

            LogWritter.WriteLog("Trying to convert a bitmap image into a string");

            try {
                JpegBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create(img));
                byte[] data;
                using MemoryStream ms = new();
                encoder.Save(ms);
                data = ms.ToArray();
                ms.Dispose();
                LogWritter.WriteLog("Bitmap image converted");
                return Convert.ToBase64String(data);
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return "";
            }
        }
    }
}
