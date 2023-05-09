using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MySchool_Firebase.MainWindow;

namespace MySchool_Firebase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private IFirebaseConfig _config;
        private IFirebaseClient _client;
        List<String> patient_number = new List<String>();
        List<Patient> patient_data = new List<Patient>();

        public MainWindow()
        {
            InitializeComponent();

            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "AIzaSyB8QNITQJlIS8pYEXEKF1KR421uR7-2kK0",
                BasePath = "https://akilli-triyaj-sistemi-default-rtdb.europe-west1.firebasedatabase.app/",
            };

            _config = config;

            this.Loaded += MainWindow_Loaded;
            //btSet.Click += btSet_Click;
            //btPush.Click += btPush_Click;
            btGet.Click +=btGet_Click;
            //btUpdate.Click += btUpdate_Click;
            //btDelete.Click += btDelete_Click;
            btClose.Click += btClose_Click;

            this.DataContext = this;
        }

        

        void btClose_Click(object sender, RoutedEventArgs e)
        {
            ColorRow(data_grid);


        }


        private void ColorRow(DataGrid dg)
        {
            for (int i = 1; i < dg.Items.Count; i++)
            {
                DataGridRow row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(i - 1);

                if (row != null)
                {
                    int index = row.GetIndex();
                    if (patient_data[i-1].SpO2 > 150)
                    {
                        SolidColorBrush brush = new SolidColorBrush(Colors.Black);
                        row.Background = brush;
                    }
                    else if (patient_data[i - 1].SpO2>100)
                    {
                        SolidColorBrush brush = new SolidColorBrush(Colors.Red);
                        row.Background = brush;
                    }
                    else if (patient_data[i - 1].SpO2 > 80)
                    {
                        SolidColorBrush brush = new SolidColorBrush(Colors.Yellow);
                        row.Background = brush;
                    }
                    else if (patient_data[i - 1].SpO2 > -1)
                    {
                        SolidColorBrush brush = new SolidColorBrush(Colors.Green);
                        row.Background = brush;
                    }
                }
            }
        }
        async void btGet_Click(object sender, RoutedEventArgs e)
        {
            FirebaseResponse response = await _client.GetAsync("");

            dynamic array = JsonConvert.DeserializeObject(response.Body.ToString());

            foreach (var site in (JObject)array)
            {
               patient_number.Add(site.Key.ToString());
               FirebaseResponse resp = await _client.GetAsync(patient_number[patient_number.Count-1]);
               var cls = new Patient();
               cls = resp.ResultAs<Patient>();
               cls.Patient_Name = patient_number[patient_number.Count - 1];
               patient_data.Add(cls);

            }

            data_grid.ItemsSource = patient_data;














            //data_grid.Columns[0].CellStyle = new Style(typeof(DataGridCell));
            //data_grid.Columns[0].CellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.PaleVioletRed)));

            //data_grid.RowStyle = new Style(typeof(DataGridRow));

            //data_grid.RowStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty,new SolidColorBrush(Colors.Red)));

            //data_grid.RowBackground= new SolidColorBrush(Colors.BlueViolet);









        }
        async void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            //var student = GetStudent();

            //FirebaseResponse response = await _client.UpdateAsync("students/" + student.ID, student);

           // lbResponse.Text = String.Format("Updated {0}",response.ResultAs<Student>().Name);
        }

        async void btPush_Click(object sender, RoutedEventArgs e)
        {
            //var student = GetStudent();

            //PushResponse response = await _client.PushAsync("students/", student);

           // lbResponse.Text = response.Result.name;
        }

        async void btDelete_Click(object sender, RoutedEventArgs e)
        {
            //FirebaseResponse response = await _client.DeleteAsync("students/" + tbID.Text);

            //lbResponse.Text = response.StatusCode.ToString();
        }

        async void btSet_Click(object sender, RoutedEventArgs e)
        {

            //var student = GetStudent();

            //SetResponse response = await _client.SetAsync("students/"+student.ID,student);

            //lbResponse.Text = String.Format("Added {0}",response.ResultAs<Student>().Name);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _client = new FirebaseClient(_config);
        }

        //Student GetStudent()
        //{
        //    //return new Student(Convert.ToInt32(tbID.Text), tbName.Text, tbSername.Text);
        //}

        public class Student
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string SureName { get; set; }

            public Student()
            {

            }

            public Student(int id,string name,string surename):this()
            {
                ID = id;
                Name = name;
                SureName = surename;
            }
        }

        public class Patient
        {
            public string Patient_Name { get; set; }
            public float SpO2 { get; set; }
            public float BPM { get; set; }
            public float Ambient_Temp { get; set; }
            public float Object_Temp { get; set; }
            public int ID { get; set; }

            public Patient()
            {

            }

            public Patient(int id, float bPM, float spO2 , float object_Temp, float ambient_Temp, string patient_Name) : this()
            {
                ID = id;
                BPM = bPM;
                SpO2 = spO2;

                Object_Temp = object_Temp;

                Ambient_Temp = ambient_Temp;

                Patient_Name = patient_Name;

            }
        }

        private void btClose_Copy_Click(object sender, RoutedEventArgs e)
        {
            data_grid.DataContext= null;
            data_grid.ItemsSource = null;
            patient_data.Clear();
            data_grid.Items.Refresh();
        }
    }
}
