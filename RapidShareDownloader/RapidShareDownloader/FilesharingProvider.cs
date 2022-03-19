
namespace RapidShareDownloader
{
    public abstract class FilesharingProvider
    {
        protected HTMLBrowserClient Browser = null;

        public FilesharingProvider()
        {
        }

        public FilesharingProvider(HTMLBrowserClient browser)
        {
            this.Browser = browser;
        }

        public abstract long Download(string url, string zielverzeichnis);
    }
}
