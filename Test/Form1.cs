using System;
using System.Drawing;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonInput_Click(object sender, EventArgs e)
        {
            if (openFileDialogInput.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string name = "";
                    if ((name = openFileDialogInput.FileName) != null)
                    {
                        textBoxInput.Text = name;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void buttonOuput_Click(object sender, EventArgs e)
        {
            if (saveFileDialogOutput.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string name = "";
                    if ((name = saveFileDialogOutput.FileName) != null)
                    {
                        textBoxOutput.Text = name;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            Bitmap input = new Bitmap(textBoxInput.Text);
            Bitmap output = ConvertImage.ConvertTo1Bit(input);
            output.Save(textBoxOutput.Text);
        }
    }
}
