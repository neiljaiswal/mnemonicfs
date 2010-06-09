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

namespace MnemonicFS.MFSCore {
    public enum FileLogEntryType {
        UNDEFINED,
        CREATED = 1,
        NEW_VERSION_CREATED,
        DELETED,
        ACCESSED_ORIGINAL,
        ACCESSED_VERSION,
        DELETION_DATETIME_SET,
        DELETION_DATETIME_UPDATED,
        DELETION_DATETIME_RESET,
        FILENAME_UPDATED,
        FILE_NARRATION_UPDATED,
        FILE_SAVEDATETIME_UPDATED,
    }

    public class FileLogEntry {
        private string _userIdStr;
        private ulong _fileID;
        private FileLogEntryType _fileLogEntryType;
        private DateTime _when;
        private string _extraInfo1;
        private string _extraInfo2;

        public FileLogEntry (string userIdStr, ulong fileID, FileLogEntryType fileLogEntryType, DateTime when, string extraInfo1, string extraInfo2) {
            _userIdStr = userIdStr;
            _fileID = fileID;
            _fileLogEntryType = fileLogEntryType;
            _when = when;
            _extraInfo1 = extraInfo1;
            _extraInfo2 = extraInfo2;
        }

        public string UserIDStr {
            get {
                return _userIdStr;
            }
        }

        public ulong FileID {
            get {
                return _fileID;
            }
        }

        public FileLogEntryType FileLogEntryType {
            get {
                return _fileLogEntryType;
            }
        }

        public DateTime DateTime {
            get {
                return _when;
            }
        }

        public string ExtraInfo1 {
            get {
                return _extraInfo1;
            }
        }

        public string ExtraInfo2 {
            get {
                return _extraInfo2;
            }
        }
    }
}
