using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ASoft.Db
{
    public class DbCommandParame
    {
        public String SqlParamString { set; get; }

        public IDbDataParameter[] SqlParamList { set; get; }
    }
}
