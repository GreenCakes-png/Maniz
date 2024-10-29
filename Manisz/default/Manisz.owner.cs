using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

using System;
using System.ComponentModel;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        private const byte Prefix_Owner = 0xff;

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(new[] { Prefix_Owner });
        }

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public delegate void OnSetOwnerDelegate(UInt160 previousOwner, UInt160 newOwner);

        [DisplayName("SetOwner")]
        public static event OnSetOwnerDelegate OnSetOwner;

        public static void SetOwner(UInt160 newOwner)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No Authorization!");

            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, "owner must be valid");

            UInt160 previous = GetOwner();
            Storage.Put(new[] { Prefix_Owner }, newOwner);
            OnSetOwner(previous, newOwner);
        }
    }
}
