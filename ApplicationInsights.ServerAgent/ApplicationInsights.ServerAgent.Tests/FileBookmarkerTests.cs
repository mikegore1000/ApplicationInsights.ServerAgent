using System.IO;
using Xunit;

namespace ApplicationInsights.ServerAgent.Tests
{
    public class FileBookmarkerTests
    {
        [Fact]
        public void when_getting_the_latest_bookmark_and_no_bookmark_file_exists_the_bookmark_is_null()
        {
            var sut = new FileBookmarker();

            Assert.Null(sut.GetLatest("doesnotexist"));
        }

        [Fact]
        public void when_getting_the_latest_bookmark_and_the_bookmark_file_exists_the_bookmark_is_provided()
        {
            var sut = new FileBookmarker();

            Assert.NotNull(sut.GetLatest("test-bookmark"));
        }

        [Fact]
        public void when_writing_a_bookmark_a_bookmark_file_is_written_to()
        {
            var sut = new FileBookmarker();
            var bookmark = sut.GetLatest("test-bookmark");
            sut.Bookmark(bookmark, "testnew-bookmark");

        }
    }
}
