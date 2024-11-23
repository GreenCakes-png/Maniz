using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace Neo.SmartContract.Template
{
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        public delegate void OnTeamCaptainsSelectedDelegate(UInt160 from, ByteString league, ByteString team, ByteString captain, ByteString CoCaptain);

        [DisplayName("TeamCaptainsSelected")]
        public static event OnTeamCaptainsSelectedDelegate OnTeamCaptainsSelected;


        private static readonly byte[] Prefix_Captain = new byte[] { 0x03 };
        private static readonly byte[] Prefix_CoCaptain = new byte[] { 0x04 };

        public static void SetCaptains(ByteString league, ByteString team, ByteString Captain, ByteString CoCaptain)
        {
            ExecutionEngine.Assert(Runtime.CheckWitness(Runtime.Transaction.Sender), "??");

            var ownerOfCaptain = (UInt160)Contract.Call(GetMintContractAddress(), "ownerOf", CallFlags.All, Captain);
            ExecutionEngine.Assert(ownerOfCaptain.Equals(Runtime.Transaction.Sender), "Not your player");

            var ownerOfCoCaptain = (UInt160)Contract.Call(GetMintContractAddress(), "ownerOf", CallFlags.All, CoCaptain);
            ExecutionEngine.Assert(ownerOfCoCaptain.Equals(Runtime.Transaction.Sender), "Not your player");

            SetCaptain(Runtime.Transaction.Sender, league, Captain);
            SetCoCaptain(Runtime.Transaction.Sender, league, CoCaptain);

            OnTeamCaptainsSelected(Runtime.Transaction.Sender, league, team, Captain, CoCaptain);
        }

        private static void SetCaptain(UInt160 from, ByteString league, ByteString tokenId) 
            => Storage.Put(Prefix_Captain.Concat(league).Concat(from), tokenId);

        private static void SetCoCaptain(UInt160 from, ByteString league, ByteString tokenId) 
            => Storage.Put(Prefix_Captain.Concat(league).Concat(from), tokenId);

        [Safe]
        public static ByteString GetCaptain(UInt160 from, ByteString league) => 
            Storage.Get(Prefix_Captain.Concat(league).Concat(from));

        [Safe]
        public static ByteString GetCoCaptain(UInt160 from, ByteString league) => 
            Storage.Get(Prefix_CoCaptain.Concat(league).Concat(from));
    }
}
