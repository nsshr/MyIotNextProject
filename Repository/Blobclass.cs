using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System; // Namespace for Console output
using System.Configuration; // Namespace for ConfigurationManager
using System.Threading.Tasks; // Namespace for Task
using Azure.Identity;
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;


namespace AzureBlobStorage.Helpers
{
    public interface IBlobStorage
    {

        Task<Azure.Response> DeleteContainer(string _connectionString, string ContainerName);
        Task<BlobContainerClient> CreateContainer(string _connectionString, string ContainerName);
        public Task<List<string>> GetAllDocuments(string connectionString, string containerName);
        Task UploadDocument(string connectionString, string containerName, string fileName, Stream fileContent);
        Task<Stream> GetDocument(string connectionString, string containerName, string fileName);
        Task<bool> DeleteDocument(string connectionString, string containerName, string fileName);

    }
    
    public class BlobStorage : IBlobStorage
    {
        public async Task<BlobContainerClient> CreateContainer(string _connectionString, string ContainerName)
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

            // Create the container and return a container client object
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(ContainerName);
            return containerClient;
        }
        public async Task<Azure.Response> DeleteContainer(string _connectionString, string ContainerName)
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

            // Create the container and return a container client object
            Azure.Response response = await blobServiceClient.DeleteBlobContainerAsync(ContainerName);
            return response;
        }
        public static class BlobExtensions
        {
            public static BlobContainerClient GetContainer(string connectionString, string containerName)
            {
                BlobServiceClient blobServiceClient = new(connectionString);
                return blobServiceClient.GetBlobContainerClient(containerName);
            }
        }
        public async Task<List<string>> GetAllDocuments(string connectionString, string containerName)
        {
            var container = BlobExtensions.GetContainer(connectionString, containerName);

            if (!await container.ExistsAsync())
            {
                return new List<string>();
            }

            List<string> blobs = new();

            await foreach (BlobItem blobItem in container.GetBlobsAsync())
            {
                blobs.Add(blobItem.Name);
            }
            return blobs;
        }

        public async Task UploadDocument(string connectionString, string containerName, string fileName, Stream fileStreamContent)
        {
            var container = BlobExtensions.GetContainer(connectionString, containerName);
            //Create a Container If Not exists with BlobServiceClient.BlobServiceClient Represents the Blob Storage endpoint for your storage account.
            if (!await container.ExistsAsync())
            {
                BlobServiceClient blobServiceClient = new(connectionString);
                await blobServiceClient.CreateBlobContainerAsync(containerName);
                container = blobServiceClient.GetBlobContainerClient(containerName);
            }

            var bobclient = container.GetBlobClient(fileName);
            //The BlobContainerClient.UploadBlobAsync(string, Stream, CancellationToken)operation
            // creates a NEW BLOCK BLOB If not exist. 
            if (!bobclient.Exists())
            {
                fileStreamContent.Position = 0;
                await container.UploadBlobAsync(fileName, fileStreamContent);
            }
            else
            {//For Partial Block-Blob Updates .
                fileStreamContent.Position = 0;
                await bobclient.UploadAsync(fileStreamContent, overwrite: true);
            }
        }
        public async Task<Stream> GetDocument(string connectionString, string containerName, string fileName)
        {
            var container = BlobExtensions.GetContainer(connectionString, containerName);
            if (await container.ExistsAsync())
            {
                var blobClient = container.GetBlobClient(fileName);
                if (blobClient.Exists())
                {
                    var content = await blobClient.DownloadStreamingAsync();
                    return content.Value.Content;
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            else
            {
                throw new FileNotFoundException();
            }

        }

        public async Task<bool> DeleteDocument(string connectionString, string containerName, string fileName)
        {
            var container = BlobExtensions.GetContainer(connectionString, containerName);
            if (!await container.ExistsAsync())
            {
                return false;
            }

            var blobClient = container.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteIfExistsAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}