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
using System.IO;

namespace MnemonicFS.MfsUtils.MfsConfig {
    /// <summary>
    /// This class, very simply, reads the contents of the properties file,
    /// the path of which is specified within the constructor, and provides
    /// simple methods for either getting a list of all keys, or getting the
    /// value against any particular key.
    /// It has only three public members: one, the constructor; and two
    /// methods: one for obtaining a list of all properties, and the other
    /// for obtaining the property (value) for a particular key.
    /// </summary>
    internal class PropertiesFileReader {
        private Dictionary<string, string> _dictionaryKeysValues = null;
        private List<KeyValuePair<string, string>> _allPairs = null;
        private string _filename = null;

        public PropertiesFileReader (string filename) {
            _filename = filename;
            ReadFileOnce ();
        }

        public string GetValueForKey (string key) {
            foreach (KeyValuePair<string, string> pair in _allPairs) {
                if (pair.Key.Equals (key)) {
                    return pair.Value;
                }
            }

            throw new Exception ("Key not found.");
        }

        public List<string> GetAllKeys () {
            List<string> allKeys = new List<string> ();

            foreach (KeyValuePair<string, string> pair in _allPairs) {
                allKeys.Add (pair.Key);
            }

            return allKeys;
        }

        private void ReadFileOnce () {
            ReadFileLineByLine ();
            Dictionary<string, string>.Enumerator enumerator = _dictionaryKeysValues.GetEnumerator ();
            _allPairs = new List<KeyValuePair<string, string>> ();
            while (enumerator.MoveNext ()) {
                KeyValuePair<string, string> pair = enumerator.Current;
                _allPairs.Add (pair);
            }
        }

        private void ReadFileLineByLine () {
            TextReader textReader = new StreamReader (_filename);
            _dictionaryKeysValues = new Dictionary<string, string> ();

            while (textReader.Peek () != -1) {
                string line = textReader.ReadLine ();
                if (line.StartsWith (" ") ||
                    line.StartsWith ("#") ||
                    line.StartsWith (";") ||
                    line.StartsWith ("//") ||
                    line.Length < 1) {
                    continue;
                }

                int index = line.IndexOf ('=');
                if (index == -1) {
                    continue;
                }

                string key = line.Substring (0, index);
                string value = line.Substring (index + 1, line.Length - key.Length - 1);

                _dictionaryKeysValues.Add (key.Trim (), value.Trim ());
            }

            textReader.Close ();
        }
    }
}
