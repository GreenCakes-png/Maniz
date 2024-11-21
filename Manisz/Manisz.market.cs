using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        public delegate void OnPlayerForSaleDelegate(ByteString TokenId, BigInteger Price, UInt160 from);

        [DisplayName("PlayerForSale")]
        public static event OnPlayerForSaleDelegate OnPlayerForSale;

        private static readonly byte[] Prefix_Market = new byte[] { 0x01, 0x02 };
        private static byte[] GetMarketKey(string tokenId) => Prefix_Market.Concat(tokenId);
        private static readonly byte[] Prefix_Market_Player_Prev_Owner = new byte[] { 0x01, 0x03 };
        private static byte[] GetMarketPlayerPrevOwnerKey(string tokenId) => Prefix_Market_Player_Prev_Owner.Concat(tokenId);

        private static void InitPlayer(ByteString tokenId, BigInteger amount)
        {
            UpdatePlayerPrice(tokenId, amount);
            UpdatePlayerOwner(tokenId, Runtime.ExecutingScriptHash);
            OnPlayerForSale(tokenId, amount, Runtime.ExecutingScriptHash);
        }

        [Safe]
        public static bool IsPlayerForSale(ByteString tokenId) 
            => PriceOfPlayer(tokenId) > 0;

        [Safe]
        public static BigInteger PriceOfPlayer(ByteString tokenId) 
            => (BigInteger)Storage.Get(GetMarketKey(tokenId));

        private static void UpdatePlayerPrice(ByteString tokenId, BigInteger amount) =>
            Storage.Put(GetMarketKey(tokenId), amount);

        private static void UpdatePlayerOwner(ByteString tokenId, UInt160 owner) =>
            Storage.Put(GetMarketPlayerPrevOwnerKey(tokenId), owner);
    }
}
