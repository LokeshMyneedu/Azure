using System;

namespace AzureLearn
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //VirtualMachines.CreateVirtualMachines();
            // var vm = VirtualMachines.GetVm();
            // VirtualMachines.GetVMDetails(vm);
            // VirtualMachines.StopVm(vm);
            //VirtualMachines.DeallocateVm(vm);
            // ResourceGroup.DeleteResourceGroup("myResourceGroup");
            StorageAccount.ConnnectStorageAccount();

        }
    }
}
