using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace kf_f5
{
    public partial class Form3 : Form
    {
        private string tableName;
        private SQLiteConnection connection;
        private DataTable tableSchema;

        public Form3(string tableName, SQLiteConnection connection)
        {
            InitializeComponent();
            this.tableName = tableName;
            this.connection = connection;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Text = $"Добавление записи в таблицу: {tableName}";
            LoadTableSchema();
            CreateDynamicForm();
        }

        private void LoadTableSchema()
        {
            // Получаем структуру таблицы
            string query = $"PRAGMA table_info([{tableName}])";
            SQLiteCommand command = new SQLiteCommand(query, connection);

            tableSchema = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(tableSchema);
        }

        private void CreateDynamicForm()
        {
            this.SuspendLayout();
            this.Controls.Clear();

            int yPos = 20;

            foreach (DataRow column in tableSchema.Rows)
            {
                string columnName = column["name"].ToString();
                string dataType = column["type"].ToString();

                // Пропускаем автоинкрементные поля (ID)
                if (column["pk"].ToString() == "1" && dataType.ToUpper().Contains("INT"))
                    continue;

                // Создаем Label
                Label label = new Label();
                label.Text = columnName + ":";
                label.Location = new Point(20, yPos);
                label.Size = new Size(180, 20);
                label.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
                label.Tag = columnName;
                this.Controls.Add(label);

                // Создаем TextBox
                TextBox textBox = new TextBox();
                textBox.Location = new Point(200, yPos);
                textBox.Size = new Size(200, 23);
                textBox.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
                textBox.Name = "txt_" + columnName;
                textBox.Tag = columnName;

                // Для числовых полей можно добавить валидацию
                if (dataType.ToUpper().Contains("INT") || dataType.ToUpper().Contains("FLOAT") || dataType.ToUpper().Contains("DECIMAL"))
                {
                    textBox.KeyPress += NumericTextBox_KeyPress;
                }

                this.Controls.Add(textBox);

                yPos += 35;
            }

            // Создаем кнопку сохранения
            Button btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Location = new Point(200, yPos + 20);
            btnSave.Size = new Size(90, 30);
            btnSave.BackColor = Color.LightGreen;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            // Создаем кнопку отмены
            Button btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(300, yPos + 20);
            btnCancel.Size = new Size(90, 30);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);

            // Настраиваем размер формы
            this.ClientSize = new Size(450, yPos + 80);
            this.ResumeLayout();
        }

        private void NumericTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем только цифры, точку и управляющие клавиши
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // Проверяем, что точка только одна
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Формируем SQL запрос для вставки
                string columns = "";
                string values = "";

                foreach (Control control in this.Controls)
                {
                    if (control is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
                    {
                        string columnName = textBox.Tag.ToString();

                        if (!string.IsNullOrEmpty(columns))
                        {
                            columns += ", ";
                            values += ", ";
                        }

                        columns += $"[{columnName}]";
                        values += $"@{columnName}";
                    }
                }

                if (string.IsNullOrEmpty(columns))
                {
                    MessageBox.Show("Нет данных для сохранения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string insertQuery = $"INSERT INTO [{tableName}] ({columns}) VALUES ({values})";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);

                // Добавляем параметры
                foreach (Control control in this.Controls)
                {
                    if (control is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
                    {
                        string columnName = textBox.Tag.ToString();
                        command.Parameters.AddWithValue($"@{columnName}", textBox.Text);
                    }
                }

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Данные успешно добавлены!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}