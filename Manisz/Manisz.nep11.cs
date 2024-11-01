using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        public delegate void OnPlayerForSaleDelegate(ByteString TokenId, BigInteger Price, UInt160 from);

        [DisplayName("Player for sale")]
        public static event OnPlayerForSaleDelegate OnPlayerForSale;


        private static readonly byte[] Prefix_Price_Of_Player_For_Sale = new byte[] { 0x05 };
        private static readonly byte[] Prefix_Owner_Of_Player_For_Sale = new byte[] { 0x06 };

        [Safe]
        public static bool IsPlayerForSale(ByteString tokenId) 
            => PriceOfPlayer(tokenId) > 0;
        
        [Safe]
        public static BigInteger PriceOfPlayer(ByteString tokenId) 
            => (BigInteger)Storage.Get(Prefix_Price_Of_Player_For_Sale.Concat(tokenId));

        private static void UpdatePlayerPrice(ByteString tokenId, BigInteger amount) =>
            Storage.Put(Prefix_Price_Of_Player_For_Sale.Concat(tokenId), amount);

        private static void UpdatePlayerOwner(ByteString tokenId, UInt160 owner) =>
            Storage.Put(Prefix_Owner_Of_Player_For_Sale.Concat(tokenId), owner);

        public static void onNEP11Payment(UInt160 from, BigInteger amount, ByteString tokenId, object data)
        {
            ExecutionEngine.Assert(amount > 0, "Must be greater than 0 ");

            //Check data for quick sale
            
            //Else continue
            
            //Owner & Price
            UpdatePlayerPrice(tokenId, amount);
            UpdatePlayerOwner(tokenId, from);

            //Move to the bench
            MoveToTheBench(tokenId, from, "league from data");
            OnPlayerForSale(tokenId, amount, from);
        }

        //Cancel sale
    }
}
