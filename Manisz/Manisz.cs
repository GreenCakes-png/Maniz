using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

using System;
using System.ComponentModel;

namespace Neo.SmartContract.Template
{
    [DisplayName(nameof(Manisz) + "2")]
    [ContractAuthor("<Your Name Or Company Here>", "<Your Public Email Here>")]
    [ContractDescription( "<Description Here>")]
    [ContractVersion("<Version String Here>")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractowner/Manisz.cs")]
    [ContractPermission(Permission.Any, Method.Any)]
    public partial class Manisz : Neo.SmartContract.Framework.SmartContract
    {
        // TODO: Replace it with your methods.
        public static string MyMethod()
        {
            return Storage.Get(Storage.CurrentContext, "Hello");
        }
    }
}
