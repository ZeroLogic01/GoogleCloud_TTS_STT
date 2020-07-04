using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Storage.v1.Data;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Interfaces;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Enums;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators;
using Newtonsoft.Json;
using Prism.Events;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business
{
    public class GoogleCloudStorage : ICloudStorage
    {
        #region Private Fields

        public const string DefaultBucketName = @"polar-valor-240206-audio-files-storage-bucket";
        private readonly string _projectId;
        private readonly IEventAggregator _eventAggregator;

        #endregion

        public GoogleCloudStorage(IEventAggregator eventAggregator)
        {

            var jsonFile = Environment.GetEnvironmentVariable(AppConstants.GOOGLE_APPLICATION_CREDENTIALS);

            if (!File.Exists(jsonFile))
            {
                throw new FileNotFoundException($"Could not read the environment variable {AppConstants.GOOGLE_APPLICATION_CREDENTIALS}");
            }

            using (StreamReader file = File.OpenText(jsonFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                var cred = (Dictionary<string, string>)serializer.Deserialize(file, typeof(Dictionary<string, string>));
                _projectId = cred["project_id"];
            }

            _eventAggregator = eventAggregator;
        }

        #region Methods

        /// <summary>
        /// Uploads the file to the cloud storage asynchronously. 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> UploadAsync(string filePath, string mimeType = "audio/flac", CancellationToken cancellationToken = default)
        {
            var newObject = new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = DefaultBucketName,
                Name = Path.GetFileNameWithoutExtension(filePath),
                ContentType = mimeType
            };

            try
            {
                using (var client = await StorageClient.CreateAsync())
                {
                    Bucket bucket = null;
                    try
                    {
                        UpdateStatus(statusText: "Retrieving bucket");

                        bucket = await client.GetBucketAsync(DefaultBucketName, null, cancellationToken);
                    }
                    catch (Google.GoogleApiException)
                    {
                        UpdateStatus(statusText: $"Creating a new bucket {DefaultBucketName}");

                        try
                        {
                            bucket = await client.CreateBucketAsync(_projectId, DefaultBucketName, cancellationToken: cancellationToken);
                        }
                        catch (Google.GoogleApiException e) when (e.Error.Code == 409)
                        {
                            // The bucket already exists.  That's fine it might be created by another user.
                            Console.WriteLine(e.Error.Message);
                        }
                        UpdateStatus(statusText: "A new Bucket has been created");

                    }
                    catch (TaskCanceledException)
                    {
                        return false;
                    }

                    // Open the audio file filestream
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // update maximum value
                        UpdateProgressBar(type: ProgressType.ProgressBarMaximumValue, value: fileStream.Length);

                        // status
                        UpdateStatus(statusText: "Uploading audio file to the cloud bucket");

                        // set minimum chunksize just to see progress updating
                        var uploadObjectOptions = new UploadObjectOptions
                        {
                            ChunkSize = UploadObjectOptions.MinimumChunkSize
                        };

                        // Hook up the progress callback
                        var progressReporter = new Progress<IUploadProgress>(UploadProgress);


                        await client.UploadObjectAsync(
                                newObject,
                                fileStream,
                                uploadObjectOptions, cancellationToken: cancellationToken,
                                progress: progressReporter)
                            .ConfigureAwait(false);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the uploaded file URI w.r.t the Google cloud.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetFileUri(string fileName)
        {
            return $"gs://{DefaultBucketName}/{fileName}";
        }

        /// <summary>
        /// Updates the progress bar progress.
        /// </summary>
        /// <param name="progress"></param>
        private void UploadProgress(IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Starting:
                    // updates minimum value
                    UpdateProgressBar(type: ProgressType.ProgressBarMinimumValue, value: 0);

                    // Current Value
                    UpdateProgressBar(type: ProgressType.ProgressBarCurrentValue, value: 0);
                    break;

                case UploadStatus.Completed:
                    // Current Value
                    UpdateProgressBar(type: ProgressType.ProgressBarCurrentValue, value: progress.BytesSent);

                    // Status
                    UpdateStatus(statusText: "Upload completed");
                    break;

                case UploadStatus.Uploading:
                    // Current Value
                    UpdateProgressBar(type: ProgressType.ProgressBarCurrentValue, value: progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    // Status
                    UpdateStatus(statusText: "Upload failed/Canceled");

                    // Set Current Value to 0
                    UpdateProgressBar(type: ProgressType.ProgressBarCurrentValue, value: 0);
                    break;
            }
        }


        private void UpdateStatus(string statusText)
        {
            _eventAggregator.GetEvent<StatusEvent>().Publish(new StatusEventParameters()
            {
                Message = statusText
            });
        }

        private void UpdateProgressBar(ProgressType type, double value = default)
        {
            _eventAggregator.GetEvent<ProgressBarEvent>().Publish(new ProgressBarEventParameters()
            {
                Type = type,
                Value = value
            });
        }

        public async Task DeleteObject(IEnumerable<string> objectNames, string bucketName = DefaultBucketName, CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var storage = StorageClient.Create();
                    foreach (string objectName in objectNames)
                    {
                        UpdateStatus($"Deleting {objectName} from the cloud...");
                        await storage.DeleteObjectAsync(bucketName, objectName, null, cancellationToken);
                        Console.WriteLine($"Deleted {objectName}.");
                    }
                });
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Deletes all objects from the bucket.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public static async Task ClearBucketObjects(CancellationToken cancellationToken, string bucketName = DefaultBucketName)
        {
            try
            {
                await Task.Run(async () =>
                {
                    using (var client = StorageClient.Create())
                    {
                        var bucket = await client.GetBucketAsync(DefaultBucketName, null, cancellationToken);
                        var objects = client.ListObjects(bucketName, string.Empty);
                        Console.WriteLine($"Total number of files in bucket: {objects.Count()}");
                        // List objects
                        foreach (var obj in objects)
                        {
                            Console.WriteLine(obj.Name);
                            await client.DeleteObjectAsync(bucketName, obj.Name, null, cancellationToken);
                        }
                    }
                }, cancellationToken);
            }
            catch (TaskCanceledException) { return; }
        }

        #endregion
    }
}
