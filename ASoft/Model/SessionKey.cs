using System;
using ASoft.Model;
using ASoft.Db;
namespace ASoft.Model
{
    public class SessionKey : BaseModel
    {
        [DataProperty(Field = "Key")]
        public String Key { set; get; }
        [DataProperty(Field = "IP")]
        public String IP { set; get; }
        [DataProperty(Field = "EndTime")]
        public String EndTime { set; get; }
        [DataProperty(Field = "Type")]
        public String Type { set; get; }
        [DataProperty(Field = "State")]
        public String State { set; get; }
    }
}
