using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProcessProts;

namespace NoEscape
{
    public partial class Form1 : Form
    {
        GameMode Mode;
        bool Started = false;
        Process TaskMgr;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Utils.IsElevated())
            {
                //enable impossible mode
                radioButton4.Enabled = true;
            }
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.github.com/adryzz/NoEscape/");
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Mode = GameMode.Easy;
            }
            else if (radioButton2.Checked)
            {
                Mode = GameMode.Normal;
            }
            else if(radioButton3.Checked)
            {
                Mode = GameMode.Advanced;
            }
            else
            {
                Mode = GameMode.Impossible;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(this, String.Format("Start in {0} mode?", Mode.ToString()), "NoEscape - Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (res == DialogResult.Yes)
            {
                Start();
            }
        }

        private void Start()
        {
            //disable the mode selection and the start button
            groupBox1.Enabled = false;
            button1.Enabled = false;
            Started = true;
            //setup for each mode
            if (Mode == GameMode.Easy)
            {
                SetupEasy();
            }
            else if (Mode == GameMode.Normal)
            {
                SetupNormal();
            }
            else if (Mode == GameMode.Advanced)
            {
                SetupAdvanced();
            }
            else
            {
                SetupImpossible();
            }
        }

        private void SetupEasy()
        {
            //nothing here. easy mode can be beaten using task manager
        }

        private void SetupNormal()
        {
            //everything from the previous level(s)
            SetupEasy();
            //open task manager in a hidden window
            SetupTaskMgr();
        }

        private void SetupTaskMgr()
        {
            TaskMgr = new Process();
            TaskMgr.StartInfo.FileName = @"C:\Windows\system32\taskmgr.exe";
            TaskMgr.StartInfo.RedirectStandardOutput = false;
            TaskMgr.StartInfo.CreateNoWindow = true;
            TaskMgr.StartInfo.UseShellExecute = false;
            TaskMgr.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            TaskMgr.Exited += (s, e) => { SetupTaskMgr(); MessageBox.Show("Well done!\nI'm still up and running!", "NoEscape", MessageBoxButtons.OK, MessageBoxIcon.Information); };
            TaskMgr.Start();
            TaskMgr.EnableRaisingEvents = true;
        }

        private void SetupAdvanced()
        {
            //everything from the previous level(s)
            SetupNormal();
        }

        private void SetupImpossible()
        {
            //everything from the previous level(s)
            SetupAdvanced();
            ProcessProtection.Protect();//this will cause a bsod each time you kill the program
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Started)
            {
                MessageBox.Show(this, "Not so easy...", "NoEscape");
                e.Cancel = true;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Started)
            {
                if (TaskMgr != null)
                {
                    if (!TaskMgr.HasExited)
                    {
                        TaskMgr.Kill();//this is to restore access to the task manager
                    }
                }
                MessageBox.Show("You are not so bad...", "NoEscape");
            }
            else
            {
                MessageBox.Show("Don't run away!!!!", "NoEscape");
            }
        }
    }
}
