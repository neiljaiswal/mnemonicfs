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
        public class _Collection : IDisposable {
            private MfsOperations _parent;
            private MfsDBOperations _dbOperations;

            private _Collection (MfsOperations parent) {
                _parent = parent;
                _dbOperations = new MfsDBOperations (_parent._userID, _parent._userSpecificPath);
            }

            private static _Collection _theObject = null;

            internal static _Collection GetObject (MfsOperations parent) {
                if (_theObject == null) {
                    _theObject = new _Collection (parent);
                }

                return _theObject;
            }

            #region << IDisposable Members >>

            public void Dispose () {
                _theObject = null;
            }

            #endregion << IDisposable Members >>

            #region << Client-input Check Methods >>

            internal void DoCollectionChecks (ulong collectionID) {
                if (collectionID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Collection id")
                    );
                }

                if (!_dbOperations.DoesCollectionExist (collectionID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Collection")
                    );
                }
            }

            internal void DoCollectionChecks (string collectionName) {
                if (collectionName == null || collectionName.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Collection name")
                    );
                }

                // NO! Don't check for collection existence here.
            }

            #endregion << Client-input Check Methods >>

            #region << Collection-related Operations >>

            /// <summary>
            /// This method creates a new collection.
            /// </summary>
            /// <param name="collectionName">Name of the new collection.</param>
            /// <param name="collectionDesc">Description of the new collection.</param>
            /// <returns>A unique integer that identifies the collection across the entire system.</returns>
            public ulong New (string collectionName, string collectionDesc) {
                ValidateString (collectionName, ValidationCheckType.COLLECTION_NAME);
                ValidateString (collectionDesc, ValidationCheckType.COLLECTION_DESC);
                DoCollectionChecks (collectionName);
                if (Exists (collectionName)) {
                    throw new MfsDuplicateNameException (
                        MfsErrorMessages.GetMessage (MessageType.ALREADY_EXISTS, "Collection name")
                    );
                }

                return _dbOperations.CreateCollection (collectionName, collectionDesc);
            }

            /// <summary>
            /// This method deletes a collection.
            /// </summary>
            /// <param name="collectionID">The collection to be deleted.</param>
            /// <returns>Number of collections deleted. Always one.</returns>
            public int Delete (ulong collectionID) {
                DoCollectionChecks (collectionID);

                return _dbOperations.DeleteCollection (collectionID);
            }

            /// <summary>
            /// This method tells the caller if a collection exists.
            /// </summary>
            /// <param name="collectionID">The collection name for which this information is sought.</param>
            /// <returns>A boolean value indicating whether the collection exists or not.</returns>
            public bool Exists (ulong collectionID) {
                if (collectionID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Collection id")
                    );
                }

                return _dbOperations.DoesCollectionExist (collectionID);
            }

            /// <summary>
            /// This method tells the caller if a collection exists.
            /// </summary>
            /// <param name="collectionName">The collection name for which this information is sought.</param>
            /// <returns>A boolean value indicating whether the collection exists or not.</returns>
            public bool Exists (string collectionName) {
                DoCollectionChecks (collectionName);

                return _dbOperations.DoesCollectionExist (collectionName);
            }

            /// <summary>
            /// This method retrieves a collection's name.
            /// </summary>
            /// <param name="collectionID">Id of the collection the name of which is sought.</param>
            /// <returns>Name of the collection.</returns>
            public void Get (ulong collectionID, out string collectionName, out string collectionDesc) {
                DoCollectionChecks (collectionID);

                _dbOperations.GetCollectionNameAndDesc (collectionID, out collectionName, out collectionDesc);
            }

            public ulong IDFromName (string collectionName) {
                DoCollectionChecks (collectionName);
                if (!Exists (collectionName)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Collection name")
                    );
                }

                return _dbOperations.GetCollectionIDFromName (collectionName);
            }

            public List<ulong> All () {
                return _dbOperations.GetAllCollections ();
            }

            /// <summary>
            /// This method updates a collection's name.
            /// </summary>
            /// <param name="collectionID">Id of the collection the name of which is to be updated.</param>
            /// <param name="newCollectionName">New name for the collection.</param>
            /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
            public bool UpdateName (ulong collectionID, string newCollectionName) {
                DoCollectionChecks (collectionID);
                ValidateString (newCollectionName, ValidationCheckType.COLLECTION_NAME);

                return _dbOperations.UpdateCollectionName (collectionID, newCollectionName);
            }

            /// <summary>
            /// This method updates the description of a collection.
            /// </summary>
            /// <param name="collectionID">Id of the collection the description of which is to be updated.</param>
            /// <param name="newCollectionDesc">New description for the collection.</param>
            /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
            public bool UpdateDesc (ulong collectionID, string newCollectionDesc) {
                DoCollectionChecks (collectionID);
                ValidateString (newCollectionDesc, ValidationCheckType.COLLECTION_DESC);

                return _dbOperations.UpdateCollectionDesc (collectionID, newCollectionDesc);
            }

            /// <summary>
            /// This method deletes all collections within the system. Needless to add, this should be used very carefully.
            /// </summary>
            /// <returns>An integer value indicating how many collections have been deleted.</returns>
            public int DeleteAll () {
                return _dbOperations.DeleteAllCollectionsInSystem ();
            }

            #endregion << Collection-related Operations >>

            #region << Collections-Documents Operations >>

            public bool AddDocument (ulong documentID, ulong collectionID) {
                _parent.DocumentObj.DoDocumentChecks (documentID);
                DoCollectionChecks (collectionID);

                return _dbOperations.AddDocumentToCollection (documentID, collectionID);
            }

            public bool RemoveFrom (ulong documentID, ulong collectionID) {
                _parent.DocumentObj.DoDocumentChecks (documentID);
                DoCollectionChecks (collectionID);

                return _dbOperations.RemoveDocumentFromCollection (documentID, collectionID);
            }

            public List<ulong> CollectionsWith (ulong documentID) {
                _parent.DocumentObj.DoDocumentChecks (documentID);

                return _dbOperations.GetCollectionsWithDocument (documentID);
            }

            public List<ulong> GetDocuments (ulong collectionID) {
                DoCollectionChecks (collectionID);

                return _dbOperations.GetDocumentsInCollection (collectionID);
            }

            public void AddToMultiple (ulong documentID, List<ulong> collectionIDs) {
                _parent.DocumentObj.DoDocumentChecks (documentID);
                _parent.ValidateList (collectionIDs, false, "Collection");

                foreach (ulong collectionID in collectionIDs) {
                    DoCollectionChecks (collectionID);
                }

                foreach (ulong collectionID in collectionIDs) {
                    _dbOperations.AddDocumentToCollection (documentID, collectionID);
                }
            }

            public void AddTo (List<ulong> documentIDs, ulong collectionID) {
                _parent.ValidateList (documentIDs, false, "Document");
                DoCollectionChecks (collectionID);

                foreach (ulong fileID in documentIDs) {
                    _parent.DocumentObj.DoDocumentChecks (fileID);
                }

                foreach (ulong documentID in documentIDs) {
                    _dbOperations.AddDocumentToCollection (documentID, collectionID);
                }
            }

            public void AddToMultiple (List<ulong> documentIDs, List<ulong> collectionIDs) {
                _parent.ValidateList (documentIDs, false, "Document");
                _parent.ValidateList (collectionIDs, false, "Collection");

                foreach (ulong documentID in documentIDs) {
                    _parent.DocumentObj.DoDocumentChecks (documentID);
                }

                foreach (ulong collectionID in collectionIDs) {
                    DoCollectionChecks (collectionID);
                }

                foreach (ulong documentID in documentIDs) {
                    foreach (ulong collectionID in collectionIDs) {
                        _dbOperations.AddDocumentToCollection (documentID, collectionID);
                    }
                }
            }

            public bool IsDocumentIn (ulong documentID, ulong collectionID) {
                _parent.DocumentObj.DoDocumentChecks (documentID);
                DoCollectionChecks (collectionID);

                return _dbOperations.IsDocumentInCollection (documentID, collectionID);
            }

            public int RemoveDocumentFromAll (ulong documentID) {
                _parent.DocumentObj.DoDocumentChecks (documentID);

                return _dbOperations.RemoveDocumentFromAllCollections (documentID);
            }

            public int RemoveAllDocuments (ulong collectionID) {
                DoCollectionChecks (collectionID);

                return _dbOperations.RemoveAllDocumentsFromCollection (collectionID);
            }

            #endregion << Collections-Documents Operations >>
        }
    }
}
