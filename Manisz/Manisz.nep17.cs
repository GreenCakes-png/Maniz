using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        private static readonly byte[] Prefix_Token_Script_Hash = new byte[] { 0xb1 };

        public static void SetTokenScriptHash(UInt160 token)
        {
            ExecutionEngine.Assert(IsOwner(), "Only owner can do this");
            Storage.Put(new[] { Prefix_Ban_Counter }, token);
        }

        [Safe]
        public static UInt160 GetTokenScriptHash() => (UInt160)Storage.Get(new[] { Prefix_Ban_Counter });

        public static void onNEP17Payment(UInt160 from, BigInteger amount, object data)
        {
            //Check data for action to take
            
            //Join League
            ExecutionEngine.Assert(Runtime.CallingScriptHash == GAS.Hash, "Unknown token");
            ExecutionEngine.Assert(GetBuyInPrice("get league from data") == amount, "?");
            JoinLeague("get league from data");
            //Mint and send FANTASY to user

            
            //Buy player
            ExecutionEngine.Assert(Runtime.CallingScriptHash == GetTokenScriptHash(), "Unknown token");
            ExecutionEngine.Assert(IsPlayerForSale("tokenid from data"), "Not for sale");
            ExecutionEngine.Assert(PriceOfPlayer("tokenid from data") == amount, "Price error");
            //Send amount to old owner
            //Send player to new owner
        }
    }
}
