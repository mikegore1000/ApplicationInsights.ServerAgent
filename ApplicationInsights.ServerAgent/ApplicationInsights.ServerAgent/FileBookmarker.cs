using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ApplicationInsights.ServerAgent
{
    public class FileBookmarker : IBookmarker
    {
        public EventBookmark GetLatest(string bookmarkName)
        {
            var serializer = new BinaryFormatter();
            var fileName = GetFileName(bookmarkName);

            EventBookmark bookmark = null;

            if (File.Exists(fileName))
            {
                using (var reader = File.OpenRead(fileName))
                {
                    bookmark = (EventBookmark) serializer.Deserialize(reader);
                }
            }

            return bookmark;
        }

        public void Bookmark(EventBookmark bookmark, string bookmarkName)
        {
            var serializer = new BinaryFormatter();

            using (var writer = File.OpenWrite(GetFileName(bookmarkName)))
            {
                serializer.Serialize(writer, bookmark);
            }
        }

        private string GetFileName(string bookmarkName) => $"{bookmarkName}.txt";
    }
}