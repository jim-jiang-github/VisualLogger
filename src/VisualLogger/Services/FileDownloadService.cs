using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Services
{
    internal class FileDownloadService
    {
        public delegate void DownloadProgress(long downloadLength, long totalLength);
        private const int DOWNLOAD_BUFFER_SIZE = 1024 * 8;

        public FileDownloadService()
        {
        }

        public async Task<bool> DownloadFile(string requestUri, FileInfo downloadFile, DownloadProgress downloadProgressCallback, CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();
                using var httpResponseMessage = await httpClient.GetAsync(requestUri, cancellationToken);
                using var responseStream = await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);
                using var fileStream = downloadFile.Create();
                var contentLength = httpResponseMessage.Content?.Headers?.ContentLength;
                if (contentLength.HasValue)
                {
                    downloadProgressCallback?.Invoke(0, contentLength.Value);
                }
                var buffer = new byte[DOWNLOAD_BUFFER_SIZE];
                var readLength = 0;
                int length;

                while ((length = await responseStream.ReadAsync(buffer, 0, DOWNLOAD_BUFFER_SIZE, cancellationToken)) > 0)
                {
                    readLength += length;
                    fileStream.Write(buffer, 0, length);
                    downloadProgressCallback?.Invoke(readLength, contentLength.Value);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
