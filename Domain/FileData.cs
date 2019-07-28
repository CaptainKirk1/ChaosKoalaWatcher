using CustomTypes;
using System;

namespace Domain
{
    public class FileData
    {
        public FileData(string fileName, DateTime modifyTime, int? linesInFile = null)
        {
            Filename = fileName;
            ModifyTime = modifyTime;
            if (linesInFile.HasValue)
            {
                LinesInFile = linesInFile.Value;
            }
        }

        public string Filename { get; set; }
        public int LinesInFile { get; set; }
        public DateTime ModifyTime { get; set; }
    }
}
