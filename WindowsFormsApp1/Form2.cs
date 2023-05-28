using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
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
        public static string connectString = string.Format("Provider = Microsoft.ACE.OLEDB.12.0; Data Source= |DataDirectory|\\Database1.accdb;");

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
                try
                {
                    double R = Convert.ToDouble(textBox1.Text.Replace(".", ","));
                    result += R;
                    label1.Text = Convert.ToString(result);
                    label8.Text = Convert.ToString(result);
                    textBox1.Clear();
                }
                catch {
                    MessageBox.Show("Введите кооректные значения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else {
                try
                {
                    double R = Convert.ToDouble(textBox1.Text.Replace(".", ","));
                    double corner = Convert.ToDouble(textBox2.Text.Replace(".", ","));
                    double count_R = Convert.ToDouble(textBox3.Text.Replace(".", ","));
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
                catch {
                    MessageBox.Show("Введите кооректные значения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'database1DataSet.Ролики' table. You can move, or remove it, as needed.
            this.роликиTableAdapter.Fill(this.database1DataSet.Ролики);
            // TODO: This line of code loads data into the 'database1DataSet.Зенковки' table. You can move, or remove it, as needed.
            this.зенковкиTableAdapter.Fill(this.database1DataSet.Зенковки);

        }
        public DataTable dataTable;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = comboBox1.SelectedItem.ToString();
            switch (selectedTable)
            {
                case "Зенковки":
                    dataTable = LoadDataFromTable("Зенковки");
                    break;
                case "Ролики":
                    dataTable = LoadDataFromTable("Ролики");
                    break;
            }
            UpdateDataGridView(dataTable);
        }
        
        private DataTable LoadDataFromTable(string tableName)
        {
            DataTable dataTable = new DataTable();

            using (OleDbConnection connection = new OleDbConnection(connectString))
            {
                string query = $"SELECT * FROM {tableName}";

                using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }
        private void UpdateDataGridView(DataTable dataTable)
        {
            dataGridView1.DataSource = dataTable;
        }
    }
}