using System;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace AzureLearn
{
    public static class AzureAuthentication
    {
        public static IAzure Authenticate()
        {           
                var credentials = SdkContext.AzureCredentialsFactory
                .FromFile("/Users/lokesh/Projects/Azure/Azure/azureauth.properties");

                return Azure.Configure()
                    .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                    .Authenticate(credentials)
                    .WithDefaultSubscription();           

        }
    }
}
