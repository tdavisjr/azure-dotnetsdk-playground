using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudBlobContainer blobContainer = GetBlobContainer();

            UploadFileToBlob(blobContainer);

            ListBlobContents(blobContainer);

        }

        private static void UploadFileToBlob(CloudBlobContainer blobContainer)
        {
            var suffix = $"{DateTime.Now.ToString("s")}".Replace(":", "-");

            var blockBlob = blobContainer.GetBlockBlobReference($"whitehouse-{suffix}.jpg");

            using (var fileStream = System.IO.File.OpenRead(@"..\..\whitehouse.jpg"))
            {
                blockBlob.UploadFromStream(fileStream);
            }

        }

        private static void ListBlobContents(CloudBlobContainer blobContainer)
        {
            foreach (var item in blobContainer.ListBlobs())
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    Console.WriteLine($"Block blob of length {blob.Properties.Length}: {blob.Uri}");

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;

                    Console.WriteLine($"Page blob of length {pageBlob.Properties.Length}: {pageBlob.Uri}");

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;

                    Console.WriteLine($"Directory: {directory.Uri}");
                }

            }
        }

        private static CloudBlobContainer GetBlobContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            var blobClient = storageAccount.CreateCloudBlobClient();

            var blobContainer = blobClient.GetContainerReference("images");

            blobContainer.SetPermissions(
    new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });


            blobContainer.CreateIfNotExists();
            return blobContainer;
        }
    }
}
