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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MnemonicFS.MfsUtils.MfsDB {
    internal static class SystemSchema {
        internal static string GetSystemSchema () {
            return @"
                CREATE TABLE [L_Users] (
                [key_UserID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                [UserIDStr] NVARCHAR(128) UNIQUE NOT NULL,
                [UserFName] NVARCHAR(128) DEFAULT NULL,
                [UserLName] NVARCHAR(128) DEFAULT NULL,
                [PasswordHash] VARCHAR(40) NOT NULL,
                [UserSpecificPath] NVARCHAR(1024) NOT NULL,
                [WhenDateTime] TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
                [UserDeleted] SHORT DEFAULT 0
                );

                CREATE TABLE [L_ByteStreams] (
                [key_ByteStreamID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                [ReferenceNumber] INTEGER DEFAULT NULL,
                [AssumedFileName] NVARCHAR(128) NOT NULL,
                [ArchiveName] NVARCHAR(64) NOT NULL,
                [ArchivePath] NVARCHAR(128) NOT NULL
                );
            ";
        }
    }
}
