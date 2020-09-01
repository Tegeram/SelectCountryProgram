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

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private SqlConnection sqlConnection = null;        
        private string City;
        private string Region;
        private string idCity;
        private string idRegion;

        public Form2(SqlConnection connection)
        {
            InitializeComponent();

            sqlConnection = connection;
        }        

        private async void Button1_Click(object sender, EventArgs e)
        {
            SqlDataReader sqlReader1;  
            
            City = textBox3.Text;
            Region = textBox6.Text;

        
            string query = String.Format("SELECT * FROM Cities WHERE NameCity = '{0}'", City);
            SqlCommand selectCity = new SqlCommand(query, sqlConnection);
            sqlReader1 = await selectCity.ExecuteReaderAsync();
            if (sqlReader1.HasRows)
                {
                while (sqlReader1.Read())
                {

                    idCity = Convert.ToString(sqlReader1["idCity"]);
                }
                    sqlReader1.Close();
                }
            else
                {
                    sqlReader1.Close();
                    string insquery = String.Format("INSERT INTO Cities (NameCity) VALUES ('{0}')", City);
                    SqlCommand insertCity = new SqlCommand(insquery, sqlConnection);
                    await insertCity.ExecuteNonQueryAsync();
                    SqlCommand selectCity1 = new SqlCommand(query, sqlConnection);
                    sqlReader1 = selectCity1.ExecuteReader();
                    while (sqlReader1.Read())
                    {
                        idCity = Convert.ToString(sqlReader1["idCity"]);
                    }                 
                    sqlReader1.Close();
                }


            string query2 = String.Format("SELECT idRegion FROM Regions WHERE NameRegion = '{0}'", Region);
            SqlCommand selectRegion = new SqlCommand(query2, sqlConnection);
            sqlReader1 = await selectRegion.ExecuteReaderAsync();
            if (sqlReader1.HasRows)
            {
                while (sqlReader1.Read())
                {
                    idRegion = Convert.ToString(sqlReader1["idRegion"]);
                }
                sqlReader1.Close();
            }
            else
            {
                sqlReader1.Close();
                string insquery = String.Format("INSERT INTO Regions (NameRegion) VALUES ('{0}')", Region);
                SqlCommand insertRegion = new SqlCommand(insquery, sqlConnection);
                await insertRegion.ExecuteNonQueryAsync();
                SqlCommand selectRegion1 = new SqlCommand(query2, sqlConnection);
                sqlReader1 = selectRegion1.ExecuteReader();
                while (sqlReader1.Read())
                {
                    idRegion = Convert.ToString(sqlReader1["idRegion"]);
                }

                sqlReader1.Close();
            }     

            
            SqlDataReader sqlReader2;
            string Code = textBox2.Text;
            string codequery = String.Format("SELECT * FROM Countries WHERE CodeCountry = '{0}'", Code);
            SqlCommand selectCode = new SqlCommand(codequery, sqlConnection);
            sqlReader2 = await selectCode.ExecuteReaderAsync();
            if (sqlReader2.HasRows)
            {
                MessageBox.Show("Такая страна уже есть в списке", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sqlReader2.Close();
            }
            else
            {
                sqlReader2.Close();
                string insertquery = String.Format("INSERT INTO Countries VALUES (@NameCountry, @CodeCountry, @Capital, @Area, @Population, @Region)");
                SqlCommand insertCountry = new SqlCommand(insertquery, sqlConnection);
                insertCountry.Parameters.AddWithValue("NameCountry", textBox1.Text);
                insertCountry.Parameters.AddWithValue("CodeCountry", Code);
                insertCountry.Parameters.AddWithValue("Capital", Convert.ToInt32(idCity));
                insertCountry.Parameters.AddWithValue("Area", Convert.ToDecimal(textBox4.Text));
                insertCountry.Parameters.AddWithValue("Population", Convert.ToInt32(textBox5.Text));
                insertCountry.Parameters.AddWithValue("Region", Convert.ToInt32(idRegion));

                try
                {
                    await insertCountry.ExecuteNonQueryAsync();

                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }            
        }       

            private void Button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
