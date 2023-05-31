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
            try
            {

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
            else
            {
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

        List<string> newRequiredRollersList = new List<string>();
        private string GetRollers() // нужно считывать все нужные ролики, а не один и в случае если к параметрам нет ролика писать просто его параметры (12/12)
        {
            if (requiredRollers.Count == 0)
            {
                return "Недостаточно данных!";
            }
            string radiiString = string.Join(", ", requiredRollers.Select(p => $"'{p}'"));
            string query = $"SELECT Наименование FROM Ролики WHERE Параметры IN ({radiiString})";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader reader = command.ExecuteReader();
            List<string> denominations = new List<string>();
            
            while (reader.Read())
            {
                string denomination = reader["Наименование"].ToString();
                denominations.Add(denomination);
            }

                string newDenominations = Convert.ToString(denominations);

                Perebor();

                foreach (string sTemp in newRequiredRollersList)
                {
                    requiredRollers.Remove(sTemp);
                }

                foreach(string sTemp in requiredRollers)
                {
                    newDenominations += $", {sTemp}";
                }

                if (newDenominations != null)
                {
                    return newDenominations;
                }
                else
                {
                    return "Под такие параметры роликов нет!";
                }
        }

        private void Perebor()
        {
            myConnection = new OleDbConnection(connectString);
            myConnection.Open();

            string sqlQuery = $"SELECT [Параметры] FROM [Ролики]";

            using (OleDbCommand command = new OleDbCommand(sqlQuery, myConnection))
            {
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string columnValue = reader.GetValue(0).ToString();
                            newRequiredRollersList.Add(columnValue);
                        }
                    }
                }
            }
            myConnection.Close();
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
            string diameter = textBox4.Text;

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
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Недопустимое значение выбранного поля.",
                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
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
            AddForm newFormAdd = new AddForm();
            newFormAdd.ShowDialog();
        }

        //кнопка удаления записи из таблицы
        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                DialogResult dialogResult =
                    MessageBox.Show("Вы действительно хотите удалить выбранные записи?",
                "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    string selectedTableClick = comboBox1.SelectedItem.ToString();
                    switch (selectedTableClick)
                    {
                        case "Зенковки":
                            LoadDataDeleteTable("Зенковки");
                            break;
                        case "Ролики":
                            LoadDataDeleteTable("Ролики");
                            break;
                    }
                }
            }
            else MessageBox.Show("Чтобы удалить запись выберите остнастку.",
                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        //функция удаления записей в разных таблицах, в зависимости от ее выбора, методом выделения
        private void LoadDataDeleteTable(string tableNameOsnast)
        {
            DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;

            // Проверяем, есть ли выбранные строки
            if (selectedRows.Count > 0)
            {
                // Создаем новый объект команды SQL для удаления строк из таблицы
                OleDbCommand command = new OleDbCommand();
                command.Connection = myConnection;
                command.CommandType = CommandType.Text;
                command.CommandText = $"DELETE FROM [{tableNameOsnast}] WHERE [Код] IN (";

                // Создаем массив для хранения ID выбранных строк
                List<int> selectedIDs = new List<int>();

                // Заполняем массив ID выбранных строк
                foreach (DataGridViewRow row in selectedRows)
                {
                    int id = Convert.ToInt32(row.Cells[0].Value);
                    selectedIDs.Add(id);
                }

                // Добавляем ID выбранных строк в текст команды SQL
                for (int i = 0; i < selectedIDs.Count; i++)
                {
                    command.CommandText += selectedIDs[i];

                    if (i < selectedIDs.Count - 1)
                    {
                        command.CommandText += ",";
                    }
                }

                command.CommandText += ")";

                // Выполняем команду SQL для удаления строк из таблицы
                int numRowsDeleted = command.ExecuteNonQuery();

                // Удаляем выбранные строки из DataGridView
                foreach (DataGridViewRow row in selectedRows)
                {
                    dataGridView1.Rows.Remove(row);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}