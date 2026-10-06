using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLD.BusinessLogic;

namespace DVLD.Presentation.ApplicationTypes
{
    public partial class frmUpdateApplicationType : Form
    {
        int _ApplicationTypeID;
        clsApplicationType _ApplicationType;
        public frmUpdateApplicationType(int ApplicationTypeID)
        {
            InitializeComponent();
            _ApplicationTypeID = ApplicationTypeID;
        }

        private void _LoadData()
        {
            _ApplicationType = clsApplicationType.FindApplicationTypeByID(_ApplicationTypeID);
            if (_ApplicationType != null)
            {
                lblApplicationTypeID.Text = _ApplicationType.ID.ToString();
                txtTitle.Text = _ApplicationType.Title.ToString();
                txtFees.Text = _ApplicationType.Fees.ToString();
            }
        }
        private void frmUpdateApplicationTypes_Load(object sender, EventArgs e)
        {
            _LoadData();
        }
        private bool HasChanges(decimal fees)
        {
            return txtTitle.Text.Trim() != _ApplicationType.Title ||
                   fees != _ApplicationType.Fees;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtFees.Text, out decimal fees))
                return;

            if (!HasChanges(fees))
            {
                MessageBox.Show("No changes have been made.");
                return;
            }

            _ApplicationType.Title = txtTitle.Text.Trim();
            _ApplicationType.Fees = fees;

            if (_ApplicationType.UpdateApplicationType())
            {
                MessageBox.Show("Data saved successfully.");
                _LoadData();
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
