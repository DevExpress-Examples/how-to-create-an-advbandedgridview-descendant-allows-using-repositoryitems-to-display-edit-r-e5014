using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.BandedGrid;

namespace GridView_RowPreview
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Users myUsers = new Users();
        private void Form1_Load(object sender, EventArgs e)
        {
            myUsers.Add(new User("Antuan", "Acapulco", 23));
            myUsers[0].About = "Acapulco (Officially known as Acapulco de Juárez) is a city, and major sea port in the state of Guerrero on the Pacific coast of Mexico, 300 kilometres (190 mi) southwest from Mexico City";
            myUsers.Add(new User("Bill", "Brussels", 17));
            myUsers[1].About = "Brussels is the de facto capital city of the European Union (EU) and the largest urban area in Belgium.[6][7] It comprises 19 municipalities, including the City of Brussels proper, which is the capital of Belgium, Flanders and the French Community of Belgium.";
            myUsers.Add(new User("Charli", "Chicago", 45));
            myUsers[2].About = "Chicago is the largest city in the U.S. state of Illinois, and with more than 2.8 million people, the 3rd largest city in the United States";
            myUsers.Add(new User("Denn", "Denver", 20));
            myUsers[3].About = "Denver";
            myUsers.Add(new User("Eva", "Ernakulam", 23));
            myUsers[4].About = "The name 'Ernakulam' is derived from the name of a very famous temple of Lord Shiva called the Ernakulathappan Temple";
            customGridControl1.DataSource = myUsers;
            BandedGridColumn gridColumn1 = new BandedGridColumn();
            BandedGridColumn gridColumn2 = new BandedGridColumn();
            BandedGridColumn gridColumn3 = new BandedGridColumn();

            gridColumn1.FieldName = "Name";
            gridColumn1.Caption = "Name";
            gridColumn2.FieldName = "City";
            gridColumn2.Caption = "City";
            gridColumn3.FieldName = "Age";
            gridColumn3.Caption = "Age";
            customGridView1.Columns.Add(gridColumn1);
            customGridView1.Columns.Add(gridColumn2);
            customGridView1.Columns.Add(gridColumn3);
            customGridView1.OptionsView.ShowPreview = true;
            customGridView1.PreviewFieldName = "About";
        }
    }
    public class User
    {
        string name, city, about;
        int age;
        public User(string name, string city, int age)
        {
            this.name = name;
            this.city = city;
            this.age = age;
            this.about = String.Empty;
        }
        public int Age { set { age = value; } get { return age; } }
        public string Name { set { name = value; } get { return name; } }
        public string City { set { city = value; } get { return city; } }
        public string About { get { return about; } set { if (About != value) about = value; } }
    }
    public class Users : ArrayList
    {
        public new virtual User this[int index] { get { return (User)base[index]; } set { base[index] = value; } }
    }
}