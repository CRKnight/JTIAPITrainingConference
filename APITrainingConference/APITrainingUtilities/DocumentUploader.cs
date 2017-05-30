using System.IO;
using System.Net;

namespace APITrainingUtilities
{
	public static class DocumentUploader
	{
		public static void StreamFileToJustWare(string url, string filename)
		{
			HttpWebRequest headRequest = (HttpWebRequest)HttpWebRequest.Create(url);
			headRequest.Credentials = CredentialCache.DefaultCredentials; // This works for integrated security, use NetworkCredential for basic authentication
			headRequest.PreAuthenticate = true; // In order to stream a large file PreAuthenticate must be set to true, and an initial call is used to authenticate.
			headRequest.UnsafeAuthenticatedConnectionSharing = true;
			headRequest.Method = "HEAD";
			headRequest.GetResponse();

			HttpWebRequest uploadRequest = (HttpWebRequest)HttpWebRequest.Create(url);
			uploadRequest.Credentials = headRequest.Credentials;
			uploadRequest.Method = "POST";
			uploadRequest.AllowWriteStreamBuffering = false; // Setting AllowWriteStreamBuffering instructs the web request to not load the file into memory.
			uploadRequest.SendChunked = true; // SendChunked allows a large file to be sent in smaller pieces.  When sending chunked, the content length should not be set.
			uploadRequest.PreAuthenticate = true;
			uploadRequest.Timeout = -1; // Infinite timeout
			uploadRequest.UnsafeAuthenticatedConnectionSharing = true;

			const int bufferSize = 65536;
			byte[] buffer = new byte[bufferSize];

			using (var uploadStream = uploadRequest.GetRequestStream())
			using (var fileStream = File.OpenRead(filename))
			{
				int bytesRead;
				while ((bytesRead = fileStream.Read(buffer, 0, bufferSize)) > 0)
				{
					uploadStream.Write(buffer, 0, bytesRead);
				}

				uploadStream.Flush();
			}

			uploadRequest.GetResponse();
		}
	}
}
