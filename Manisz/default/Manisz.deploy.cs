using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        // This will be executed during deploy
        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                // This will be executed during update
                return;
            }

            // Init method, you must deploy the contract with the owner as an argument, or it will take the sender
            if (data is null) data = Runtime.Transaction.Sender;

            UInt160 initialOwner = (UInt160)data;

            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, "owner must exists");

            Storage.Put(new[] { Prefix_Owner }, initialOwner);
            OnSetOwner(null, initialOwner);
            Storage.Put(Storage.CurrentContext, "Hello", "World");
        }
    }
}
