using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using System;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No authorization.");
            ContractManagement.Update(nefFile, manifest, data);
        }

        public static void Destroy()
        {
            if (!IsOwner())
                throw new InvalidOperationException("No authorization.");
            ContractManagement.Destroy();
        }
    }
}
