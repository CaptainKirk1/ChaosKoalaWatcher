namespace Domain
{
    public interface IWatcher
    {
        void Start(string path, string filter);
    }
}
