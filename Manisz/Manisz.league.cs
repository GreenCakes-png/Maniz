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
        public delegate void OnLeagueCreatedDelegate(string league, string img);

        [DisplayName("LeagueCreated")]
        public static event OnLeagueCreatedDelegate OnLeagueCreated;

        public delegate void OnTeamCreatedDelegate(string team, string img, string league);

        [DisplayName("TeamCreated")]
        public static event OnTeamCreatedDelegate OnTeamCreated;

        [Safe]
        public bool DoesLeagueExist(ByteString league) => Storage.Get(GetLeagueKey(league)) != null;

        private static byte[] GetLeagueKey(string partialKey) 
            => (new byte[] { 0x01 }).Concat(partialKey);

        private static byte[] GetTeamKey(string league, string team) 
            => (new byte[] { 0x02 }).Concat(league).Concat(team);

        public static void CreateLeague(ByteString league, string img)
        {
            ExecutionEngine.Assert(IsOwner(), "Only owner can do this");
            ExecutionEngine.Assert(Storage.Get(GetLeagueKey(league)) == null, "League exist!");
            Storage.Put(GetLeagueKey(league), league);

            Contract.Call(GetMintContractAddress(), "createLeague", CallFlags.All, league, Runtime.Time, Runtime.Time);
            // SetLeaguePlayerBanCount(banCounter, league);

            OnLeagueCreated(league, img);
        }

        private static string GetPlayerLeague(string tokenId)
        {
            var playerProperties = (Map<string, object>)Contract.Call(GetMintContractAddress(), "properties", CallFlags.All, tokenId);
            return (string)playerProperties["league"];
        }

        public void createTeam(ByteString league, string team, string img, string data)
        {
            ExecutionEngine.Assert(IsOwner(), "Only owner can do this");
            ExecutionEngine.Assert(Storage.Get(GetTeamKey(league,team)) == null, "Team exist!");

            List<List<string>> players = (List<List<string>>)StdLib.JsonDeserialize(data);

            for(var i = 0; i < players.Count; i++)
            {
                var tokenId = (BigInteger)Contract.Call(
                    GetMintContractAddress(), 
                    "myMint", 
                    CallFlags.All, 
                    players[i][0] + " " + players[i][1],
                    players[i][0] + " " + players[i][1],
                    team,
                    players[i][2],
                    league,
                    players[i][3]);

                if(tokenId > 0)
                {
                    InitPlayer(tokenId.ToString(),100, league, team);
                }
                else
                {
                    ExecutionEngine.Abort("something went wront");
                }
            }

            Storage.Put(GetTeamKey(league,team), 1);
            OnTeamCreated(team, img, league);
        }
    }
}
