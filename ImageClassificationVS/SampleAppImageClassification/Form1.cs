using ImageClassificationVSML.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleAppImageClassification
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            setup();
                
        }

        void setup()
        {
            FlowLayoutPanel panel1 = new FlowLayoutPanel();
            panel1.Dock = DockStyle.Fill;
            panel1.FlowDirection = FlowDirection.TopDown;
            this.Controls.Add(panel1);
            Button btn = new Button() { Text="Select Folder", Width=120  };
            var txtFolder = new TextBox() { ReadOnly=true, Width=400 };
            var lblStatus = new Label() { Width=500 };
            panel1.Controls.Add(txtFolder);
            panel1.Controls.Add(lblStatus);
            panel1.Controls.Add(btn);
            var allFiles = new List<string>();
            btn.Click += (a, b) => {
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();
                   
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        txtFolder.Text = fbd.SelectedPath;
                        var exts = "*.jpg,*.png,*.bmp";
                        allFiles.Clear();
                        foreach(var ext in exts.Split(",")) {
                            string[] files = Directory.GetFiles(fbd.SelectedPath, ext);
                            allFiles.AddRange(files);
                        }
                        lblStatus.Text = "Images found: " + allFiles.Count.ToString();
                        //System.Windows.Forms.MessageBox.Show("Files found: " + allFiles.Count.ToString(), "Message");
                    }
                }
            };
            var btn2 = new Button() { Text = "Predicts" };
            var grid = new DataGridView();
            var progress1 = new ProgressBar();
            panel1.Controls.Add(btn2);
            panel1.Controls.Add(grid);
            panel1.Controls.Add(progress1);
            grid.AutoGenerateColumns = true;
            grid.Width = 600;
            btn2.Click += (a, b) => {
                if (allFiles.Count <= 0)
                {
                    System.Windows.Forms.MessageBox.Show("Please select folder with images first.", "Message");
                    return;
                }
                progress1.Maximum = allFiles.Count;
                var datas = new List<ImageData>();
                var counter = 0;
                foreach (var item in allFiles)
                {
                    var hasil = ConsumeModel.Predict(new ModelInput() { ImageSource = item });
                    datas.Add(new ImageData() { FileName = item, Label = hasil.Prediction, Accuracy= hasil.Score.Max() });
                    progress1.Value = ++counter;
                }
                if (datas.Count > 0)
                {
                    grid.DataSource = datas;
                    
                }
            };
            

        }
    }

    public class ImageData
    {
        public string FileName { get; set; }
        public string Label { get; set; }
        public float Accuracy { get; set; }
    }
}
