﻿/**
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

namespace MnemonicFS.MfsCore {
    public partial class MfsOperations {
        [Serializable]
        public class _MasterPassword : IDisposable {
            private MfsOperations _parent;
            private MfsDBOperations _dbOperations;

            private _MasterPassword (MfsOperations parent) {
                _parent = parent;
                _dbOperations = new MfsDBOperations (_parent._userID, _parent._userSpecificPath);
            }

            private static _MasterPassword _theObject = null;

            internal static _MasterPassword GetObject (MfsOperations parent) {
                if (_theObject == null) {
                    _theObject = new _MasterPassword (parent);
                }

                return _theObject;
            }

            #region << IDisposable Members >>

            public void Dispose () {
                _theObject = null;
            }

            #endregion << IDisposable Members >>

            #region << Master Password Operations >>

            public bool IsSet () {
                return _dbOperations.IsMasterPasswordSet ();
            }

            public int Set (string passwordHash) {
                ValidateString (passwordHash, ValidationCheckType.PASSWORD_HASH);

                return _dbOperations.SetMasterPassword (passwordHash);
            }

            public int Reset () {
                return _dbOperations.ResetMasterPassword ();
            }

            public string GetHash () {
                return _dbOperations.GetMasterPasswordHash ();
            }

            public bool Authenticate (string passwordHash) {
                ValidateString (passwordHash, ValidationCheckType.PASSWORD_HASH);

                return _dbOperations.AuthenticateMasterPassword (passwordHash);
            }

            #endregion << Master Password Operations >>
        }
    }
}
