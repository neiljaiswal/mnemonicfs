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
        public static class Bytestream {
            #region << Bare Bytestream Storage / Retrieval Operations >>

            /// <summary>
            /// This is a static method for user-independent operation that can done by the client utility
            /// for saving a byte stream to the storage. This method is especially useful if the client would
            /// want to, say, stripe a file across multiple clients across a network.
            /// </summary>
            /// <param name="byteStream">The byte stream to be saved.</param>
            /// <param name="password">Password based on which the byte stream can be recovered.</param>
            /// <param name="referenceNumber">A reference number that could be used by the client to
            /// specify some custom value.
            /// </param>
            /// <returns>A unique id that identifies the byte stream on the storage.</returns>
            public static ulong Store (byte[] byteStream, string passphrase, int referenceNumber) {
                if (byteStream == null || byteStream.Length == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Byte stream")
                    );
                }

                if (passphrase == null || passphrase.Length == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Passphrase")
                    );
                }

                string assumedFileName;
                string archiveName;
                string destDir;

                MfsStorageDevice.SaveByteArray (byteStream, passphrase, out assumedFileName, out archiveName, out destDir);

                return MfsDBOperations.SaveByteArrayMetaData (assumedFileName, archiveName, destDir, referenceNumber);
            }

            public static byte[] Retrieve (ulong byteStreamID, string passphrase, out int referenceNumber) {
                if (byteStreamID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Byte stream id")
                    );
                }

                if (passphrase == null || passphrase.Length == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Passphrase")
                    );
                }

                if (!Exists (byteStreamID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Byte stream")
                    );
                }

                string assumedFileName = null;
                string archiveName = null;
                string archivePath = null;

                MfsDBOperations.GetByteStreamMetaData (byteStreamID, out assumedFileName, out archiveName, out archivePath, out referenceNumber);

                return MfsStorageDevice.RetrieveByteArrayFromZippedFile (archivePath + archiveName, assumedFileName, passphrase);
            }

            public static int GetReferenceNumber (ulong byteStreamID) {
                if (byteStreamID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Byte stream id")
                    );
                }

                if (!Exists (byteStreamID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Byte stream")
                    );
                }

                return MfsDBOperations.GetByteStreamReferenceNumber (byteStreamID);
            }

            public static void Delete (ulong byteStreamID, string passphrase) {
                if (byteStreamID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Byte stream id")
                    );
                }

                if (passphrase == null || passphrase.Length == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Passphrase")
                    );
                }

                if (!Exists (byteStreamID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Byte stream")
                    );
                }

                string assumedFileName = null;
                string archiveName = null;
                string archivePath = null;
                int referenceNumber;

                MfsDBOperations.GetByteStreamMetaData (byteStreamID, out assumedFileName, out archiveName, out archivePath, out referenceNumber);
                MfsDBOperations.DeleteByteStreamMetaData (byteStreamID);

                MfsStorageDevice.DeleteFile (archivePath, archiveName);
            }

            public static bool Exists (ulong byteStreamID) {
                if (byteStreamID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Byte stream id")
                    );
                }

                return MfsDBOperations.DoesByteStreamExist (byteStreamID);
            }

            #endregion << Bare Bytestream Storage / Retrieval Operations >>
        }
    }
}
