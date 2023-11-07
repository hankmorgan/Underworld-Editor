using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using UnderworldEditor.Underworld;

namespace UnderworldEditor
{
    public partial class FrmSelect : Form
    {
        string appfolder;

        public FrmSelect()
        {
            InitializeComponent();
        }


        private void FrmSelect_Load(object sender, EventArgs e)
        {
            appfolder = Path.GetDirectoryName(Application.ExecutablePath);

            var settingsfile = System.IO.Path.Combine(appfolder, "uwsettings.json");

            if (!System.IO.File.Exists(settingsfile))
            {
                MessageBox.Show("missing file uwsettings.json at " + settingsfile);
                return;
            }
            uwsettings gamesettings = JsonConvert.DeserializeObject<uwsettings>(File.ReadAllText(settingsfile));
            uwsettings.instance = gamesettings;

            if (System.IO.Directory.Exists(uwsettings.instance.pathuw1))
            {               
                btnLoadUW1.Enabled = true;
                btnLoadUW1.Text = $"Load UW1 {uwsettings.instance.pathuw1}";               
            }
            else
            {
                btnLoadUW1.Enabled = false;
                btnLoadUW1.Text = $"Folder does not exist {uwsettings.instance.pathuw1}";
            }

            if (System.IO.Directory.Exists(uwsettings.instance.pathuw2))
            {
                btnLoadUW2.Enabled = true;
                btnLoadUW2.Text = $"Load UW2 {uwsettings.instance.pathuw2}";
            }
            else
            {
                btnLoadUW2.Enabled = false;
                btnLoadUW2.Text = $"Folder does not exist {uwsettings.instance.pathuw2}";
            }
        }

        //void WriteLastPath(string path)
        //{
        //    StreamWriter sw = new StreamWriter(apppath + "\\previous.txt",false);
        //    sw.Write(path);
        //    sw.Close();
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result= openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = Path.GetDirectoryName(openFileDialog1.FileName);
                switch (Path.GetFileName(openFileDialog1.FileName).ToUpper())
                {
                    case "UW.EXE"://underworld 1
                        main.curgame = main.GAME_UW1;
                        main.basepath = path;
                        //WriteLastPath(path);
                        this.Dispose();
                        break;
                    case "UW2.EXE"://underworld 2
                        main.curgame = main.GAME_UW2;
                        main.basepath = path;
                        //WriteLastPath(path);
                        this.Dispose();
                        break;
                }
            }
        }

        private void btnUW1_CLICK(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(uwsettings.instance.pathuw1))
            {
                main.curgame = main.GAME_UW1;
                main.basepath = uwsettings.instance.pathuw1;// btnLoadUW1.Text;
                this.Dispose();
            }

        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLoadUW2_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(uwsettings.instance.pathuw2))
            {
                main.curgame = main.GAME_UW2;
                main.basepath = uwsettings.instance.pathuw2;// btnLoadUW1.Text;
                this.Dispose();
            }
        }
    }
}
