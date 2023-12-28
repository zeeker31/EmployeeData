using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EmployeeData
{
    public partial class LoadAddEmployee : Form
    {
        private SqlConnection conn;
        private bool IsEditMode { get; set; }
        private int EditEmployeeID { get; set; }

        public LoadAddEmployee()
        {
            InitializeComponent();
            conn = new SqlConnection("Data Source=zareen\\SQLEXPRESS;Initial Catalog=EMPLOYEEDATAdb;Integrated Security=True");
            btnClear.Click += btnClear_Click;
            LoadGenderComboBox();
            InitializeForm();
        }

        

        private void InitializeForm()
        {
            if (IsEditMode)
            {
                
                LoadEmployeeDetails(EditEmployeeID);
                btnInsert.Enabled = false; 
            }
            else
            {
                btnUpdate.Enabled = false; 
            }
        }

        private void LoadEmployeeDetails(int employeeID)
        {
            
            try
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("GetEmployeeList", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtFirstName.Text = reader["FirstName"].ToString();
                            txtLastName.Text = reader["LastName"].ToString();
                            txtSalary.Text = reader["Salary"].ToString();
                            txtAddress.Text = reader["Address"].ToString();
                            txtEmail.Text = reader["Email"].ToString();
                            txtMobileNumber.Text = reader["Mobile"].ToString();
                            txtRole.Text = reader["Role"].ToString();
                            dptDOB.Text = reader["DOB"].ToString();
                            dptDOJ.Text = reader["DateOfJoining"].ToString();
                            cmbGender.Text = reader["Gender"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                // If all validations pass, proceed with adding the employee
                decimal salary;

                if (decimal.TryParse(txtSalary.Text, out salary))
                {
                    string firstName = txtFirstName.Text;
                    string lastName = txtLastName.Text;
                    DateTime dob = dptDOB.Value;
                    string mobile = txtMobileNumber.Text;
                    string gender = cmbGender.SelectedItem.ToString();
                    DateTime dateOfJoining = dptDOJ.Value;
                    string email = txtEmail.Text;
                    string role = txtRole.Text;
                    string address = txtAddress.Text;

                    AddEmployee(firstName, lastName, salary, dob, mobile, gender, dateOfJoining, email, role, address);
                }
                else
                {
                    MessageBox.Show("Invalid salary value. Please enter a valid numeric value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            // Validate each textbox and show an error message if validation fails
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
       string.IsNullOrWhiteSpace(txtLastName.Text) ||
       string.IsNullOrWhiteSpace(txtSalary.Text) ||
       string.IsNullOrWhiteSpace(txtMobileNumber.Text) ||
       cmbGender.SelectedItem == null ||
       string.IsNullOrWhiteSpace(txtEmail.Text) ||
       string.IsNullOrWhiteSpace(txtRole.Text) ||
       string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Please fill in all the fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!DateTime.TryParse(dptDOB.Text, out _))
            {
                MessageBox.Show("Invalid Date of Birth. Please enter a valid date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            
            if (!DateTime.TryParse(dptDOJ.Text, out _))
            {
                MessageBox.Show("Invalid Date of Joining. Please enter a valid date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true; 
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            decimal salary;

            if (decimal.TryParse(txtSalary.Text, out salary))
            {
                DateTime dob;
                if (DateTime.TryParse(dptDOB.Text, out dob))
                {
                    DateTime dateOfJoining;
                    if (DateTime.TryParse(dptDOJ.Text, out dateOfJoining))
                    {
                        // Assuming you have controls for the new columns, such as txtMobile, cmbGender, txtEmail, txtRole, and txtAddress
                        string mobile = txtMobileNumber.Text;
                        string gender = cmbGender.SelectedItem?.ToString();
                        string email = txtEmail.Text;
                        string role = txtRole.Text;
                        string address = txtAddress.Text;


                        UpdateEmployee(Convert.ToInt32(lblEmployeeId.Text), txtFirstName.Text, txtLastName.Text, salary, dob, mobile, gender, dateOfJoining, email, role, address);
                    }
                    else
                    {
                        MessageBox.Show("Invalid Date of Joining. Please enter a valid date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Date of Birth. Please enter a valid date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid salary value. Please enter a valid numeric value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void AddEmployee(string firstName, string lastName, decimal salary, DateTime dob, string mobile, string gender, DateTime dateOfJoining, string email, string role, string address)
        {

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || salary == 0 || string.IsNullOrWhiteSpace(gender)
                || dob == DateTime.MinValue || string.IsNullOrWhiteSpace(mobile) || dateOfJoining == DateTime.MinValue
                || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Please fill in all the fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dob > DateTime.Now)
            {
                MessageBox.Show("Invalid Date of Birth. Please select a date in the past.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dateOfJoining > DateTime.Now)
            {
                MessageBox.Show("Invalid Date of Joining. Please select a date on or before today.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlCommand command = new SqlCommand("AddEmployee", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Salary", salary);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@DOB", dob);
                    command.Parameters.AddWithValue("@Mobile", mobile);
                    command.Parameters.AddWithValue("@DateOfJoining", dateOfJoining);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@Address", address);

                    conn.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    conn.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Employee added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("No rows affected. Employee may not have been added.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateEmployee(int employeeID, string firstName, string lastName, decimal salary, DateTime dob, string mobile, string gender, DateTime dateOfJoining, string email, string role, string address)
        {
            try
            {
                using (SqlCommand command = new SqlCommand("UpdateEmployee", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Salary", salary);
                    command.Parameters.AddWithValue("@DOB", dob);
                    command.Parameters.AddWithValue("@Mobile", mobile);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@DateOfJoining", dateOfJoining);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@Address", address);

                    conn.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    conn.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Employee updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No rows affected. Employee may not have been updated.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            lblEmployeeId.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtSalary.Text = string.Empty;
            txtMobileNumber.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtRole.Text = string.Empty;
            cmbGender.SelectedIndex = 0;
            dptDOB.Text = string.Empty;
            dptDOJ.Text = string.Empty;
        }

        private void LoadGenderComboBox()
        {
            cmbGender.Items.Add("Male");
            cmbGender.Items.Add("Female");
            cmbGender.SelectedIndex = 0;
        }
    }
}

