using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Interfaces
{
    public interface ICloudStorage
    {
        Task<bool> UploadAsync(string filePath, string mimeType, CancellationToken cancellationToken = default);

        string GetFileUri(string fileName);

        Task DeleteObject(IEnumerable<string> objectNames, string bucketName = default, CancellationToken cancellationToken = default);

    }
}
