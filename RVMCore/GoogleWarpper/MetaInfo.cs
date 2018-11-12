﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.GoogleWarpper
{
    static class MetaInfo
    {
        /// <summary>
        /// Get the file's Mime Type infomation.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        /// <param name="fileName">Target file's full path.</param>
        /// <returns>A MimeType string looks like "Application/Unknown"</returns>
        public static string GetMimeType(this string fileName)
        {
            if (!System.IO.File.Exists(fileName)) throw new FormatException("File name invaled or file doesn't exists!");
            string mimeType = "application/octet-stream";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            if (ext == ".ts") return ("video/MP2T");
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        public static string CheckStringForQuerry(this string querryString)
        {
            return querryString.Replace(@"'", @"\'");
        }

        public static bool IsFolder(this Google.Apis.Drive.v3.Data.File mfile) {
            if (mfile.MimeType == null) return false;
            return (mfile.MimeType.Equals("application/vnd.google-apps.folder"));
        }
    }
}
