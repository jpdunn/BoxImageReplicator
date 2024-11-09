using Box.V2.CCGAuth;
using Box.V2.Config;
using Box.V2.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BoxImageReplicator;

public class BoxImageUploadWorker(IOptions<MySettings> options, ILogger<BoxImageUploadWorker> logger)
{
	private readonly MySettings _settings = options.Value;

	public async Task<bool> UploadAsync(string fileName, Stream file)
	{
		var boxConfig = new BoxConfigBuilder(_settings.ClientID, _settings.ClientSecret)
			.SetEnterpriseId(_settings.EnterpriseID)
			.Build();

		var boxCcg = new BoxCCGAuth(boxConfig);
		var adminClient = boxCcg.AdminClient();

		try
		{
			logger.LogInformation("Beginning Box file upload.");

			var fileRequest = new BoxFileRequest
			{
				Name = fileName,
				Parent = new BoxFolderRequest { Id = _settings.UploadFolderID },
			};

			var boxFile = await adminClient.FilesManager.UploadAsync(fileRequest, file);

			if (boxFile != null)
			{
				logger.LogInformation("Box file upload successful, file ID: {FileID}", boxFile.Id);
				return true;
			}
		}
		catch (Exception e)
		{
			logger.LogError("An unknown error occurred when uploading file to to Box. {Error}", e.Message);
			throw;
		}

		return false;
	}
}
