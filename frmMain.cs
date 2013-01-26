using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DiskCleanUp
{
    public partial class frmMain : Form
    {
        private Regex filename=new Regex("(?<name>[^\\\\/]+?)$",RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private Regex foldername = new Regex("(?<name>[^\\\\/]+?)[\\\\/]?$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private Dictionary<string,List<string>> findings;

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (fbdFolder.ShowDialog() == DialogResult.OK)
            {
                txtFolder.Text = fbdFolder.SelectedPath;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            findings = new Dictionary<string, List<string>>();
            lstFolders.Items.Clear();
            lstFiles.Items.Clear();
            ScanFolder(txtFolder.Text);
            foreach (string k in findings.Keys)
            {
                lstFolders.Items.Add(k);
            }
        }

        private void ScanFolder(string directory)
        {
            try
            {
                List<string> folders = new List<string>();
                List<string> files = new List<string>();
                folders.AddRange(System.IO.Directory.GetDirectories(directory));
                files.AddRange(System.IO.Directory.GetFiles(directory));
                string f1, f2;
                Match m1, m2;
                Regex derived;

                foreach (string folder in folders)
                {
                    if (foldername.IsMatch(folder))
                    {
                        m1 = foldername.Match(folder);
                        f1 = m1.Groups["name"].Value;
                        //derived = new Regex("^"+f1 + "+", RegexOptions.IgnoreCase);
                        foreach (string file in files)
                        {
                            if (filename.IsMatch(file))
                            {
                                m2 = filename.Match(file);
                                f2 = m2.Groups["name"].Value;
                                if (f2.Length >= f1.Length)
                                    if (f2.Substring(0, f1.Length).Equals(f1, StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (!findings.ContainsKey(folder))
                                            findings[folder] = new List<string>();
                                        findings[folder].Add(f2);
                                    }
                            }
                        }
                    }
                }

                foreach (string k in findings.Keys)
                {
                    folders.Remove(k);
                }

                foreach (string folder in folders)
                {
                    ScanFolder(folder);
                }
            }
            catch { }
            
        }

        private void lstFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstFiles.Items.Clear();
            if(lstFolders.SelectedItem!=null)
            foreach( string file in findings[lstFolders.SelectedItem.ToString()])
            {
                lstFiles.Items.Add(file);
            }
        }

        private void btnDeleteFolder_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are your sure?","Confirmation",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    System.IO.Directory.Delete(lstFolders.SelectedItem.ToString(),true);
                    findings.Remove(lstFolders.SelectedItem.ToString());
                    lstFolders.Items.Remove(lstFolders.SelectedItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The folder could not be removed!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }

            }
        }

        private void btRemoveFile_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are your sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    string file;
                    file = lstFolders.SelectedItem.ToString() + "/" + lstFiles.SelectedItem.ToString();
                    System.IO.File.Delete(file);
                    findings[lstFolders.SelectedItem.ToString()].Remove(lstFiles.SelectedItem.ToString());
                    lstFiles.Items.Remove(lstFiles.SelectedItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The file could not be removed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
