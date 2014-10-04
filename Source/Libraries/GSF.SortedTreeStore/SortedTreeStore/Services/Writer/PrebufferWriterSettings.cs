﻿//******************************************************************************************************
//  PrebufferWriterSettings.cs - Gbtc
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
//  09/18/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;

namespace GSF.SortedTreeStore.Services.Writer
{
    /// <summary>
    /// All of the settings for the prebuffer writer
    /// </summary>
    public class PrebufferWriterSettings
    {
        private int m_rolloverInterval = 100;
        private int m_maximumPointCount = 10000;
        private int m_rolloverPointCount = 5000;

        /// <summary>
        /// The maximum interval to wait in milliseconds before taking the prebuffer and rolling it into a Stage 0 Archive.
        /// </summary>
        /// <remarks>
        /// Must be between 1 and 1,000
        /// </remarks>
        public int RolloverInterval
        {
            get
            {
                return m_rolloverInterval;
            }
            set
            {
                if (value < 1)
                {
                    m_rolloverInterval = 1;
                }
                else if (value > 1000)
                {
                    m_rolloverInterval = 1000;
                }
                else
                {
                    m_rolloverInterval = value;
                }
            }
        }

        /// <summary>
        /// The maximum number of points to have in the prebuffer before rolling this into the Stage 0 Archive.
        /// </summary>
        /// <remarks>
        /// Must be between 1,000 and 100,000
        /// </remarks>
        public int MaximumPointCount
        {
            get
            {
                return m_maximumPointCount;
            }
            set
            {
                if (value < 1000)
                {
                    m_maximumPointCount = 1000;
                }
                else if (value > 100000)
                {
                    m_maximumPointCount = 100000;
                }
                else
                {
                    m_maximumPointCount = value;
                }
            }
        }

        /// <summary>
        /// The number of points before a rollover is queued. This should be before the maximum point 
        /// count since once the maximum point count has been reached, a thread pause will result.
        /// </summary>
        /// <remarks>
        /// Must be between 1,000 and 100,000
        /// </remarks>
        public int RolloverPointCount
        {
            get
            {
                return Math.Min(m_rolloverPointCount, m_maximumPointCount);
            }
            set
            {
                if (value < 1000)
                {
                    m_rolloverPointCount = 1000;
                }
                else if (value > 100000)
                {
                    m_rolloverPointCount = 100000;
                }
                else
                {
                    m_rolloverPointCount = value;
                }
            }
        }

        /// <summary>
        /// Clones the <see cref="PrebufferWriterSettings"/>
        /// </summary>
        /// <returns></returns>
        public PrebufferWriterSettings Clone()
        {
            var other = (PrebufferWriterSettings)MemberwiseClone();
            return other;
        }

    }
}