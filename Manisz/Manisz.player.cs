using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System.ComponentModel;
using System.Linq;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        public delegate void OnPlayerActiveDelegate(string Id, string playerName, bool active);

        [DisplayName("PlayerActive")]
        public static event OnPlayerActiveDelegate OnPlayerActive;

        private static readonly byte[] Prefix_Player = new byte[] { 0x02 };

        private static byte[] GetPlayerKey(string partialKey) => Prefix_Player.Concat(partialKey);

        public static void PlayerActive(bool active)
        {
            if(!Runtime.CheckWitness(Runtime.Transaction.Sender)) return;

            var player = Storage.Get(GetPlayerKey(Runtime.Transaction.Sender));
            if(player == null) return;

            var deserialised = (Player)StdLib.JsonDeserialize(player);

            if(!deserialised.Owner.Equals(Runtime.Transaction.Sender)) return;

            deserialised.Active = active;

            Storage.Put(GetPlayerKey(Runtime.Transaction.Sender), StdLib.JsonSerialize(deserialised));

            OnPlayerActive(deserialised.Id, deserialised.Name, active);
        }
    }

    public class Player
    {
        public string Id { get;set; }
        public string Name { get;set; }
        public string Owner { get;set; }
        public bool Active { get;set; }
    }
}
