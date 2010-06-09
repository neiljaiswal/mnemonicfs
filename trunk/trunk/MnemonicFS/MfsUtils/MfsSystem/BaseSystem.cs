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
using System.Data.SQLite;
using MnemonicFS.MfsUtils.MfsConfig;
using MnemonicFS.MfsUtils.MfsCrypto;

namespace MnemonicFS.MfsUtils.MfsSystem {
    internal static class BaseSystem {
        #region << Constants & Initialization >>

        private static string _systemDBName;
        private static string _systemDBFileName;
        private static string _systemDBPassword;

        private static string _userDBName = null;
        private static string _userDBFileName;
        private static string _userDBPassword;

        private static string _logDBName = null;
        private static string _logDBFileName;
        private static string _logDBPassword;

        static BaseSystem () {
            _systemDBFileName = "sysdb";
            _systemDBPassword = Hasher.GetSHA1 ("$0me_p@$$w0rd11");

            _userDBFileName = "usrdb";
            _userDBPassword = Hasher.GetSHA1 ("$0me_p@$$w0rd12");

            _logDBFileName = "logdb";
            _logDBPassword = Hasher.GetSHA1 ("$0me_p@$$w0rd13");
        }

        #endregion

        #region << System DB-related Operations >>

        internal static string GetSystemDBBaseLocation () {
            if (_systemDBName == null) {
                string appDir = Config.GetStorageBasePath ();
                string sep = GetFileSystemSeparator ();
                if (appDir.EndsWith (sep)) {
                    _systemDBName = appDir;
                } else {
                    _systemDBName = appDir + sep;
                }
            }

            return _systemDBName;
        }

        internal static string GetSystemDBFileName () {
            return _systemDBFileName;
        }

        internal static string GetSystemDBPassword () {
            return _systemDBPassword;
        }

        #endregion

        #region << User DB-related Operations >>

        internal static string GetUserDBBaseLocation () {
            if (_userDBName == null) {
                string appDir = Config.GetStorageBasePath ();
                string sep = GetFileSystemSeparator ();
                if (appDir.EndsWith (sep)) {
                    _userDBName = appDir;
                } else {
                    _userDBName = appDir + sep;
                }
            }

            return _userDBName;
        }

        internal static string GetUserDBFileName () {
            return _userDBFileName;
        }

        internal static string GetUserDBPassword () {
            return _userDBPassword;
        }

        #endregion

        #region << Log DB-related Operations >>

        internal static string GetLogDBBaseLocation () {
            if (_logDBName == null) {
                string appDir = Config.GetStorageBasePath ();
                string sep = GetFileSystemSeparator ();
                if (appDir.EndsWith (sep)) {
                    _logDBName = appDir;
                } else {
                    _logDBName = appDir + sep;
                }
            }

            return _logDBName;
        }

        internal static string GetLogDBFileName () {
            return _logDBFileName;
        }

        internal static string GetLogDBPassword () {
            return _logDBPassword;
        }

        #endregion

        #region << Base OS-related Operations >>

        internal static string GetFileSystemSeparator () {
            return @"\";
        }

        #endregion
    }
}
