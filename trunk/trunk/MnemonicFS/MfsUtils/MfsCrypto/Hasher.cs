/**
 * Copyright © 2009, Najeeb Shaikh
 * All rights reserved.
 * http://www.mnemonicfs.org
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 * - Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 * 
 * - Neither the name of the MnemonicFS Team, nor the names of its
 * contributors may be used to endorse or promote products
 * derived from this software without specific prior written
 * permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MnemonicFS.MfsUtils.MfsCrypto {
    public static class Hasher {
        internal const int HASH_SIZE = 40;

        public static string GetSHA1 (string ipString) {
            SHA1 sha1 = new SHA1CryptoServiceProvider ();
            byte[] ipBytes = Encoding.Default.GetBytes (ipString.ToCharArray ());
            byte[] opBytes = sha1.ComputeHash (ipBytes);

            StringBuilder stringBuilder = new StringBuilder (40);
            for (int i = 0; i < opBytes.Length; i++) {
                stringBuilder.Append (opBytes[i].ToString ("x2"));
            }

            return stringBuilder.ToString ();
        }

        public static string GetFileHash (byte[] fileBytes) {
            HashAlgorithm hasher = new MD5CryptoServiceProvider ();
            byte[] bytes = hasher.ComputeHash (fileBytes);
            string hyphenedHash = BitConverter.ToString (bytes);
            return hyphenedHash.Replace ("-", "");
        }
    }
}
