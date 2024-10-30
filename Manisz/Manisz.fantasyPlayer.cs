using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System.ComponentModel;
using System.Linq;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        public delegate void OnFantasyPlayerDelegate(string Id, string playerName, bool active);

        [DisplayName("FantasyPlayerStatus")]
        public static event OnFantasyPlayerDelegate OnFantasyPlayerStatus;

        private static readonly byte[] Prefix_Player = new byte[] { 0x02 };

        private static byte[] GetFantasyPlayerKey(string partialKey) => Prefix_Player.Concat(partialKey);

        public static void UpdateFantasyPlayer(bool active)
        {
            if(!Runtime.CheckWitness(Runtime.Transaction.Sender)) return;

            var player = Storage.Get(GetFantasyPlayerKey(Runtime.Transaction.Sender));
            if(player == null) return;

            var deserialised = (FantasyPlayer)StdLib.JsonDeserialize(player);

            if(!deserialised.Owner.Equals(Runtime.Transaction.Sender)) return;

            deserialised.Active = active;

            Storage.Put(GetFantasyPlayerKey(Runtime.Transaction.Sender), StdLib.JsonSerialize(deserialised));

            OnFantasyPlayerStatus(deserialised.Id, deserialised.Name, active);
        }
    }

    public class FantasyPlayer
    {
        public string Id { get;set; }
        public string Name { get;set; }
        public string Owner { get;set; }
        public bool Active { get;set; }
    }
}
