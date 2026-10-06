using System;
using System.Windows.Forms;
using DVLD.BusinessLogic;

namespace DVLD.Presentation.TestTypes
{
    public partial class frmUpdateTestType : Form
    {
        int _TestTypeID;

        clsTestType _TestType;

        public frmUpdateTestType(int TestTypeID)
        {
            InitializeComponent();
            _TestTypeID = TestTypeID;
        }

        private void frmUpdateTestType_Load(object sender, EventArgs e)
        {
            _TestType = clsTestType.FindTestTypeByID(_TestTypeID);

            if (_TestType == null)
            {
                MessageBox.Show($"Test Type with id {_TestTypeID} was not found");
                return;
            }

            lblTestTypeID.Text = _TestTypeID.ToString();
            txtTitle.Text = _TestType.Title.ToString();
            txtDescription.Text = _TestType.Description.ToString();
            txtFees.Text = _TestType.Fees.ToString();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private bool HasChanges(decimal fees)
        {
            return txtTitle.Text != _TestType.Title ||
                txtDescription.Text != _TestType.Description ||
                fees != _TestType.Fees;
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

            _TestType.Title = txtTitle.Text;
            _TestType.Description = txtDescription.Text;
            _TestType.Fees = fees;

            if (_TestType.Save())
            {
                MessageBox.Show("Data save successfully.");
                frmUpdateTestType_Load(null, null);//refresh
            }

        }

    }
}
