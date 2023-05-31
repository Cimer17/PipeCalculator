﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace WindowsFormsApp1
{
    public partial class AddForm : Form
    {
        public static string connectString = string.Format("Provider = Microsoft.ACE.OLEDB.12.0; Data Source= |DataDirectory|\\Database1.accdb;");
        private OleDbConnection myConnection;

        public AddForm()
        {
            InitializeComponent();
        }


        private void AddForm_Load(object sender, EventArgs e)
        {
            
        }
        
        private void FunkAdd(string osnastka)
        {
            myConnection = new OleDbConnection(connectString);

            myConnection.Open();

            DataTable schemaTable = myConnection.GetSchema("Columns");
            DataRow[] rows = schemaTable.Select($"TABLE_NAME = '{osnastka}'");

            List<string> columnNames = new List<string>();

            foreach (DataRow row in rows)
            {
                string columnName = row["COLUMN_NAME"].ToString();
                columnNames.Add(columnName);
            }

            OleDbCommand commander = new OleDbCommand();
            commander.Connection = myConnection;

            commander.CommandText = $"Insert into [{osnastka}] ({columnNames[1]}, {columnNames[2]}) values (@naimenovanie, @diametr)";
            commander.Parameters.AddWithValue("@naimenovanie", textBox1.Text);
            commander.Parameters.AddWithValue("@diametr", textBox2.Text);

            commander.ExecuteNonQuery();

            columnNames.Clear();
            myConnection.Close();

            textBox1.Clear();
            textBox2.Clear();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBoxAdd.SelectedIndex != -1)
            {
                if ((textBox1.Text == "" && textBox2.Text == "")|| 
                    (textBox1.Text == ""|| textBox2.Text == ""))
                {
                    MessageBox.Show("Заполните пустые поля.",
                         "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string selectedTableClick = comboBoxAdd.SelectedItem.ToString();
                    switch (selectedTableClick)
                    {
                        case "Зенковки":
                            FunkAdd("Зенковки");
                            break;
                        case "Ролики":
                            FunkAdd("Ролики");
                            break;
                    }
                }
            }
            else MessageBox.Show("Поле 'Выбор остнастки' не может быть пустым.",
                 "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        private void comboBoxAdd_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxAdd.SelectedIndex)
            {
                case 0:
                    {
                        label1.Text = "Наименование";
                        label2.Text = "Диаметр";
                    }
                    break;
                case 1:
                    {
                        label1.Text = "Наименование";
                        label2.Text = "Параметры";
                    }
                    break;
            }
        }
    }
}