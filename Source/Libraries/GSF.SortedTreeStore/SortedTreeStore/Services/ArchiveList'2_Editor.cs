﻿//******************************************************************************************************
//  ArchiveList`2_Editor.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  7/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;
using GSF.SortedTreeStore.Storage;

namespace GSF.SortedTreeStore.Services
{
    public partial class ArchiveList<TKey, TValue>
    {
        /// <summary>
        /// Provides a way to edit an <see cref="ArchiveList{TKey,TValue}"/> since all edits must be atomic.
        /// WARNING: Instancing this class on an <see cref="ArchiveList{TKey,TValue}"/> will lock the class
        /// until <see cref="Dispose"/> is called. Therefore, keep locks to a minimum and always
        /// use a Using block.
        /// </summary>
        public class Editor
            : IDisposable
        {
            private bool m_disposed;
            private ArchiveList<TKey, TValue> m_list;

            /// <summary>
            /// Creates an editor for the ArchiveList
            /// </summary>
            /// <param name="list">the list to create the edit lock on.</param>
            public Editor(ArchiveList<TKey, TValue> list)
            {
                m_list = list;
                Monitor.Enter(m_list.m_syncRoot);
            }

            /// <summary>
            /// Renews the snapshot of the archive file. This will acquire the latest 
            /// read transaction so all new snapshots will use this later version.
            /// </summary>
            /// <param name="archiveId">the ID of the archive snapshot to renew</param>
            /// <returns></returns>
            public void RenewArchiveSnapshot(Guid archiveId)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                m_list.m_fileSummaries[archiveId] = new ArchiveTableSummary<TKey, TValue>(m_list.m_fileSummaries[archiveId].SortedTreeTable);
            }

            /// <summary>
            /// Adds an archive file to the list with the given state information.
            /// </summary>
            /// <param name="sortedTree">archive table to add</param>
            public void Add(SortedTreeTable<TKey, TValue> sortedTree)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                ArchiveTableSummary<TKey, TValue> summary = new ArchiveTableSummary<TKey, TValue>(sortedTree);
                m_list.m_fileSummaries.Add(sortedTree.ArchiveId, summary);
            }

            /// <summary>
            /// Returns true if the archive list contains the provided file.
            /// </summary>
            /// <param name="archiveId">the file</param>
            /// <returns></returns>
            public bool Contains(Guid archiveId)
            {
                return m_list.m_fileSummaries.ContainsKey(archiveId);
            }

            /// <summary>
            /// Removes the <see cref="archiveId"/> from <see cref="ArchiveList{TKey,TValue}"/> and queues it for disposal.
            /// </summary>
            /// <param name="archiveId">the archive to remove</param>
            /// <returns>True if the item was removed, False otherwise.</returns>
            /// <remarks>
            /// Also unlocks the archive file.
            /// </remarks>
            public bool TryRemoveAndDispose(Guid archiveId)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                var partitions = m_list.m_fileSummaries;
                if (!partitions.ContainsKey(archiveId))
                {
                    return false;
                }

                var tree = partitions[archiveId].SortedTreeTable;
                partitions.Remove(archiveId);

                m_list.AddFileToDispose(tree);
                return true;
            }

            /// <summary>
            /// Removes the supplied file from the <see cref="ArchiveList{TKey,TValue}"/> and queues it for deletion.
            /// </summary>
            /// <param name="archiveId">file to remove and delete.</param>
            /// <returns>true if deleted, false otherwise</returns>
            public bool TryRemoveAndDelete(Guid archiveId)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                var partitions = m_list.m_fileSummaries;
                if (!partitions.ContainsKey(archiveId))
                {
                    return false;
                }

                var tree = partitions[archiveId].SortedTreeTable;
                partitions.Remove(archiveId);

                m_list.AddFileToDelete(tree);
                return true;
            }

            /// <summary>
            /// Releases the lock on the <see cref="ArchiveList{TKey,TValue}"/>.
            /// </summary>
            public void Dispose()
            {
                if (!m_disposed)
                {
                    m_disposed = true;
                    m_list.m_listLog.SaveLogToDisk();
                    Monitor.Exit(m_list.m_syncRoot);
                    m_list = null;
                }
            }

        }
    }
}