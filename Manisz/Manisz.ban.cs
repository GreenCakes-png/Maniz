using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        public delegate void OnLeagueBanCounterDelegate(string league, BigInteger count);

        [DisplayName("FantasyLeagueBanCounter")]
        public static event OnLeagueBanCounterDelegate OnLeagueBanCounter;

        public delegate void OnFantasyPlayerBannedDelegate(UInt160 from, string league, ByteString tokenId);

        [DisplayName("FantasyPlayerBanned")]
        public static event OnFantasyPlayerBannedDelegate OnFantasyPlayerBanned;

        private const byte Prefix_Ban_Counter = 0xee;
        private const byte Prefix_Fantasy_Player_League_Ban = 0xaa;

        //Track bans
        [Safe]
        public static bool IsFantasyPlayerBanned(ByteString tokenId) =>
            (BigInteger)Storage.Get((new[] { Prefix_Fantasy_Player_League_Ban }).Concat(tokenId)) == 1;

        private static void SaveBanFantasyPlayer(ByteString tokenId) => 
            Storage.Put((new[] { Prefix_Fantasy_Player_League_Ban }).Concat(tokenId), 1);


        //Track banned fantasy player counter by user
        [Safe]
        public static BigInteger CurrentBanCountForPlayer(ByteString league) =>
            (BigInteger)Storage.Get((new[] { Prefix_Ban_Counter }).Concat(league).Concat(Runtime.Transaction.Sender));
        
        private static void IncreasePlayerBanCount(ByteString league) => 
            SetBanCount(CurrentBanCountForPlayer(league) + 1, league);
        
        private static void SetBanCount(BigInteger count, ByteString league) => 
            Storage.Put((new[] { Prefix_Ban_Counter }).Concat(league).Concat(Runtime.Transaction.Sender), count);

        
        //Ban counter for league
        public static void SetLeaguePlayerBanCount(BigInteger count, ByteString league)
        {
            ExecutionEngine.Assert(IsOwner(), "Only owner can do this");

            Storage.Put((new[] { Prefix_Ban_Counter }).Concat(league), count);
            OnLeagueBanCounter(league, count);
        }

        [Safe]
        public static BigInteger GetBanCountForLeague(ByteString league) => (BigInteger)Storage.Get((new[] { Prefix_Ban_Counter }).Concat(league));

        public static void BanFantasyPlayer(ByteString league, ByteString tokenId)
        {
            ExecutionEngine.Assert(Runtime.CheckWitness(Runtime.Transaction.Sender), "??");
            ExecutionEngine.Assert(Storage.Get(GetLeagueKey(league)) == null, "League does not exist");
            ExecutionEngine.Assert(HasUserJoined(league), "Not joined league!");
            ExecutionEngine.Assert(CanBan(league), "Used up all bans!");

            IncreasePlayerBanCount(league);
            SaveBanFantasyPlayer(tokenId);

            OnFantasyPlayerBanned(Runtime.Transaction.Sender, league, tokenId);
        }

        [Safe]
        public static bool CanBan(ByteString league)
        {
            var userBanCount = CurrentBanCountForPlayer(Runtime.Transaction.Sender);
            var leagueBanCount = GetBanCountForLeague(league);

            return userBanCount >= leagueBanCount;
        }
    }
}
