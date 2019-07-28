using Domain;
using System.IO;
using nf = System.IO.NotifyFilters;

namespace ChaosKoalaWatcher
{
    class Watcher : IWatcher
    {
        private Sync _sync;
        private FileSystemWatcher _fsWatcher;
        public Watcher(Sync sync)
        {
            _sync = sync;
        }

        public void Start(string path, string filter)
        {
            _fsWatcher = new FileSystemWatcher(path, filter);
            _fsWatcher.EnableRaisingEvents = true;
            _fsWatcher.IncludeSubdirectories = false;
            _fsWatcher.NotifyFilter =
                nf.LastWrite |
                nf.Size |
                nf.CreationTime |
                nf.FileName |
                nf.LastAccess |
                nf.LastWrite |
                nf.Size;

            var eh = new FileSystemEventHandler(
                (source, f) => _sync.RecordChangeDetected());
            _fsWatcher.Changed += eh;
            _fsWatcher.Created += eh;
            _fsWatcher.Deleted += eh;
        }

    }
}
