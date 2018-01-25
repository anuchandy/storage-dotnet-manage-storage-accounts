// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Linq;

namespace ManageStorageAccount
{
    public class Program
    {
        /**
         * Azure Storage sample for managing storage accounts -
         *  - Create a storage account
         *  - Get | regenerate storage account access keys
         *  - Create another storage account
         *  - List storage accounts
         *  - Delete a storage account.
         */
        public static void RunSample(IAzure azure)
        {
            string rgName = SdkContext.RandomResourceName("rgSTMS", 20);
            string storageAccountName = SdkContext.RandomResourceName("sa", 20);
            string storageAccountName2 = SdkContext.RandomResourceName("sa2", 20);

            try
            {
                // ============================================================
                // Create a storage account

                Console.WriteLine("Creating a Storage Account");

                var storageAccount = azure.StorageAccounts.Define(storageAccountName)
                        .WithRegion(Region.USEast)
                        .WithNewResourceGroup(rgName)
                        .Create();

                Console.WriteLine("Created a Storage Account:");

                // ============================================================
                // Get | regenerate storage account access keys

                Console.WriteLine("Getting storage account access keys");

                var storageAccountKeys = storageAccount.GetKeys();


                Console.WriteLine("Regenerating first storage account access key");

                storageAccountKeys = storageAccount.RegenerateKey(storageAccountKeys[0].KeyName);


                // ============================================================
                // Create another storage account

                Console.WriteLine("Creating a 2nd Storage Account");

                var storageAccount2 = azure.StorageAccounts.Define(storageAccountName2)
                        .WithRegion(Region.USEast)
                        .WithNewResourceGroup(rgName)
                        .Create();

                Console.WriteLine("Created a Storage Account:");

                // ============================================================
                // Update storage account by enabling encryption

                Console.WriteLine($"Enabling encryption for the storage account: {storageAccount2.Name}");

                storageAccount2.Update()
                        .WithEncryption()
                        .Apply();

                foreach (var encryptionStatus in storageAccount2.EncryptionStatuses)
                {
                    String status = encryptionStatus.Value.IsEnabled ? "Enabled" : "Not enabled";
                    Console.WriteLine($"Encryption status of the service {encryptionStatus.Key}:{status}");
                }

                // ============================================================
                // List storage accounts

                Console.WriteLine("Listing storage accounts");

                var storageAccounts = azure.StorageAccounts;

                var accounts = storageAccounts.ListByResourceGroup(rgName);

                foreach (var account in accounts)
                {
                    Console.WriteLine($"Storage Account {account.Name} created @ {account.CreationTime}");
                }

                // ============================================================
                // Delete a storage account

                Console.WriteLine($"Deleting a storage account - {accounts.ElementAt(0).Name} created @ {accounts.ElementAt(0).CreationTime}");

                azure.StorageAccounts.DeleteById(accounts.ElementAt(0).Id);

                Console.WriteLine("Deleted storage account");
            }
            finally
            {
                if (azure.ResourceGroups.GetByName(rgName) != null)
                {
                    Console.WriteLine("Deleting Resource Group: " + rgName);
                    azure.ResourceGroups.DeleteByName(rgName);
                    Console.WriteLine("Deleted Resource Group: " + rgName);
                }
                else
                {
                    Console.WriteLine("Did not create any resources in Azure. No clean up is necessary");
                }
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                //=================================================================
                // Authenticate
                var credentials = SdkContext.AzureCredentialsFactory.FromFile(Environment.GetEnvironmentVariable("AZURE_AUTH_LOCATION"));

                var azure = Azure
                    .Configure()
                    .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                    .Authenticate(credentials)
                    .WithDefaultSubscription();

                // Print selected subscription
                Console.WriteLine("Selected subscription: " + azure.SubscriptionId);

                RunSample(azure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}