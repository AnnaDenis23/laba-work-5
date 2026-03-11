using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kf_f5
{
    public partial class Form2 : Form
    {
        
        private string tableName;
        private SQLiteConnection connection;
        private DataRow productData;

        public Form2(string tableName, SQLiteConnection connection)
        {
            InitializeComponent();
           
            this.tableName = tableName;
            this.connection = connection;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
            LoadProductNames();
            
        }

        private void LoadProductNames()
        {
            // Загружаем все названия товаров в ComboBox
            string query = $"SELECT [Наименование_товара] FROM [{tableName}] ORDER BY [Наименование_товара]";
            SQLiteCommand command = new SQLiteCommand(query, connection);


            DataTable dt = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(dt);

            comboBox1.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                comboBox1.Items.Add(row["Наименование_товара"].ToString());
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void LoadProductData(string productName)
        {
            string query = $"SELECT * FROM [{tableName}] WHERE [Наименование_товара] = @ProductName";
            SQLiteCommand command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@ProductName", productName);

            DataTable dt = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                productData = dt.Rows[0];
            }
        }


        private void DisplayCurrentValues()
        {
            if (productData != null)
            {
                // Отображаем текущие значения
                if (productData.Table.Columns.Contains("Производитель"))
                    textBox1.Text = productData["Производитель"].ToString();

                if (productData.Table.Columns.Contains("Цена"))
                    textBox2.Text = productData["Цена"].ToString();

                if (productData.Table.Columns.Contains("Количество"))
                    textBox3.Text = productData["Количество"].ToString();

                if (productData.Table.Columns.Contains("Тип_Техники"))
                    textBox4.Text = productData["Тип_Техники"].ToString();

                if (productData.Table.Columns.Contains("Энергопотребление"))
                    textBox5.Text = productData["Энергопотребление"].ToString();

                if (productData.Table.Columns.Contains("Гарантия_лет"))
                    textBox6.Text = productData["Гарантия_лет"].ToString();
                if (productData.Table.Columns.Contains("Тип_Комплектующего"))
                    textBox7.Text = productData["Тип_Комплектующего"].ToString();

                if (productData.Table.Columns.Contains("Гарантия_месяцы"))
                    textBox8.Text = productData["Гарантия_месяцы"].ToString();

                if (productData.Table.Columns.Contains("Совместимость"))
                    textBox9.Text = productData["Совместимость"].ToString();

            } }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар для редактирования", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string selectedProduct = comboBox1.SelectedItem.ToString();

                // Формируем SQL запрос для обновления только измененных полей
                string updateQuery = $"UPDATE [{tableName}] SET ";
                bool hasChanges = false;

                if (productData.Table.Columns.Contains("Производитель") && textBox1.Text != productData["Производитель"].ToString())
                {
                    updateQuery += "[Производитель] = @Manufacturer, ";
                    hasChanges = true;
                }

                if (productData.Table.Columns.Contains("Цена") && textBox2.Text != productData["Цена"].ToString())
                {
                    updateQuery += "[Цена] = @Price, ";
                    hasChanges = true;
                }

                if (productData.Table.Columns.Contains("Количество") && textBox3.Text != productData["Количество"].ToString())
                {
                    updateQuery += "[Количество] = @Quantity, ";
                    hasChanges = true;
                }
                if (productData.Table.Columns.Contains("Тип_Техники") && textBox4.Text != productData["Тип_Техники"].ToString())
                {
                    updateQuery += "[Тип_Техники] = @Type, ";
                    hasChanges = true;
                }

                if (productData.Table.Columns.Contains("Энергопотребление") && textBox5.Text != productData["Энергопотребление"].ToString())
                {
                    updateQuery += "[Энергопотребление] = @Energy, ";
                    hasChanges = true;
                }

                if (productData.Table.Columns.Contains("Гарантия_лет") && textBox6.Text != productData["Гарантия_лет"].ToString())
                {
                    updateQuery += "[Гарантия_лет] = @WarrantyYears, ";
                    hasChanges = true;
                }
                if (productData.Table.Columns.Contains("Тип_Комплектующего") && textBox7.Text != productData["Тип_Комплектующего"].ToString())
                {
                    updateQuery += "[Тип_Комплектующего] = @ComponentType, ";
                    hasChanges = true;
                }

                if (productData.Table.Columns.Contains("Гарантия_месяцы") && textBox8.Text != productData["Гарантия_месяцы"].ToString())
                {
                    updateQuery += "[Гарантия_месяцы] = @WarrantyMonths, ";
                    hasChanges = true;
                }

                if (productData.Table.Columns.Contains("Совместимость") && textBox9.Text != productData["Совместимость"].ToString())
                {
                    updateQuery += "[Совместимость] = @Compatibility, ";
                    hasChanges = true;
                }
                if (!hasChanges)
                {
                    MessageBox.Show("Нет изменений для сохранения.", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Убираем последнюю запятую и пробел
                updateQuery = updateQuery.TrimEnd(',', ' ');
                updateQuery += " WHERE [Наименование_товара] = @ProductName";

                SQLiteCommand command = new SQLiteCommand(updateQuery, connection);

                // Добавляем параметры только для измененных полей
                if (productData.Table.Columns.Contains("Производитель") && textBox1.Text != productData["Производитель"].ToString())
                    command.Parameters.AddWithValue("@Manufacturer", textBox1.Text);
                if (productData.Table.Columns.Contains("Цена") && textBox2.Text != productData["Цена"].ToString())
                    command.Parameters.AddWithValue("@Price", Convert.ToDecimal(textBox2.Text));

                if (productData.Table.Columns.Contains("Количество") && textBox3.Text != productData["Количество"].ToString())
                    command.Parameters.AddWithValue("@Quantity", Convert.ToInt32(textBox3.Text));

                if (productData.Table.Columns.Contains("Тип_Техники") && textBox4.Text != productData["Тип_Техники"].ToString())
                    command.Parameters.AddWithValue("@Type", textBox4.Text);

                if (productData.Table.Columns.Contains("Энергопотребление") && textBox5.Text != productData["Энергопотребление"].ToString())
                    command.Parameters.AddWithValue("@Energy", textBox5.Text);

                if (productData.Table.Columns.Contains("Гарантия_лет") && textBox6.Text != productData["Гарантия_лет"].ToString())
                    command.Parameters.AddWithValue("@WarrantyYears", Convert.ToInt32(textBox6.Text));
                if (productData.Table.Columns.Contains("Тип_Комплектующего") && textBox7.Text != productData["Тип_Комплектующего"].ToString())
                    command.Parameters.AddWithValue("@ComponentType", textBox7.Text);

                if (productData.Table.Columns.Contains("Гарантия_месяцы") && textBox8.Text != productData["Гарантия_месяцы"].ToString())
                    command.Parameters.AddWithValue("@WarrantyMonths", Convert.ToInt32(textBox8.Text));

                if (productData.Table.Columns.Contains("Совместимость") && textBox9.Text != productData["Совместимость"].ToString())
                    command.Parameters.AddWithValue("@Compatibility", textBox9.Text);

                command.Parameters.AddWithValue("@ProductName", selectedProduct);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Данные успешно сохранены!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Не закрываем форму, чтобы можно было редактировать другие товары
                    LoadProductNames(); // Обновляем список на случай изменений названий
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string selectedProduct = comboBox1.SelectedItem.ToString();
                LoadProductData(selectedProduct);
                DisplayCurrentValues();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}


            


    

