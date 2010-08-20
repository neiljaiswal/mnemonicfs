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
using System.Diagnostics;
using System.IO;
using MnemonicFS.MfsUtils.MfsStrings;

namespace MnemonicFS.MfsUtils.MfsConfig {
    internal static class Config {
        // The config file will always be located in the DLL's current directory:
        private const string PROPERTIES_FILENAME = "mfs.config";

        private const string PROP_MAX_FILENAME_LENGTH = "max_filename_length";
        private const string PROP_MAX_FILE_NARRATION_LENGTH = "max_file_narration_length";
        private const string PROP_MAX_FILE_SIZE = "max_file_size";
        private const string PROP_MAX_ASPECT_NAME_LENGTH = "max_aspect_name_length";
        private const string PROP_MAX_ASPECT_DESC_LENGTH = "max_aspect_desc_length";
        private const string PROP_MAX_BRIEFCASE_NAME_LENGTH = "max_briefcase_name_length";
        private const string PROP_MAX_BRIEFCASE_DESC_LENGTH = "max_briefcase_desc_length";
        private const string PROP_MAX_COLLECTION_NAME_LENGTH = "max_collection_name_length";
        private const string PROP_MAX_COLLECTION_DESC_LENGTH = "max_collection_desc_length";
        private const string PROP_MAX_URL_LENGTH = "max_url_length";
        private const string PROP_STORAGE_BASE_PATH = "storage_base_path";
        private const string PROP_MAX_FILEVERSION_COMMENT_LENGTH = "max_fileversion_comment_length";
        private const string PROP_MAX_SCHEMA_FREE_DOC_NAME_LENGTH = "max_schema_free_doc_name_length";
        private const string PROP_USERNAME_REGEX = "username_regex";

        // Properties with default values:
        private static int _maxFileNameLength = 128;
        private static int _maxFileNarrationLength = 1024;
        private static int _maxFileSize = 2147483647; // 2 GB limit

        private static int _maxAspectNameLength = 128;
        private static int _maxAspectDescLength = 1024;

        private static int _maxBriefcaseNameLength = 128;
        private static int _maxBriefcaseDescLength = 1024;

        private static int _maxCollectionNameLength = 128;
        private static int _maxCollectionDescLength = 1024;

        private static int _maxUrlLength = 1024;

        private static int _maxFileVersionCommentLength = 1024;

        private static int _maxSchemaFreeDocNameLength = 1024;

        private static string _userNameRegex = @"\w+@(\w+\.)+\w+";

        private static string _basePath = @"C:\storage\";

        private static PropertiesFileReader _propertiesFileReader;

        static Config () {
            if (!DoesConfFileExist ()) {
                // Create the conf file with default schema and values:
                CreateDefaultConfFile ();
            }

            _propertiesFileReader = new PropertiesFileReader (PROPERTIES_FILENAME);
        }

        private static bool DoesConfFileExist () {
            return File.Exists (PROPERTIES_FILENAME);
        }

        private static void CreateDefaultConfFile () {
            FileStream confFileStream = File.Create (PROPERTIES_FILENAME);

            string defaultConfSchema = GetDefaultConfSchema ();
            byte[] schemaAsBytes = StringUtils.ConvertToByteArray (defaultConfSchema);

            confFileStream.Write (schemaAsBytes, 0, schemaAsBytes.Length);

            confFileStream.Close ();
        }

        private static string GetDefaultConfSchema () {
            return
                "# MnemonicFS Config Details.\n" +
                "# This file will be read exactly once on application startup.\n" +
                "# So, in case you make a change to this file and would want\n" +
                "# that change to be reflected immeditately in the application,\n" +
                "# you will have to re-start the service.\n\n" +

                "# Max values for files:\n" +
                PROP_MAX_FILENAME_LENGTH + "=" + _maxFileNameLength + "\n" +
                PROP_MAX_FILE_NARRATION_LENGTH + "=" + _maxFileNarrationLength + "\n" +
                PROP_MAX_FILE_SIZE + "=" + _maxFileSize + "\n\n" +

                "# Max values for aspects:\n" +
                PROP_MAX_ASPECT_NAME_LENGTH + "=" + _maxAspectNameLength + "\n" +
                PROP_MAX_ASPECT_DESC_LENGTH + "=" + _maxAspectDescLength + "\n\n" +

                "# Max values for briefcases:\n" +
                PROP_MAX_BRIEFCASE_NAME_LENGTH + "=" + _maxBriefcaseNameLength + "\n" +
                PROP_MAX_BRIEFCASE_DESC_LENGTH + "=" + _maxBriefcaseDescLength + "\n\n" +

                "# Max values for collections:\n" +
                PROP_MAX_COLLECTION_NAME_LENGTH + "=" + _maxCollectionNameLength + "\n" +
                PROP_MAX_COLLECTION_DESC_LENGTH + "=" + _maxCollectionDescLength + "\n\n" +

                "# Max length of comment for file versions:\n" +
                PROP_MAX_FILEVERSION_COMMENT_LENGTH + "=" + _maxFileVersionCommentLength + "\n\n" +

                "# Max length of Url:\n" +
                PROP_MAX_URL_LENGTH + "=" + _maxUrlLength + "\n\n" +

                "# Max schema-free document name length:\n" +
                PROP_MAX_SCHEMA_FREE_DOC_NAME_LENGTH + "=" + _maxSchemaFreeDocNameLength + "\n\n" +

                "# User Name Regex:\n" +
                PROP_USERNAME_REGEX + "=" + _userNameRegex + "\n\n" +

                "# Base storage directory:\n" +
                "# ACHTUNG! Please use Windows separator (backslash: \\) only!\n" +
                "# Do NOT use UNIX file separator (frontslash: /).\n" +
                "# Make sure you end the directory with a separator.\n" +
                PROP_STORAGE_BASE_PATH + "=" + _basePath + "\n";
        }

        internal static int GetMaxFileNameLength () {
            try {
                string value = _propertiesFileReader.GetValueForKey (PROP_MAX_FILENAME_LENGTH);
                _maxFileNameLength = Int32.Parse (value);
                Debug.Print ("Got value for max file length: " + _maxFileNameLength);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_MAX_FILENAME_LENGTH + ". Using default value instead.");
            }

            return _maxFileNameLength;
        }

        internal static int GetMaxFileNarrationLength () {
            try {
                string value = _propertiesFileReader.GetValueForKey (PROP_MAX_FILE_NARRATION_LENGTH);
                _maxFileNarrationLength = Int32.Parse (value);
                Debug.Print ("Got value for max file narration length: " + _maxFileNarrationLength);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_MAX_FILE_NARRATION_LENGTH + ". Using default value instead.");
            }

            return _maxFileNarrationLength;
        }

        internal static int GetMaxFileSize () {
            try {
                string value = _propertiesFileReader.GetValueForKey (PROP_MAX_FILE_SIZE);
                _maxFileSize = Int32.Parse (value);
                Debug.Print ("Got value for max file size: " + _maxFileSize);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_MAX_FILE_SIZE + ". Using default value instead.");
            }

            return _maxFileSize;
        }

        internal static int GetMaxAspectNameLength () {
            try {
                string value = _propertiesFileReader.GetValueForKey (PROP_MAX_ASPECT_NAME_LENGTH);
                _maxAspectNameLength = Int32.Parse (value);
                Debug.Print ("Got value for max aspect name length: " + _maxAspectNameLength);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_MAX_ASPECT_NAME_LENGTH + ". Using default value instead.");
            }

            return _maxAspectNameLength;
        }

        internal static int GetMaxAspectDescLength () {
            try {
                string value = _propertiesFileReader.GetValueForKey (PROP_MAX_ASPECT_DESC_LENGTH);
                _maxAspectDescLength = Int32.Parse (value);
                Debug.Print ("Got value for max aspect desc length: " + _maxAspectDescLength);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_MAX_ASPECT_DESC_LENGTH + ". Using default value instead.");
            }

            return _maxAspectDescLength;
        }

        internal static int GetMaxBriefcaseNameLength () {
            try {
                string value = _propertiesFileReader.GetValueForKey (PROP_MAX_BRIEFCASE_NAME_LENGTH);
                _maxBriefcaseNameLength = Int32.Parse (value);
                Debug.Print ("Got value for max briefcase name length: " + _maxBriefcaseNameLength);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_MAX_BRIEFCASE_NAME_LENGTH + ". Using default value instead.");
            }

            return _maxBriefcaseNameLength;
        }

        internal static int GetMaxBriefcaseDescLength () {
            try {
                string value = _propertiesFileReader.GetValueForKey (PROP_MAX_BRIEFCASE_DESC_LENGTH);
                _maxBriefcaseDescLength = Int32.Parse (value);
                Debug.Print ("Got value for max briefcase desc length: " + _maxBriefcaseDescLength);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_MAX_BRIEFCASE_DESC_LENGTH + ". Using default value instead.");
            }

            return _maxBriefcaseDescLength;
        }

        internal static int GetMaxCollectionNameLength () {
            try {
                string value = _propertiesFileReader.GetValueForKey (PROP_MAX_COLLECTION_NAME_LENGTH);
                _maxCollectionNameLength = Int32.Parse (value);
                Debug.Print ("Got value for max collection name length: " + _maxCollectionNameLength);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_MAX_COLLECTION_NAME_LENGTH + ". Using default value instead.");
            }

            return _maxCollectionNameLength;
        }

        internal static int GetMaxCollectionDescLength () {
            try {
                string value = _propertiesFileReader.GetValueForKey (PROP_MAX_COLLECTION_DESC_LENGTH);
                _maxCollectionDescLength = Int32.Parse (value);
                Debug.Print ("Got value for max collection desc length: " + _maxCollectionDescLength);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_MAX_COLLECTION_DESC_LENGTH + ". Using default value instead.");
            }

            return _maxCollectionDescLength;
        }

        internal static int GetMaxUrlLength () {
            try {
                string value = _propertiesFileReader.GetValueForKey (PROP_MAX_URL_LENGTH);
                _maxCollectionDescLength = Int32.Parse (value);
                Debug.Print ("Got value for max url length: " + _maxUrlLength);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_MAX_URL_LENGTH + ". Using default value instead.");
            }

            return _maxUrlLength;
        }

        internal static int GetMaxFileVersionCommentLength () {
            try {
                string value = _propertiesFileReader.GetValueForKey (PROP_MAX_FILEVERSION_COMMENT_LENGTH);
                _maxFileVersionCommentLength = Int32.Parse (value);
                Debug.Print ("Got value for max file version comment length: " + _maxFileVersionCommentLength);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_MAX_FILEVERSION_COMMENT_LENGTH + ". Using default value instead.");
            }

            return _maxFileVersionCommentLength;
        }

        internal static int GetMaxSchemaFreeDocNameLength () {
            try {
                string value = _propertiesFileReader.GetValueForKey (PROP_MAX_SCHEMA_FREE_DOC_NAME_LENGTH);
                _maxFileVersionCommentLength = Int32.Parse (value);
                Debug.Print ("Got value for max schema-free document name length: " + _maxSchemaFreeDocNameLength);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_MAX_SCHEMA_FREE_DOC_NAME_LENGTH + ". Using default value instead.");
            }

            return _maxSchemaFreeDocNameLength;
        }

        internal static string GetRegexString () {
            try {
                _userNameRegex = _propertiesFileReader.GetValueForKey (PROP_USERNAME_REGEX);
                Debug.Print ("Got value for user name regex: " + _userNameRegex);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_USERNAME_REGEX + ". Using default value instead.");
            }

            return _userNameRegex;
        }

        internal static string GetStorageBasePath () {
            try {
                _basePath = _propertiesFileReader.GetValueForKey (PROP_STORAGE_BASE_PATH);
                if (!_basePath.EndsWith (@"\")) {
                    _basePath = _basePath + @"\";
                }
                Debug.Print ("Got value for storage base path: " + _basePath);
            } catch (Exception) {
                Trace.TraceError ("Error reading property: " + PROP_STORAGE_BASE_PATH + ". Using default value instead.");
            }

            return _basePath;
        }
    }
}
