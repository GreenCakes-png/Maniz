using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        public delegate void OnLeagueCreatedDelegate(string league, string img);

        [DisplayName("LeagueCreated")]
        public static event OnLeagueCreatedDelegate OnLeagueCreated;

        public delegate void OnTeamCreatedDelegate(string team, string img);

        [DisplayName("TeamCreated")]
        public static event OnTeamCreatedDelegate OnTeamCreated;

        private static byte[] GetLeagueKey(string partialKey) 
            => (new byte[] { 0x01 }).Concat(partialKey);

        private static byte[] GetTeamKey(string league, string team) 
            => (new byte[] { 0x02 }).Concat(league).Concat(team);

        public static void CreateLeague(ByteString league)
        {
            ExecutionEngine.Assert(IsOwner(), "Only owner can do this");
            ExecutionEngine.Assert(Storage.Get(GetLeagueKey(league)) == null, "League exist!");
            Storage.Put(GetLeagueKey(league), league);
            // SetLeaguePlayerBanCount(banCounter, league);

            OnLeagueCreated(league, "");
        }

        private static string GetPlayerLeague(string tokenId)
        {
            var playerProperties = (Map<string, object>)Contract.Call(GetMintContractAddress(), "properties", CallFlags.All, tokenId);
            return (string)playerProperties["league"];
        }

        public void createTeam(ByteString league, string team, string data)
        {
            ExecutionEngine.Assert(IsOwner(), "Only owner can do this");
            ExecutionEngine.Assert(Storage.Get(GetTeamKey(league,team)) == null, "Team exist!");

            List<List<string>> players = (List<List<string>>)StdLib.JsonDeserialize(data);

            for(var i = 0; i < players.Count; i++)
            {
                var tokenId = (ByteString)Contract.Call(
                    GetMintContractAddress(), 
                    "myMint", 
                    CallFlags.All, 
                    players[i][0] + " " + players[i][1],
                    players[i][0] + " " + players[i][1],
                    team,
                    players[i][2],
                    league,
                    players[i][3]);

                if(tokenId != null)
                {
                    InitPlayer(tokenId,100);
                }
                else
                {
                    ExecutionEngine.Abort("something went wront");
                }
            }

            Storage.Put(GetTeamKey(league,team), 1);
            OnTeamCreated(team, "");
        }
    }
}
