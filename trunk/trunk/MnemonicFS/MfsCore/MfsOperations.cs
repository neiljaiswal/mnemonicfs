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
using System.Diagnostics;
using System.Text.RegularExpressions;
using MnemonicFS.MfsExceptions;
using MnemonicFS.MfsUtils.MfsCrypto;
using MnemonicFS.MfsUtils.MfsConfig;
using MnemonicFS.MfsUtils.MfsSystem;
using MnemonicFS.MfsUtils.MfsStrings;
using MnemonicFS.MfsUtils.MfsLogging;
using MnemonicFS.MfsUtils.MfsIndexing;

namespace MnemonicFS.MfsCore {
    internal enum ValidationCheckType {
        NONE = 0,
        PASSWORD_HASH,
        FILE_NAME,
        FILE_NARRATION,
        ASPECT_NAME,
        ASPECT_DESC,
        BRIEFCASE_NAME,
        BRIEFCASE_DESC,
        COLLECTION_NAME,
        COLLECTION_DESC,
        VERSION_COMMENT,
        SCHEMA_FREE_DOC_NAME,
        PREDICATE,
        APP_URL,
        USERNAME,
    };

    public enum DocumentType {
        NONE = 0,
        FILE,
        NOTE,
        URL,
        VCARD,
        SFD,
    };

    public enum GroupingType {
        NONE = 0,
        ASPECT,
        BRIEFCASE,
        COLLECTION,
    };

    public enum FilterType {
        NONE = 0,
        AND,
        OR,
    };

    [Serializable]
    public partial class MfsOperations : IDisposable {
        #region << Static Constructor >>

        static MfsOperations () {
            MfsStorageDevice.InitDevice ();
        }

        #endregion << Static Constructor >>

        #region << Constants Declarations >>

        private static int HASH_SIZE = 40;
        private static string LUCENE_INDEX_PATH = @"\index\";

        private static int MAX_FILENAME_LENGTH = Config.GetMaxFileNameLength ();
        private static int MAX_FILENARRATION_LENGTH = Config.GetMaxFileNarrationLength ();
        private static int MAX_FILE_SIZE = Config.GetMaxFileSize ();

        private static int MAX_ASPECTNAME_LENGTH = Config.GetMaxAspectNameLength ();
        private static int MAX_ASPECTDESC_LENGTH = Config.GetMaxAspectDescLength ();

        private static int MAX_BRIEFCASENAME_LENGTH = Config.GetMaxBriefcaseNameLength ();
        private static int MAX_BRIEFCASEDESC_LENGTH = Config.GetMaxBriefcaseDescLength ();

        private static int MAX_COLLECTIONNAME_LENGTH = Config.GetMaxCollectionNameLength ();
        private static int MAX_COLLECTIONDESC_LENGTH = Config.GetMaxCollectionDescLength ();

        private static int MAX_URL_LENGTH = Config.GetMaxUrlLength ();

        private static int MAX_FILEVERSIONCOMMENT_LENGTH = Config.GetMaxFileVersionCommentLength ();

        private static int MAX_SCHEMA_FREE_DOC_NAME_LENGTH = Config.GetMaxSchemaFreeDocNameLength ();

        private static int MAX_PREDICATE_LENGTH = Config.GetMaxPredicateLength ();

        private static string REGEX_STRING = Config.GetRegexString ();

        private static bool ENFORCE_MASTER_PASSWORD = Config.EnforceMasterPassword ();

        private const ulong GLOBAL_BRIEFCASE_ID = 1;

        #endregion << Constants Declarations >>

        #region << Property Getters >>

        public static int MaxFileNameLength {
            get {
                return MAX_FILENAME_LENGTH;
            }
        }

        public static int MaxFileNarrationLength {
            get {
                return MAX_FILENARRATION_LENGTH;
            }
        }

        public static int MaxFileSize {
            get {
                return MAX_FILE_SIZE;
            }
        }

        public static int MaxAspectNameLength {
            get {
                return MAX_ASPECTNAME_LENGTH;
            }
        }

        public static int MaxAspectDescLength {
            get {
                return MAX_ASPECTDESC_LENGTH;
            }
        }

        public static int MaxBriefcaseNameLength {
            get {
                return MAX_BRIEFCASENAME_LENGTH;
            }
        }

        public static int MaxBriefcaseDescLength {
            get {
                return MAX_BRIEFCASEDESC_LENGTH;
            }
        }

        public static int MaxCollectionNameLength {
            get {
                return MAX_COLLECTIONNAME_LENGTH;
            }
        }

        public static int MaxCollectionDescLength {
            get {
                return MAX_COLLECTIONDESC_LENGTH;
            }
        }

        public static int MaxUrlLength {
            get {
                return MAX_URL_LENGTH;
            }
        }

        public static ulong GlobalBriefcase {
            get {
                return GLOBAL_BRIEFCASE_ID;
            }
        }

        public static string RegexString {
            get {
                return REGEX_STRING;
            }
        }

        public static int MaxSchemaFreeDocNameLength {
            get {
                return MAX_SCHEMA_FREE_DOC_NAME_LENGTH;
            }
        }

        public static int MaxPredicateLength {
            get {
                return MAX_PREDICATE_LENGTH;
            }
        }

        public static bool EnforceMasterPassword {
            get {
                return ENFORCE_MASTER_PASSWORD;
            }
        }

        #endregion << Property Getters >>

        #region << Instance-specific Variable Declarations >>

        private string _userID;
        private string _userSpecificPath;
        private string _userAbsPath;
        private MfsDBOperations _dbOperations;
        private LuceneIndexer _indexer;

        // Public sub-objects:
        private _File FileObj = null;
        private _Document DocumentObj = null;
        private _Aspect AspectObj = null;
        private _AspectGroup AspectGroupObj = null;
        private _Briefcase BriefcaseObj = null;
        private _Collection CollectionObj = null;
        private _Note NoteObj = null;
        private _Url UrlObj = null;
        private _VCard VCardObj = null;
        private _Sfd SfdObj = null;
        private _Relation RelationObj = null;
        private _Archive ArchiveObj = null;
        private _Bookmark BookmarkObj = null;
        private _Index IndexObj = null;
        private _MasterPassword MasterPasswordObj = null;
        private _Credentials CredentialsObj = null;

        #endregion << Instance-specific Variable Declarations >>

        #region << Public Accessors to Subobjects >>

        public _File File {
            get {
                return FileObj;
            }
        }

        public _Document Document {
            get {
                return DocumentObj;
            }
        }

        public _Aspect Aspect {
            get {
                return AspectObj;
            }
        }

        public _AspectGroup AspectGroup {
            get {
                return AspectGroupObj;
            }
        }

        public _Briefcase Briefcase {
            get {
                return BriefcaseObj;
            }
        }

        public _Collection Collection {
            get {
                return CollectionObj;
            }
        }

        public _Note Note {
            get {
                return NoteObj;
            }
        }

        public _Url Url {
            get {
                return UrlObj;
            }
        }

        public _VCard VCard {
            get {
                return VCardObj;
            }
        }

        public _Sfd Sfd {
            get {
                return SfdObj;
            }
        }

        public _Relation Relation {
            get {
                return RelationObj;
            }
        }

        public _Archive Archive {
            get {
                return ArchiveObj;
            }
        }

        public _Bookmark Bookmark {
            get {
                return BookmarkObj;
            }
        }

        public _Index Index {
            get {
                return IndexObj;
            }
        }

        public _MasterPassword MasterPassword {
            get {
                return MasterPasswordObj;
            }
        }

        public _Credentials Credentials {
            get {
                return CredentialsObj;
            }
        }

        #endregion << Public Accessors to Subobjects >>

        #region << Object Instance >>

        public MfsOperations (string userID, string passwordHash) {
            userID = User.ProcessUserIDStr (userID);

            ulong uid = MfsDBOperations.DoesUserExist (userID);

            if (uid == 0) {
                throw new MfsNonExistentUserException (
                    MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_USER, userID)
                );
            }

            // If control has reached here, it means that the user exists, so check for password:
            bool authenticated = MfsDBOperations.AuthenticateUser (userID, passwordHash);
            if (!authenticated) {
                throw new MfsAuthenticationException (
                    MfsErrorMessages.GetMessage (MessageType.AUTH_FAILURE, userID)
                );
            }

            LoadUserValues (userID, passwordHash);

            LoadSubobjects ();
        }

        private void LoadUserValues (string userID, string passwordHash) {
            Debug.Print ("Loading values for user: " + userID);

            _userID = userID;
            _userSpecificPath = MfsDBOperations.GetUserSpecificPath (userID);

            _userAbsPath = Config.GetStorageBasePath () + _userSpecificPath;
            Debug.Print ("Got absolute path for user: " + _userAbsPath);

            Debug.Print ("Creating DBOperations object.");
            _dbOperations = new MfsDBOperations (userID, _userSpecificPath);

            _indexer = new LuceneIndexer (_userSpecificPath + LUCENE_INDEX_PATH);
        }

        private void LoadSubobjects () {
            FileObj = _File.GetObject (this);
            DocumentObj = _Document.GetObject (this);
            AspectObj = _Aspect.GetObject (this);
            AspectGroupObj = _AspectGroup.GetObject (this);
            BriefcaseObj = _Briefcase.GetObject (this);
            CollectionObj = _Collection.GetObject (this);
            NoteObj = _Note.GetObject (this);
            UrlObj = _Url.GetObject (this);
            VCardObj = _VCard.GetObject (this);
            SfdObj = _Sfd.GetObject (this);
            RelationObj = _Relation.GetObject (this);
            ArchiveObj = _Archive.GetObject (this);
            BookmarkObj = _Bookmark.GetObject (this);
            IndexObj = _Index.GetObject (this);
            MasterPasswordObj = _MasterPassword.GetObject (this);
            CredentialsObj = _Credentials.GetObject (this);
        }

        private void DisposeSubobjects () {
            FileObj.Dispose ();
            DocumentObj.Dispose ();
            AspectObj.Dispose ();
            AspectGroupObj.Dispose ();
            BriefcaseObj.Dispose ();
            CollectionObj.Dispose ();
            NoteObj.Dispose ();
            UrlObj.Dispose ();
            VCardObj.Dispose ();
            SfdObj.Dispose ();
            RelationObj.Dispose ();
            ArchiveObj.Dispose ();
            BookmarkObj.Dispose ();
            IndexObj.Dispose ();
            MasterPasswordObj.Dispose ();
            CredentialsObj.Dispose ();
        }

        ~MfsOperations () {
            DisposeSubobjects ();
        }

        #region << IDisposable Members >>

        public void Dispose () {
            DisposeSubobjects ();
        }

        #endregion << IDisposable Members >>

        #region << Object Overrides >>

        public override string ToString () {
            return string.Format ("MfsOperations object user: {0}", _userID);
        }

        public override bool Equals (object obj) {
            if (obj is MfsOperations) {
                MfsOperations rhs = (MfsOperations) obj;
                return this._userID.Equals (rhs._userID);
            }
            return false;
        }

        public override int GetHashCode () {
            return base.GetHashCode ();
        }

        #endregion << Object Overrides >>

        public void GetUserName (out string fName, out string lName) {
            _dbOperations.GetUserName (out fName, out lName);
        }

        #endregion << Object Instance >>

        #region << Client-input Check Methods >>

        private static void ValidateString (string str, ValidationCheckType checkType) {
            switch (checkType) {
                case ValidationCheckType.PASSWORD_HASH:
                    if (str == null) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL, "Password hash")
                        );
                    }
                    if (str.Length != HASH_SIZE) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.BAD_LENGTH, "Password hash", HASH_SIZE)
                        );
                    }
                    break;

                case ValidationCheckType.FILE_NAME:
                    if (str == null || str.Equals (string.Empty)) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "File name")
                        );
                    }
                    if (str.Length > MAX_FILENAME_LENGTH) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "File name", MAX_FILENAME_LENGTH)
                        );
                    }
                    if (StringUtils.FileNameContainsIllegalChars (str)) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.BAD_ARG, "File name contains \\ / : * ? \" < > |")
                        );
                    }
                    break;

                case ValidationCheckType.FILE_NARRATION:
                    if (str == null) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL, "File narration")
                        );
                    }
                    if (str.Length > MAX_FILENARRATION_LENGTH) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "File narration", MAX_FILENARRATION_LENGTH)
                        );
                    }
                    break;

                case ValidationCheckType.ASPECT_NAME:
                    if (str == null || str.Equals (string.Empty)) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Aspect name")
                        );
                    }
                    if (str.Length > MAX_ASPECTNAME_LENGTH) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "Aspect name", MAX_ASPECTNAME_LENGTH)
                        );
                    }
                    break;

                case ValidationCheckType.ASPECT_DESC:
                    if (str == null) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL, "Aspect description")
                        );
                    }
                    if (str.Length > MAX_ASPECTDESC_LENGTH) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "Aspect description", MAX_ASPECTDESC_LENGTH)
                        );
                    }
                    break;

                case ValidationCheckType.BRIEFCASE_NAME:
                    if (str == null || str.Equals (string.Empty)) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Briefcase name")
                        );
                    }
                    if (str.Length > MAX_BRIEFCASENAME_LENGTH) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "Briefcase name", MAX_BRIEFCASENAME_LENGTH)
                        );
                    }
                    break;

                case ValidationCheckType.BRIEFCASE_DESC:
                    if (str == null) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL, "Briefcase description")
                        );
                    }
                    if (str.Length > MAX_BRIEFCASEDESC_LENGTH) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "Briefcase description", MAX_BRIEFCASEDESC_LENGTH)
                        );
                    }
                    break;

                case ValidationCheckType.COLLECTION_NAME:
                    if (str == null || str.Equals (string.Empty)) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Collection name")
                        );
                    }
                    if (str.Length > MAX_COLLECTIONNAME_LENGTH) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "Collection name", MAX_COLLECTIONNAME_LENGTH)
                        );
                    }
                    break;

                case ValidationCheckType.COLLECTION_DESC:
                    if (str == null) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL, "Collection description")
                        );
                    }
                    if (str.Length > MAX_COLLECTIONDESC_LENGTH) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "Collection description", MAX_COLLECTIONDESC_LENGTH)
                        );
                    }
                    break;

                case ValidationCheckType.VERSION_COMMENT:
                    if (str == null) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL, "Version comment")
                        );
                    }
                    if (str.Length > MAX_FILEVERSIONCOMMENT_LENGTH) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "Schema-free document name", MAX_FILEVERSIONCOMMENT_LENGTH)
                        );
                    }
                    break;

                case ValidationCheckType.SCHEMA_FREE_DOC_NAME:
                    if (str == null || str.Equals (string.Empty)) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Schema-free document name")
                        );
                    }
                    if (str.Length > MAX_SCHEMA_FREE_DOC_NAME_LENGTH) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "Schema-free document name", MAX_SCHEMA_FREE_DOC_NAME_LENGTH)
                        );
                    }
                    break;

                case ValidationCheckType.PREDICATE:
                    if (str == null || str.Equals (string.Empty)) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Predicate")
                        );
                    }
                    if (str.Length > MAX_PREDICATE_LENGTH) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "Predicate", MAX_PREDICATE_LENGTH)
                        );
                    }
                    break;

                case ValidationCheckType.APP_URL:
                    if (str == null || str.Equals (string.Empty)) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "App url")
                        );
                    }
                    break;

                case ValidationCheckType.USERNAME:
                    if (str == null || str.Equals (string.Empty)) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "User name")
                        );
                    }
                    break;
            }
        }

        private void ValidateList (List<ulong> list, bool allowEmpty, string listName) {
            if (list == null) {
                throw new MfsIllegalArgumentException (
                    MfsErrorMessages.GetMessage (MessageType.NULL, listName)
                );
            }

            if (!allowEmpty && list.Count == 0) {
                throw new MfsIllegalArgumentException (
                    MfsErrorMessages.GetMessage (MessageType.EMPTY, listName)
                );
            }
        }

        #endregion << Client-input Check Methods >>
    }
}
