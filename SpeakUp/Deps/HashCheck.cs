using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpeakUp.Deps
{
    class HashCheck
    {
        private int hashCompareResult = -1;
        private string computedHashString = "";
        public HashCheck(string fileName, string hash)
        {
            string computedHash = GetFileChecksum(fileName, new MD5CryptoServiceProvider());
            computedHash.ToLower();
            hash = hash.ToLower();

            hashCompareResult = (hash == computedHash) ? 1 : 0;
            computedHashString = computedHash;
        }

        private string GetFileChecksum(string file, HashAlgorithm algorithm)
        {
            string result = string.Empty;

            using (FileStream fs = File.OpenRead(file))
            {
                result = BitConverter.ToString(algorithm.ComputeHash(fs)).ToLower().Replace("-", "");
            }

            return result;
        }

        public bool Result()
        {
            return (hashCompareResult == 1);
        }

        public string HashString()
        {
            return computedHashString;
        }
    }
}
