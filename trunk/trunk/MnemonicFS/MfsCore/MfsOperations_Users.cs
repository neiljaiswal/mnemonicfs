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
using System.Text.RegularExpressions;
using System.Diagnostics;
using MnemonicFS.MfsUtils.MfsConfig;

namespace MnemonicFS.MfsCore {
    public partial class MfsOperations {
        [Serializable]
        public static class User {
            #region << User-related Operations >>

            internal static string ProcessUserIDStr (string userID) {
                return userID.Trim ().ToLower ();
            }

            public static ulong New (string userID, string passwordHash) {
                if (userID == null || userID.Length == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "User id string")
                    );
                }

                userID = ProcessUserIDStr (userID);

                if (Exists (userID)) {
                    throw new MfsDuplicateNameException (
                        MfsErrorMessages.GetMessage (MessageType.ALREADY_EXISTS, "User")
                    );
                }

                ValidateString (passwordHash, ValidationCheckType.PASSWORD_HASH);

                Regex regex = new Regex (REGEX_STRING);
                if (!regex.IsMatch (userID)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.BAD_FORMAT, "User id string")
                    );
                }

                return CreateUserSpecificPaths (userID, passwordHash);
            }

            private static ulong CreateUserSpecificPaths (string userID, string passwordHash) {
                string userSpecificPath = MfsStorageDevice.CreateUserPath (userID);

                ulong uid = MfsDBOperations.CreateUser (userID, passwordHash, userSpecificPath);
                Debug.Print ("Done creating user with userID: " + uid);

                return uid;
            }

            public static int UpdatePassword (string userID, string newPasswordHash) {
                if (userID == null) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL, "User id string")
                    );
                }

                userID = ProcessUserIDStr (userID);

                if (MfsDBOperations.DoesUserExist (userID) == 0) {
                    throw new MfsNonExistentUserException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_USER, userID)
                    );
                }

                ValidateString (newPasswordHash, ValidationCheckType.PASSWORD_HASH);

                return MfsDBOperations.UpdateUserPassword (userID, newPasswordHash);
            }

            public static List<string> All () {
                return MfsDBOperations.GetMfsUsers ();
            }

            public static int GetCount () {
                return MfsDBOperations.GetUserCount ();
            }

            public static int Delete (string userID, bool deleteUserStorage, bool deleteUserLogs) {
                userID = ProcessUserIDStr (userID);

                if (MfsDBOperations.DoesUserExist (userID) == 0) {
                    throw new MfsNonExistentUserException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_USER, userID)
                    );
                }

                if (deleteUserStorage) {
                    DeleteStorage (userID);
                }

                if (deleteUserLogs) {
                    Log.DeleteUserFileLogs (userID);
                }

                return MfsDBOperations.DeleteUser (userID);
            }

            private static void DeleteStorage (string userID) {
                userID = ProcessUserIDStr (userID);

                string path = MfsDBOperations.GetUserSpecificPath (userID);
                Debug.Print ("Got user specific path: " + path);

                // First delete user db:
                MfsStorageDevice.DeleteUserDB (path);
                // And also all user file system data:
                MfsStorageDevice.DeleteUserFileSystemObjects (path);

                string dirToDelete = Config.GetStorageBasePath () + path;
                MfsStorageDevice.DeleteDirectoryIfEmpty (dirToDelete);
            }

            public static bool Exists (string userID) {
                userID = ProcessUserIDStr (userID);

                return (MfsDBOperations.DoesUserExist (userID) != 0);
            }

            public static bool IsNameCompliant (string userID) {
                userID = ProcessUserIDStr (userID);

                Regex regex = new Regex (REGEX_STRING);
                return regex.IsMatch (userID);
            }

            #endregion << User-related Operations >>
        }
    }
}
