﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;
using GameServer.DAO;
using GameServer.Model;

namespace GameServer.Controller
{
    class UserController : BaseController
    {
        private UserDAO userDAO = new UserDAO();
        private ResultDAO resultDAO = new ResultDAO();
        public UserController()
        {
            requestCode = RequestCode.User;
        }
        public string Login(string data, Client client, Server server)
        {
            string[] strs = data.Split(',');
            User user = userDAO.VerifyUser(client.MySQLConn, strs[0], strs[1]);
            if (user == null)
            {
                //Enum.GetName(typeof(ReturnCode), ReturnCode.Fail);
                return ((int)ReturnCode.Fail).ToString();
            }
            else
            {
                Result res = resultDAO.GetResultByUserid(client.MySQLConn, user.Id);
                client.SetUserData(user, res);
                return string.Format("{0},{1},{2},{3}", ((int)ReturnCode.Success).ToString(), user.Username, res.TotalCount, res.WinCount);
            }
        }
        public string Register(string data, Client client, Server server)
        {
            string[] strs = data.Split(',');
            string username = strs[0]; string password = strs[1];
            bool res = userDAO.GetUserByUsername(client.MySQLConn, username);
            if (res)
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            userDAO.AddUser(client.MySQLConn, username, password);
            return ((int)ReturnCode.Success).ToString();
        }
        public string RegisterUser(string data, Client client, Server server)
        {
            string[] strs = data.Split(',');
            string username = strs[0];
            string gender = strs[1];
            string bedNumber = strs[2];
            string disease = strs[3];
            string lrHand = strs[4];
            string age = strs[5];

            bool res = userDAO.GetUserByUsername(client.MySQLConn, username);
            if (res)
            {
                return ((int)ReturnCode.Fail).ToString();
            }

            userDAO.AddUser(client.MySQLConn, username, gender, bedNumber, disease, lrHand, age);

            return ((int)ReturnCode.Success).ToString();
        }

        public string SyncData(string data, Client client, Server server)
        {

            string[] strs = data.Split(',');
            string bednumber = strs[0]; 
           
            userDAO.AddResult(client.MySQLConn, bednumber, strs[1], strs[2], strs[3], strs[4], strs[5], strs[6], strs[7]);

            return ((int)ReturnCode.Success).ToString();
        }
    }
}
