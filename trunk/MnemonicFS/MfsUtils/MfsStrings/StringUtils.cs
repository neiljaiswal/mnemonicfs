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
using System.Text.RegularExpressions;
using System.IO;
using MnemonicFS.MfsExceptions;
using System.Globalization;

namespace MnemonicFS.MfsUtils.MfsStrings {
    internal static class StringUtils {
        public static List<string> BreakStringIntoTokens (string ipString, int numCharsInEachPiece) {
            if (ipString == null) {
                throw new MfsIllegalArgumentException (
                    MfsErrorMessages.GetMessage (MessageType.NULL, "Input string")
                );
            }
            if (numCharsInEachPiece < 1 || numCharsInEachPiece > ipString.Length) {
                throw new MfsIllegalArgumentException (
                    MfsErrorMessages.GetMessage (MessageType.INADEQUATE, "Num chars in input string")
                );
            }

            List<string> list = new List<string> ();

            int startIndex = 0;
            while (true) {
                string str = ipString.Substring (startIndex, numCharsInEachPiece);
                list.Add (str);
                startIndex += numCharsInEachPiece;

                if (startIndex >= ipString.Length) {
                    break;
                }

                if (startIndex + numCharsInEachPiece > ipString.Length) {
                    // Last case:
                    str = ipString.Substring (startIndex, ipString.Length - startIndex);
                    list.Add (str);
                    break;
                }
            }

            return list;
        }

        public static string GetFirstNChars (string ipStr, int numChars) {
            if (ipStr.Length < numChars) {
                return ipStr;
            }

            return ipStr.Substring (0, numChars);
        }

        public static string GetPureAlphaStr (string ipStr) {
            string opStr = "";

            foreach (char ch in ipStr) {
                if (ch >= '0' && ch <= '9') {
                    switch (ch) {
                        case '0':
                            opStr += 'a';
                            break;
                        case '1':
                            opStr += 'b';
                            break;
                        case '2':
                            opStr += 'c';
                            break;
                        case '3':
                            opStr += 'd';
                            break;
                        case '4':
                            opStr += 'e';
                            break;
                        case '5':
                            opStr += 'f';
                            break;
                        case '6':
                            opStr += 'g';
                            break;
                        case '7':
                            opStr += 'h';
                            break;
                        case '8':
                            opStr += 'i';
                            break;
                        case '9':
                            opStr += 'j';
                            break;
                    }
                } else {
                    opStr += ch;
                }
            }

            return opStr;
        }

        public static string GetAsZeroPaddedTwoCharString (int number) {
            if (number >= 0 && number <= 9) {
                return "0" + number.ToString (CultureInfo.InvariantCulture);
            }
            return number.ToString (CultureInfo.InvariantCulture);
        }

        public static string GetAsZeroPaddedThreeCharString (int number) {
            if (number >= 0 && number <= 9) {
                return "00" + number.ToString (CultureInfo.InvariantCulture);
            }
            if (number >= 10 && number <= 99) {
                return "0" + number.ToString (CultureInfo.InvariantCulture);
            }
            return number.ToString (CultureInfo.InvariantCulture);
        }

        public static string GetAsZeroPaddedFourCharString (int number) {
            if (number >= 0 && number <= 9) {
                return "000" + number.ToString (CultureInfo.InvariantCulture);
            }
            if (number >= 10 && number <= 99) {
                return "00" + number.ToString (CultureInfo.InvariantCulture);
            }
            if (number >= 100 && number <= 999) {
                return "0" + number.ToString (CultureInfo.InvariantCulture);
            }
            return number.ToString (CultureInfo.InvariantCulture);
        }

        public static byte[] ConvertToByteArray (string str) {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding ();
            return encoding.GetBytes (str);
        }

        public static string ConvertToString (byte[] bytes) {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding ();
            return encoding.GetString (bytes);
        }

        public static string GetFileExtension (string fileName) {
            int dotIndex = fileName.LastIndexOf ('.');
            return fileName.Substring (dotIndex + 1);
        }

        internal static bool FileNameContainsIllegalChars (string fileName) {
            return (fileName.IndexOfAny (Path.GetInvalidFileNameChars ()) != -1);
        }
    }
}
