using System;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using Nethereum.Web3;

namespace SkalePOWCSharp
{
    internal class Program
    {
        const int DIFFICULTY = 1000000;
        static async Task Main(string[] args)
        {
           
            Web3 web3 = new Web3("https://staging-v3.skalenodes.com/v1/staging-faint-slimy-achird");

            var gasAmount = new BigInteger(100000);
            var address = "0x66999527F9c9a197bF5D8B4e45C58842A8A9F4DC";
            var nonce = "0x12345678901234567890123456789012";

            await MineFreeGas(gasAmount, address, nonce, web3);
        }
        
        
        public static async Task MineFreeGas(BigInteger gasAmount, string address, string nonce, Web3 web3)
        {
            Console.WriteLine($"Mining free gas: {gasAmount}");
            var nonceHash = new BigInteger(Sha3Keccack.Current.CalculateHash(nonce.HexToByteArray().ToArray()));
            Console.WriteLine(nonceHash.ToString());
            var addressHash = new BigInteger(Sha3Keccack.Current.CalculateHash(address.HexToByteArray().ToArray()));
            Console.WriteLine(addressHash.ToString());
            var nonceAddressXOR = nonceHash ^ addressHash;
            Console.WriteLine(nonceAddressXOR.ToString());
            var maxNumber = BigInteger.Pow(2, 256) - 1;
            Console.WriteLine(maxNumber.ToString());
            var divConstant = maxNumber / DIFFICULTY;
            BigInteger? candidate;
            while (true)
            {
                candidate = new BigInteger(Nethereum.Signer.EthECKey.GenerateKey().GetPrivateKeyAsBytes());
                {
                    var candidateHash = new BigInteger(Sha3Keccack.Current.CalculateHash(candidate.Value.ToByteArray()));
                    var resultHash = nonceAddressXOR ^ candidateHash;
                    var externalGas = divConstant / resultHash;
                    if (externalGas >= gasAmount)
                    {
                        break;
                    }
                }
                Console.WriteLine(candidate.ToString());
            }
        }
    }
}