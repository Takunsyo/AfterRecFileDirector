using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.MirakurunWarpper.Apis
{


    public class Tuner
    {
        public int index { get; set; }
        public string name { get; set; }
        public string[] types { get; set; }
        public string command { get; set; }
        public int? pid { get; set; }
        public User[] users { get; set; }
        public bool isAvailable { get; set; }
        public bool isRemote { get; set; }
        public bool isFree { get; set; }
        public bool isUsing { get; set; }
        public bool isFault { get; set; }

        public string Type
        {
            get
            {
                string tmp = "";
                foreach(var i in types)
                {
                    tmp += i + ",";
                }
                tmp.Remove(tmp.Length - 1);
                return tmp;
            }
        }
        public string User
        {
            get
            {
                string tmp = "";
                foreach(var i in users)
                {
                    tmp += i.ToString() + "\n";
                }
                return tmp.Trim();
            }
        }
    }

    public class User
    {
        public string id { get; set; }
        public int priority { get; set; }
        public string agent { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:({1}) [{2}]",agent,id,priority.ToString());
        }
    }
}
