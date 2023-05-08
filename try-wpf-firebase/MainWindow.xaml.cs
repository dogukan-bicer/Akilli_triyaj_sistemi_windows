using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
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
            btSet.Click += btSet_Click;
            btPush.Click += btPush_Click;
            btGet.Click +=btGet_Click;
            btUpdate.Click += btUpdate_Click;
            btDelete.Click += btDelete_Click;
            btClose.Click += btClose_Click;
        }

        void btClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        async void btGet_Click(object sender, RoutedEventArgs e)
        {
            //string studentID = tbID.Text;

            //FirebaseResponse response = await _client.GetAsync("students/" + tbID.Text);

            //Student student = response.ResultAs<Student>();

            //lbResponse.Text = string.Format("Found {0} {1} {2} in realtime database",student.ID,student.Name,student.SureName);



            //FirebaseResponse response = await _client.GetAsync("Hasta_0/");

            //Patient patient = response.ResultAs<Patient>();

            //lbResponse.Text = string.Format("Found: \n ID: {0} \n BPM: {1} \n Ambient_Temp: {2} \n Object_Temp: {3}\n SpO2: {4}",
            //    patient.ID,
            //    patient.BPM,
            //    patient.Ambient_Temp,
            //    patient.Object_Temp,
            //    patient.SpO2);

            //FirebaseResponse response = await _client.GetAsync("");

            //dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body.ToString());

            //lbResponse.Text = data.ToString();

            FirebaseResponse response = await _client.GetAsync("");

            dynamic array = JsonConvert.DeserializeObject(response.Body.ToString());


            foreach (var site in (JObject)array)
            {
                patient_number.Add(site.Key.ToString());
               FirebaseResponse resp = await _client.GetAsync(patient_number[patient_number.Count-1]);
               var cls = new Patient();
               cls = resp.ResultAs<Patient>();
               patient_data.Add(cls);

            }

            string temp_str="";

            for (int i = 0; i < patient_number.Count ; i++)
            {
                temp_str= temp_str + string.Format("{5}: \n ID: {0} \n BPM: {1} \n Ambient_Temp: {2} \n Object_Temp: {3}\n SpO2: {4}",
                patient_data[i].ID,
                patient_data[i].BPM,
                patient_data[i].Ambient_Temp,
                patient_data[i].Object_Temp,
                patient_data[i].SpO2,
                patient_number[i]) + "\n";
                
            }


            lbResponse.Text = temp_str;

            //string.Join(",", patient_list);
        }
        async void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            var student = GetStudent();

            FirebaseResponse response = await _client.UpdateAsync("students/" + student.ID, student);

            lbResponse.Text = String.Format("Updated {0}",response.ResultAs<Student>().Name);
        }

        async void btPush_Click(object sender, RoutedEventArgs e)
        {
            var student = GetStudent();

            PushResponse response = await _client.PushAsync("students/", student);

            lbResponse.Text = response.Result.name;
        }

        async void btDelete_Click(object sender, RoutedEventArgs e)
        {
            FirebaseResponse response = await _client.DeleteAsync("students/" + tbID.Text);

            lbResponse.Text = response.StatusCode.ToString();
        }

        async void btSet_Click(object sender, RoutedEventArgs e)
        {

            var student = GetStudent();

            SetResponse response = await _client.SetAsync("students/"+student.ID,student);

            lbResponse.Text = String.Format("Added {0}",response.ResultAs<Student>().Name);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _client = new FirebaseClient(_config);
        }

        Student GetStudent()
        {
            return new Student(Convert.ToInt32(tbID.Text), tbName.Text, tbSername.Text);
        }

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
            public float Ambient_Temp { get; set; }
            public float Object_Temp { get; set; }
            public float SpO2 { get; set; }
            public float BPM { get; set; }
            public int ID { get; set; }

            public Patient()
            {

            }

            public Patient(int id, float bPM, float spO2 ,float object_Temp,float ambient_Temp) : this()
            {
                ID = id;
                BPM = bPM;
                SpO2 = spO2;

                Object_Temp = object_Temp;

                Ambient_Temp = ambient_Temp;
            }
        }
    }
}
