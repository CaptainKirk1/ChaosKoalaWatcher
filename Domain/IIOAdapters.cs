using System;
using System.Collections.Generic;

namespace Domain
{
    public interface IIOAdapters
    {
        void SendScreenOutput(string output);
        IEnumerable<string> GetDirectoryNameList();
        int GetFileCount();
        int GetLineCount(string fileName);
        DateTime GetLastModifyTime(string fileName);
        string CurrentDirectory { get; }
        string CurrentFilter { get; }
        IDictionary<T, T2> GetNewDictionary<T, T2>(
            IDictionary<T, T2> existingForCopy = null
        );
        IDictionary<string, FileData> GetDirectoryListAsDictionary();
    }
}
