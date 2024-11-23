using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        private static readonly byte[] Prefix_Token_Script_Hash = new byte[] { 0xb1 };

        public static void SetFantasyTokenScriptHash(UInt160 token)
        {
            ExecutionEngine.Assert(IsOwner(), "Only owner can do this");
            Storage.Put(new[] { Prefix_Ban_Counter }, token);
        }

        [Safe]
        public static UInt160 GetFantasyTokenScriptHash() => (UInt160)Storage.Get(new[] { Prefix_Ban_Counter });

        public static void onNEP17Payment(UInt160 from, BigInteger amount, object[] data)
        {
            //Check data for action to take
            if(data == null) ExecutionEngine.Abort("No action");

            if((string)data[0] == "Join")
            {
                var league = (string)data[1];
                //Join League
                ExecutionEngine.Assert(Runtime.CallingScriptHash == GAS.Hash, "Unknown token");
                ExecutionEngine.Assert(GetBuyInPrice(league) == amount, "?");
                // ExecutionEngine.Assert(!HasUserJoined(league), "Already in");

                JoinLeague(league);
            }

            if((string)data[0] == "Buy")
            {
                //Buy player
                var tokenId = (string)data[1];
                var team = (string)data[2];

                ExecutionEngine.Assert(Runtime.CallingScriptHash == GetFantasyTokenScriptHash(), "Unknown token");
                ExecutionEngine.Assert(IsPlayerForSale(tokenId), "Not for sale");
                ExecutionEngine.Assert(PriceOfPlayer(tokenId) == amount, "Price error");
                var playerLeague = GetPlayerLeague(tokenId);
                ExecutionEngine.Assert(HasUserJoined(playerLeague), "Wrong league");
                var owner = (UInt160)Contract.Call(GetMintContractAddress(), "ownerOf", CallFlags.All, tokenId);

                // Contract.Call(GetFantasyTokenScriptHash(), "transfer", CallFlags.All, from, amount, "GettingPaid");
                Contract.Call(GetMintContractAddress(), "transfer", CallFlags.All, from, tokenId, null);
                OnPlayerSold(tokenId, from, playerLeague, team);
            }
        }
    }
}
