using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        private const byte Prefix_Buy_In = 0xae;

        [Safe]
        public static BigInteger GetBuyInPrice(ByteString league) 
            => (BigInteger)Storage.Get((new[] { Prefix_Owner }).Concat(league));

        public delegate void OnSetBuyInPriceDelegate(BigInteger amount, ByteString league);

        [DisplayName("BuyInPrice")]
        public static event OnSetBuyInPriceDelegate OnBuyInPrice;

        public static void SetBuyInPrice(ByteString league, BigInteger amount)
        {
            ExecutionEngine.Assert(IsOwner(), "No Authorization!");

            UInt160 previous = GetOwner();
            Storage.Put((new[] { Prefix_Owner }).Concat(league), amount);
            OnBuyInPrice(amount, league);
        }
    }
}
