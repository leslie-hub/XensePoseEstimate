using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;
using GameServer.DAO;
using System.Data;

namespace GameServer.Controller
{
    class RoomController : BaseController
    {
        private UserDAO userDAO = new UserDAO();
        public RoomController()
        {
            requestCode = RequestCode.Room;
        }
        public string CreateRoom(string data, Client client, Server server)
        {
            server.CreateRoom(client);
            return ((int)ReturnCode.Success).ToString() + "," + ((int)RoleType.Blue).ToString();
        }
        public string ListRoom(string data, Client client, Server server)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Room room in server.GetRoomList())
            {
                if (room.IsWaitingJoin())
                {
                    sb.Append(room.GetHouseOwnerData() + "|");
                }
            }
            if (sb.Length == 0)
            {
                sb.Append("0");
            }
            else
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        public string ListUser(string data, Client client, Server server)
        {
            DataSet ds = UserDAO.Select("user", new string[] { "id", "username", "gender", "bednumber", "disease", "lrhand", "age" }, client.MySQLConn);
            string s = "";
            //拿到的table，一行多列
            if (ds != null)
            {
                DataTable table = ds.Tables[0];// 由数据集得到数据表
                                               //Debug.LogFormat("列数：{0}", table.Columns.Count);
                Console.WriteLine("查找数据库行数：{0}", table.Rows.Count);//输出数据表的行数
                if (table.Rows.Count>0)
                {
                    // 逐行输出数据
                    s += table.Rows[0][0].ToString() + "," + table.Rows[0][1].ToString() + "," + table.Rows[0][2].ToString() + "," + table.Rows[0][3].ToString() + "," + table.Rows[0][4].ToString() + "," + table.Rows[0][5].ToString() + "," + table.Rows[0][6].ToString();
                    for (int i = 1; i < table.Rows.Count; i++)
                    {
                        // 这个表呢，直接是把查到的所有用户数据输出到了服务器端的控制台上了

                        s += "|";
                        s += table.Rows[i][0].ToString() + "," + table.Rows[i][1].ToString() + "," + table.Rows[i][2].ToString() + "," + table.Rows[i][3].ToString() + "," + table.Rows[i][4].ToString() + "," + table.Rows[i][5].ToString() + "," + table.Rows[i][6].ToString();

                    }
                }
            }
            else
            {
                return "0";//一条数据都没有查到
            }

            return s;
        }





        public string JoinRoom(string data, Client client, Server server)
        {
            int id = int.Parse(data);
            Room room = server.GetRoomById(id);
            if (room == null)
            {
                return ((int)ReturnCode.NotFound).ToString();
            }
            else if (room.IsWaitingJoin() == false)
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            else
            {
                room.AddClient(client);
                string roomData = room.GetRoomData();//"returncode,roletype-id,username,tc,wc|id,username,tc,wc"
                room.BroadcastMessage(client, ActionCode.UpdateRoom, roomData);
                return ((int)ReturnCode.Success).ToString() + "," + ((int)RoleType.Red).ToString() + "-" + roomData;
            }
        }
        public string QuitRoom(string data, Client client, Server server)
        {
            bool isHouseOwner = client.IsHouseOwner();
            Room room = client.Room;
            if (isHouseOwner)
            {
                room.BroadcastMessage(client, ActionCode.QuitRoom, ((int)ReturnCode.Success).ToString());
                room.Close();
                return ((int)ReturnCode.Success).ToString();
            }
            else
            {
                client.Room.RemoveClient(client);
                room.BroadcastMessage(client, ActionCode.UpdateRoom, room.GetRoomData());
                return ((int)ReturnCode.Success).ToString();
            }
        }
    }
}
