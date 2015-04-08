using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.PartitionResolvers
{
    public class HashPartitionResolver : IPartitionResolver 
    {
        private string databaseLink;
        private DocumentClient client;
        private List<string> partitions;

        public HashPartitionResolver(string databaseLink)
        {
            this.databaseLink = databaseLink;
            partitions = new List<string>();
        }

        public object GetPartitionKey(string databaseLink, object document)
        {
            throw new NotImplementedException();
        }

        public string ResolveForCreate(string databaseLink, object partitionKey)
        {
            if (partitionKey == null || partitionKey.GetType() != typeof(String))
            {
                throw new ArgumentException("Document partitionKey property cannont be null and must be of type System.String");
            }

            int partitionIndex = GetPartitionIndex(partitionKey);
            var resolvedPartition = this.partitions[partitionIndex];
            
            return resolvedPartition;
        }

        public IEnumerable<string> ResolveForRead(string databaseLink, object partitionKey)
        {
            if (partitionKey != null && partitionKey.GetType() != typeof(String))
            {
                throw new ArgumentException("Document partitionKey property must be null or of type System.String");
            }

            List<string> resolvedPartitionLinks = new List<string>();

            if (partitionKey == null)
            {//if no partitionKey supplied, return all collections we know about
                //should never really be doing this, should always have a partitionKey when trying to read
                //but in case you want to do a full scan using a field other than the partitionKey

                resolvedPartitionLinks.AddRange(this.partitions.ToList());
            }
            else
            {//we have the partitionKey, so resolve that to an index
                //should only ever resolve to 1 collection link
                int partitionIndex = GetPartitionIndex(partitionKey);
                resolvedPartitionLinks.Add(this.partitions[partitionIndex]);
            }

            return resolvedPartitionLinks;
        }

        public void Initialize(DocumentClient client)
        {
            this.client = client;
            partitions.Add("dbs/W-x4AA==/colls/W-x4APT77gI=/");
            partitions.Add("dbs/W-x4AA==/colls/W-x4AL9bCgM=/");
        }

        private int GetPartitionIndex(object partitionKey)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            MurmurHash2Simple hasher = new MurmurHash2Simple();

            int bytes = (int)hasher.Hash(encoding.GetBytes((string)partitionKey));
            int partitionIndex = Math.Abs(bytes % this.partitions.Count);
            return partitionIndex;
        }

        private class MurmurHash2Simple
        {
            //***** BEGIN LICENSE BLOCK *****
            //* Version: MPL 1.1/GPL 2.0/LGPL 2.1
            //*
            //* The contents of this file are subject to the Mozilla Public License Version
            //* 1.1 (the "License"); you may not use this file except in compliance with
            //* the License. You may obtain a copy of the License at
            //* http://www.mozilla.org/MPL/
            //*
            //* Software distributed under the License is distributed on an "AS IS" basis,
            //* WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
            //* for the specific language governing rights and limitations under the
            //* License.
            //*
            //* The Original Code is HashTableHashing.MurmurHash2.
            //*
            //* The Initial Developer of the Original Code is
            //* Davy Landman.
            //* Portions created by the Initial Developer are Copyright (C) 2009
            //* the Initial Developer. All Rights Reserved.
            //*
            //* Contributor(s):
            //*
            //*
            //* Alternatively, the contents of this file may be used under the terms of
            //* either the GNU General Public License Version 2 or later (the "GPL"), or
            //* the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
            //* in which case the provisions of the GPL or the LGPL are applicable instead
            //* of those above. If you wish to allow use of your version of this file only
            //* under the terms of either the GPL or the LGPL, and not to allow others to
            //* use your version of this file under the terms of the MPL, indicate your
            //* decision by deleting the provisions above and replace them with the notice
            //* and other provisions required by the GPL or the LGPL. If you do not delete
            //* the provisions above, a recipient may use your version of this file under
            //* the terms of any one of the MPL, the GPL or the LGPL.
            //*
            //* ***** END LICENSE BLOCK ***** */

            public UInt32 Hash(Byte[] data)
            {
                return Hash(data, 0xc58f1a7b);
            }
            const UInt32 m = 0x5bd1e995;
            const Int32 r = 24;

            public UInt32 Hash(Byte[] data, UInt32 seed)
            {
                Int32 length = data.Length;
                if (length == 0)
                    return 0;
                UInt32 h = seed ^ (UInt32)length;
                Int32 currentIndex = 0;
                while (length >= 4)
                {
                    UInt32 k = BitConverter.ToUInt32(data, currentIndex);
                    k *= m;
                    k ^= k >> r;
                    k *= m;

                    h *= m;
                    h ^= k;
                    currentIndex += 4;
                    length -= 4;
                }
                switch (length)
                {
                    case 3:
                        h ^= BitConverter.ToUInt16(data, currentIndex);
                        h ^= (UInt32)data[currentIndex + 2] << 16;
                        h *= m;
                        break;
                    case 2:
                        h ^= BitConverter.ToUInt16(data, currentIndex);
                        h *= m;
                        break;
                    case 1:
                        h ^= data[currentIndex];
                        h *= m;
                        break;
                    default:
                        break;
                }

                // Do a few final mixes of the hash to ensure the last few
                // bytes are well-incorporated.

                h ^= h >> 13;
                h *= m;
                h ^= h >> 15;

                return h;
            }
        }
    }
}
