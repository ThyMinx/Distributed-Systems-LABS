﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

using TranslationServer;

namespace TranslationClient
{
    public partial class Form1 : Form
    {
        Translator translationObject = null;

        public Form1()
        {
            InitializeComponent();
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);

            translationObject = (Translator)Activator.GetObject(typeof(Translator), "tcp://localhost:5000/Translate");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = translationObject.Translate(textBox1.Text);
        }
    }
}
