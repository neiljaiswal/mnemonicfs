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
using System.Collections;

namespace MnemonicFS.MfsCore {
    public class MfsVCard {
        #region << Object Construction >>

        public MfsVCard () {
            Name = null;
            FormattedName = null;
            Photograph = null;
            Birthday = DateTime.MaxValue;
            DeliveryAddress = null;
            Telephone = null;
            Email = null;
            TimeZone = TimeZone.CurrentTimeZone;
            Latitude = 0;
            Longitude = 0;
            Title = null;
            Role = null;
            Logo = null;
            OrganizationName = null;
            Note = null;
            Url = null;
        }

        #endregion        

        #region << VCard Getter / Setter Properties >>

        public string Name {
            get;
            set;
        }

        public string FormattedName {
            get;
            set;
        }

        public byte[] Photograph {
            get;
            set;
        }

        public DateTime Birthday {
            get;
            set;
        }

        public string DeliveryAddress {
            get;
            set;
        }

        public string Telephone {
            get;
            set;
        }

        public string Email {
            get;
            set;
        }

        public TimeZone TimeZone {
            get;
            set;
        }

        public float Latitude {
            get;
            set;
        }

        public float Longitude {
            get;
            set;
        }

        public string Title {
            get;
            set;
        }

        public string Role {
            get;
            set;
        }

        public byte[] Logo {
            get;
            set;
        }

        public string OrganizationName {
            get;
            set;
        }

        public string Note {
            get;
            set;
        }

        public string Url {
            get;
            set;
        }

        #endregion
    }
}
