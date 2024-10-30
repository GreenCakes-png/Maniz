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
        public delegate void OnPlayerJoinedLeagueDelegate(UInt160 player, string league);

        [DisplayName("PlayerJoinedLeague")]
        public static event OnPlayerJoinedLeagueDelegate OnPlayerJoinedLeague;

        private static readonly byte[] Prefix_Players = new byte[] { 0xa1 };

        private static byte[] GetPlayersLeagueKey(string partialKey, UInt160 user) => Prefix_Players.Concat(partialKey).Concat(user);

        public static void JoinLeague(string league)
        {
            ExecutionEngine.Assert(Runtime.CheckWitness(Runtime.Transaction.Sender), "??");
            ExecutionEngine.Assert(Storage.Get(GetLeagueKey(league)) != null, "League does not exist");

            Storage.Put(GetPlayersLeagueKey(league, Runtime.Transaction.Sender), 1);
            OnPlayerJoinedLeague(Runtime.Transaction.Sender, league);

            //Call to reset user NEP17 tokens
        }

        [Safe]
        public static bool HasUserJoined(string league)
        {
            ExecutionEngine.Assert(Runtime.CheckWitness(Runtime.Transaction.Sender), "??");
            ExecutionEngine.Assert(Storage.Get(GetLeagueKey(league)) != null, "League does not exist");

            return (BigInteger)Storage.Get(GetPlayersLeagueKey(league, Runtime.Transaction.Sender)) == 1;
        }
    }
}
