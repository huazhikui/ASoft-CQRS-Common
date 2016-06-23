using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASoft.Model
{
    public class RestResult
    {
        public bool ret { set; get; }

        public string message { set; get; } 

        public object data { set; get; }
    }
    public class RestResult<E>
    {
        public bool ret { set; get; }

        public string message { set; get; }

        public int total { set; get; }

        public E data { set; get; }

        private List<E> _rows = null;
        public List<E> rows
        {
            set
            {
                _rows = value;
            }
            get
            {
                if (_rows == null)
                {
                    _rows = new List<E>();
                }
                return _rows;
            }
        }

    }
}
