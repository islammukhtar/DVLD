using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD.DataAccessLayer;

namespace DVLD.BusinessLogic
{
    public class clsTestType
    {
        public enum enTestTypes
        { 
            VisionTest = 1,
            WrittenTest = 2,
            PracticalTest=3
        }

        enum enMode { AddNew, Update }
        enMode _Mode;
        public int ID { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Fees { get; set; }

        clsTestType(int id, string title, string description, decimal fees)
        {

            this.ID = id;
            this.Title = title;
            this.Description = description;
            this.Fees = fees;
            this._Mode = enMode.Update;
        }

        static public DataTable GetListTestTypes()
        {
            return clsTestTypeData.GetListTestTypes();
        }

        public static clsTestType FindTestTypeByID(int id)
        {

            string title = string.Empty, description = string.Empty;
            decimal fees = 0;
            if (clsTestTypeData.GetTestTypeInfoByID(id, ref title, ref description, ref fees))
            {
                return new clsTestType(id, title, description, fees);
            }
            else
            {
                return null;
            }


        }

        private bool _UpdateTestType()
        {
            return clsTestTypeData.UpdateTestType(this.ID, this.Title, this.Description, this.Fees);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.Update:
                    return _UpdateTestType();


            }
            return false;
        }
    }
}
