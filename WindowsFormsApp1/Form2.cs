using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        
        public double result = 0;
        public List<string> radii = new List<string>();

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                double R = Convert.ToDouble(textBox1.Text);
                result += R;
                label1.Text = Convert.ToString(result);
                label8.Text = Convert.ToString(result);
                textBox1.Clear();
            }
            else {
                
                double R = Convert.ToDouble(textBox1.Text);
                double corner = Convert.ToDouble(textBox2.Text);
                double count_R = Convert.ToDouble(textBox3.Text);
                double counting = (Math.Round((2 * Math.PI * R * corner) / 360)) * count_R;
                
                result += counting;
                label1.Text = Convert.ToString(counting) + " " + "мм";
                label8.Text = Convert.ToString(result) + " " + "мм";
                
                string RollerSize = textBox4.Text + "/" + textBox1.Text;
                
                if (!radii.Contains(RollerSize))
                {
                    radii.Add(RollerSize);
                }


                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label3.Text = "L участка";
                textBox2.Enabled = false;
                textBox3.Enabled = false;
            }
            else
            {
                label3.Text = "Rср гиба";
                textBox2.Enabled = true;
                textBox3.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            radii.Clear();
            result = 0;
            label1.Text = "result";
            label8.Text = "result";
        }

        private void label8_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(label8.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string elements = "Нужные ролики для гибки данной трубы:\n" + " " + string.Join(", ", radii);
            MessageBox.Show(elements, "Нужные ролики", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}