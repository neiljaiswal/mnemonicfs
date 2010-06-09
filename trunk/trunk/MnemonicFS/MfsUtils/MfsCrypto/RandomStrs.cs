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

namespace MnemonicFS.MfsUtils.MfsCrypto {
    internal static class RandomStrs {
        internal static string GetRandomPassword (int numChars, bool allowSpecialChars) {
            return GetRandomString (numChars);
        }

        internal static string GetRandomFileName () {
            return GetRandomString (5);
        }

        private static int callNo = 1;
        /// <summary>
        /// This method returns a string of random characters.
        /// </summary>
        /// <param name="length">Number of characters expected within the returned string.</param>
        /// <returns>A string of the size requested.</returns>
        internal static string GetRandomString (int length) {
            DateTime dateTime = DateTime.Now;
            int ms = dateTime.Millisecond;
            string opStr = "";

            for (int i = 0; i < length; ++i) {
                Random random = new Random ((i + 1) * callNo++ * ms);
                int val1 = random.Next () % 26 + 97;
                char ch = (char) val1;
                opStr += ch;
            }

            return opStr;
        }
    }
}
