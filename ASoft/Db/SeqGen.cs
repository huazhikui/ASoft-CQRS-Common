using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ASoft.Db
{
    /// <summary>
    /// 序列生成器
    /// </summary>
    /// <summary>
    /// <para> 序列号生成器,使用时要配合数据库来完成,序列好保存在数据库中.</para>
    /// <para> 数据库设计:数据库表名:SequenceManage,包含六个字段:SeqName(序列名称VARCHAR(10)),SeqValue(INT 序列值,不空默认1),SeqStep(INT 不空,默认1),SeqMin(INT 不空),SeqMax(INT 不空),SeqLoop(INT 0或1).</para>
    /// <para>类型参数T可以是整型(如:int,long</para>
    /// </summary>
    public class SeqGen
    {
        private static Dictionary<string, Sequence> dict = new Dictionary<string, Sequence>();
        private const string nextsql = "UPDATE ASoft_Id_Gen SET SeqValue=SeqValue + {1} WHERE LOWER(SeqName)={0}";
        public IDataAccess db = null;
        public const string table = "ASOFT_ID_GEN";

        public string key = "";

        /// <summary>
        /// 序列号生成器类
        /// </summary>
        public SeqGen(IDataAccess db)
        {
            this.db = db;
            #region 创建ID生成器表
            if (!db.Tables.Contains(table))
            {
                db.ExecuteNonQuery(db.SequenceTableSql);
            }
            #endregion
            using (DataReader dr = db.ExecuteReader("select * from ASoft_Id_Gen"))
            {
                dict.Clear();
                while (dr.Read())
                {
                    dict.Add(dr.GetString("SeqName"), new Sequence(this, dr.GetString("SeqName"), dr.GetInt64("SeqValue", true), dr.GetInt32("SeqStep", true), dr.GetInt64("SeqMin", true), dr.GetInt64("SeqMax", true), dr.GetInt16("SeqLoop", true)));
                }
                dr.Close();
            }
        }

        /// <summary>
        /// 返回参数指定的位置处的序列实例
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <returns>序列实例</returns>
        public Sequence this[string name]
        {
            get
            {
                name = name.ToLower();
                if (!dict.ContainsKey(name))
                {
                    this.db.ExecuteScalar(string.Format("INSERT INTO ASoft_Id_Gen (SeqName,SeqValue) VALUES ({0},1)", db.ToSqlValue(name)));
                    dict.Add(name, new Sequence(this, name, 1));
                }
                return dict[name.ToLower()];
            }
        }

        /// <summary>
        /// 将指定的序列向前进一个长度
        /// </summary>
        /// <param name="s">指定的序列</param>
        public void Next(Sequence s)
        {
            //LogAdapter.GetLogger("服务").Error("nextsql:" + nextsql);
            // LogAdapter.GetLogger("服务").Error("string.Format(nextsql, db.ToSqlValue(s.name),dict[s.name].Step):" + string.Format(nextsql, db.ToSqlValue(s.name), dict[s.name].Step)); 
            db.ExecuteNonQuery(string.Format(nextsql, db.ToSqlValue(s.name), dict[s.name].Step));
            Current(s);
        } 

        public void NextLuhm(Sequence s)
        {
            //LogAdapter.GetLogger("服务").Error("nextsql:" + nextsql);
            // LogAdapter.GetLogger("服务").Error("string.Format(nextsql, db.ToSqlValue(s.name),dict[s.name].Step):" + string.Format(nextsql, db.ToSqlValue(s.name), dict[s.name].Step)); 
            db.ExecuteNonQuery(string.Format(nextsql, db.ToSqlValue(s.name), dict[s.name].Step));
            CurrentLuhm(s);
        }

        /// <summary>
        /// 获取当前值
        /// </summary>
        /// <param name="s"></param>
        public void Current(Sequence s)
        {
            s.currentValue = db.ExecuteScalar(string.Format("SELECT SeqValue FROM ASoft_Id_Gen WHERE LOWER(SeqName) = {0}", db.ToSqlValue(s.name))).LongValue;
        }

        public long Current(Sequence s, int length)
        {
            Current(s);
            if (length > 2)
            {
                length = length - 2;
            } 
             var result = (long)Math.Pow(10, length) + s.currentValue;
             result = result * 10 + this.GenLuhm(result);
             return result;
        } 

        /// <summary>
        /// 获取当前值(带luhm校验码),16位
        /// </summary>
        /// <param name="s"></param>
        public void CurrentLuhm(Sequence s)
        {
            Current(s);
            long pre = (Convert.ToInt64(DateTime.Now.ToString("yyMMdd")) * 1000000000) + s.currentValue;
            s.currentLuhmValue = pre.ToString() + GenLuhm(pre);
        }


        public int GenLuhm(long togen)
        {
            int curval = 0;
            int total = 0;
            long bitNum = 1;
            String strTogen = togen.ToString();
            int lenght = strTogen.Length;
            for (int i = 0; i < lenght; i++)
            {
                curval = int.Parse(strTogen.Substring(i, 1));
                if (bitNum == 1)
                {
                    bitNum = 0;
                    curval = curval * 2;
                    if (curval > 9)
                    {
                        curval -= 9;
                    }
                }
                else
                {
                    bitNum = 1;
                }
                total += curval;
            }
            int mod = total % 10;
            if (mod == 0)
            {
                return 0;
            }
            else
            {
                return 10 - mod;
            }
        }
    }

    /// <summary>
    /// 序列
    /// </summary>
    public class Sequence
    {
        private SeqGen seqgen = null;
        public string add = string.Empty;
        public string select = string.Empty;

        public string name = string.Empty;


        private static Dictionary<String, Sequence> seqDict = new Dictionary<string, Sequence>();
        private static Sequence _defaultInstance = null;
        public static Sequence GetInstance(IDataAccess db)
        {
            if (_defaultInstance == null)
            {
                _defaultInstance = new SeqGen(db)["default"];
                seqDict["default"] = _defaultInstance;
            }
            return _defaultInstance;
        }

        public static Sequence GetInstance(IDataAccess db, String key)
        {
            Sequence instance = null;
            if (!seqDict.Keys.Contains(key) || seqDict[key] == null)
            {
                instance = new SeqGen(db)[key];
                seqDict[key] = instance;
            }
            else
            {
                instance = seqDict[key];
            }
            return instance;
        }

        /// <summary>
        /// 序列号生成器的实例
        /// </summary>
        /// <param name="seqgen"></param>   
        /// <param name="name">序列名称</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="step">步长</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="loop">是否循环</param>
        public Sequence(SeqGen seqgen, string name, long currentValue, int step, long min, long max, int loop)
        {
            this.seqgen = seqgen;
            this.name = name.ToLower();
            this.currentValue = currentValue;
            this.step = step;
            this.min = min;
            this.max = max;
            this.loop = loop;
        }

        public Sequence(SeqGen seqgen, string name, long currentValue)
        {
            this.seqgen = seqgen;
            this.name = name.ToLower();
            this.currentValue = currentValue;

        }

        private int step = 1;
        public int Step
        {
            get
            {
                return step;
            }
        }

        private long min;
        public long Min
        {
            get
            {
                return min;
            }
        }

        private long max;
        public long Max
        {
            get
            {
                return max;
            }
        }

        private int loop = 0;
        public int Loop
        {
            get
            {
                return loop;
            }
        }


        internal long currentValue;
        /// <summary>
        /// 返回序列的当前值
        /// </summary>
        public long CurrentValue
        {
            get
            {
                seqgen.Current(this);
                return currentValue;
            }
        }

        internal String currentLuhmValue;
        /// <summary>
        /// 返回16位序列的当前值，最后一位为luhm校验码
        /// </summary>
        public String CurrentLuhmValue
        {
            get
            {
                seqgen.CurrentLuhm(this);
                return currentLuhmValue;
            }
        }

        /// <summary>
        /// 返回序列的下一个值
        /// </summary>
        public long NextValue
        {
            get
            {
                seqgen.Next(this);
                return currentValue;
            }
        }

        /// <summary>
        /// 返回序列的下一个值
        /// </summary>
        public String NextLuhmValue
        {
            get
            {
                seqgen.NextLuhm(this);
                return currentLuhmValue;
            }
        }

        public String Next(int length)
        {
            seqgen.Next(this);
            return seqgen.Current(this, length).ToString();
        }

        /// <summary>
        /// 最后一位为校验位
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public Int64 NextInt64(int length)
        {
            seqgen.Next(this);
            return seqgen.Current(this, length);
        }
    }
}