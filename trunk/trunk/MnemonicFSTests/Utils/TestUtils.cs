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
using MnemonicFS.Tests.Base;
using System.Security.Cryptography;
using System.IO;
using Ionic.Zip;

namespace MnemonicFS.Tests.Utils {
    public static class TestUtils {
        /// <summary>
        /// This method returns a randomly generated file name.
        /// </summary>
        /// <returns>A randomly generated file name.</returns>
        public static string GetAnyFileName () {
            return GetAWord (5) + "." + GetAWord (3);
        }

        /// <summary>
        /// This method returns a byte array of the file size as specified.
        /// If fileSize == ZERO_FILE_SIZE, byte array of zero-length is returned.
        /// If fileSize == SMALL_FILE_SIZE, byte array of one hundred bytes is returned.
        /// If fileSize == MEDIUM_FILE_SIZE, byte array of five hundred bytes is returned.
        /// If fileSize == LARGE_FILE_SIZE, byte array of one thousand bytes is returned.
        /// </summary>
        /// <param name="fileSize">File size as requested by the caller.</param>
        /// <returns>A byte array of the requested size.</returns>
        public static byte[] GetAnyFileData (FileSize fileSize) {
            byte[] fileData = null;
            switch (fileSize) {
                case FileSize.ZERO_FILE_SIZE:
                    fileData = new byte[0];
                    break;
                case FileSize.SMALL_FILE_SIZE:
                    fileData = new byte[100];
                    break;
                default:
                case FileSize.MEDIUM_FILE_SIZE:
                    fileData = new byte[500];
                    break;
                case FileSize.LARGE_FILE_SIZE:
                    fileData = new byte[1000];
                    break;
            }

            return fileData;
        }

        /// <summary>
        /// This method returns a word of the size as requested by the caller.
        /// </summary>
        /// <param name="numLetters">Number of characters expected within the returned string.</param>
        /// <returns>A string of the size requested.</returns>
        public static string GetAWord (int numLetters) {
            return GetRandomString (numLetters);
        }

        /// <summary>
        /// This method returns a sentence of the size as requested by the caller.
        /// </summary>
        /// <param name="numWords">Number of words expected within the returned string.</param>
        /// <param name="numCharsPerWord">Number of characters per word expected within the returned string.</param>
        /// <returns>A string of the size requested.</returns>
        public static string GetASentence (int numWords, int numCharsPerWord) {
            string opStr = "";

            for (int i = 0; i < numWords; ++i) {
                opStr += GetAWord (numCharsPerWord);
                if (i < numWords - 1) {
                    opStr += " ";
                }
            }

            return opStr;
        }

        private static int callNo = 1;
        /// <summary>
        /// This method returns a string of random characters.
        /// </summary>
        /// <param name="length">Number of characters expected within the returned string.</param>
        /// <returns>A string of the size requested.</returns>
        public static string GetRandomString (int length) {
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

        internal static string GetAnyEmailID () {
            return GetAWord (5) + "@" + GetAWord (5) + ".org";
        }

        internal static string GetAnyUrl () {
            return "http://" + GetAWord (5) + ".org/" + GetAWord (5);
        }

        internal static string GetFileHash (byte[] fileBytes) {
            HashAlgorithm hasher = new MD5CryptoServiceProvider ();
            byte[] bytes = hasher.ComputeHash (fileBytes);
            string hyphenedHash = BitConverter.ToString (bytes);
            return hyphenedHash.Replace ("-", "");
        }

        internal static byte[] RetrieveByteArrayFromZippedFile (string srcZipFileWithPath, string contentNameInZip, string password) {
            Stream stream = null;
            using (ZipFile zip = ZipFile.Read (srcZipFileWithPath)) {
                ZipEntry e = zip[contentNameInZip];
                if (e == null) {
                    throw new Exception ("Illegal content!");
                }
                stream = new MemoryStream ((int) e.UncompressedSize);
                if (password == null) {
                    e.Extract (stream);
                } else {
                    e.ExtractWithPassword (stream, password);
                }
            }

            byte[] bytes = new byte[stream.Length];
            stream.Seek (0, SeekOrigin.Begin);
            for (int i = 0; i < stream.Length; ++i) {
                int val = stream.ReadByte ();
                byte b = (byte) val;
                bytes[i] = b;
            }

            return bytes;
        }
    }
}
