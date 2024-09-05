using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace NUnitTest02;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        string sourceZipFile = @"";
        string password = "";
        string targetDir = @"";

        using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(sourceZipFile)))
        {
            zipStream.Password = password; // Set the password if the ZIP file is encrypted

            ZipEntry entry;
            while ((entry = zipStream.GetNextEntry()) != null)
            {
                entry.IsCompressionMethodSupported();
                string entryFileName = Path.GetFileName(entry.Name);
                string outputPath = Path.Combine(targetDir, entryFileName);

                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(outputPath);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                    using (FileStream fileStream = File.Create(outputPath))
                    {
                        int sourceBytes;
                        byte[] buffer = new byte[4096];
                        while ((sourceBytes = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, sourceBytes);
                        }
                    }
                }
            }
        }

        Assert.Pass();
    }

    [Test]
    public void Test2()
    {
        string FileZipPath = @"";
        string password = "";
        string OutputFolder = @"";

        ZipFile file = null;
        try
        {
            FileStream fs = File.OpenRead(FileZipPath);
            file = new ZipFile(fs);

            if (!String.IsNullOrEmpty(password))
            {
                // AES encrypted entries are handled automatically
                file.Password = password;
            }

            foreach (ZipEntry zipEntry in file)
            {
                if (!zipEntry.IsFile)
                {
                    // Ignore directories
                    continue;
                }

                String entryFileName = zipEntry.Name;
                // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                // Optionally match entrynames against a selection list here to skip as desired.
                // The unpacked length is available in the zipEntry.Size property.

                // 4K is optimum
                byte[] buffer = new byte[4096];
                Stream zipStream = file.GetInputStream(zipEntry);

                // Manipulate the output filename here as desired.
                String fullZipToPath = Path.Combine(OutputFolder, entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);

                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(directoryName);
                }

                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                // of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
            }
        }
        finally
        {
            if (file != null)
            {
                file.IsStreamOwner = true; // Makes close also shut the underlying stream
                file.Close(); // Ensure we release resources
            }
        }
    }
}
