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

        private void directCount(double sectionlength) // функция подсчёта прямого участка
        {
            result += sectionlength;
            label1.Text = Convert.ToString(result) + " " + "мм";
            label8.Text = Convert.ToString(result) + " " + "мм";
            textBox1.Clear();
        }

        private void curvedSection() // функция подсчёта кривых
        {

        }


        private void button1_Click(object sender, EventArgs e) // тут стоит добавить учёт допусков, оптимизировать
        {
            if (checkBox1.Checked)
            {
                try
                {
                    double R = Convert.ToDouble(textBox1.Text.Replace(".", ","));
                    directCount(R);
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
    }
}