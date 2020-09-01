using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;

        private bool click;
        private string query;
        public Form1()
        {
            InitializeComponent();
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CountriesCS"].ConnectionString;

            sqlConnection = new SqlConnection(connectionString);

            await sqlConnection.OpenAsync();

            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.View = View.Details;

            listView1.Columns.Add("Название");
            listView1.Columns.Add("Код Страны");
            listView1.Columns.Add("Столица");
            listView1.Columns.Add("Площадь");
            listView1.Columns.Add("Население");
            listView1.Columns.Add("Регион");
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            click = true;
            listView1.Items.Clear();                       
            string text = textBox1.Text;
            query = String.Format("SELECT t1.NameCountry, t1.CodeCountry, t2.NameCity , t1.Area, t1.Population, t3.NameRegion FROM Countries AS t1 join Cities AS t2 on t2.idCity = t1.Capital join Regions as t3 on t1.Region = t3.idRegion WHERE t1.NameCountry = '{0}'", text);
            await LoadCountries(query);     
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                sqlConnection.Close();
        }

        private async void Button2_Click(object sender, EventArgs e)
        {
            click = false;
            listView1.Items.Clear();
            query = "SELECT t1.NameCountry, t1.CodeCountry, t2.NameCity , t1.Area, t1.Population, t3.NameRegion FROM Countries AS t1 join Cities AS t2 on t2.idCity = t1.Capital join Regions as t3 on t1.Region = t3.idRegion";
            await LoadCountries(query);                  
        }

        private async Task LoadCountries(string query)
        {
            SqlDataReader sqlReader = null;           

            SqlCommand getCountries = new SqlCommand(query, sqlConnection);            

            sqlReader = await getCountries.ExecuteReaderAsync();

            try
            {
                if (sqlReader.HasRows)
                {
                    while (await sqlReader.ReadAsync())
                    {
                        ListViewItem item = new ListViewItem(new string[]
                        {
                        Convert.ToString(sqlReader["NameCountry"]),
                        Convert.ToString(sqlReader["CodeCountry"]),
                        Convert.ToString(sqlReader["NameCity"]),
                        Convert.ToString(sqlReader["Area"]),
                        Convert.ToString(sqlReader["Population"]),
                        Convert.ToString(sqlReader["NameRegion"])
                        });

                        listView1.Items.Add(item);
                    }
                }
                else
                
                {
                    if (click == true)
                    {
                        sqlReader.Close();
                        DialogResult result = MessageBox.Show(
                              "Страна не найдена. Добавить?",
                              "Сообщение",
                              MessageBoxButtons.YesNo,
                              MessageBoxIcon.Information,
                              MessageBoxDefaultButton.Button1,
                              MessageBoxOptions.DefaultDesktopOnly);

                        if (result == DialogResult.Yes)
                        {
                            Form2 newForm = new Form2(sqlConnection);
                            newForm.Show();
                        }
                    }
                    else
                    {
                        sqlReader.Close();
                        MessageBox.Show("Таблица пуста", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlReader != null && !sqlReader.IsClosed)
                {
                    sqlReader.Close();
                }
            }

            

        }
    }
}
