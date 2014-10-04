//******************************************************************************************************
//  CombineFilesSettings.cs - Gbtc
//
//  Copyright � 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  10/03/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//******************************************************************************************************

using System;
using GSF.IO;

namespace GSF.SortedTreeStore.Services.Writer
{
    /// <summary>
    /// A collection of settings for <see cref="CombineFiles{TKey,TValue}"/>.
    /// </summary>
    public class CombineFilesSettings
    {
        private int m_executeTimer = 60000;
        private string m_logPath = string.Empty;
        private int m_combineOnFileCount = 100;
        private long m_combineOnFileSize = 1024 * 1024 * 1024;
        private Guid m_matchFlag = Guid.Empty;
        private string m_finalFileExtension = ".d2i";
        private ArchiveInitializerSettings m_archiveSettings = new ArchiveInitializerSettings();
       
        /// <summary>
        /// Gets the rate a which a rollover check is executed
        /// Time is in milliseconds.
        /// </summary>
        /// <remarks>
        /// Default is every 60,000 milliseconds. 
        /// Must be between 1 second and 10 minutes. 
        /// Anything outside this range will substitute for the closest valid value.
        /// </remarks>
        public int ExecuteTimer
        {
            get
            {
                return m_executeTimer;
            }
            set
            {
                if (value < 1000)
                {
                    m_executeTimer = 1000;
                }
                else if (value > 600000)
                {
                    m_executeTimer = 600000;
                }
                else
                {
                    m_executeTimer = value;
                }
            }
        }

        /// <summary>
        /// The path to write the log file for the rollover process.
        /// </summary>
        /// <remarks>
        /// A value of String.Empty means that rollover logs will not be created.
        /// </remarks>
        public string LogPath
        {
            get
            {
                return m_logPath;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    m_logPath = string.Empty;
                }
                else
                {
                    m_logPath = value;
                }
            }
        }

        /// <summary>
        /// The number of files with the specified <see cref="MatchFlag"/>
        /// before they will be combined. 
        /// </summary>
        /// <remarks>
        /// Must be between 2 and 1000.
        /// </remarks>
        public int CombineOnFileCount
        {
            get
            {
                return m_combineOnFileCount;
            }
            set
            {
                if (value < 2)
                {
                    m_combineOnFileCount = 2;
                }
                else if (value > 1000)
                {
                    m_combineOnFileCount = 1000;
                }
                else
                {
                    m_combineOnFileCount = value;
                }
            }
        }

        /// <summary>
        /// The size at which to create a rolled over file
        /// </summary>
        /// <remarks>
        /// Must be between 1MB and 100GB
        /// </remarks>
        public long CombineOnFileSize
        {
            get
            {
                return m_combineOnFileSize;
            }
            set
            {
                if (value < 1 * 1024L * 1024L)
                {
                    m_combineOnFileSize = 1 * 1024L * 1024L;
                }
                else if (value > 100 * 1024L * 1024L * 1024L)
                {
                    m_combineOnFileSize = 100 * 1024L * 1024L * 1024L;
                }
                else
                {
                    m_combineOnFileSize = value;
                }
            }
        }

        /// <summary>
        /// The archive flag to do the file combination on.
        /// </summary>
        public Guid MatchFlag
        {
            get
            {
                return m_matchFlag;
            }
            set
            {
                m_matchFlag = value;
            }
        }

        /// <summary>
        /// The extension to change the file to after writing all of the data.
        /// </summary>
        public string FinalFileExtension
        {
            get
            {
                return m_finalFileExtension;
            }
            set
            {
                m_finalFileExtension = PathHelpers.FormatExtension(value);
            }
        }

        /// <summary>
        /// The settings for the archive initializer. This value cannot be null.
        /// </summary>
        public ArchiveInitializerSettings ArchiveSettings
        {
            get
            {
                return m_archiveSettings;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                m_archiveSettings = value;
            }
        }

        /// <summary>
        /// Clones the current settings, so they cannot be modified by the sending class.
        /// </summary>
        /// <returns></returns>
        public CombineFilesSettings Clone()
        {
            var clone = (CombineFilesSettings)MemberwiseClone();
            clone.ArchiveSettings = ArchiveSettings.Clone();
            return clone;
        }

    }
}