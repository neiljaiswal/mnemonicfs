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
using NUnit.Framework;
using MnemonicFS.Tests.Base;
using MnemonicFS.MfsCore;
using MnemonicFS.Tests.Utils;
using MnemonicFS.MfsExceptions;

namespace MnemonicFS.Tests.Config {
    [TestFixture]
    public class Tests_ConfigMethod_GetConfigValues : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            List<string> configKeys = MfsOperations.ConfigFile.AllKeys ();
            Assert.IsNotNull (configKeys, "Returned a null object as config key list.");
        }
    }

    [TestFixture]
    public class Tests_ConfigMethod_DoesConfigKeyExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            List<string> configKeys = MfsOperations.ConfigFile.AllKeys ();
            Assert.IsTrue (configKeys.Count >= 1, "Test cannot be done since there should be at least one key.");

            // Take any one key:
            string key = configKeys[0];
            bool keyExists = MfsOperations.ConfigFile.Exists (key);
            Assert.IsTrue (keyExists, "Shows existing key as non-existent.");
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            // We will take this test of the faith that such a large key does not already
            // exist in the config file:
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE * TYPICAL_MULTI_VALUE);
            bool keyExists = MfsOperations.ConfigFile.Exists (key);
            Assert.IsFalse (keyExists, "Shows non-existing key as existent.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullKey_Illegal () {
            string nullKey = null;
            MfsOperations.ConfigFile.Exists (nullKey);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyKey_Illegal () {
            string emptyKey = string.Empty;
            MfsOperations.ConfigFile.Exists (emptyKey);
        }
    }

    [TestFixture]
    public class Tests_ConfigMethod_GetConfigValue : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            List<string> configKeys = MfsOperations.ConfigFile.AllKeys ();

            foreach (string key in configKeys) {
                string value = MfsOperations.ConfigFile.Value (key);
                Assert.IsNotNull (value, "Returned a null value for key.");
                Assert.IsNotEmpty (value, "Returned an empty value for key.");
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentKey_Illegal () {
            string nonExistentKey = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            do {
                nonExistentKey = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (MfsOperations.ConfigFile.Exists (nonExistentKey));

            MfsOperations.ConfigFile.Value (nonExistentKey);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullKey_Illegal () {
            string nullKey = null;
            MfsOperations.ConfigFile.Value (nullKey);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyKey_Illegal () {
            string emptyKey = string.Empty;
            MfsOperations.ConfigFile.Value (emptyKey);
        }
    }

    [TestFixture]
    public class Tests_ConfigMethod_GetConfigComment : TestMfsOperationsBase {
        [Test]
        public void Test_Sanity_Check () {
        }
    }

    [TestFixture]
    public class Tests_ConfigMethod_AddConfigKeyValue : TestMfsOperationsBase {
        [Test]
        public void Test_Sanity_Check () {
            string anyKey = null;
            string anyVal = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            do {
                anyKey = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (MfsOperations.ConfigFile.Exists (anyKey));

            bool success = MfsOperations.ConfigFile.New (anyKey, anyVal);
            Assert.IsTrue (success, "Key-value pair not added successfully to config file.");
            Assert.IsTrue (MfsOperations.ConfigFile.Exists (anyKey), "Config file does not have specified key.");

            string val = MfsOperations.ConfigFile.Value (anyKey);
            Assert.AreEqual (anyVal, val, "Retrieved value for newly-added key is not as expected.");

            MfsOperations.ConfigFile.Delete (anyKey);
        }

        [Test]
        public void Test_Sanity_Check_WithComments () {
            string anyKey = null;
            string anyVal = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            List<string> commentLines = new List<string> ();
            for (int i = 0; i < TYPICAL_MULTI_VALUE; ++i) {
                string commentLine = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                commentLines.Add (commentLine);
            }

            do {
                anyKey = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (MfsOperations.ConfigFile.Exists (anyKey));

            bool success = MfsOperations.ConfigFile.New (anyKey, anyVal, commentLines);
            Assert.IsTrue (success, "Key-value pair not added successfully to config file.");
            Assert.IsTrue (MfsOperations.ConfigFile.Exists (anyKey), "Config file does not have specified key.");

            string val = MfsOperations.ConfigFile.Value (anyKey);
            Assert.AreEqual (anyVal, val, "Retrieved value for newly-added key is not as expected.");

            MfsOperations.ConfigFile.Delete (anyKey);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullKey_Illegal () {
            string nullKey = null;
            string anyVal = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            MfsOperations.ConfigFile.New (nullKey, anyVal);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyKey_Illegal () {
            string emptyKey = string.Empty;
            string anyVal = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            MfsOperations.ConfigFile.New (emptyKey, anyVal);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullValue_Illegal () {
            string anyKey = null;
            string nullVal = null;

            do {
                anyKey = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (MfsOperations.ConfigFile.Exists (anyKey));

            MfsOperations.ConfigFile.New (anyKey, nullVal);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyValue_Illegal () {
            string anyKey = null;
            string emptyVal = string.Empty;

            do {
                anyKey = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (MfsOperations.ConfigFile.Exists (anyKey));

            MfsOperations.ConfigFile.New (anyKey, emptyVal);
        }
    }

    [TestFixture]
    public class Tests_ConfigMethod_ModifyConfigValue : TestMfsOperationsBase {
        [Test]
        public void Test_Sanity_Check () {
        }
    }

    [TestFixture]
    public class Tests_ConfigMethod_DeleteConfigComment : TestMfsOperationsBase {
        [Test]
        public void Test_Sanity_Check () {
        }
    }
}
