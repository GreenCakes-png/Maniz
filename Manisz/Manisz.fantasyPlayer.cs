using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System.ComponentModel;
using System.Linq;
using Neo.SmartContract.Framework.Attributes;
using System.Numerics;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        public delegate void OnFantasyPlayerDelegate(string Id, string playerName, bool active);

        [DisplayName("FantasyPlayerStatus")]
        public static event OnFantasyPlayerDelegate OnFantasyPlayerStatus;

        private static readonly byte[] Prefix_Player = new byte[] { 0x02 };
        private static readonly byte[] Prefix_User_Active_Players = new byte[] { 0x03 };

        private static byte[] GetFantasyPlayerKey(ByteString playerId, UInt160 from) 
            => Prefix_Player.Concat(playerId).Concat(from);

        private static byte[] GetActivePlayersForUserKey(UInt160 from, ByteString league) 
            => Prefix_User_Active_Players.Concat(league).Concat(from);


        //TODO: Update multiple players at once

        private static void MoveToTheBench(ByteString tokenId, UInt160 from, ByteString league)
        {
            var player = (BigInteger)Storage.Get(GetFantasyPlayerKey(tokenId,from));
            if(player == 0)
            {
                return;
            }

            UpdateActiveCount(from, league, -1);
            Storage.Put(GetFantasyPlayerKey(tokenId,from), 0);

            OnFantasyPlayerStatus(tokenId, from, false);
        }

        public static void UpdateFantasyPlayer(ByteString tokenId, UInt160 from, ByteString league, bool active)
        {
            ExecutionEngine.Assert(Runtime.CheckWitness(Runtime.Transaction.Sender), "?");
            var owner = (UInt160)Contract.Call("Nep11Contract", "ownerOf", CallFlags.All, tokenId);
            ExecutionEngine.Assert(owner.Equals(from), "Not your player");

            var player = (BigInteger)Storage.Get(GetFantasyPlayerKey(tokenId,from));
            if(player == 0 && !active) return;

            UpdateActiveCount(from, league, active ? 1 : -1);
            Storage.Put(GetFantasyPlayerKey(tokenId,from), active ? 1 : -1);

            OnFantasyPlayerStatus(tokenId, from, active);
        }


        private static bool UpdateActiveCount(UInt160 from, ByteString league, BigInteger count)
        {
            var activePlayers = (BigInteger)Storage.Get(GetActivePlayersForUserKey(from, league)) + count;
            if(activePlayers < 0 || activePlayers > 11)
            {
                return false;
            }
            Storage.Put(GetActivePlayersForUserKey(from, league), activePlayers);
            return true;
        }
    }
}
