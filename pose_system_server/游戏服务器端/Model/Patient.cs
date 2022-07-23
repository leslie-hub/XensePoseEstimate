using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    class Patient
    {
        public Patient(int id, string username,string gender,string bedNumber, string disease,string lrHand,string age)
        {
            this.Id = id;
            this.Username = username;
            this.Gender = gender;
            this.BedNumber = bedNumber;
            this.Disease = disease;
            this.lrHand = lrHand;
            this.Age = age;
        }
        public int Id { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public string BedNumber { get; set; }
        public string Disease { get; set; }
        public string lrHand { get; set; }
        public string Age { get; set;  }

    }
}
