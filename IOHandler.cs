﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace VK
{


    class Synchronizer<T> {

        private List<T> existFiles;
        private List<T> skippedFiles;
        
        private FileInfo[] folderFiles;
        private DirectoryInfo dir;

        public Synchronizer(string dir)
        {
            if (Directory.Exists(dir))
            {
                this.existFiles = new List<T>();
                this.dir = new DirectoryInfo(dir);
                this.skippedFiles = new List<T>();
            }
            else
            {
                throw new Exception("Folder doesnt exist");
            }
        }

        public void ComputeFileList()
        {
            folderFiles = dir.GetFiles();
        }

        public void Synchronize(List<T> containValues, Func<FileInfo[], T, bool> Check)
        {
            for (int i=0; i< containValues.Count; i++)
                if (!Check(folderFiles,containValues[i]))
                    existFiles.Add(containValues[i]);
        }


        public List<T> GetDownloadList()
        {
            return existFiles;
        }

    }

    public class SynhronizeAdapter
    {
        public void SyncFolderWithList<T>(List<T> sounds, string directory) where T : IData
        {
            Downloader downloader = new Downloader(directory);

            if (!System.IO.File.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);

            try
            {
                SyncFiles<T>(sounds, directory, downloader);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void SyncFiles<T>(List<T> values, string directory, Downloader downloader) where T : IData
        {
            Synchronizer<T> sync = new Synchronizer<T>(directory);
            sync.ComputeFileList();

            sync.Synchronize(values, CompareAudioAndFile);
            List<T> valuesToDownload = sync.GetDownloadList();
            for (int i = 0; i < valuesToDownload.Count; i++)
                downloader.Download(valuesToDownload[i].GetUrl(),
                                    valuesToDownload[i].GenerateFileName() + valuesToDownload[i].GenerateFileExtention());
        }

        private bool CompareAudioAndFile<T>(System.IO.FileInfo[] files, T value) where T : IData
        {
            string valueFileName = value.GenerateFileName();
            return Array.Exists<System.IO.FileInfo>(files, p => (IOHandler.ParseFileName(p) == valueFileName));
        }
    }

    class IOHandler
    {

        public static string ParseFileName(System.IO.FileInfo file)
        {
            return file.Name.Remove(file.Name.IndexOf(file.Extension));
        }

        public static void Write(string path, string value)
        {
            System.IO.File.WriteAllText(path, value);
        }

        public static void ClearFolder(string dir) 
        {
            foreach (string file in Directory.GetFiles(dir))
                File.Delete(file);
        }

        public static void OpenPath(string dir)
        {
            System.Diagnostics.Process.Start(dir);
        }
    }
}