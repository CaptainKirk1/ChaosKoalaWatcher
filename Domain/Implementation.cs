using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Domain
{
    public class Implementation
    {
        private Sync _sync;
        private IIOAdapters _IO;
        private IDictionary<string, FileData> _fileInfoBag;
        private IDictionary<string, bool> _currentFilesBag;
        private IDictionary<string, string> _changesTracking;

        public Implementation(Sync sync, 
            IIOAdapters IO, 
            IDictionary<string, FileData> fileInfoBag,
            IDictionary<string, bool> currentFilesBag)
        {
            _sync = sync;
            _IO = IO;
            _fileInfoBag = fileInfoBag;
            _currentFilesBag = currentFilesBag;

            if (!_sync.WasChangeDetected()) return; //get out, nothing to be done
            _sync.Reset(); //reset for next round

            var newCurrentFileInfoBag = _IO.GetDirectoryListAsDictionary();
            _changesTracking = _IO.GetNewDictionary<string, string>();

            fileInfoBag.ForEach(item =>
            {
                if (newCurrentFileInfoBag.ContainsKey(item.Key)) //file still exists from before
                {
                    if (newCurrentFileInfoBag[item.Key].ModifyTime > item.Value.ModifyTime)
                    {
                        _changesTracking.Add(item.Key, "Modified");
                    }
                }
                else //file was deleted
                {
                    _changesTracking.Add(item.Key, "Deleted");
                }
            });

            newCurrentFileInfoBag.ForEach(item =>
            {
                if (!fileInfoBag.ContainsKey(item.Key)) //old collection doesn't have it.  was added
                {
                    _changesTracking.Add(item.Key, "Added");
                    fileInfoBag.Add(item.Key, newCurrentFileInfoBag[item.Key]);
                }
            });

            int instances = _changesTracking.Count;
            if (instances < 1) return;

            foreach(var instance in _changesTracking.Keys)
            {
                if (!_currentFilesBag.ContainsKey(instance))
                {
                    _currentFilesBag.Add(instance, true);
                    Task.Factory.StartNew(ScanFilesAndReport);
                }
            }

        }

        private async Task ScanFilesAndReport()
        {
            string currentFile = string.Empty;
            foreach (var m in _changesTracking)
            {
                if (_currentFilesBag[m.Key] == false)
                {
                    continue;
                }
                else
                {
                    _currentFilesBag[m.Key] = false; //grab this one, make it unavailable to the next task
                    currentFile = m.Key;
                    break;
                }
            }
            if (!currentFile.HasLength())
            {
                await Task.CompletedTask; //make the compiler happy without a return real type
                return;
            }

            string operation = _changesTracking[currentFile];
            string output = $"File {currentFile} {operation}.";

            if (operation == "Modified")
            {
                int currentLineCount = _IO.GetLineCount(currentFile);
                int oldLineCount = _fileInfoBag[currentFile].LinesInFile;

                int difference = Math.Abs(currentLineCount - oldLineCount);
                string lessOrMore = currentLineCount > oldLineCount ? "+" : "-";

                output += difference < 1 ? "  No change in lines." :
                    $" Change in lines: {lessOrMore}{Math.Abs(currentLineCount - oldLineCount)}";
                _fileInfoBag[currentFile].LinesInFile = currentLineCount;
                _fileInfoBag[currentFile].ModifyTime = _IO.GetLastModifyTime(currentFile);
            }

            _IO.SendScreenOutput(output);
            if (operation == "Deleted")
            {
                _fileInfoBag.Remove(currentFile);
            }
            _changesTracking.Remove(currentFile);
            _currentFilesBag.Remove(currentFile);
            await Task.CompletedTask; //make the compiler happy without a return real type
        }
    }
}
