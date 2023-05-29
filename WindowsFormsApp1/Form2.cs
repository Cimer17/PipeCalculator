using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        
        public static string connectString = string.Format("Provider = Microsoft.ACE.OLEDB.12.0; Data Source= |DataDirectory|\\Database1.accdb;");
        private OleDbConnection myConnection;


        public double result = 0; // переменная развертки роликов
        public List<string> requiredRollers = new List<string>(); // список нужных параметров роликов для гибки


        public Form2()
        {
            InitializeComponent();
            myConnection = new OleDbConnection(connectString); // создаем соединение и подключаемся
            myConnection.Open();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            myConnection.Close(); // закрытие соединения при выключении программы
        }


        private void button1_Click(object sender, EventArgs e) // тут стоит добавить учёт допусков, оптимизировать
        {
            if (checkBox1.Checked)
            {
                try
                {
                    double R = Convert.ToDouble(textBox1.Text.Replace(".", ","));
                    result += R;
                    label1.Text = Convert.ToString(result) + " " + "мм";
                    label8.Text = Convert.ToString(result) + " " + "мм";
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
                    if (!requiredRollers.Contains(RollerSize))
                    {
                        requiredRollers.Add(RollerSize);
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


        private void checkBox1_CheckedChanged(object sender, EventArgs e) // проверка прямой участок или изгиб
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
            requiredRollers.Clear();
            result = 0;
            label1.Text = "result";
            label8.Text = "result";
        }

        private void label8_Click(object sender, EventArgs e) // копирование текста развертки
        {
            Clipboard.SetText(label8.Text);
        }

        private string GetRollers() // нужно считывать все нужные ролики, а не один и в случае если к параметрам нет ролика писать просто его параметры (12/12)
        {
            if (requiredRollers.Count != 0) { 
                string radiiString = string.Join(", ", requiredRollers.Select(p => $"'{p}'"));
                string query = $"SELECT Наименование FROM Ролики WHERE Параметры IN ({radiiString})";
                OleDbCommand command = new OleDbCommand(query, myConnection);
                object denominations = command.ExecuteScalar();
                if (denominations != null)
                {
                    return denominations.ToString();
                }
                else
                {
                    return "Под такие параметры роликов нет!";
                }
            }
            else
            {
                return "Недостаточно данных!";
            }
        }

        private string GetСountersink(string diameter)
        {
            string query = $"SELECT Наименование FROM Зенковки WHERE Диаметр='{diameter}'";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            object countersink = command.ExecuteScalar();
            if (countersink != null)
            {
                return countersink.ToString();
            }
            else
            {
                return "Не найдена!";
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            string diameter  = textBox4.Text;

            if (string.IsNullOrEmpty(diameter))
            {
                MessageBox.Show("Введите значение диаметра");
                return;
            }
            var countersink = GetСountersink(diameter);
            var rollers = GetRollers();
            MessageBox.Show($"Зенковка: {countersink}\nРолики: {rollers}");
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