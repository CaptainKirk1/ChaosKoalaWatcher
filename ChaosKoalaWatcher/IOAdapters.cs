using Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utils;

namespace ChaosKoalaWatcher
{
    class IOAdapters : IIOAdapters
    {
        public IOAdapters(string directory, string filter)
        {
            CurrentDirectory = directory;
            CurrentFilter = filter;
        }

        public string CurrentDirectory { get; private set; }
        public string CurrentFilter { get; private set; }

        private FileData GetFileData(FileInfo file) =>
            new FileData(fileName: file.FullName,
                modifyTime: file.LastWriteTime);

        private FileData GetInitializedFileData(FileInfo file) => 
            new FileData(file.FullName,
                file.LastWriteTime, 
                GetLineCount(file.FullName));

        private IEnumerable<FileInfo> GetDirectoryAsFileInfoList() =>
            Directory.GetFiles(CurrentDirectory, CurrentFilter)
                .Select(f => new FileInfo(f));

        public IDictionary<string, FileData> GetDirectoryListAsDictionary()
        {
            IDictionary<string, FileData> bag = new ConcurrentDictionary<string, FileData>();
            GetDirectoryAsFileInfoList().ForEach(item => bag.Add(item.FullName, GetFileData(item)));
            return bag;
        }

        public IDictionary<string, FileData> GetInitializedDirectoryListAsDictionary()
        {
            IDictionary<string, FileData> bag = new ConcurrentDictionary<string, FileData>();
            GetDirectoryAsFileInfoList().ForEach(item => bag.Add(item.FullName, GetInitializedFileData(item)));
            return bag;
        }

        public int GetFileCount() => Directory.GetFiles(CurrentDirectory, CurrentFilter).Length;

        public void SendScreenOutput(string output) => output.WriteLine();

        public IEnumerable<string> GetDirectoryNameList() => Directory.GetFiles(CurrentDirectory, CurrentFilter);

        public IDictionary<T, T2> GetNewDictionary<T, T2>(
            IDictionary<T, T2> existingForCopy = null
        ) => existingForCopy.IsNull() ?
            new ConcurrentDictionary<T, T2>() : 
            new ConcurrentDictionary<T, T2>(existingForCopy);

        public int GetLineCount(string fileName) => File.ReadLines(fileName).Count();

        public DateTime GetLastModifyTime(string fileName) => File.GetLastWriteTime(fileName);
    }
}
