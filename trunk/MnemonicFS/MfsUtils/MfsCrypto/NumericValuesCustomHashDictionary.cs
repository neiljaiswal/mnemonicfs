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
using System.Diagnostics;
using MnemonicFS.MfsUtils.MfsStrings;
using MnemonicFS.MfsUtils.MfsCrypto;

namespace MnemonicFS.MfsUtils.MfsCrypto {
    internal static class NumericValuesCustomHashDictionary {
        private static Dictionary<int, string> _dictionary = new Dictionary<int, string> ();

        static NumericValuesCustomHashDictionary () {
            Trace.TraceInformation ("Creating dictionary.");
            for (int i = 1; i <= 9999; ++i) {
                string val = HashAndFirst4Chars (i);
                _dictionary.Add (i, val);
            }
        }

        private static string HashAndFirst4Chars (int number) {
            string hash = Hasher.GetSHA1 (number.ToString ());
            string first4Chars = StringUtils.GetFirstNChars (hash, 4);
            string pureAlphaStr = StringUtils.GetPureAlphaStr (first4Chars);

            return pureAlphaStr;
        }

        internal static string GetCustomHashValue (int number) {
            if (!_dictionary.ContainsKey (number)) {
                string val = HashAndFirst4Chars (number);
                _dictionary.Add (number, val);
            }

            return _dictionary[number];
        }
    }
}
