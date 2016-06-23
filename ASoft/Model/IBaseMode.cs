using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASoft.Model
{
    public interface IBaseMode
    {
         List<string> updates{ set; get; }

        void PropertyUpdate(string propertyName);


    }
}
