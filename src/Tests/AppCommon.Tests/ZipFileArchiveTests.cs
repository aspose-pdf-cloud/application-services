using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Aspose.Cloud.Marketplace.Common.Tests
{
    [Trait("AppCommon", "ZipFileArchiveTests")]
    public class ZipFileArchiveTests
    {
        byte[] _databuf;
        public ZipFileArchiveTests()
        {
            _databuf = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x34 };
        }
        [Fact]
        public async void ZipFileArchiveTests_Archive()
        {
            ZipFileArchive z = new ZipFileArchive();
            z.AddFile("testfile.bin", _databuf);
            byte[] archived = await z.Archive();

            ZipFileArchive unz = new ZipFileArchive();

            await unz.Extract(archived);
            Assert.Single(unz.Files);
            Assert.Equal("testfile.bin", unz.Files[0].Name);
            Assert.True(unz.Files[0].Data.SequenceEqual(_databuf), "Wrong zip file content");
        }

        [Fact]
        public async void ZipFileArchiveTests_Unzip()
        {
            ZipFileArchive z = new ZipFileArchive();
            z.AddFile("testfile.bin", _databuf);
            byte[] archived = await z.Archive();

            ZipFileArchive unz = await ZipFileArchive.Unzip(archived);
            Assert.Single(unz.Files);
            Assert.Equal("testfile.bin", unz.Files[0].Name);
            Assert.True(unz.Files[0].Data.SequenceEqual(_databuf), "Wrong zip file content");
        }

    }
}
