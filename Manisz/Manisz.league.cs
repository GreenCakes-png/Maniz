using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Linq;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        public delegate void OnLeagueCreatedDelegate(string league);

        [DisplayName("LeagueCreated")]
        public static event OnLeagueCreatedDelegate OnLeagueCreated;


        private static readonly byte[] Prefix_League = new byte[] { 0x01 };

        private static byte[] GetLeagueKey(string partialKey) => Prefix_League.Concat(partialKey);

        public static void CreateLeague(string league)
        {
            if(Storage.Get(GetLeagueKey(league)) == null)
            {
                Storage.Put(GetLeagueKey(league), league);
                OnLeagueCreated(league);
            }
        }
    }
}
