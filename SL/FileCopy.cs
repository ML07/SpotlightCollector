/* -------------------------------------------------------------------------------------------------
   Restricted - Copyright (C) Siemens Healthcare GmbH/Siemens Medical Solutions USA, Inc., 2019. All rights reserved
   ------------------------------------------------------------------------------------------------- */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace SL
{
    public class FileCopy
    {
        public string StartFileCopy()
        {
            // XmlSerializer xml = new XmlSerializer(myImportPath.GetType());
            // using (FileStream fs = File.Create("ExportedPath.xml"))
            // {
            //     xml.Serialize(fs, myImportPath);
            // }

            if (File.Exists(alreadyImportedFilenamesJson))
            {
                myImprtedFiles = JsonConvert.DeserializeObject<HashSet<string>>(File.ReadAllText(alreadyImportedFilenamesJson));
            }

            string myOutputPath = String.Empty;
            XmlSerializer destinationPath = new XmlSerializer(typeof(string));
            if (File.Exists(destinationPathConfigFileName))
            {
                using (var fs = File.OpenRead(destinationPathConfigFileName))
                {
                    myOutputPath = (string)destinationPath.Deserialize(fs);
                }
            }

            if (!Directory.Exists(myOutputPath))
            {
                myOutputPath = fallbackOutputPath;
            }

            string myOutputPathLandscape = Path.Combine(myOutputPath, "Landscape");
            string myOutputPathPortrait = Path.Combine(myOutputPath, "Portrait");

            myFilesToImport = @"c:\Users\" + Environment.UserName + @"\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";

            DirectoryInfo sourceFilesPath = new DirectoryInfo(myFilesToImport);
            if (!Directory.Exists(myOutputPathLandscape)) Directory.CreateDirectory(myOutputPathLandscape);
            if (!Directory.Exists(myOutputPathPortrait)) Directory.CreateDirectory(myOutputPathPortrait);

            bool newFilesNotFound = true;

            foreach (FileInfo sourceFile in sourceFilesPath.EnumerateFiles())
            {
                if (!myImprtedFiles.Contains(sourceFile.Name))
                {
                    newFilesNotFound = false;
                    myImprtedFiles.Add(sourceFile.Name);
                    if (sourceFile.Length < fileSizeLimitInKB * 1024)
                    {
                        myLogToFile.AppendLine(DateTime.Now + " - " + sourceFile.Name + $" - Skipped, because smaller than {fileSizeLimitInKB} KB.");
                        continue;
                    }
                    using (Image image = Image.FromFile(sourceFile.FullName))
                    {
                        if (image.Width >= image.Height)
                        {
                            sourceFile.CopyTo($"{myOutputPathLandscape}\\{sourceFile.Name}.png", true);
                            // myLogToFile.Insert(0, $"{DateTime.Now} - {sourceFile.Name} - Imported to Landscape.{Environment.NewLine}");
                            myLogToFile.AppendLine(DateTime.Now + " - " + sourceFile.Name + " - Imported to Landscape.");
                        }
                        else
                        {
                            sourceFile.CopyTo($"{myOutputPathPortrait}\\{sourceFile.Name}.png", true);
                            // myLogToFile.Insert(0, $"{DateTime.Now} - {sourceFile.Name} - Imported to Portrait.{Environment.NewLine}");
                            myLogToFile.AppendLine(DateTime.Now + " - " + sourceFile.Name + " - Imported to Portrait.");
                        }
                    }
                }
            }

            if (newFilesNotFound)
            {
                myLogToFile.AppendLine(DateTime.Now + " - No new files found.");
            }

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };

            string jsonString = JsonConvert.SerializeObject(myImprtedFiles, settings);
            File.WriteAllText(alreadyImportedFilenamesJson, jsonString);

            string LogsToUI = $"User: {Environment.UserName}\tOutputPath: {myOutputPath}\tBinDir: {Process.GetCurrentProcess().MainModule.FileName}\n\n" + myLogToFile.ToString();
            List<string> oldEntriesFromLogFileDescandingByDate = new List<string>();
            if (File.Exists(myLogFileName))
            {
                oldEntriesFromLogFileDescandingByDate = File.ReadLines(myLogFileName).Reverse().Take(20).ToList();

            }
            foreach (string oldLine in oldEntriesFromLogFileDescandingByDate)
            {
                LogsToUI = LogsToUI + oldLine + "\n";
            }

            File.AppendAllText(myLogFileName, myLogToFile.ToString());

            //List<string> result = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("ImportedFilenames.json"));

            return LogsToUI;
        }

        private StringBuilder myLogToFile = new StringBuilder();
        private HashSet<string> myImprtedFiles = new HashSet<string>();
        private string myFilesToImport;

        private const string alreadyImportedFilenamesJson = "AlreadyImportedFilenames.json";
        private const string destinationPathConfigFileName = "DestinationPath.xml";
        private const string fallbackOutputPath = @"c:\WindowsLogonScreenImageCollector";
        private const string myLogFileName = "LogFile.txt";
        private const long fileSizeLimitInKB = 100;
    }
}
