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
using MnemonicFS.MfsExceptions;

namespace MnemonicFS.MfsCore {
    public partial class MfsOperations {
        [Serializable]
        public class _Credentials : IDisposable {
            private MfsOperations _parent;
            private MfsDBOperations _dbOperations;

            private _Credentials (MfsOperations parent) {
                _parent = parent;
                _dbOperations = new MfsDBOperations (_parent._userID, _parent._userSpecificPath);
            }

            private static _Credentials _theObject = null;

            internal static _Credentials GetObject (MfsOperations parent) {
                if (_theObject == null) {
                    _theObject = new _Credentials (parent);
                }

                return _theObject;
            }

            #region << IDisposable Members >>

            public void Dispose () {
                _theObject = null;
            }

            #endregion << IDisposable Members >>

            #region << Credentials Operations >>

            public ulong New (string appUrl, string username, string accessKey) {
                ValidateString (appUrl, ValidationCheckType.APP_URL);
                ValidateString (username, ValidationCheckType.USERNAME);
                if (accessKey == null) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL, "Access key")
                    );
                }

                if (_dbOperations.DoesCredentialExist (appUrl, username)) {
                    throw new MfsIllegalOperationException (
                        MfsErrorMessages.GetMessage (MessageType.ALREADY_EXISTS, "App url and username")
                    );
                }

                return _dbOperations.AddCredentials (appUrl, username, accessKey);
            }

            public int Delete (ulong credentialID) {
                return _dbOperations.DeleteCredentials (credentialID);
            }

            public bool Exists (ulong credentialID) {
                if (credentialID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Credentials id")
                    );
                }

                return _dbOperations.DoesCredentialExist (credentialID);
            }

            public bool Exists (string appUrl, string username) {
                ValidateString (appUrl, ValidationCheckType.APP_URL);
                ValidateString (username, ValidationCheckType.USERNAME);

                return _dbOperations.DoesCredentialExist (appUrl, username);
            }

            #endregion << Credentials Operations >>
        }
    }
}
