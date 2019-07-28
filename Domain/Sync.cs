namespace Domain
{
    public class Sync
    {
        //class for syncing of timer threads
        private readonly object _syncObject = new object();
        private bool _changeDetected = false;

        public bool WasChangeDetected()
        {
            lock (_syncObject) return _changeDetected;
        }

        public void RecordChangeDetected()
        {
            lock (_syncObject) _changeDetected = true;
        }

        public void Reset()
        {
            lock (_syncObject) _changeDetected = false;
        }

    }
}
