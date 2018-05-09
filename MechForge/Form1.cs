﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MechForge
{
    public partial class Form1 : Form
    {
        private string directory;
        private const string DEFAULT_DIR = "D:\\SteamLibrary\\steamapps\\common\\BATTLETECH\\BattleTech_Data\\StreamingAssets\\data\\mech";
        private string filename;

        public Form1()
        {
            InitializeComponent();
            FolderTextBox.Text = DEFAULT_DIR;
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            directory = FolderTextBox.Text;
            
            DirectoryInfo di = new DirectoryInfo(directory);

            List<FileInfo> filenames = new List<FileInfo>();

            try
            {
                filenames = di.GetFiles().ToList<FileInfo>();
            }
            catch (DirectoryNotFoundException ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }

            FileListBox.DataSource = filenames;
            FileListBox.DisplayMember = "Name";
            

        }

        private void FileListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            filename = FileListBox.Text;          

            string text = File.ReadAllText($"{directory}\\{filename}");

            try
            {
                string formatted = JValue.Parse(text).ToString(Newtonsoft.Json.Formatting.Indented);
                fastColoredTextBox1.Text = formatted;
            }
            catch (JsonReaderException exception)
            {
                //swallow for now
            }

            
        }

        private string SyntaxHighlightJson(string original)
        {
            return Regex.Replace(
              original,
              @"(¤(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\¤])*¤(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)".Replace('¤', '"'),
              match => {
                  var cls = "number";
                  if (Regex.IsMatch(match.Value, @"^¤".Replace('¤', '"')))
                  {
                      if (Regex.IsMatch(match.Value, ":$"))
                      {
                          cls = "key";
                      }
                      else
                      {
                          cls = "string";
                      }
                  }
                  else if (Regex.IsMatch(match.Value, "true|false"))
                  {
                      cls = "boolean";
                  }
                  else if (Regex.IsMatch(match.Value, "null"))
                  {
                      cls = "null";
                  }
                  return "<span class=\"" + cls + "\">" + match + "</span>";
              });
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string textToSave = fastColoredTextBox1.Text;
            File.WriteAllText($"{directory}\\{filename}",textToSave);

        }
    }
}
