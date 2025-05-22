using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App5.Models;
using MySql.Data.MySqlClient;
using Windows.System;

namespace App5.Services
{
    public class DatabaseService
    {
        public static string connectionString = "Server=localhost;Database=exam_manage;User ID=root;Password=123456";
        public DatabaseService()
        {
        }
        // 获取所有考试信息
        public async Task<List<ExamInfo>> GetExamsAsync()
        {
            var exams = new List<ExamInfo>();
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand("select * from exams order by date desc", connection);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        exams.Add(new ExamInfo
                        {
                            id = reader.GetInt32("id"),
                            name = reader.GetString("name"),
                            date = reader.GetDateTime("date").ToString("yyyy-MM-dd"),
                            timestart = reader.GetTimeSpan("timestart").ToString(@"hh\:mm"),
                            timeend = reader.GetTimeSpan("timeend").ToString(@"hh\:mm"),
                            room = reader.GetString("room")
                        });
                    }
                }
                return exams;
            } 
            catch(Exception ex)
            {
                throw new Exception("获取考试数据失败", ex);
            }
        }
        // 获取未过期考试信息
        public async Task<List<ExamInfo>> GetUnfinishedExamsAsync()
        {
            var exams = new List<ExamInfo>();
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand("select * from exams where (date > curdate()) or (date = curdate() and timestart > curtime())", connection);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        exams.Add(new ExamInfo
                        {
                            id = reader.GetInt32("id"),
                            name = reader.GetString("name"),
                            date = reader.GetDateTime("date").ToString("yyyy-MM-dd"),
                            timestart = reader.GetTimeSpan("timestart").ToString(@"hh\:mm"),
                            timeend = reader.GetTimeSpan("timeend").ToString(@"hh\:mm"),
                            room = reader.GetString("room")
                        });
                    }
                }
                return exams;
            }
            catch (Exception ex)
            {
                throw new Exception("获取考试数据失败", ex);
            }
        }
        // 获取考生信息
        public async Task<List<UserInfo>> GetStudentAsync()
        {
            var students = new List<UserInfo>();
            try
            {
                using(var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var command = new MySqlCommand("select * from users where name != 'admin'", connection);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        UserInfo student = new UserInfo
                        {
                            id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Gender = reader.GetString("gender"),
                            BirthDate = reader.GetDateTime("birthdate").ToString("yyyy-MM-dd"),
                            Password = reader.GetString("password"),
                            Role = reader.GetString("role"),
                            ID_number = reader.GetString("ID_number"),
                            Phone = reader.GetString("phone")
                        };
                        students.Add(student);
                    }
                }
                return students;
            }
            catch(Exception ex)
            {
                throw new Exception("获取考生数据失败", ex);
            }
        }
        //添加考试
        public async Task AddExamAsync(ExamInfo exam)
        {
            try
            {
                using(var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var command = connection.CreateCommand();
                    command.CommandText = @"insert into exams (name,date,timestart,timeend,room) values (@name,@date,@timestart,@timeend,@room)";
                    command.Parameters.AddWithValue("@name", exam.name);
                    command.Parameters.AddWithValue("@date", exam.date);
                    command.Parameters.AddWithValue("@timestart", exam.timestart);
                    command.Parameters.AddWithValue("@timeend", exam.timeend);
                    command.Parameters.AddWithValue("room", exam.room);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("添加考试信息失败", ex);
            }
        }
        // 删除考试
        public async Task DeleteExamAsync(ExamInfo exam)
        {
            using(var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"delete from exams where id = @id";
                command.Parameters.AddWithValue("@id", exam.id);

                await command.ExecuteNonQueryAsync();
            }
        }
        // 添加考生
        public async Task AddStudentAsync(UserInfo student)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var command = connection.CreateCommand();
                    command.CommandText = @"insert into users (name,password,gender,birthdate,role,ID_number,phone) values (@name,@password,@gender,@birthdate,@role,@ID_number,@phone)";
                    command.Parameters.AddWithValue("@name", student.Name);
                    command.Parameters.AddWithValue("@password", student.Password);
                    command.Parameters.AddWithValue("@gender", student.Gender);
                    command.Parameters.AddWithValue("@birthdate", student.BirthDate);
                    command.Parameters.AddWithValue("@role", student.Role);
                    command.Parameters.AddWithValue("@ID_number", student.ID_number);
                    command.Parameters.AddWithValue("@phone", student.Phone);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch(Exception ex)
            {
                throw new Exception("添加考生信息失败", ex);
            }
        }
        // 删除考生
        public async Task DeleteStudentAsync(UserInfo student)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"delete from exams where id = @id";
                command.Parameters.AddWithValue("@id", student.id);

                await command.ExecuteNonQueryAsync();
            }

        }
        // 更新学生信息
        public async Task UpdateStudentInfoAsync(UserInfo student)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"update users set name = @name, gender = @gender, birthdate=@birthdate, password=@password, ID_number = @ID_number, phone = @phone WHERE id=@id";
                command.Parameters.AddWithValue("@name", student.Name);
                command.Parameters.AddWithValue("@gender", student.Gender);
                command.Parameters.AddWithValue("@birthdate", student.BirthDate);
                command.Parameters.AddWithValue("@password", student.Password);
                command.Parameters.AddWithValue("@ID_number", student.ID_number);
                command.Parameters.AddWithValue("@phone", student.Phone);
                command.Parameters.AddWithValue("@id", student.id);

                await command.ExecuteNonQueryAsync();
            }
        }
        // 报名考试
        public async Task<bool> RegisterExamAsync(int userid, int examid)
        {
            using(var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "insert into registrations (user_id, exam_id, registration_date, status) values (@userid, @examid, now(), 'registered')";
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@examid", examid);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        // 检查是否已经报名
        public async Task<bool> IsStudentRegisteredAsync(int userid, int examid)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                var command = new MySqlCommand(
                    "select count(*) from registrations where user_id = @userid and exam_id = @examid",
                    connection);
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@examid", examid);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }
        // 加载当前考生报名的考试信息
        public async Task<List<ExamInfo>> LoadRegisteredExamAsync(int userid)
        {
            List<ExamInfo> exams = new List<ExamInfo>();
            try
            {
                using(var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var command = connection.CreateCommand();
                    command.CommandText = @"select e.id, e.name, e.date, e.timestart, e.timeend, e.room from exams e join registrations er on e.id = er.exam_id where er.user_id = @userid";
                    command.Parameters.AddWithValue("@userid", userid);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var exam = new ExamInfo
                        {
                            id = reader.GetInt32("id"),
                            name = reader.GetString("name"),
                            date = reader.GetDateTime("date").ToString("yyyy-MM-dd"),
                            timestart = reader.GetTimeSpan("timestart").ToString(@"hh\:mm"),
                            timeend = reader.GetTimeSpan("timeend").ToString(@"hh\:mm"),
                            room = reader.GetString("room")
                        };
                        exam.CalculateExamStatus();
                        exams.Add(exam);
                    }
                }
                return exams;
            }
            catch(Exception ex)
            {
                throw new Exception("加载考生报名的考试信息失败", ex);
            }
        }
        // 登录
        public async Task<UserInfo> Login(string username, string password)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM users WHERE name=@name AND password=@password";
                command.Parameters.AddWithValue("name", username);
                command.Parameters.AddWithValue("password", password);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new UserInfo
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Password = reader.GetString(reader.GetOrdinal("password")),
                            Gender = reader.GetString(reader.GetOrdinal("gender")),
                            BirthDate = reader.GetDateTime(reader.GetOrdinal("birthdate")).ToString("yyyy-MM-dd"),
                            Role = reader.GetString(reader.GetOrdinal("role")),
                            ID_number = reader.GetString(reader.GetOrdinal("id_number")),
                            Phone = reader.GetString(reader.GetOrdinal("phone"))
                        };
                    }
                }
            }
            return null;
        }
    }
}
