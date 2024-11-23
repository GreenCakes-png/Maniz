using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        public delegate void OnFantasyPlayerDelegate(ByteString TokenId, string league, string team, ByteString owner, bool active);

        [DisplayName("FantasyPlayerStatus")]
        public static event OnFantasyPlayerDelegate OnFantasyPlayerStatus;

        private static readonly byte[] Prefix_Player = new byte[] { 0x02 };
        private static readonly byte[] Prefix_User_Active_Players = new byte[] { 0x03, 0x0f };

        private static byte[] GetFantasyPlayerKey(ByteString playerId, UInt160 from) 
            => Prefix_Player.Concat(playerId).Concat(from);

        private static byte[] GetActivePlayersForUserKey(UInt160 from, ByteString league) 
            => Prefix_User_Active_Players.Concat(league).Concat(from);


        //TODO: Update multiple players at once

        private static void MoveToTheBench(ByteString tokenId, UInt160 from, ByteString league, ByteString team)
        {
            var player = (BigInteger)Storage.Get(GetFantasyPlayerKey(tokenId,from));
            if(player == 0)
            {
                return;
            }

            if(!UpdateActiveCount(from, league, -1)) return;

            Storage.Put(GetFantasyPlayerKey(tokenId,from), 0);

            OnFantasyPlayerStatus(tokenId, league, team, from, false);
        }

        public static void UpdateFantasyPlayer(ByteString tokenId, UInt160 from, ByteString league, bool active, ByteString team)
        {
            ExecutionEngine.Assert(Runtime.CheckWitness(Runtime.Transaction.Sender), "?");
            var owner = (UInt160)Contract.Call(GetMintContractAddress(), "ownerOf", CallFlags.All, tokenId);
            ExecutionEngine.Assert(owner.Equals(from), "Not your player");
            ExecutionEngine.Assert(from.Equals(Runtime.Transaction.Sender), "??");

            if(!UpdateActiveCount(from, league, active ? 1 : -1)) return;
            Storage.Put(GetFantasyPlayerKey(tokenId,from), active ? 1 : -1);

            OnFantasyPlayerStatus(tokenId, league, team, from, active);
        }


        private static bool UpdateActiveCount(UInt160 from, ByteString league, BigInteger count)
        {
            var activePlayers = (BigInteger)Storage.Get(GetActivePlayersForUserKey(from, league)) + count;
            if(activePlayers < 0 || activePlayers > 11)
            {
                ExecutionEngine.Abort("Not working! " + activePlayers);
                return false;
            }
            Storage.Put(GetActivePlayersForUserKey(from, league), activePlayers);
            return true;
        }
    }
}
