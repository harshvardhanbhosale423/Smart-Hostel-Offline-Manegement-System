using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SmartHostelApp
{
    public class Student
    {
        public string Id;
        public string Name;
        public string Room;
        public string Mobile;
        public string Department;
        public string Year;

        // Converts student data to a line for text storage.
        public string ToLine()
        {
            return string.Join("|", Id, Name, Room, Mobile, Department, Year);
        }

        // Builds a student from a stored line.
        public static Student FromLine(string line)
        {
            var p = line.Split('|');
            return p.Length == 6 ? new Student { Id = p[0], Name = p[1], Room = p[2], Mobile = p[3], Department = p[4], Year = p[5] } : null;
        }
    }

    public class LogRecord
    {
        public string StudentId;
        public string Date;
        public string Time;

        // Converts log data to a line for text storage.
        public string ToLine()
        {
            return string.Join("|", StudentId, Date, Time);
        }

        // Builds a log record from a stored line.
        public static LogRecord FromLine(string line)
        {
            var p = line.Split('|');
            return p.Length == 3 ? new LogRecord { StudentId = p[0], Date = p[1], Time = p[2] } : null;
        }
    }

    public class VisitorRecord
    {
        public string Name;
        public string StudentId;
        public string Mobile;
        public string EntryDate;
        public string EntryTime;
        public string ExitTime;

        // Converts visitor data to a line for text storage.
        public string ToLine()
        {
            return string.Join("|", Name, StudentId, Mobile, EntryDate, EntryTime, ExitTime);
        }

        // Builds a visitor record from a stored line.
        public static VisitorRecord FromLine(string line)
        {
            var p = line.Split('|');
            return p.Length == 6 ? new VisitorRecord { Name = p[0], StudentId = p[1], Mobile = p[2], EntryDate = p[3], EntryTime = p[4], ExitTime = p[5] } : null;
        }
    }

    public static class Store
    {
        public static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string StudentsFile = Path.Combine(BaseDir, "students.txt");
        public static readonly string EntryFile = Path.Combine(BaseDir, "entrylog.txt");
        public static readonly string ExitFile = Path.Combine(BaseDir, "exitlog.txt");
        public static readonly string VisitorsFile = Path.Combine(BaseDir, "visitors.txt");
        public static readonly string ExportFile = Path.Combine(BaseDir, "export_report.txt");

        // Ensures that all required data files exist.
        public static void EnsureFiles()
        {
            if (!File.Exists(StudentsFile))
            {
                File.WriteAllLines(StudentsFile, new[]
                {
                    "S1001|Aarav Sharma|A-101|9876543210|Computer Science|2",
                    "S1002|Meera Nair|B-204|9123456780|Electronics|3",
                    "S1003|Rahul Verma|C-312|9988776655|Mechanical|1"
                });
            }

            if (!File.Exists(EntryFile))
            {
                File.WriteAllLines(EntryFile, new[]
                {
                    "S1001|2026-07-25|08:15:00",
                    "S1002|2026-07-25|22:30:00"
                });
            }

            if (!File.Exists(ExitFile))
            {
                File.WriteAllLines(ExitFile, new[]
                {
                    "S1001|2026-07-25|17:45:00"
                });
            }

            if (!File.Exists(VisitorsFile))
            {
                File.WriteAllLines(VisitorsFile, new[]
                {
                    "Anita Sharma|S1001|9000011111|2026-07-25|10:30:00|11:15:00"
                });
            }
        }

        // Loads all students.
        public static List<Student> LoadStudents()
        {
            EnsureFiles();
            return File.ReadAllLines(StudentsFile).Where(x => x.Trim().Length > 0).Select(Student.FromLine).Where(x => x != null).ToList();
        }

        // Saves all students.
        public static void SaveStudents(List<Student> students)
        {
            File.WriteAllLines(StudentsFile, students.Select(s => s.ToLine()).ToArray());
        }

        // Loads student entry or exit logs.
        public static List<LogRecord> LoadLogs(string path)
        {
            EnsureFiles();
            return File.ReadAllLines(path).Where(x => x.Trim().Length > 0).Select(LogRecord.FromLine).Where(x => x != null).ToList();
        }

        // Appends one log record.
        public static void AppendLog(string path, LogRecord record)
        {
            File.AppendAllText(path, record.ToLine() + Environment.NewLine);
        }

        // Loads visitors.
        public static List<VisitorRecord> LoadVisitors()
        {
            EnsureFiles();
            return File.ReadAllLines(VisitorsFile).Where(x => x.Trim().Length > 0).Select(VisitorRecord.FromLine).Where(x => x != null).ToList();
        }

        // Saves visitors.
        public static void SaveVisitors(List<VisitorRecord> visitors)
        {
            File.WriteAllLines(VisitorsFile, visitors.Select(v => v.ToLine()).ToArray());
        }
    }

    public class LoginForm : Form
    {
        TextBox userBox;
        TextBox passBox;
        int attempts;

        // Builds the login window.
        public LoginForm()
        {
            Text = "Smart Hostel Login";
            Width = 460;
            Height = 300;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(245, 247, 250);

            var title = new Label { Text = "Smart Hostel Entry & Exit Management System", Font = new Font("Segoe UI", 15, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Left = 20, Top = 20, Width = 400, Height = 42 };
            Controls.Add(title);

            Controls.Add(new Label { Text = "Username", Left = 60, Top = 82, Width = 100 });
            userBox = new TextBox { Left = 165, Top = 79, Width = 200 };
            Controls.Add(userBox);

            Controls.Add(new Label { Text = "Password", Left = 60, Top = 122, Width = 100 });
            passBox = new TextBox { Left = 165, Top = 119, Width = 200, PasswordChar = '*' };
            Controls.Add(passBox);

            var login = new Button { Text = "Login", Left = 165, Top = 170, Width = 90 };
            login.Click += Login_Click;
            Controls.Add(login);

            var exit = new Button { Text = "Exit", Left = 275, Top = 170, Width = 90 };
            exit.Click += (s, e) => Close();
            Controls.Add(exit);

            AcceptButton = login;
        }

        // Checks admin credentials with three tries.
        void Login_Click(object sender, EventArgs e)
        {
            if (userBox.Text == "admin" && passBox.Text == "admin123")
            {
                Hide();
                var main = new MainForm();
                main.FormClosed += (s, a) => Close();
                main.Show();
                return;
            }

            attempts++;
            if (attempts >= 3)
            {
                MessageBox.Show("Maximum login attempts reached.");
                Close();
                return;
            }

            MessageBox.Show("Invalid login. Attempts left: " + (3 - attempts));
            passBox.Clear();
            passBox.Focus();
        }
    }

    public class MainForm : Form
    {
        TabControl tabs;
        DataGridView studentsGrid, entryGrid, exitGrid, searchGrid, visitorsGrid;
        TextBox idBox, nameBox, roomBox, mobileBox, deptBox, yearBox;
        TextBox entryIdBox, searchBox, monthBox, visitorNameBox, visitorStudentBox, visitorMobileBox;
        TextBox reportBox;
        Label statsLabel;

        // Builds the main dashboard.
        public MainForm()
        {
            Store.EnsureFiles();
            Text = "Smart Hostel Entry & Exit Management System";
            Width = 1220;
            Height = 780;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1040, 680);

            tabs = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tabs);

            BuildDashboardTab();
            BuildStudentsTab();
            BuildEntryExitTab();
            BuildSearchTab();
            BuildReportsTab();
            BuildVisitorsTab();

            RefreshAll();
        }

        // Creates a simple button.
        Button MakeButton(string text, int x, int y, int w, int h)
        {
            return new Button { Text = text, Left = x, Top = y, Width = w, Height = h, BackColor = Color.FromArgb(32, 96, 160), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        }

        // Creates a text field and label pair.
        TextBox AddField(Control parent, string label, int x, int y)
        {
            parent.Controls.Add(new Label { Text = label, Left = x, Top = y + 3, Width = 120 });
            var box = new TextBox { Left = x + 130, Top = y, Width = 180 };
            parent.Controls.Add(box);
            return box;
        }

        // Creates a grid with common settings.
        DataGridView MakeGrid()
        {
            return new DataGridView { ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
        }

        // Adds a section title.
        Label AddTitle(Control parent, string text, int x, int y)
        {
            var lbl = new Label { Text = text, Font = new Font("Segoe UI", 13, FontStyle.Bold), Left = x, Top = y, Width = 240, Height = 28 };
            parent.Controls.Add(lbl);
            return lbl;
        }

        // Dashboard tab.
        void BuildDashboardTab()
        {
            var page = new TabPage("Dashboard");
            tabs.TabPages.Add(page);

            var title = new Label
            {
                Text = "SMART HOSTEL ENTRY & EXIT MANAGEMENT SYSTEM",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Left = 30,
                Top = 28,
                Width = 1120,
                Height = 48,
                ForeColor = Color.FromArgb(22, 53, 83)
            };
            page.Controls.Add(title);

            statsLabel = new Label
            {
                Left = 270,
                Top = 115,
                Width = 660,
                Height = 250,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 14),
                BackColor = Color.White
            };
            page.Controls.Add(statsLabel);

            var refresh = MakeButton("Refresh Statistics", 490, 390, 230, 40);
            refresh.Click += (s, e) => RefreshAll();
            page.Controls.Add(refresh);
        }

        // Student tab.
        void BuildStudentsTab()
        {
            var page = new TabPage("Students");
            tabs.TabPages.Add(page);

            idBox = AddField(page, "Student ID", 30, 30);
            nameBox = AddField(page, "Full Name", 30, 70);
            roomBox = AddField(page, "Room No.", 30, 110);
            mobileBox = AddField(page, "Mobile", 30, 150);
            deptBox = AddField(page, "Department", 30, 190);
            yearBox = AddField(page, "Year", 30, 230);

            var add = MakeButton("Register", 160, 280, 100, 34);
            add.Click += AddStudent_Click;
            page.Controls.Add(add);

            var update = MakeButton("Update", 270, 280, 100, 34);
            update.Click += UpdateStudent_Click;
            page.Controls.Add(update);

            var delete = MakeButton("Delete", 380, 280, 100, 34);
            delete.Click += DeleteStudent_Click;
            page.Controls.Add(delete);

            var clear = MakeButton("Clear", 490, 280, 100, 34);
            clear.Click += (s, e) => ClearStudentFields();
            page.Controls.Add(clear);

            studentsGrid = MakeGrid();
            studentsGrid.Left = 30;
            studentsGrid.Top = 335;
            studentsGrid.Width = 1120;
            studentsGrid.Height = 340;
            studentsGrid.CellClick += StudentsGrid_CellClick;
            page.Controls.Add(studentsGrid);
        }

        // Entry / exit tab.
        void BuildEntryExitTab()
        {
            var page = new TabPage("Entry / Exit");
            tabs.TabPages.Add(page);

            entryIdBox = AddField(page, "Student ID", 30, 30);

            var markEntry = MakeButton("Mark Entry", 360, 28, 120, 34);
            markEntry.Click += MarkEntry_Click;
            page.Controls.Add(markEntry);

            var markExit = MakeButton("Mark Exit", 495, 28, 120, 34);
            markExit.Click += MarkExit_Click;
            page.Controls.Add(markExit);

            AddTitle(page, "Entry History", 30, 84);
            entryGrid = MakeGrid();
            entryGrid.Left = 30;
            entryGrid.Top = 118;
            entryGrid.Width = 540;
            entryGrid.Height = 520;
            page.Controls.Add(entryGrid);

            AddTitle(page, "Exit History", 595, 84);
            exitGrid = MakeGrid();
            exitGrid.Left = 595;
            exitGrid.Top = 118;
            exitGrid.Width = 540;
            exitGrid.Height = 520;
            page.Controls.Add(exitGrid);
        }

        // Search tab.
        void BuildSearchTab()
        {
            var page = new TabPage("Search");
            tabs.TabPages.Add(page);

            searchBox = AddField(page, "Search Text", 30, 32);

            var byId = MakeButton("Search by ID", 360, 30, 140, 34);
            byId.Click += SearchById_Click;
            page.Controls.Add(byId);

            var byName = MakeButton("Search by Name", 515, 30, 150, 34);
            byName.Click += SearchByName_Click;
            page.Controls.Add(byName);

            searchGrid = MakeGrid();
            searchGrid.Left = 30;
            searchGrid.Top = 90;
            searchGrid.Width = 1120;
            searchGrid.Height = 545;
            page.Controls.Add(searchGrid);
        }

        // Reports tab.
        void BuildReportsTab()
        {
            var page = new TabPage("Reports");
            tabs.TabPages.Add(page);

            var daily = MakeButton("Daily Report", 30, 28, 130, 34);
            daily.Click += (s, e) => ShowDailyReport();
            page.Controls.Add(daily);

            monthBox = new TextBox { Left = 180, Top = 31, Width = 100, Text = DateTime.Now.ToString("yyyy-MM") };
            page.Controls.Add(monthBox);

            var monthly = MakeButton("Monthly Report", 295, 28, 145, 34);
            monthly.Click += (s, e) => ShowMonthlyReport();
            page.Controls.Add(monthly);

            var late = MakeButton("Late Entry Report", 455, 28, 165, 34);
            late.Click += (s, e) => ShowLateReport();
            page.Controls.Add(late);

            var export = MakeButton("Export TXT", 635, 28, 120, 34);
            export.Click += ExportReport_Click;
            page.Controls.Add(export);

            reportBox = new TextBox
            {
                Left = 30,
                Top = 82,
                Width = 1120,
                Height = 560,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 11)
            };
            page.Controls.Add(reportBox);
        }

        // Visitors tab.
        void BuildVisitorsTab()
        {
            var page = new TabPage("Visitors");
            tabs.TabPages.Add(page);

            visitorNameBox = AddField(page, "Visitor Name", 30, 30);
            visitorStudentBox = AddField(page, "Student ID", 30, 70);
            visitorMobileBox = AddField(page, "Mobile", 30, 110);

            var add = MakeButton("Add Visitor Entry", 360, 30, 165, 34);
            add.Click += AddVisitor_Click;
            page.Controls.Add(add);

            var exit = MakeButton("Mark Visitor Exit", 540, 30, 160, 34);
            exit.Click += MarkVisitorExit_Click;
            page.Controls.Add(exit);

            visitorsGrid = MakeGrid();
            visitorsGrid.Left = 30;
            visitorsGrid.Top = 170;
            visitorsGrid.Width = 1120;
            visitorsGrid.Height = 470;
            page.Controls.Add(visitorsGrid);
        }

        // Refreshes all tables and stats.
        void RefreshAll()
        {
            studentsGrid.DataSource = Store.LoadStudents().Select(s => new { Student_ID = s.Id, Full_Name = s.Name, Room = s.Room, Mobile = s.Mobile, Department = s.Department, Year = s.Year }).ToList();
            entryGrid.DataSource = Store.LoadLogs(Store.EntryFile).Select(r => new { Student_ID = r.StudentId, Date = r.Date, Time = r.Time }).ToList();
            exitGrid.DataSource = Store.LoadLogs(Store.ExitFile).Select(r => new { Student_ID = r.StudentId, Date = r.Date, Time = r.Time }).ToList();
            searchGrid.DataSource = new List<object>();
            visitorsGrid.DataSource = Store.LoadVisitors().Select(v => new { Visitor_Name = v.Name, Student_ID = v.StudentId, Mobile = v.Mobile, Entry_Date = v.EntryDate, Entry_Time = v.EntryTime, Exit_Time = v.ExitTime }).ToList();

            var today = DateTime.Now.ToString("yyyy-MM-dd");
            statsLabel.Text =
                "  Total Students  : " + Store.LoadStudents().Count + Environment.NewLine + Environment.NewLine +
                "  Today's Entries : " + Store.LoadLogs(Store.EntryFile).Count(x => x.Date == today) + Environment.NewLine + Environment.NewLine +
                "  Today's Exits   : " + Store.LoadLogs(Store.ExitFile).Count(x => x.Date == today) + Environment.NewLine + Environment.NewLine +
                "  Visitors Count  : " + Store.LoadVisitors().Count(x => x.EntryDate == today);
        }

        // Clears student fields.
        void ClearStudentFields()
        {
            idBox.Clear();
            nameBox.Clear();
            roomBox.Clear();
            mobileBox.Clear();
            deptBox.Clear();
            yearBox.Clear();
        }

        // Selects a student row.
        void StudentsGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = studentsGrid.Rows[e.RowIndex];
            idBox.Text = Convert.ToString(row.Cells["Student_ID"].Value);
            nameBox.Text = Convert.ToString(row.Cells["Full_Name"].Value);
            roomBox.Text = Convert.ToString(row.Cells["Room"].Value);
            mobileBox.Text = Convert.ToString(row.Cells["Mobile"].Value);
            deptBox.Text = Convert.ToString(row.Cells["Department"].Value);
            yearBox.Text = Convert.ToString(row.Cells["Year"].Value);
        }

        // Adds a student.
        void AddStudent_Click(object sender, EventArgs e)
        {
            var students = Store.LoadStudents();
            if (string.IsNullOrWhiteSpace(idBox.Text))
            {
                MessageBox.Show("Student ID is required.");
                return;
            }
            if (students.Any(s => s.Id == idBox.Text.Trim()))
            {
                MessageBox.Show("Student already exists.");
                return;
            }

            students.Add(new Student
            {
                Id = idBox.Text.Trim(),
                Name = nameBox.Text.Trim(),
                Room = roomBox.Text.Trim(),
                Mobile = mobileBox.Text.Trim(),
                Department = deptBox.Text.Trim(),
                Year = yearBox.Text.Trim()
            });
            Store.SaveStudents(students);
            ClearStudentFields();
            RefreshAll();
            MessageBox.Show("Student registered successfully.");
        }

        // Updates a student.
        void UpdateStudent_Click(object sender, EventArgs e)
        {
            var students = Store.LoadStudents();
            var s = students.FirstOrDefault(x => x.Id == idBox.Text.Trim());
            if (s == null)
            {
                MessageBox.Show("Student not found.");
                return;
            }
            s.Name = nameBox.Text.Trim();
            s.Room = roomBox.Text.Trim();
            s.Mobile = mobileBox.Text.Trim();
            s.Department = deptBox.Text.Trim();
            s.Year = yearBox.Text.Trim();
            Store.SaveStudents(students);
            RefreshAll();
            MessageBox.Show("Student updated successfully.");
        }

        // Deletes a student.
        void DeleteStudent_Click(object sender, EventArgs e)
        {
            var students = Store.LoadStudents();
            var s = students.FirstOrDefault(x => x.Id == idBox.Text.Trim());
            if (s == null)
            {
                MessageBox.Show("Student not found.");
                return;
            }
            students.Remove(s);
            Store.SaveStudents(students);
            ClearStudentFields();
            RefreshAll();
            MessageBox.Show("Student deleted successfully.");
        }

        // Marks entry.
        void MarkEntry_Click(object sender, EventArgs e)
        {
            string id = entryIdBox.Text.Trim();
            if (!Store.LoadStudents().Any(s => s.Id == id))
            {
                MessageBox.Show("Student not found.");
                return;
            }
            Store.AppendLog(Store.EntryFile, new LogRecord { StudentId = id, Date = DateTime.Now.ToString("yyyy-MM-dd"), Time = DateTime.Now.ToString("HH:mm:ss") });
            entryIdBox.Clear();
            RefreshAll();
            MessageBox.Show("Entry marked successfully.");
        }

        // Marks exit.
        void MarkExit_Click(object sender, EventArgs e)
        {
            string id = entryIdBox.Text.Trim();
            if (!Store.LoadStudents().Any(s => s.Id == id))
            {
                MessageBox.Show("Student not found.");
                return;
            }
            Store.AppendLog(Store.ExitFile, new LogRecord { StudentId = id, Date = DateTime.Now.ToString("yyyy-MM-dd"), Time = DateTime.Now.ToString("HH:mm:ss") });
            entryIdBox.Clear();
            RefreshAll();
            MessageBox.Show("Exit marked successfully.");
        }

        // Searches by ID.
        void SearchById_Click(object sender, EventArgs e)
        {
            string q = searchBox.Text.Trim();
            searchGrid.DataSource = Store.LoadStudents().Where(s => s.Id.Equals(q, StringComparison.OrdinalIgnoreCase)).Select(s => new { Student_ID = s.Id, Full_Name = s.Name, Room = s.Room, Mobile = s.Mobile, Department = s.Department, Year = s.Year }).ToList();
        }

        // Searches by name.
        void SearchByName_Click(object sender, EventArgs e)
        {
            string q = searchBox.Text.Trim().ToLower();
            searchGrid.DataSource = Store.LoadStudents().Where(s => s.Name.ToLower().Contains(q)).Select(s => new { Student_ID = s.Id, Full_Name = s.Name, Room = s.Room, Mobile = s.Mobile, Department = s.Department, Year = s.Year }).ToList();
        }

        // Creates daily report text.
        void ShowDailyReport()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            reportBox.Text =
                "DAILY REPORT (" + today + ")" + Environment.NewLine +
                "--------------------------" + Environment.NewLine +
                "Student Entries : " + Store.LoadLogs(Store.EntryFile).Count(x => x.Date == today) + Environment.NewLine +
                "Student Exits   : " + Store.LoadLogs(Store.ExitFile).Count(x => x.Date == today) + Environment.NewLine +
                "Visitors        : " + Store.LoadVisitors().Count(x => x.EntryDate == today);
        }

        // Creates monthly report text.
        void ShowMonthlyReport()
        {
            string month = monthBox.Text.Trim();
            reportBox.Text =
                "MONTHLY REPORT (" + month + ")" + Environment.NewLine +
                "--------------------------" + Environment.NewLine +
                "Student Entries : " + Store.LoadLogs(Store.EntryFile).Count(x => x.Date.StartsWith(month)) + Environment.NewLine +
                "Student Exits   : " + Store.LoadLogs(Store.ExitFile).Count(x => x.Date.StartsWith(month));
        }

        // Creates late-entry report text.
        void ShowLateReport()
        {
            var late = Store.LoadLogs(Store.EntryFile).Where(x => string.Compare(x.Time, "22:00:00", StringComparison.Ordinal) >= 0).ToList();
            reportBox.Text = "LATE ENTRY REPORT - AFTER 10 PM" + Environment.NewLine + "--------------------------------" + Environment.NewLine;
            foreach (var r in late)
            {
                reportBox.AppendText(r.StudentId + " | " + r.Date + " | " + r.Time + Environment.NewLine);
            }
            if (late.Count == 0) reportBox.AppendText("No late entries found.");
        }

        // Exports report to TXT.
        void ExportReport_Click(object sender, EventArgs e)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            var lines = new List<string>
            {
                "Smart Hostel Entry & Exit Management System Report",
                "Generated Date: " + today,
                "",
                "Total Students  : " + Store.LoadStudents().Count,
                "Today's Entries : " + Store.LoadLogs(Store.EntryFile).Count(x => x.Date == today),
                "Today's Exits   : " + Store.LoadLogs(Store.ExitFile).Count(x => x.Date == today),
                "Visitors Count  : " + Store.LoadVisitors().Count(x => x.EntryDate == today)
            };
            File.WriteAllLines(Store.ExportFile, lines.ToArray());
            MessageBox.Show("Report exported to " + Store.ExportFile);
        }

        // Adds visitor entry.
        void AddVisitor_Click(object sender, EventArgs e)
        {
            var visitors = Store.LoadVisitors();
            visitors.Add(new VisitorRecord
            {
                Name = visitorNameBox.Text.Trim(),
                StudentId = visitorStudentBox.Text.Trim(),
                Mobile = visitorMobileBox.Text.Trim(),
                EntryDate = DateTime.Now.ToString("yyyy-MM-dd"),
                EntryTime = DateTime.Now.ToString("HH:mm:ss"),
                ExitTime = "Not Exited"
            });
            Store.SaveVisitors(visitors);
            visitorNameBox.Clear();
            visitorStudentBox.Clear();
            visitorMobileBox.Clear();
            RefreshAll();
            MessageBox.Show("Visitor entry saved.");
        }

        // Marks visitor exit.
        void MarkVisitorExit_Click(object sender, EventArgs e)
        {
            string name = visitorNameBox.Text.Trim();
            string sid = visitorStudentBox.Text.Trim();
            var visitors = Store.LoadVisitors();
            for (int i = visitors.Count - 1; i >= 0; i--)
            {
                if (visitors[i].Name == name && visitors[i].StudentId == sid && visitors[i].ExitTime == "Not Exited")
                {
                    visitors[i].ExitTime = DateTime.Now.ToString("HH:mm:ss");
                    Store.SaveVisitors(visitors);
                    RefreshAll();
                    MessageBox.Show("Visitor exit marked.");
                    return;
                }
            }
            MessageBox.Show("Active visitor record not found.");
        }
    }

    // Starts the GUI application.
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}
