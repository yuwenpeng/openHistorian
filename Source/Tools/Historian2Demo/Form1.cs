﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using openHistorian.Server;
using openHistorian.Server.Database;
using TVA.Media;

namespace Historian2Demo
{
    public partial class Form1 : Form
    {
        public HistorianEngine m_server;
        public ArchiveDatabaseEngine m_database;

        public Form1()
        {
            InitializeComponent();
        }

        //private void BtnAttachFile_Click(object sender, EventArgs e)
        //{
        //    OpenFileDialog dlg = new OpenFileDialog();
        //    dlg.Filter = "Wav File|*.wav";
        //    if (dlg.ShowDialog() == DialogResult.OK)
        //    {
        //        ProcessAudioFile(dlg.FileName);
        //    }
        //}

        private void BtnStartEngine_Click(object sender, EventArgs e)
        {
            //m_server = new HistorianEngine(null);
            //m_server.Create("Audio",null);
            //m_database = m_server.Get("Audio");
        }

        private void BtnCreatePoints_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < 100000; x++)
            {
                Thread.Sleep(1);
                DateTime currentTime = DateTime.Now;
                m_database.WriteData((ulong)currentTime.Ticks, 1, 2, 3);
            }
        }

        //private void ProcessAudioFile(string fileName)
        //{
        //    //var wave = WaveFile.Load(fileName, false);
        //    var reader = WaveDataReader.FromFile(fileName);

        //    while (true)
        //    {
        //        var sample = reader.GetNextSample();
        //        if (sample != null)
        //        {
        //            double value = sample[0].ConvertToType(TypeCode.Double);
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //}


    }
}
