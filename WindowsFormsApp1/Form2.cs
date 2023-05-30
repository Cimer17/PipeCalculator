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
using WindowsFormsApp1.Database1DataSetTableAdapters;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        
        public static string connectString = string.Format("Provider = Microsoft.ACE.OLEDB.12.0; Data Source= |DataDirectory|\\Database1.accdb;");
        private OleDbConnection myConnection;


        public double result = 0; // переменная развертки роликов
        public double tolerance = 0; // переменная подсчета допуска
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

        public List<double> ExtractNumber(string input) // разбитие числа на число и  допуск
        {
            
            List<double> numbers = new List<double>();
            int index = input.IndexOf('±');
            
            if (index != -1)
            {
                string numberString = input.Substring(index + 1);
                string numberOne = input.Substring(0, index);

                double Onetotal = Convert.ToDouble(numberOne.Replace(".", ","));
                double Twototal = Convert.ToDouble(numberString.Replace(".", ","));

                numbers.Add(Onetotal);
                numbers.Add(Twototal);
                return numbers;
            }
            
            double total = Convert.ToDouble(input.Replace(".", ","));
            
            numbers.Add(total);
            return numbers;
        }


        private void directCount() // функция подсчёта прямого участка
        {
            try {

                List<double> numbers = ExtractNumber(textBox1.Text.Replace(" ", string.Empty));
                double total = numbers[0];
                double tolTotal = 0;
                
                if (numbers.Count == 2)
                {
                    tolTotal = numbers[1];
                }
                
                result += total;
                tolerance += tolTotal;
                label1.Text = Convert.ToString(total) + " " + "мм" + " ± " + Convert.ToString(tolTotal) + " мм";
                label8.Text = Convert.ToString(result) + " " + "мм" + " ± " + Convert.ToString(tolerance) + " мм";
                textBox1.Clear();
            }
             catch
            {
                MessageBox.Show("Введите кооректные значения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
       }


        private void curvedSection() // функция подсчёта кривых
        {
            try
            {
                List<double> numbersR = ExtractNumber(textBox1.Text.Replace(" ", string.Empty));
                List<double> numbersCorner = ExtractNumber(textBox2.Text.Replace(" ", string.Empty));
                double tolTotal = 0;
                double R = numbersR[0];
                double Corner = numbersCorner[0];
                if (numbersR.Count == 2)
                {
                    tolTotal += numbersR[1];
                }
                if (numbersCorner.Count == 2)
                {
                    tolTotal += numbersCorner[1];
                }
                double count_R = Convert.ToDouble(textBox3.Text.Replace(".", ","));
                double total = (Math.Round((2 * Math.PI * numbersR[0] * numbersCorner[0]) / 360)) * count_R;
                result += total;
                label1.Text = Convert.ToString(total) + " " + "мм" + " ± " + Convert.ToString(tolTotal) + " мм";
                label8.Text = Convert.ToString(result) + " " + "мм" + " ± " + Convert.ToString(tolerance) + " мм";
                string RollerSize = textBox4.Text + "/" + textBox1.Text;
                
                if (!requiredRollers.Contains(RollerSize))
                {
                    requiredRollers.Add(RollerSize);
                }
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
                
            catch
            {
                MessageBox.Show("Введите кооректные значения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                directCount();

            }
            else {
                curvedSection();
            }
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e) // проверка прямой участок или изгиб
        {
            if (checkBox1.Checked)
            {
                label3.Text = "L участка, мм";
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

            dataGridView1.DataSource = зенковкиBindingSource;
            dataGridView1.DataError += new DataGridViewDataErrorEventHandler(dataGridView1_DataError);

        }

        void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // you can obtain current editing value like this:
            string value = null;
            var ctl = dataGridView1.EditingControl as DataGridViewTextBoxEditingControl;

            if (ctl != null)
                value = ctl.Text;

            // you can obtain the current commited value
            object current = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            string message;
            switch (e.ColumnIndex)
            {
                case 0:
                    // bound to integer field
                    message = "the value should be a number";
                    break;
                case 1:
                    // bound to date time field
                    message = "the value should be in date time format yyyy/MM/dd hh:mm:ss";
                    break;
                // other columns
                default:
                    message = "Invalid data";
                    break;
            }

            MessageBox.Show(message);
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
            dataGridView1.DataSource = dataTable;
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

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                if (comboBox1.SelectedIndex == 0) MessageBox.Show("zen");
                if (comboBox1.SelectedIndex == 1) MessageBox.Show("rol");
                //int selectedTable = comboBox1.SelectedIndex;
                //switch (selectedTable)
                //{
                //    case "Зенковки":
                //        this.роликиTableAdapter.Update(this.database1DataSet.Ролики);
                //        break;
                //    case "Ролики":
                //        this.зенковкиTableAdapter.Update(this.database1DataSet.Зенковки);
                //        break;
                //}
            }
            if (comboBox1.SelectedIndex == -1) MessageBox.Show("eror");
        }
    }
}