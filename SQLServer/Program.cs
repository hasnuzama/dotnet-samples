using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServer
{
	public class Program
	{
		static void Main(string[] args)
		{
			try
			{
				// Insert some dummy data in db
				List<Student> students = GetTestData();
				int rowsInserted = InsertStudentsToDB(students);
				Console.WriteLine($"No of rows inserted : {rowsInserted}");

				// Read students from db and print
				students = GetStudentsFromDB();
				Console.WriteLine("------------ALL STUDENTS--------------");
				PrintStudentsToConsole(students);
				Console.WriteLine("-----------------------------------");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Some unknown error occurred.");
				Console.WriteLine($"Error message: {ex.Message}");
			}
			finally
			{
				Console.WriteLine("Press anykey to exit.");
				Console.ReadKey();
			}
		}

		private static int InsertStudentsToDB(List<Student> students)
		{
			string query = "insert into student(FirstName,LastName,DOB,Percentage) values(@FirstName,@LastName,@DOB,@Percentage)";
			int totalInsertedRecords = 0;
			// Get connection
			using (var conn = GetSqlConnection())
			{
				// Open connection
				conn.Open();

				foreach (Student student in students)
				{
					// Create command
					using (SqlCommand command = new SqlCommand(query, conn))
					{
						command.Parameters.AddWithValue("FirstName", student.FirstName);
						command.Parameters.AddWithValue("LastName", student.LastName);
						command.Parameters.AddWithValue("DOB", student.DOB);
						command.Parameters.AddWithValue("Percentage", student.Percentage);

						// Execute command
						totalInsertedRecords += command.ExecuteNonQuery();
					}
				}
			}
			return totalInsertedRecords;
		}

		private static List<Student> GetStudentsFromDB()
		{
			string query = "select FirstName,LastName,DOB,Percentage from student";
			// Get connection
			using (var conn = GetSqlConnection())
			{
				// Open connection
				conn.Open();
				
				// Create command
				using (SqlCommand command = new SqlCommand(query, conn))
				{
					DataTable dtStudents = new DataTable();
					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						// Read all data from db and fill that in datatable
						adapter.Fill(dtStudents);
					}

					// Now iterate over datatable and convert that to list of students
					return ConvertDatatableToStudents(dtStudents);
				}
			}
		}

		private static SqlConnection GetSqlConnection()
		{
			return new SqlConnection(ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString);
		}

		private static List<Student> GetTestData()
		{
			List<Student> students = new List<Student>();
			students.Add(new Student()
			{
				FirstName = "John",
				LastName = "Doe",
				DOB = new DateTime(1994, 02, 13),
				Percentage = 84.2
			});
			students.Add(new Student()
			{
				FirstName = "Ben",
				LastName = "Stokes",
				DOB = new DateTime(1993, 07, 28),
				Percentage = 70
			});
			return students;
		}

		private static List<Student> ConvertDatatableToStudents(DataTable dt)
		{
			List<Student> students = new List<Student>();
			foreach(DataRow row in dt.Rows)
			{
				Student student = new Student();
				student.FirstName = (string)row["FirstName"];
				student.LastName = (string)row["LastName"];
				student.DOB = (DateTime)row["DOB"];
				student.Percentage = Convert.ToDouble(row["Percentage"]);
				students.Add(student);
			}
			return students;
		}

		private static void PrintStudentsToConsole(List<Student> students)
		{
			foreach (Student student in students)
			{
				Console.WriteLine(student);
			}
		}
	}

	public class Student
	{
		public int ID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime DOB { get; set; }
		public double Percentage { get; set; }

		public override string ToString()
		{
			return $"FirstName={FirstName},LastName={LastName},DOB={DOB.ToShortDateString()},Percentage={Percentage}";
		}
	}
}
