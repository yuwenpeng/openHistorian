﻿//******************************************************************************************************
//  ProcessClient.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Diagnostics;
using GSF.Communications;
using GSF.IO;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace openHistorian.Communications
{
    internal class ProcessClient<TKey, TValue>
        : IDisposable
        where TKey : HistorianKeyBase<TKey>, new()
        where TValue : HistorianValueBase<TValue>, new()
    {
        public event SocketErrorEventHandler SocketError;

        public delegate void SocketErrorEventHandler(Exception ex);

        private NetworkBinaryStream m_netStream;
        private readonly BinaryStreamWrapper m_stream;
        private readonly IHistorianDatabaseCollection<TKey, TValue> m_historian;
        private IHistorianDatabase<TKey, TValue> m_historianDatabase;
        private HistorianDataReaderBase<TKey, TValue> m_historianReaderBase;

        public ProcessClient(NetworkBinaryStream netStream, IHistorianDatabaseCollection<TKey, TValue> historian)
        {
            m_netStream = netStream;
            m_stream = new BinaryStreamWrapper(m_netStream);
            m_historian = historian;
        }

        /// <summary>
        /// This function will verify the connection, create all necessary streams, set timeouts, and catch any exceptions and terminate the connection
        /// </summary>
        /// <remarks></remarks>
        public void Run()
        {
            //m_netStream.Timeout = 5000;

            try
            {
                long code = m_stream.ReadInt64();
                if (code != 1122334455667788990L)
                {
                    //m_stream.Write((byte)ServerResponse.Error);
                    //m_stream.Write("Wrong Username Or Password");
                    //m_netStream.Flush();
                    return;
                }
                ProcessRequestsLevel1();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                if (SocketError != null)
                {
                    SocketError(ex);
                }
            }
            finally
            {
                try
                {
                    m_netStream.Disconnect();
                    m_netStream.Close();
                }
                catch (Exception ex)
                {
                }
                m_netStream = null;
            }
        }

        /// <summary>
        /// This function will process any of the packets that come in.  It will throw an error if anything happens.  
        /// This will cause the calling function to close the connection.
        /// </summary>
        /// <remarks></remarks>
        private void ProcessRequestsLevel1()
        {
            while (true)
            {
                ServerCommand command = (ServerCommand)m_stream.ReadByte();
                switch (command)
                {
                    case ServerCommand.ConnectToDatabase:
                        if (m_historianDatabase != null)
                        {
                            //m_stream.Write((byte)ServerResponse.Error);
                            //m_stream.Write("Already connected to a database.");
                            //m_netStream.Flush();
                            return;
                        }
                        string databaseName = m_stream.ReadString();
                        m_historianDatabase = m_historian[databaseName];
                        ProcessRequestsLevel2();
                        break;
                    case ServerCommand.Disconnect:
                        return;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <remarks></remarks>
        private void ProcessRequestsLevel2()
        {
            while (true)
            {
                switch ((ServerCommand)m_stream.ReadByte())
                {
                    case ServerCommand.OpenReader:
                        m_historianReaderBase = m_historianDatabase.OpenDataReader();
                        ProcessRequestsLevel3();
                        break;
                    case ServerCommand.DisconnectDatabase:
                        m_historianDatabase.Disconnect();
                        m_historianDatabase = null;
                        return;
                        break;
                    case ServerCommand.Write:
                        ProcessWrite();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ProcessRequestsLevel3()
        {
            while (true)
            {
                switch ((ServerCommand)m_stream.ReadByte())
                {
                    case ServerCommand.DisconnectReader:
                        m_historianReaderBase.Close();
                        m_historianReaderBase = null;
                        return;
                        break;
                    case ServerCommand.Read:
                        ProcessRead();
                        break;
                    case ServerCommand.CancelRead:
                        //m_stream.Write((byte)ServerResponse.Success);
                        //m_stream.Write("Read has been canceled");
                        //m_netStream.Flush();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ProcessRead()
        {
            QueryFilterTimestamp key1Parser = QueryFilterTimestamp.CreateFromStream(m_stream);
            QueryFilterPointId key2Parser = QueryFilterPointId.CreateFromStream(m_stream);
            DataReaderOptions readerOptions = new DataReaderOptions(m_stream);

            TreeStream<TKey, TValue> scanner = m_historianReaderBase.Read(key1Parser, key2Parser, readerOptions);

            TKey oldKey = new TKey();
            TValue oldValue = new TValue();
            while (scanner.Read())
            {
                m_stream.Write(true);
                scanner.CurrentKey.WriteCompressed(m_stream, oldKey);
                scanner.CurrentValue.WriteCompressed(m_stream, oldValue);
                if (m_netStream.AvailableReadBytes > 0)
                {
                    m_stream.Write(false);
                    m_netStream.Flush();
                    return;
                }
                scanner.CurrentKey.CopyTo(oldKey);
                scanner.CurrentValue.CopyTo(oldValue);
            }
            m_stream.Write(false);
            m_netStream.Flush();
        }

        private void ProcessWrite()
        {
            TKey key = new TKey();
            TValue value = new TValue();
            while (m_stream.ReadBoolean())
            {
                key.ReadCompressed(m_stream, key);
                value.ReadCompressed(m_stream, value);
                m_historianDatabase.Write(key, value);
            }
        }

        public void Dispose()
        {
            if (m_netStream.Connected)
                m_netStream.Disconnect();
        }
    }
}