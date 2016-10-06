using System.Diagnostics.Eventing.Reader;

namespace ApplicationInsights.ServerAgent
{
    public interface IBookmarker
    {
        void Bookmark(EventBookmark bookmark, string bookmarkName);
        EventBookmark GetLatest(string bookmarkName);
    }
}