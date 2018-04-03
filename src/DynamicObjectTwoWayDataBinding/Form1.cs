using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynamicObjectTwoWayDataBinding
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        dynamic data;
        private void Form1_Load(object sender, EventArgs e)
        {
            data = new MyCustomObject(this);

            data.StringProperty = "Some Text";
            data.BooleanProperty = true;
            data.DateTimeProperty = DateTime.Now.AddYears(10);
            data.IntegerProperty = 100;

            textBox1.DataBindings.Add("Text", data, "StringProperty", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBox1.DataBindings.Add("Checked", data, "BooleanProperty", true, DataSourceUpdateMode.OnPropertyChanged);
            dateTimePicker1.DataBindings.Add("Value", data, "DateTimeProperty", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDown1.DataBindings.Add("Value", data, "IntegerProperty", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            data.StringProperty = "Some Other Text";
            data.BooleanProperty = false;
            data.DateTimeProperty = DateTime.Now.AddYears(20);
            data.IntegerProperty = 200;
        }

    private void button2_Click(object sender, EventArgs e)
    {
        Task.Run(() =>
        {
            data.StringProperty = "Another Text";
            data.BooleanProperty = true;
            data.DateTimeProperty = DateTime.Now.AddYears(30);
            data.IntegerProperty = 300;
        });
    }
    }
}
