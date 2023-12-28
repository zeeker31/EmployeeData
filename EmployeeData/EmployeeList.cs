using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EmployeeData
{
    public partial class EmployeeList : Form
    {
        private SqlConnection conn;
        private bool IsEditMode { get; set; }

        public EmployeeList()
        {
            InitializeComponent();
            conn = new SqlConnection("Data Source=zareen\\SQLEXPRESS;Initial Catalog=EMPLOYEEDATAdb;Integrated Security=True");
            //DataGridViewCheckBoxColumn checkboxColumn = new DataGridViewCheckBoxColumn();
            //checkboxColumn.HeaderText = "Select";
            //checkboxColumn.Name = "checkBoxColumn";
            //dataGridViewEmployees.Columns.Add(checkboxColumn);
        }

        private void EmployeeList_Load(object sender, EventArgs e)
        {
            LoadEmployeeList();
        }

        private void LoadEmployeeList()
        {
            AddHeaderCheckBox();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (SqlCommand command = new SqlCommand("GetEmployeeList", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable employeeTable = new DataTable();
                    adapter.Fill(employeeTable);
                    dataGridViewEmployees.DataSource = employeeTable;
                    lblTotalRecords.Text = $"Total Records: {employeeTable.Rows.Count}";
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


        private void SearchEmployees(string searchTerm)
        {
            try
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("SearchEmployees", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SearchTerm", searchTerm);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable employeeTable = new DataTable();
                    adapter.Fill(employeeTable);
                    dataGridViewEmployees.DataSource = employeeTable;
                    lblTotalRecords.Text = $"Total Records: {employeeTable.Rows.Count}";
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

        private void DeleteEmployee(int employeeID)
        {
            try
            {
                // Display a confirmation dialog
                DialogResult result = MessageBox.Show("Are you sure you want to delete this employee?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // If the user clicks "Yes," proceed with deletion
                if (result == DialogResult.Yes)
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }

                    using (SqlCommand command = new SqlCommand("DeleteEmployee", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EmployeeID", employeeID);
                        command.ExecuteNonQuery();
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



        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                MessageBox.Show("Please enter a valid search term.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Exit the method if the search term is invalid
            }

            SearchEmployees(searchTerm);
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                

                foreach (DataGridViewRow row in dataGridViewEmployees.Rows)
                {
                    DataGridViewCheckBoxCell chk = row.Cells["checkBoxColumn"] as DataGridViewCheckBoxCell;

                    if (chk != null && chk.Value != null && (bool)chk.Value)
                    {
                        int id = Convert.ToInt32(row.Cells["EmployeeID"].Value);
                        DeleteEmployee(id);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

            // Reload the employee list after deletion
            LoadEmployeeList();
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            LoadEmployeeList();
        }

        private void ClearFields()
        {
            txtSearch.Text = string.Empty;
        }

        

        private void dataGridViewEmployees_CellClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // SetEmployeeValues(e);
            
        }

        private void dataGridViewEmployees_MouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SetEmployeeValues(e);
            
        }

        private void dataGridViewEmployees_MouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SetEmployeeValues(e);

            
        }

        private void SetEmployeeValues(DataGridViewCellMouseEventArgs e)
        {
            if (dataGridViewEmployees.Rows.Count > 0)
            {

                if (dataGridViewEmployees.SelectedRows.Count > 0)
                {
                    DataGridViewRow dr = dataGridViewEmployees.SelectedRows[0];
                    this.Hide();
                    LoadAddEmployee frmAddEMP = new LoadAddEmployee();
                    
                    frmAddEMP.lblEmployeeId.Text = dr.Cells["EmployeeID"].Value.ToString();
                    frmAddEMP.txtFirstName.Text = dr.Cells["FirstName"].Value.ToString();
                    frmAddEMP.txtLastName.Text = dr.Cells["LastName"].Value.ToString();
                    frmAddEMP.txtEmail.Text = dr.Cells["Email"].Value.ToString();
                    frmAddEMP.txtMobileNumber.Text = dr.Cells["Mobile"].Value.ToString();
                    frmAddEMP.txtRole.Text = dr.Cells["Role"].Value.ToString();
                    frmAddEMP.txtSalary.Text = dr.Cells["Salary"].Value.ToString();
                    frmAddEMP.txtAddress.Text = dr.Cells["Address"].Value.ToString();
                    frmAddEMP.dptDOB.Text = dr.Cells["DOB"].Value.ToString();
                    frmAddEMP.dptDOJ.Text = dr.Cells["DateOfJoining"].Value.ToString();
                    frmAddEMP.btnUpdate.Enabled = true;
                    frmAddEMP.ShowDialog();
                    
                    }
                else
                {
                   // MessageBox.Show("Please select a row before trying to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            }
        private void dataGridViewEmployees_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewEmployees.SelectedRows)
            {
                DataGridViewCheckBoxCell checkBoxCell = row.Cells["checkBoxColumn"] as DataGridViewCheckBoxCell;
                if (checkBoxCell != null && !Convert.ToBoolean(checkBoxCell.Value))
                {
                    // If the checkbox is not checked, check it
                    checkBoxCell.Value = true;
                }
            }
        }
        CheckBox HeaderCheckBox = null;
        private void AddHeaderCheckBox()
        {
            if (HeaderCheckBox == null)
            {
                HeaderCheckBox = new CheckBox();
                HeaderCheckBox.Size = new System.Drawing.Size(18, 18);
                HeaderCheckBox.Location = new System.Drawing.Point(19, 4);
                HeaderCheckBox.CheckedChanged += HeaderCheckBox_CheckedChanged;

                // Add checkbox column dynamically
                DataGridViewCheckBoxColumn checkboxColumn = new DataGridViewCheckBoxColumn();
                checkboxColumn.HeaderText = "Select";
                checkboxColumn.Name = "checkBoxColumn";
                dataGridViewEmployees.Columns.Insert(0, checkboxColumn);

                this.dataGridViewEmployees.Controls.Add(HeaderCheckBox);
            }
        }

        private void HeaderCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewEmployees.Rows)
            {
                DataGridViewCheckBoxCell checkBoxCell = row.Cells["checkBoxColumn"] as DataGridViewCheckBoxCell;
                if (checkBoxCell != null)
                {
                    checkBoxCell.Value = HeaderCheckBox.Checked;
                }
            }
        }



        private void dataGridViewEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            


            
        }
       

    }

}



