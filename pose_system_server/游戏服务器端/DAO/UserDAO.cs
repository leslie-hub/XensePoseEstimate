using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using GameServer.Model;
using System.Data;

namespace GameServer.DAO
{
    class UserDAO
    {
        public User VerifyUser(MySqlConnection conn, string username, string password)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from user where username = @username and password = @password", conn);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    User user = new User(id, username, password);
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在VerifyUser的时候出现异常：" + e);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            return null;
        }

        public bool GetUserByUsername(MySqlConnection conn, string username)
        {
            MySqlDataReader reader = null;
            try
            {
                if (conn != null)
                {
                    conn.Open();
                }
                MySqlCommand cmd = new MySqlCommand("select * from user where username = @username", conn);
                cmd.Parameters.AddWithValue("username", username);
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在GetUserByUsername的时候出现异常：" + e);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return false;
        }
        public List<Patient> GetPatients(MySqlConnection conn, string username, string password)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from user where username = @username ", conn);
                cmd.Parameters.AddWithValue("username", username);
                reader = cmd.ExecuteReader();
                List<Patient> patients = new List<Patient>();

                while (reader.HasRows)
                {
                    int id = reader.GetInt32("id");
                    string name = reader.GetString("username");
                    string gender = reader.GetString("gender");
                    string bednumber = reader.GetString("bednumber");
                    string disease = reader.GetString("disease");
                    string lrhand = reader.GetString("lrhand");
                    string age = reader.GetString("age");

                    patients.Add(new Patient(id, name, gender, bednumber, disease, lrhand, age));
                }
                return patients;
            }
            catch (Exception e)
            {
                Console.WriteLine("在VerifyUser的时候出现异常：" + e);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            return null;
        }
        public void AddUser(MySqlConnection conn, string username, string password)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("insert into user set username = @username , password = @password", conn);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("在AddUser的时候出现异常：" + e);
            }
        }

        public void AddUser(MySqlConnection conn, string username, string gender, string bednumber, string disease, string lrhand, string age)
        {
            try
            {
                if (conn != null)
                {
                    conn.Open();
                }
                MySqlCommand cmd = new MySqlCommand("insert into user (username, gender, bednumber, disease, lrhand, age, date,time) values(@username, @gender, @bednumber, @disease, @lrhand, @age, curdate(), curtime())", conn);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("gender", gender);
                cmd.Parameters.AddWithValue("bednumber", bednumber);
                cmd.Parameters.AddWithValue("disease", disease);
                cmd.Parameters.AddWithValue("lrhand", lrhand);
                cmd.Parameters.AddWithValue("age", age);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("在AddUser的时候出现异常：" + e);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public void AddResult(MySqlConnection conn, string bednumber, string angle1, string angle2, string angle3, string angle4, string angle5, string angle6, string date)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("insert into result set bednumber = @bednumber , angle1 = @angle1 , angle2 = @angle2 , angle3 = @angle3" +
                    " , angle4 = @angle4 , angle5 = @angle5 , angle6 = @angle6 , date = @date", conn);
                cmd.Parameters.AddWithValue("bednumber", bednumber);
                cmd.Parameters.AddWithValue("angle1", angle1);
                cmd.Parameters.AddWithValue("angle2", angle2);
                cmd.Parameters.AddWithValue("angle3", angle3);
                cmd.Parameters.AddWithValue("angle4", angle4);
                cmd.Parameters.AddWithValue("angle5", angle5);
                cmd.Parameters.AddWithValue("angle6", angle5);
                cmd.Parameters.AddWithValue("date", date);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("在AddUser的时候出现异常：" + e);
            }
        }


        /// <summary>
        /// 查询几列的数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="items">需要查询的列名组成的字符串数组</param>
        /// <param name="conn">与数据库的连接</param>
        /// <returns></returns>
        public static DataSet Select(string tableName, string[] items, MySqlConnection conn)
        {
            // items[0]指的是id那一列
            string query = "SELECT " + items[0];
            for (int i = 1; i < items.Length; ++i)
            {
                query += ", " + items[i];
            }
            query += " FROM " + tableName;
            Console.WriteLine("query: {0}", query);
            return ExecuteQuery(query, conn);
        }
        public static DataSet Select(string tableName, MySqlConnection conn)
        {
            string query = "SELECT * FROM " + tableName;
            return ExecuteQuery(query, conn);
        }
        public static DataSet ExecuteQuery(string SQLString, MySqlConnection conn)
        {
            // using 指定数据库连接，执行填充数据集的操作
            using (var connection = conn)
            {
                DataSet ds = new DataSet();
                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();

                    }

                    MySqlDataAdapter da = new MySqlDataAdapter(SQLString, connection);
                    da.Fill(ds);
                }
                catch (MySqlException ex)
                {
                    connection.Close();
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return ds;
            }
        }
    }
}
