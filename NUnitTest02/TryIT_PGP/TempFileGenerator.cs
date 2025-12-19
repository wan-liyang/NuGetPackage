// Pseudocode / Plan (detailed):
// 1. Provide a static utility class `TempFileGenerator` in namespace `TryIT.TestUtils`.
// 2. Implement a private helper `CreateTempFileAsync(long sizeBytes, string label, CancellationToken ct)` that:
//    - Builds a unique filename in the system temp folder using Path.GetTempPath() and a GUID/timestamp.
//    - Creates the file with FileMode.CreateNew and an async FileStream.
//    - Efficiently ensures the file has the requested length. Use FileStream.SetLength(sizeBytes) to avoid allocating large managed buffers.
//    - Flush asynchronously and return the full path.
//    - Accept a CancellationToken for callers that want to cancel.
// 3. Implement three public convenience methods that call the helper with:
//    - 10 MB  -> `Generate10MBAsync()`
//    - 100 MB -> `Generate100MBAsync()`
//    - 1.5 GB -> `Generate1_5GBAsync()`
// 4. Keep method signatures async and return the created file path so tests can consume the file.
// 5. Add minimal validation (size > 0) and throw appropriate exceptions for invalid input.
// 6. Keep naming and style compatible with typical .NET conventions (PascalCase, async suffix).
//
// Implementation:
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NUnitTest02.TryIT_PGP
{
    /// <summary>
    /// Utility to create temporary files of specific sizes for testing.
    /// Files are created in the system temp folder (Path.GetTempPath()) and a full path is returned.
    /// </summary>
    public static class TempFileGenerator
    {
        /// <summary>
        /// Creates a 10 MB temporary file and returns its full path.
        /// </summary>
        public static Task<string> Generate10MBAsync(CancellationToken cancellationToken = default)
            => CreateTempFileAsync(10L * 1024 * 1024, "10MB", cancellationToken);

        /// <summary>
        /// Creates a 100 MB temporary file and returns its full path.
        /// </summary>
        public static Task<string> Generate100MBAsync(CancellationToken cancellationToken = default)
            => CreateTempFileAsync(100L * 1024 * 1024, "100MB", cancellationToken);

        /// <summary>
        /// Creates a 1.5 GB temporary file and returns its full path.
        /// </summary>
        public static Task<string> Generate1_5GBAsync(CancellationToken cancellationToken = default)
            => CreateTempFileAsync((long)(1.5 * 1024 * 1024 * 1024), "1_5GB", cancellationToken);

        /// <summary>
        /// Creates a file in the system temp folder with the requested size in bytes and returns the file path.
        /// Uses FileStream.SetLength for efficiency (avoids large managed buffers).
        /// </summary>
        private static async Task<string> CreateTempFileAsync(long sizeBytes, string label, CancellationToken cancellationToken)
        {
            if (sizeBytes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeBytes), "Size must be greater than zero.");
            }

            string tempFolder = Path.GetTempPath();
            string fileName = $"testfile_{label}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}.tmp";
            string fullPath = Path.Combine(tempFolder, fileName);

            // Create file and set length. Use asynchronous FileStream to allow async flush.
            // FileOptions.SequentialScan is a hint; FileOptions.Asynchronous enables async IO.
            var options = FileOptions.Asynchronous | FileOptions.SequentialScan;
            const int bufferSize = 81920;

            // Create the file first (ensures we don't accidentally overwrite).
            using (var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, options))
            {
                // Set the length to the requested size. This is efficient and avoids allocating a large buffer.
                fs.SetLength(sizeBytes);

                // Ensure metadata is flushed to disk before returning.
                await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            return fullPath;
        }
    }
}