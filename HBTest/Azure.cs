using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
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

        
        public MemoryStream Download(string containerName, string blobName)
        {
            CloudBlobClient blobClientDownload = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer containerDownload = blobClientDownload.GetContainerReference(containerName);
            CloudBlockBlob blockBlobDownload = containerDownload.GetBlockBlobReference(blobName);

            using (var fileStream = new MemoryStream())
            {
                blockBlobDownload.DownloadToStreamAsync(fileStream).Wait();

                return fileStream;
            }
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

        public void UploadList(string containerName, List<MemoryStream> outputFileList)
        {
            CloudBlobClient blobClientUpload = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer containerUpload = blobClientUpload.GetContainerReference(containerName);
            ///CloudBlockBlob blockBlobUpload = containerUpload.GetBlockBlobReference(blobName);
            int index = 1;
            //outputFile.Position = 0;
            //using (outputFile)
            //{
            //    await blockBlobUpload.UploadFromStreamAsync(outputFile);
            //}

            Parallel.ForEach(outputFileList, async file =>
            {
                CloudBlockBlob blockBlobUpload = containerUpload.GetBlockBlobReference("frame" + index + ".png");
                await  blockBlobUpload.UploadFromStreamAsync(file);

                file.Close();

                index++;
            });

        }

    }
}
