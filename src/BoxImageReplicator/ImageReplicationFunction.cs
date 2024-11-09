// ReSharper disable ClassNeverInstantiated.Global - Triggered by Azure

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoxImageReplicator;

public class ImageReplicationFunction(ILogger<ImageReplicationFunction> logger, BoxImageUploadWorker uploadWorker)
{
	[Function(nameof(ImageReplicationFunction))]
	public async Task Run(
		[BlobTrigger("upload-directory/{name}", Connection = "MySettings:StorageAccountConnectionString")]
			Stream stream,
		string name
	)
	{
		logger.LogInformation($"Image Replication function triggered.");

		// We have to copy the given Stream to a new Stream as Box cannot handle the type
		// of Stream that Azure provides. Box expects a Stream of type `System.IO.Stream`
		// whereas an Azure function is provided a Stream of type `Azure.Storage.NonDisposingStream`.
		using (MemoryStream memoryStream = new())
		{
			stream.CopyTo(memoryStream);

			await uploadWorker.UploadAsync(name, memoryStream);
		}

		logger.LogInformation($"Project Image Replication run to completion.");
	}
}
