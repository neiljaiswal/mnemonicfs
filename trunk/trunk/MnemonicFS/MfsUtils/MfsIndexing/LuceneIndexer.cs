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
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

namespace MnemonicFS.MfsUtils.MfsIndexing {

    internal enum IndexContentType {
        FILE_CONTENT,
        FILE_NARRRATION
    }

    internal class LuceneIndexer {
        private const string DOC_ID_FIELD_NAME = "ID_FIELD";

        private string _indexDir;

        public LuceneIndexer (string indexDir) {
            _indexDir = indexDir;
        }

        /// <summary>
        /// This method indexes the content that is sent across to it. Each piece of content (or "document")
        /// that is indexed has to have a unique identifier (so that the caller can take action based on the
        /// document id). Therefore, this method accepts key-value pairs in the form of a dictionary. The key
        /// is a ulong which uniquely identifies the string to be indexed. The string itself is the value
        /// within the dictionary for that key. Be aware that stop words (like the, this, at, etc.) are _not_
        /// indexed.
        /// </summary>
        /// <param name="txtIdPairToBeIndexed">A dictionary of key-value pairs that are sent by the caller
        /// to uniquely identify each string that is to be indexed.</param>
        /// <returns>The number of documents indexed.</returns>
        public int Index (Dictionary<ulong, string> txtIdPairToBeIndexed, IndexContentType indexContentType) {
            IndexWriter indexWriter = new IndexWriter (_indexDir, new StandardAnalyzer (), true);
            indexWriter.SetUseCompoundFile (false);

            Dictionary<ulong, string>.KeyCollection keys = txtIdPairToBeIndexed.Keys;

            foreach (ulong id in keys) {
                string text = txtIdPairToBeIndexed[id];
                Document document = new Document ();
                Field bodyField = new Field (indexContentType.ToString (), text, Field.Store.YES, Field.Index.TOKENIZED);
                document.Add (bodyField);
                Field idField = new Field (DOC_ID_FIELD_NAME, (id).ToString (), Field.Store.YES, Field.Index.TOKENIZED);
                document.Add (idField);
                indexWriter.AddDocument (document);
            }

            int numIndexed = indexWriter.DocCount ();
            indexWriter.Optimize ();
            indexWriter.Close ();

            return numIndexed;
        }

        /// <summary>
        /// This method searches for the search term passed by the caller.
        /// </summary>
        /// <param name="searchTerm">The search term as a string that the caller wants to search for within the
        /// index as referenced by this object.</param>
        /// <param name="ids">An out parameter that is populated by this method for the caller with docments ids.</param>
        /// <param name="results">An out parameter that is populated by this method for the caller with docments text.</param>
        /// <param name="scores">An out parameter that is populated by this method for the caller with docments scores.</param>
        public void Search (string searchTerm, IndexContentType indexContentType, out ulong[] ids, out string[] results, out float[] scores) {
            IndexSearcher indexSearcher = new IndexSearcher (_indexDir);
            try {
                QueryParser queryParser = new QueryParser (indexContentType.ToString (), new StandardAnalyzer ());
                Query query = queryParser.Parse (searchTerm);
                Hits hits = indexSearcher.Search (query);
                int numHits = hits.Length ();

                ids = new ulong[numHits];
                results = new string[numHits];
                scores = new float[numHits];

                for (int i = 0; i < numHits; ++i) {
                    float score = hits.Score (i);
                    string text = hits.Doc (i).Get (indexContentType.ToString ());
                    string idAsText = hits.Doc (i).Get (LuceneIndexer.DOC_ID_FIELD_NAME);
                    ids[i] = UInt64.Parse (idAsText);
                    results[i] = text;
                    scores[i] = score;
                }
            } finally {
                indexSearcher.Close ();
            }
        }
    }
}
