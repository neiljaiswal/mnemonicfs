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

namespace MnemonicFS.MfsUtils.MfsDB {
    internal static class UserSchema {
        internal static string GetUserSchema () {
            return @"
                CREATE TABLE [L_Files] (
                [key_FileID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                [FileName] NVARCHAR(128) NOT NULL,
                [FileNarration] NVARCHAR(1024) NOT NULL,
                [FileSize] INTEGER NOT NULL,
                [FileHash] VARCHAR NOT NULL,
                [ArchiveName] NVARCHAR(64) NOT NULL,
                [FilePath] NVARCHAR(2048) NOT NULL,
                [fkey_BriefcaseID] INTEGER NOT NULL DEFAULT 1,
                [AssumedFileName] NVARCHAR(128) NOT NULL,
                [FilePassword] VARCHAR(20) NOT NULL,
                [WhenDateTime] TIMESTAMP NOT NULL,
                [DeletionDateTime] TIMESTAMP DEFAULT NULL
                );

                CREATE TABLE [M_Files_Versions] (
                [fkey_FileID] INTEGER NOT NULL,
                [VersionNumber] INTEGER NOT NULL,
                [FileHash] VARCHAR NOT NULL,
                [Comments] NVARCHAR(1024) NOT NULL,
                [ArchiveNameWithPath] NVARCHAR(2048) NOT NULL,
                [WhenDateTime] TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
                );

                CREATE TABLE [L_AspectGroups] (
                [key_AspectGroupID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                [fkey_ParentAspectGroupID] INTEGER DEFAULT 0,
                [AspectGroupName] NVARCHAR(128) NOT NULL,
                [AspectGroupDesc] NVARCHAR(1024) NOT NULL
                );

                INSERT INTO L_AspectGroups (key_AspectGroupID, AspectGroupName, AspectGroupDesc)
                VALUES (0, 'Root Aspect Group', 'Root aspect group');

                CREATE TABLE [L_Aspects] (
                [key_AspectID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                [fkey_AspectGroupID] INTEGER DEFAULT NULL,
                [AspectName] NVARCHAR(128) UNIQUE NOT NULL,
                [AspectDesc] NVARCHAR(1024) NOT NULL
                );

                CREATE TABLE [M_Aspects_Files] (
                [fkey_AspectID] INTEGER NOT NULL,
                [fkey_FileID] INTEGER NOT NULL
                );

                CREATE TABLE [L_Briefcases] (
                [key_BriefcaseID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                [BriefcaseName] NVARCHAR(128) UNIQUE NOT NULL,
                [BriefcaseDesc] NVARCHAR(1024) NOT NULL
                );

                INSERT INTO L_Briefcases (key_BriefcaseID, BriefcaseName, BriefcaseDesc)
                VALUES (1, 'Default', 'Default Briefcase');

                CREATE TABLE [L_Collections] (
                [key_CollectionID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                [CollectionName] NVARCHAR(128) UNIQUE NOT NULL,
                [CollectionDesc] NVARCHAR(1024) NOT NULL
                );

                CREATE TABLE [M_Files_Collections] (
                [fkey_FileID] INTEGER NOT NULL,
                [fkey_CollectionID] INTEGER NOT NULL
                );

                CREATE TABLE [L_Urls] (
                [key_UrlID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                [Url] NVARCHAR(4096) NOT NULL,
                [Description] NVARCHAR(1024),
                [WhenDateTime] TIMESTAMP NOT NULL
                );

                CREATE TABLE [L_Notes] (
                [key_NoteID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                [NoteContent] NVARCHAR(4096) NOT NULL,
                [WhenDateTime] TIMESTAMP NOT NULL
                );

                CREATE TABLE [M_Aspects_Urls] (
                [fkey_AspectID] INTEGER NOT NULL,
                [fkey_UrlID] INTEGER NOT NULL
                );

                CREATE TABLE [M_Aspects_Notes] (
                [fkey_AspectID] INTEGER NOT NULL,
                [fkey_NoteID] INTEGER NOT NULL
                );

                CREATE TABLE [L_VCards] (
                [key_VCardID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                [Name] NVARCHAR(1024) DEFAULT NULL,
                [FormattedName] NVARCHAR(1024) DEFAULT NULL,
                [Photograph] BLOB DEFAULT NULL,
                [Birthday] TIMESTAMP DEFAULT NULL,
                [DeliveryAddress] NVARCHAR(4096) DEFAULT NULL,
                [Telephone] VARCHAR(128) DEFAULT NULL,
                [Email] VARCHAR(256) DEFAULT NULL,
                [TimeZone] VARCHAR(128) DEFAULT NULL,
                [Latitude] REAL DEFAULT NULL,
                [Longitude] REAL DEFAULT NULL,
                [Title] NVARCHAR(512) DEFAULT NULL,
                [Role] NVARCHAR(512) DEFAULT NULL,
                [Logo] BLOB DEFAULT NULL,
                [OrganizationName] NVARCHAR(1024) DEFAULT NULL,
                [Note] NVARCHAR(2048) DEFAULT NULL,
                [Url] NVARCHAR(4096) DEFAULT NULL
                );

                CREATE TABLE [L_FileBookmarks] (
                [fkey_FileID] INTEGER NOT NULL
                );

                CREATE TABLE [L_NoteBookmarks] (
                [fkey_NoteID] INTEGER NOT NULL
                );

                CREATE TABLE [L_UrlBookmarks] (
                [fkey_UrlID] INTEGER NOT NULL
                );

                CREATE TABLE [L_VCardBookmarks] (
                [fkey_VCardID] INTEGER NOT NULL
                );
            ";
        }
    }
}
