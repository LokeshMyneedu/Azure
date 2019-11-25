using System;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
namespace AzureLearn
{
    public static class ResourceGroup
    {
        private static IAzure azure = AzureAuthentication.Authenticate();
        public static IResourceGroup CreateResourceGroup(string groupName, Region location)
        {            
            return azure.ResourceGroups.Define(groupName)
                .WithRegion(location)
                .Create();
        }

        public static void DeleteResourceGroup(string groupName)
        {
               azure.ResourceGroups.DeleteByName(groupName);
        }
    }
}
