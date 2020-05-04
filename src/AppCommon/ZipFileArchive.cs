using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Aspose.Cloud.Marketplace.Common
{
    /// <summary>
    /// Implements zip compression/decompression on the fly
    /// </summary>
    public class ZipFileArchive
    {
        public class FileInfo
        {
            public string Name { get; set; }
            public byte[] Data { get; set; }

            public override string ToString()
            {
                return $"{Name}";
            }
        }
        private readonly List<FileInfo> _files;
        public List<FileInfo> Files => _files;

        public ZipFileArchive()
        {
            _files = new List<FileInfo>();
        }

        public ZipFileArchive AddFile(string name, byte[] data)
        {
            _files.Add(new FileInfo
            {
                Name = name,
                Data = data
            });
            return this;
        }
        public ZipFileArchive AddFile(string name, string data)
        {
            return AddFile(name, Encoding.UTF8.GetBytes(data));
        }

        public ZipFileArchive AddFile(string name, object data)
        {
            return 
                data != null && data is string s ? AddFile(name, s)
                : AddFile(name, JsonConvert.SerializeObject(data,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                }));
        }
        /// <summary>
        /// archive files, return zip content
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> Archive()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var f in _files)
                    {
                        var zipEntry = archive.CreateEntry(f.Name);

                        using (var entryStream = zipEntry.Open())
                        {
                            await entryStream.WriteAsync(f.Data, 0, f.Data.Length);
                        }
                    }
                }
                memoryStream.Position = 0;
                return memoryStream.ToArray();
            }
        }

        public static async Task<ZipFileArchive> Unzip(byte[] bytes)
        {
            var result = new ZipFileArchive();
            await result.Extract(bytes);
            return result;
        }

        public async Task Extract(byte[] bytes)
        {
            _files.Clear();
            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read, true))
                {
                    /*
                    await Task.WhenAll(archive.Entries.Select(async zipEntry =>  
                    {
                        using (var entryStream = zipEntry.Open())
                        {
                            using (var entryMemoryStream = new MemoryStream())
                            {
                                await entryStream.CopyToAsync(entryMemoryStream);
                                entryMemoryStream.Position = 0;
                                AddFile(zipEntry.Name, entryMemoryStream.ToArray());
                            }
                        }
                    }));
                    */
                    foreach (var zipEntry in archive.Entries)
                    {
                        using (var entryStream = zipEntry.Open())
                        {
                            using (var entryMemoryStream = new MemoryStream())
                            {
                                await entryStream.CopyToAsync(entryMemoryStream);
                                entryMemoryStream.Position = 0;
                                AddFile(zipEntry.Name, entryMemoryStream.ToArray());
                            }
                        }
                    }
                }
            }
        }
    }
}
