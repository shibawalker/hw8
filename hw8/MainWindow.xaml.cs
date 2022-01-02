using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;
using Newtonsoft.Json;

namespace hw8
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow
    {
        List<Teacher> teachers = new List<Teacher>();//所有老師的List
        List<Student> students = new List<Student>();//所有學生的List
        List<Record> records = new List<Record>();//所有選課紀錄的List
        List<Course> courses = new List<Course>();//所有課程的List
        Teacher selectedTeacher = null; //選取的老師
        Course selectedCourse = null;//選取的課程
        Student selectedStudent = null;//選取的學生
        Record selectedRecord = null;//選取的選課紀錄

        public MainWindow()
        {
            InitializeComponent();


            Teacher teacher1 = new Teacher() { TeacherName = "陳定宏" };
            teacher1.Courses.Add(new Course(teacher1) { CourseName = "視窗程式設計", Type = "選修", Point = 3, OpeningClass = "五專三甲" });
            teacher1.Courses.Add(new Course(teacher1) { CourseName = "視窗程式設計", Type = "選修", Point = 3, OpeningClass = "四技資工二甲" });
            teacher1.Courses.Add(new Course(teacher1) { CourseName = "視窗程式設計", Type = "選修", Point = 3, OpeningClass = "四季資工二乙" });
            teachers.Add(teacher1);

            Teacher teacher2 = new Teacher() { TeacherName = "陳福坤" };
            teacher2.Courses.Add(new Course(teacher2) { CourseName = "計算機概論", Type = "必修", Point = 3, OpeningClass = "四資工一丙" });
            teacher2.Courses.Add(new Course(teacher2) { CourseName = "數位系統導論(A)", Type = "管制必修", Point = 3, OpeningClass = "四技資工一甲等合開" });
            teacher2.Courses.Add(new Course(teacher2) { CourseName = "視窗程式設計", Type = "管制必修", Point = 3, OpeningClass = "四技資工一甲等合開" });
            teachers.Add(teacher2);

            Teacher teacher3 = new Teacher() { TeacherName = "鄭錦楸" };
            teacher3.Courses.Add(new Course(teacher3) { CourseName = "導師時間", Type = "必修", Point = 0, OpeningClass = "五專資工三甲" });
            teacher3.Courses.Add(new Course(teacher3) { CourseName = "區塊鏈技術", Type = "選修", Point = 3, OpeningClass = "四技資工三甲" });
            teacher3.Courses.Add(new Course(teacher3) { CourseName = "網路攻防技術", Type = "選修", Point = 3, OpeningClass = "四技資工三甲" });
            teachers.Add(teacher3);

            trvTeacher.ItemsSource = teachers;
            //將教師所授課的所有課程毒入COURSE內
            foreach (Teacher teacher in teachers)
            {
                foreach (Course course in teacher.Courses)
                {
                    courses.Add(course);
                }
            }

            lbCourse.ItemsSource = courses;
            lvRegisRecord.Items.Clear();

            OpenFileDialog studenetFileDialog = new OpenFileDialog();
            studenetFileDialog.Title = "讀取學生資料";
            studenetFileDialog.Filter = "Json Files(*.json)|*.json|All files|*.*";
            studenetFileDialog.DefaultExt = "*.json";
            if (studenetFileDialog.ShowDialog() == true)
            {
                string filename = studenetFileDialog.FileName;
                string json = File.ReadAllText(filename);
                students = JsonConvert.DeserializeObject<List<Student>>(json);
            }
            cmbStudent.ItemsSource = students;

        }


        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //MessageBox.Show("Open a file");
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "儲存選課紀錄";
            saveFileDialog.Filter = "Json File (*.json)|*.json|All Files|*.*";
            saveFileDialog.DefaultExt = ".json";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                JsonSerializer serializer = new JsonSerializer();
                serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                serializer.Formatting = Formatting.Indented;
                using (StreamWriter sw = new StreamWriter(filename))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, records);
                }
            }
        }
        private void TrvTeacher_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (trvTeacher.SelectedItem is Course)
            {
                selectedCourse = trvTeacher.SelectedItem as Course;
                selectedTeacher = selectedCourse.Tutor;

                statusLabel.Content = selectedTeacher.ToString() + " " + selectedCourse.ToString();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            //selectedCourse = trvTeacher.SelectedItem as Course;
            //selectedTeacher = selectedCourse.Tutor;
            //selectedStudent = cmbStudent.SelectedItem as Student;

            if (selectedStudent != null && selectedCourse != null)
            {
                Record currentRecord = new Record()
                {
                    SelectedStudent = selectedStudent,
                    TeacherName = selectedTeacher.TeacherName,
                    SelectedCourse = selectedCourse
                };
                foreach (Record r in records)
                {
                    if (r.Equals(currentRecord))
                    {
                        MessageBox.Show($"{selectedStudent.StudentName}已經選過{selectedCourse.CourseName}了，請重新選擇未選過的課程。");
                        return;
                    }
                }
                records.Add(currentRecord);
                lvRegisRecord.ItemsSource = records;
                lvRegisRecord.Items.Refresh();
            }
            else MessageBox.Show("請先選擇學生或課程");
        }

        private void CmbStudent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedStudent = cmbStudent.SelectedItem as Student;
            statusLabel.Content = selectedStudent.ToString();

        }

        private void lbCourse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCourse = lbCourse.SelectedItem as Course;
            selectedTeacher = selectedCourse.Tutor;
            statusLabel.Content = selectedCourse.ToString();
        }

        private void withdrawButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecord != null)
            {
                records.Remove(selectedRecord);
                lvRegisRecord.ItemsSource = records;
                lvRegisRecord.Items.Refresh();
            }
            else MessageBox.Show("請選擇要退選的選課紀錄");
        }

        private void lvRegisRecord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRecord = lvRegisRecord.SelectedItem as Record;
            //statusLabel.Content = lvRegisRecord.SelectedItem.ToString();
        }
    }
    public class Course
    {
        public string CourseName { get; set; }
        public string Type { get; set; }
        public int Point { get; set; }
        public string OpeningClass { get; set; }
        public Teacher Tutor { get; set; }
        public Course(Teacher tutor)
        {
            this.Tutor = tutor;
        }
        public override string ToString()
        {
            return $"課程名稱:{CourseName} {Type} {Point}學分 開課班級:{OpeningClass}";
        }
    }
    public class Teacher
    {
        public string TeacherName { get; set; }

        public ObservableCollection<Course> Courses { get; set; }

        public Teacher()
        {
            this.Courses = new ObservableCollection<Course>();
        }
        public override string ToString()
        {
            return $"{TeacherName}";
        }
    }
    public class Student
    {
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public override string ToString()
        {
            return $"{StudentID} {StudentName}";
        }
    }
    public class Record
    {
        public Student SelectedStudent { get; set; }
        public Course SelectedCourse { get; set; }
        public string TeacherName { get; set; }

        public bool Equals(Record r)
        {
            if (this.SelectedStudent.StudentID == r.SelectedStudent.StudentID && this.SelectedCourse.CourseName == r.SelectedCourse.CourseName)
                return true;
            else return false;
        }
        public override string ToString()
        {
            return $"{SelectedStudent}選課紀錄 -- {SelectedCourse}";
        }
    }
}
