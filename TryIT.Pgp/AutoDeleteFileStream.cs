using System.IO;

namespace TryIT.Pgp
{
    internal sealed class AutoDeleteFileStream : FileStream
    {
        private readonly string _filePath;

        /// <summary>
        /// FileStream that deletes the <paramref name="filePath"/> file on Dispose
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <param name="bufferSize"></param>
        /// <param name="useAsync"></param>
        public AutoDeleteFileStream(
            string filePath,
            FileMode mode,
            FileAccess access,
            FileShare share,
            int bufferSize,
            bool useAsync)
            : base(filePath, mode, access, share, bufferSize, useAsync)
        {
            _filePath = filePath;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            try
            {
                if (File.Exists(_filePath))
                    File.Delete(_filePath);
            }
            catch
            {
                // Best effort cleanup only
                // Never throw from Dispose
            }
        }
    }
}
