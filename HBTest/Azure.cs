using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HBTest
{
    public class Azure
    {
        private CloudStorageAccount storageAccount;


        public Azure(string accountName, string keyValue)
        {
            storageAccount = Connection(accountName, keyValue);
        }


        private CloudStorageAccount Connection(string accountName, string keyValue)
        {            
            return new CloudStorageAccount( new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(accountName, keyValue), true);
        }

        
        public async Task<MemoryStream> Download(string containerName, string blobName)
        {
            CloudBlobClient blobClientDownload = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer containerDownload = blobClientDownload.GetContainerReference(containerName);
            CloudBlockBlob blockBlobDownload = containerDownload.GetBlockBlobReference(blobName);
            var fileStream = new MemoryStream();
            await blockBlobDownload.DownloadToStreamAsync(fileStream);

            return fileStream;
        }


        public async Task Upload(string containerName, string blobName, MemoryStream outputFile)
        {
            CloudBlobClient blobClientUpload = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer containerUpload = blobClientUpload.GetContainerReference(containerName);
            CloudBlockBlob blockBlobUpload = containerUpload.GetBlockBlobReference(blobName);

            outputFile.Position = 0;
            using (outputFile)
            {
                await blockBlobUpload.UploadFromStreamAsync(outputFile);                
            }
        }


        public async Task Upload(string containerName, string blobName, byte[] frame)
        {
            CloudBlobClient blobClientUpload = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer containerUpload = blobClientUpload.GetContainerReference(containerName);
            CloudBlockBlob blockBlobUpload = containerUpload.GetBlockBlobReference(blobName);

            await blockBlobUpload.UploadFromByteArrayAsync(frame, 0, frame.Length);
        }


    }
}
