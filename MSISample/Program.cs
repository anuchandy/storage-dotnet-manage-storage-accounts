using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Threading;

namespace MSISample
{
    class Program
    {
        private static Region region = Region.USWestCentral;

        public static void Main(string[] args)
        {
            // This sample required to be run from a MSI (User Assigned or System Assigned) enabled virtual machine with role
            // based contributor access to the resource group specified as the second command line argument.
            //
            // see https://github.com/Azure-Samples/compute-net-manage-user-assigned-msi-enabled-virtual-machine.git
            //

            string usage = "Usage: dotnet run <subscription-id> <rg-name> [<client-id>]";
            if (args.Length < 2)
            {
                throw new ArgumentException(usage);
            }

            try
            {
                string subscriptionId = args[0];
                string resourceGroupName = args[1];
                string clientId = args.Length > 2 ? args[2] : null;
                string linuxVMName = SdkContext.RandomResourceName("vm", 30);
                string userName = "tirekicker";
                string password = "12NewPA$$w0rd!";

                var provider = new MSITokenProvider(AzureEnvironment.AzureGlobalCloud.ResourceManagerEndpoint, new MSILoginInformation()
                {
                    UserAssignedIdentityClientId = ""
                });

                var header =  provider.GetAuthenticationHeaderAsync(CancellationToken.None).Result;
                Console.WriteLine(header.ToString());

                //=============================================================
                // MSI Authenticate

                /**
                AzureCredentials msiCredentails = new AzureCredentials(new MSILoginInformation
                {
                    UserAssignedIdentityClientId = ""
                }, 
                AzureEnvironment.AzureGlobalCloud);

                var azure = Azure
                    .Configure()
                    .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                    .Authenticate(msiCredentails)
                    .WithDefaultSubscription();

                Console.WriteLine("Selected subscription: " + azure.SubscriptionId);

                //=============================================================
                // Create a Linux VM using MSI credentials

                Console.WriteLine("Creating a Linux VM using MSI credentials");

                var virtualMachine = azure.VirtualMachines
                        .Define(linuxVMName)
                        .WithRegion(region)
                        .WithExistingResourceGroup(resourceGroupName)
                        .WithNewPrimaryNetwork("10.0.0.0/28")
                        .WithPrimaryPrivateIPAddressDynamic()
                        .WithoutPrimaryPublicIPAddress()
                        .WithPopularLinuxImage(KnownLinuxVirtualMachineImage.UbuntuServer16_04_Lts)
                        .WithRootUsername(userName)
                        .WithRootPassword(password)
                        .WithSize(VirtualMachineSizeTypes.StandardDS2V2)
                        .Create();

                **/
                Console.WriteLine($"Created virtual machine using MSI credentials: {virtualMachine.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
