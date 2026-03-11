using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace kf_f5
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            // Отключение выпадающих списков
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;

            // Отключение кнопок
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;

            // Отключение переключателей
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
        }

        private SQLiteConnection SQLiteConn;
        private DataTable dTable;

        private void MainForm_Load(object sender, EventArgs e)
        {
            SQLiteConn = new SQLiteConnection();
            dTable = new DataTable();

        }

        private bool OpenDBFile()
        {
            // создание диалогового окна
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // установка начальной папки в диалоговом окне
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // установка фильтра файлов по расширению
            openFileDialog.Filter = "Текстовые файлы (*.db)|*.db|Все файлы (*.*)|*.*";
            // отображение диалогового окна и проверка выбора файла
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                // создание подключения к БД
                SQLiteConn = new SQLiteConnection("Data Source=" + openFileDialog.FileName + ";Version=3;");
                SQLiteConn.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = SQLiteConn;
                return true;
            }
            else return false;
        }
        // функция для заполнения comboBox1 названиями таблиц в БД
        // </summary>
        private void GetTableNames()
        {
            // создание запроса к служебной таблице для получения списка таблиц
            string SQLQuery = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            // выполнение запроса и получение списка
            SQLiteDataReader reader = command.ExecuteReader();
            // заполнение comboBox1 названиями таблиц
            comboBox1.Items.Clear();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader[0].ToString());
            }
        }

        private string SQL_AllTable()
        {
            return "SELECT * FROM [" + comboBox1.SelectedItem + "] order by 1 ";

        }

        private string SQL_FilterByManufacture()
        {
            return "SELECT * FROM [" + comboBox1.SelectedItem + "] " + "WHERE Производитель = \"" + comboBox3.SelectedItem + "\";";
        }

        private string SQl_FilterByProduct()
        {
            return "SELECT * FROM [" + comboBox1.SelectedItem + "]" + " WHERE [Количество] <= 5;";
        }

        private void ShowTable(string SQLQuery)
        {
            dTable.Clear();
            // выполнение SQL-запроса и заполнение таблицы dTable
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(SQLQuery, SQLiteConn);
            adapter.Fill(dTable);

            // очистка datadridView1
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            // создание столбцов в табличном компоненте datadridView1
            for (int col = 0; col < dTable.Columns.Count; col++)
            {
                string ColName = dTable.Columns[col].ColumnName;
                dataGridView1.Columns.Add(ColName, ColName);
                // автоширина столбцов по содержимому
                dataGridView1.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            // заполнение строк в табличном компоненте datadridView1
            for (int row = 0; row < dTable.Rows.Count; row++)
            {
                dataGridView1.Rows.Add(dTable.Rows[row].ItemArray);
            }
        }


        private void GetTableColumns()
        {
            // формирование SQL-запроса и его выполнение
            string SQLQuery = "PRAGMA table_info(\"" + comboBox1.SelectedItem + "\");";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            SQLiteDataReader read = command.ExecuteReader();
            // заполнение comboBox2 списком полей (столбцов)
            comboBox2.Items.Clear();
            while (read.Read())
            {
                comboBox2.Items.Add((string)read[1]);
            }
        }


        private void GetManufactures()
        {
            int kol = 0;
            string s1, s2;
            comboBox3.Items.Clear();
            for (int row = 0; row < dTable.Rows.Count; row++)
            {
                for (int i = 0; i < comboBox3.Items.Count; i++)
                {
                    s1 = (string)dTable.Rows[row].ItemArray[2];
                    s2 = (string)comboBox3.Items[i];
                    // поиск совпадений с ранее добавленными строками в comboBox3
                    if (String.Compare(s1, s2) == 0) kol++;
                }
                if (kol == 0) comboBox3.Items.Add(dTable.Rows[row].ItemArray[2]); else kol = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (OpenDBFile() == true)
            {
                GetTableNames();
                comboBox1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // проверка выбора таблицы для открытия в comboBox1
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите таблицу!", "Ошибка", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return;
            }

            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
            radioButton3.Enabled = true;

            ShowTable(SQL_AllTable());
            // отображение таблицы
            GetTableColumns();
            // получение списка столбцов
            GetManufactures();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Проверка выбора поля для расчета
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите поле для расчета", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //определение максимального, минимального, суммы и среднего значения в выбранном поле таблицы
            double max;
            double min;
            double sum = 0;
            double value;
            try
            {
                max = Convert.ToDouble(dTable.Rows[0].ItemArray[comboBox2.SelectedIndex]);
                min = Convert.ToDouble(dTable.Rows[0].ItemArray[comboBox2.SelectedIndex]);
            }
            catch
            {
                //если поле не является числовым, то обрабатывается ошибка преобразования типов данных
                MessageBox.Show("Поле не является числовым", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            for (int row = 0; row < dTable.Rows.Count; row++)
            {
                value = Convert.ToDouble(dTable.Rows[row].ItemArray[comboBox2.SelectedIndex]);
                if (value > max) max = value;
                if (value < min) min = value;
                sum = sum + value;
            }

            //в зависимости от того, какая кнопка была нажата, формируется и выводится нужное сообщение
            string MyMessage = "";
            if ((sender as Button).Name == "button3")
                MyMessage = "Минимальное значение в поле " + comboBox2.Text + " = " + min.ToString();
            if ((sender as Button).Name == "button4")
                MyMessage = "Максимальное значение в поле " + comboBox2.Text + " = " + max.ToString();
            if ((sender as Button).Name == "button5")
                MyMessage = "Среднее значение в поле " + comboBox2.Text + " = " + (sum / dTable.Rows.Count).ToString();
            if ((sender as Button).Name == "button6")
                MyMessage = "Сумма значений в поле " + comboBox2.Text + " = " + sum.ToString();

            MessageBox.Show(MyMessage, "Расчеты", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button7_Click(object sender, EventArgs e)
        {


            try
            {
                //проверка выбора производителя для фильтрации данных
                if (comboBox3.SelectedIndex == -1 && radioButton2.Checked == true)
                {
                    MessageBox.Show("Выберите производителя товара", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (radioButton1.Checked == true)
                    ShowTable(SQL_AllTable());
                //вывод всей таблицы
                else if (radioButton2.Checked == true)
                    ShowTable(SQL_FilterByManufacture());
                //вывод записей, отфильтрованных по производителю
                else if (radioButton3.Checked == true)
                {
                    ShowTable(SQl_FilterByProduct());
                    //вывод записей, отфильтрованных по продукту

                    // Получаем отфильтрованные данные для сообщения
                    DataTable filteredTable = new DataTable();
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(SQl_FilterByProduct(), SQLiteConn);
                    adapter.Fill(filteredTable);

                    // Формируем сообщение с информацией о товарах
                    if (filteredTable.Rows.Count > 0)
                    {
                        string message = "Заканчивающиеся товары (количество < 5):\n\n";

                        // Предполагаем, что столбец с названием товара имеет индекс 1, а с количеством - индекс 3
                        // Вам нужно настроить индексы в соответствии с вашей структурой таблицы
                        int nameColumnIndex = 1; // Индекс столбца с названием товара
                        int quantityColumnIndex = 3; // Индекс столбца с количеством

                        foreach (DataRow row in filteredTable.Rows)
                        {
                            string productName = row[nameColumnIndex].ToString();
                            string quantity = row[quantityColumnIndex].ToString();
                            message += $"- {productName}: {quantity} руб.\n";
                        }

                        MessageBox.Show(message, "Заканчивающиеся товары",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    else
                    {
                        MessageBox.Show("Нет товаров с количеством меньше 5", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при применении фильтра: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Сначала откройте таблицу", "Ошибка", // ИСПРАВЛЕНО: закрыта кавычка
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string tableName = comboBox1.SelectedItem.ToString();

            // Создаем и показываем форму редактирования
            Form2 editForm = new Form2(tableName, SQLiteConn); // ИСПРАВЛЕНО: правильное имя переменной
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // Обновляем таблицу после редактирования
                ShowTable(SQL_AllTable());
                MessageBox.Show("Данные обновлены!", "Успех",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Сначала откройте таблицу", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string tableName = comboBox1.SelectedItem.ToString();

            // Создаем и показываем форму добавления
            Form3 addForm = new Form3(tableName, SQLiteConn);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                // Обновляем таблицу после добавления
                ShowTable(SQL_AllTable());
                MessageBox.Show("Данные добавлены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
