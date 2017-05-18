using System;
using System.IO;
using System.Net;
using APITrainingUtilities.Logging;

namespace APITrainingUtilities
{
	public static class UploadHelper
	{
		public static void UploadToApi(this string url, string filePath)
		{
			ILog logger = LogFactory.Create();
			if (!File.Exists(filePath))
			{
				logger.Error("File '{FilePath}' does not exist.  Upload failed.");
				return;
			}
			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
			{
				logger.Error("Url '{Url}' is invalid.  Upload failed.", url);
				return;
			}
			HttpWebRequest headRequest = (HttpWebRequest)WebRequest.Create(url);
			headRequest.Credentials = CredentialCache.DefaultCredentials;
			headRequest.PreAuthenticate = true; // In order to stream a large file PreAuthenticate must be set to true, and an initial call is used to authenticate.
			headRequest.UnsafeAuthenticatedConnectionSharing = true;
			headRequest.Method = "HEAD";
			try
			{
				headRequest.GetResponse();
			}
			catch (WebException exception)
			{
				logger.Error("Error communicating with url '{Url}': {Exception}", url, exception);
				return;
			}

			HttpWebRequest uploadRequest = (HttpWebRequest)WebRequest.Create(url);
			uploadRequest.Credentials = headRequest.Credentials;
			uploadRequest.Method = "POST";
			uploadRequest.AllowWriteStreamBuffering = false; // Setting AllowWriteStreamBuffering instructs the web request to not load the file into memory.
			uploadRequest.SendChunked = true; // SendChunked allows a large file to be sent in smaller pieces.  When sending chunked, the content length should not be set.
			uploadRequest.PreAuthenticate = true;
			uploadRequest.Timeout = -1; // Infinite timeout
			uploadRequest.UnsafeAuthenticatedConnectionSharing = true;

			const int bufferSize = 65536;
			byte[] buffer = new byte[bufferSize];

			logger.Debug("Uploading {Url} starting...", url);
			using (var uploadStream = uploadRequest.GetRequestStream())
			using (var fileStream = File.OpenRead(filePath))
			{
				int bytesRead;
				while ((bytesRead = fileStream.Read(buffer, 0, bufferSize)) > 0)
				{
					uploadStream.Write(buffer, 0, bytesRead);
				}
				uploadStream.Flush();
			}
			uploadRequest.GetResponse();
			logger.Debug("Upload {Url} complete", url);
		}
	}
}