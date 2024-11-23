using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        private static readonly byte[] Prefix_Mint_Contract_Address = new byte[] { 0x07 };

        public static void onNEP11Payment(UInt160 from, BigInteger amount, ByteString tokenId, object[] data)
        {
            if(data == null) return;

            var operation = (string)data[0];

            if(operation == "Sell")
            {
                var price = (BigInteger)data[0];
                var league = (string)data[1];
                var team = (string)data[2];

                UpdatePlayerPrice(tokenId, price);
                UpdatePlayerOwner(tokenId, from);
                MoveToTheBench(tokenId, from, league, team);
                OnPlayerForSale(tokenId, price, from, league, team);
            }

            if(operation == "QuickSell")
            {
                //Set default price ?
                var league = (string)data[1];
                var team = (string)data[2];
                UpdatePlayerPrice(tokenId, 100);
                UpdatePlayerOwner(tokenId, Runtime.ExecutingScriptHash);
                MoveToTheBench(tokenId, from, league, team);
                OnPlayerForSale(tokenId, 100, Runtime.ExecutingScriptHash, league, team);
            }
        }

        public static void SetMintContractAddress(UInt160 hash)
        {
            ExecutionEngine.Assert(IsOwner(), "Only owner can do this");
            Storage.Put(Prefix_Mint_Contract_Address, hash);
        }

        [Safe]
        public static UInt160 GetMintContractAddress() => (UInt160)Storage.Get(Prefix_Mint_Contract_Address);
    }
}
