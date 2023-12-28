using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EmployeeData
{
    public partial class LoginPage : Form
    {
        private SqlConnection conn;

        public LoginPage()
        {
            InitializeComponent();
            conn = new SqlConnection("Data Source=zareen\\SQLEXPRESS;Initial Catalog=EMPLOYEEDATAdb;Integrated Security=True");
        }

        private bool AuthenticateAdmin(string username, string password)
        {
            try
            {
                using (SqlCommand command = new SqlCommand("ValidateAdmin", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;

                   
                    command.Parameters.Add("@Username", SqlDbType.VarChar).Value = username;
                    command.Parameters.Add("@Password", SqlDbType.VarChar).Value = password;

                    
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable resultTable = new DataTable();
                    adapter.Fill(resultTable);

                   
                    if (resultTable.Rows.Count > 0)
                    {
                        
                        int isValidAdmin = Convert.ToInt32(resultTable.Rows[0][0]);

                        
                        return isValidAdmin == 1;
                    }
                    else
                    {
                        
                        MessageBox.Show("Invalid result from the stored procedure.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"An error occurred: {ex.Message}");
                return false;
            }
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            
            if (AuthenticateAdmin(username, password))
            {
               // MessageBox.Show("Login successful!");

                
                MainForm form2 = new MainForm();
                form2.ShowDialog();

                
                this.Hide();
            }
            else
            {
                MessageBox.Show("Please Enter Valid Username or Password.");
            }
        }

    }
}
